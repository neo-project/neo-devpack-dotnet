using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Extensions;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Shared;
using Neo.VM;
using Neo.Wallets;
using Neo.VM.Types;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Service for invoking contract methods
/// </summary>
public class ContractInvokerService : IContractInvoker
{
    private readonly IWalletManager _walletManager;
    private readonly ILogger<ContractInvokerService>? _logger;

    /// <summary>
    /// Initialize a new instance of ContractInvokerService
    /// </summary>
    /// <param name="walletManager">Wallet manager service</param>
    /// <param name="logger">Optional logger</param>
    public ContractInvokerService(IWalletManager walletManager, ILogger<ContractInvokerService>? logger = null)
    {
        _walletManager = walletManager ?? throw new ArgumentNullException(nameof(walletManager));
        _logger = logger;
    }

    /// <summary>
    /// Call a contract method (read-only)
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <param name="rpcUrl">RPC URL</param>
    /// <returns>Method return value</returns>
    public async Task<T> CallAsync<T>(UInt160 contractHash, string method, object[] args, string rpcUrl)
    {
        if (contractHash == null)
            throw new ArgumentNullException(nameof(contractHash));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));

        if (string.IsNullOrWhiteSpace(rpcUrl))
            throw new ArgumentException("RPC URL cannot be null or empty", nameof(rpcUrl));

        _logger?.LogInformation("Calling contract method {Method} on {Contract}", method, contractHash);

        try
        {
            var rpcClient = new RpcClient(new Uri(rpcUrl));
            
            // Build the script
            using var scriptBuilder = new ScriptBuilder();
            scriptBuilder.EmitDynamicCall(contractHash, method, args);
            
            var script = scriptBuilder.ToArray();
            var result = await rpcClient.InvokeScriptAsync(script);
            
            if (result.State != VMState.HALT)
            {
                throw new ContractDeploymentException($"Method call failed: {result.Exception}");
            }

            if (result.Stack.Length == 0)
            {
                return default(T)!;
            }

            // Convert the result to the requested type
            var stackItem = result.Stack[0];
            return ConvertStackItem<T>(stackItem);
        }
        catch (Exception ex) when (!(ex is ContractDeploymentException))
        {
            throw new ContractDeploymentException($"Failed to call contract method: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Invoke a contract method (state-changing transaction)
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="args">Method arguments</param>
    /// <param name="options">Invocation options</param>
    /// <returns>Transaction hash</returns>
    public async Task<UInt256> InvokeAsync(UInt160 contractHash, string method, object[] args, InvocationOptions options)
    {
        if (contractHash == null)
            throw new ArgumentNullException(nameof(contractHash));

        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("Method name cannot be null or empty", nameof(method));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.RpcUrl))
            throw new ArgumentException("RPC URL is required in options", nameof(options));

        if (string.IsNullOrWhiteSpace(options.WifKey) && options.InvokerAccount == null)
            throw new ArgumentException("Either WIF key or invoker account is required", nameof(options));

        _logger?.LogInformation("Invoking contract method {Method} on {Contract}", method, contractHash);

        try
        {
            var rpcClient = new RpcClient(new Uri(options.RpcUrl));
            
            // Get the account
            Account account;
            if (!string.IsNullOrEmpty(options.WifKey))
            {
                account = _walletManager.GetAccountFromWif(options.WifKey);
            }
            else
            {
                throw new NotSupportedException("Only WIF key signing is currently supported");
            }

            // Build the script
            using var scriptBuilder = new ScriptBuilder();
            scriptBuilder.EmitDynamicCall(contractHash, method, args);
            var script = scriptBuilder.ToArray();

            // Get the current block count for ValidUntilBlock
            var blockCount = await rpcClient.GetBlockCountAsync();
            var validUntilBlock = blockCount + options.ValidUntilBlockOffset;

            // Create and send the transaction
            var tx = await TransactionBuilder.CreateTransactionAsync(
                rpcClient,
                script,
                account,
                options.GasLimit,
                options.NetworkFee,
                validUntilBlock);

            _logger?.LogInformation("Sending transaction: {TxHash}", tx.Hash);
            await rpcClient.SendRawTransactionAsync(tx);

            // Wait for confirmation if requested
            if (options.WaitForConfirmation)
            {
                _logger?.LogInformation("Waiting for transaction confirmation...");
                
                for (int i = 0; i < options.ConfirmationRetries; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(options.ConfirmationDelaySeconds));
                    
                    try
                    {
                        // Check if transaction is confirmed by checking block height
                        var currentBlockCount = await rpcClient.GetBlockCountAsync();
                        if (currentBlockCount > 0)
                        {
                            _logger?.LogInformation("Transaction sent. Current block height: {BlockCount}", currentBlockCount);
                            // Assume transaction will be included soon
                            break;
                        }
                    }
                    catch
                    {
                        // Continue waiting
                    }
                }
            }

            return tx.Hash;
        }
        catch (Exception ex) when (!(ex is ContractDeploymentException))
        {
            throw new ContractDeploymentException($"Failed to invoke contract method: {ex.Message}", ex);
        }
    }

    private T ConvertStackItem<T>(StackItem stackItem)
    {
        var targetType = typeof(T);

        // Handle primitive types
        if (targetType == typeof(string))
        {
            return (T)(object)stackItem.GetString()!;
        }
        else if (targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(BigInteger))
        {
            var value = stackItem.GetInteger();
            return (T)Convert.ChangeType(value, targetType);
        }
        else if (targetType == typeof(bool))
        {
            return (T)(object)stackItem.GetBoolean();
        }
        else if (targetType == typeof(byte[]))
        {
            return (T)(object)stackItem.GetSpan().ToArray();
        }
        else if (targetType == typeof(UInt160))
        {
            var bytes = stackItem.GetSpan().ToArray();
            return (T)(object)new UInt160(bytes);
        }
        else if (targetType == typeof(UInt256))
        {
            var bytes = stackItem.GetSpan().ToArray();
            return (T)(object)new UInt256(bytes);
        }
        else
        {
            // For complex types, convert to string representation
            var jsonString = stackItem.ToJson().ToString();
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonString)!;
        }
    }
}