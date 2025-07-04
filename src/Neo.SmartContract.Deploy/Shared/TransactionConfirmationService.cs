using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.RPC;

namespace Neo.SmartContract.Deploy.Shared;

/// <summary>
/// Service for waiting and confirming transaction execution
/// </summary>
public class TransactionConfirmationService
{
    private readonly ILogger<TransactionConfirmationService> _logger;
    private readonly TransactionConfirmationOptions _defaultOptions;

    public TransactionConfirmationService(ILogger<TransactionConfirmationService> logger)
    {
        _logger = logger;
        _defaultOptions = new TransactionConfirmationOptions();
    }

    /// <summary>
    /// Wait for a transaction to be confirmed on the blockchain
    /// </summary>
    /// <param name="client">RPC client</param>
    /// <param name="txHash">Transaction hash to wait for</param>
    /// <param name="options">Confirmation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if confirmed, false if timeout or cancelled</returns>
    public async Task<bool> WaitForConfirmationAsync(
        RpcClient client,
        UInt256 txHash,
        TransactionConfirmationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= _defaultOptions;

        _logger.LogInformation("Waiting for transaction {TxHash} confirmation...", txHash);

        var startTime = DateTime.UtcNow;
        var attempts = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            attempts++;

            try
            {
                // Check if transaction is in a block
                var transaction = await client.GetRawTransactionAsync(txHash.ToString());
                if (transaction != null && transaction.BlockHash != null)
                {
                    _logger.LogInformation("Transaction {TxHash} confirmed in block {BlockHash} after {Attempts} attempts",
                        txHash, transaction.BlockHash, attempts);

                    // Report progress
                    options.ProgressCallback?.Invoke(new TransactionProgress
                    {
                        Status = TransactionStatus.Confirmed,
                        BlockHash = transaction.BlockHash?.ToString(),
                        Confirmations = (int)(transaction.Confirmations ?? 0)
                    });

                    return true;
                }

                // Check if we've exceeded the timeout
                if (DateTime.UtcNow - startTime > options.Timeout)
                {
                    _logger.LogWarning("Transaction {TxHash} confirmation timeout after {Timeout}",
                        txHash, options.Timeout);

                    options.ProgressCallback?.Invoke(new TransactionProgress
                    {
                        Status = TransactionStatus.Timeout
                    });

                    return false;
                }

                // Check if we've exceeded max attempts
                if (attempts >= options.MaxAttempts)
                {
                    _logger.LogWarning("Transaction {TxHash} confirmation failed after {MaxAttempts} attempts",
                        txHash, options.MaxAttempts);

                    options.ProgressCallback?.Invoke(new TransactionProgress
                    {
                        Status = TransactionStatus.Failed,
                        ErrorMessage = "Max attempts exceeded"
                    });

                    return false;
                }

                // Report progress
                if (attempts % 5 == 0) // Report every 5 attempts
                {
                    options.ProgressCallback?.Invoke(new TransactionProgress
                    {
                        Status = TransactionStatus.Pending,
                        Attempts = attempts
                    });
                }

                // Wait before next attempt
                await Task.Delay(options.PollingInterval, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking transaction {TxHash} status", txHash);

                // Continue trying unless it's a critical error
                if (options.FailOnError)
                {
                    options.ProgressCallback?.Invoke(new TransactionProgress
                    {
                        Status = TransactionStatus.Failed,
                        ErrorMessage = ex.Message
                    });

                    return false;
                }
            }
        }

        _logger.LogInformation("Transaction {TxHash} confirmation cancelled", txHash);

        options.ProgressCallback?.Invoke(new TransactionProgress
        {
            Status = TransactionStatus.Cancelled
        });

        return false;
    }

    /// <summary>
    /// Wait for multiple transactions to be confirmed
    /// </summary>
    /// <param name="client">RPC client</param>
    /// <param name="txHashes">Transaction hashes to wait for</param>
    /// <param name="options">Confirmation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of transaction hash to confirmation result</returns>
    public async Task<Dictionary<UInt256, bool>> WaitForMultipleConfirmationsAsync(
        RpcClient client,
        UInt256[] txHashes,
        TransactionConfirmationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = txHashes.Select(async txHash =>
        {
            var result = await WaitForConfirmationAsync(client, txHash, options, cancellationToken);
            return (txHash, result);
        });

        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(r => r.txHash, r => r.result);
    }
}

/// <summary>
/// Options for transaction confirmation
/// </summary>
public class TransactionConfirmationOptions
{
    /// <summary>
    /// Maximum time to wait for confirmation
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Interval between polling attempts
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Maximum number of polling attempts
    /// </summary>
    public int MaxAttempts { get; set; } = 150;

    /// <summary>
    /// Whether to fail immediately on error or continue trying
    /// </summary>
    public bool FailOnError { get; set; } = false;

    /// <summary>
    /// Callback for progress updates
    /// </summary>
    public Action<TransactionProgress>? ProgressCallback { get; set; }
}

/// <summary>
/// Transaction confirmation progress
/// </summary>
public class TransactionProgress
{
    public TransactionStatus Status { get; set; }
    public string? BlockHash { get; set; }
    public int Confirmations { get; set; }
    public int Attempts { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Transaction status
/// </summary>
public enum TransactionStatus
{
    Pending,
    Confirmed,
    Failed,
    Timeout,
    Cancelled
}
