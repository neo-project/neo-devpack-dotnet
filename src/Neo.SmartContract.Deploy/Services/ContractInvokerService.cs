using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.VM;
using Neo.SmartContract.Deploy.Exceptions;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Contract invocation service implementation
/// </summary>
public class ContractInvokerService : IContractInvoker
{
    private readonly ILogger<ContractInvokerService> _logger;
    private readonly IWalletManager _walletManager;
    private readonly IConfiguration _configuration;
    private readonly IRpcClientFactory _rpcClientFactory;
    private readonly TransactionBuilder _transactionBuilder;
    private readonly TransactionConfirmationService _confirmationService;

    public ContractInvokerService(
        ILogger<ContractInvokerService> logger,
        IWalletManager walletManager,
        IConfiguration configuration,
        IRpcClientFactory rpcClientFactory,
        TransactionBuilder transactionBuilder,
        TransactionConfirmationService confirmationService)
    {
        _logger = logger;
        _walletManager = walletManager;
        _configuration = configuration;
        _rpcClientFactory = rpcClientFactory;
        _transactionBuilder = transactionBuilder;
        _confirmationService = confirmationService;
    }

    /// <summary>
    /// Call a contract method without sending a transaction
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Method return value</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ContractInvocationException">Thrown when invocation fails</exception>
    public async Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters)
    {
        if (contractHash == null) throw new ArgumentNullException(nameof(contractHash));
        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("Method name cannot be empty", nameof(method));

        try
        {
            var client = _rpcClientFactory.CreateClient();

            // Build contract call script using helper
            var script = ScriptBuilderHelper.BuildContractCallScript(contractHash, method, parameters);

            var result = await client.InvokeScriptAsync(script);

            if (result.State != VMState.HALT)
            {
                throw new ContractInvocationException(contractHash, method, result.Exception ?? "Script execution failed");
            }

            if (result.Stack.Length == 0)
            {
                return default;
            }

            return ConvertStackItem<T>(result.Stack[0]);
        }
        catch (ContractInvocationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call {Method} on {Contract}", method, contractHash);
            throw new ContractInvocationException(contractHash, method, $"Failed to call contract method: {ex.Message}");
        }
    }

    /// <summary>
    /// Send a transaction to invoke a contract method
    /// </summary>
    /// <param name="contractHash">Contract hash</param>
    /// <param name="method">Method name</param>
    /// <param name="parameters">Method parameters</param>
    /// <returns>Transaction hash</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ContractInvocationException">Thrown when invocation fails</exception>
    public async Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters)
    {
        if (contractHash == null) throw new ArgumentNullException(nameof(contractHash));
        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("Method name cannot be empty", nameof(method));

        try
        {
            var client = _rpcClientFactory.CreateClient();
            var signerAccount = _walletManager.GetAccount();

            // Build contract call script using helper
            var script = ScriptBuilderHelper.BuildContractCallScript(contractHash, method, parameters);

            // Create signer
            var signer = new Signer
            {
                Account = signerAccount,
                Scopes = WitnessScope.CalledByEntry
            };

            // Test invoke to get gas consumption
            var testResult = await client.InvokeScriptAsync(script, signer);
            if (testResult.State != VMState.HALT)
            {
                throw new ContractInvocationException(contractHash, method, testResult.Exception ?? "Contract invocation test failed");
            }

            // Get current block count
            var blockCount = await client.GetBlockCountAsync();

            // Get deployment configuration
            var deploymentConfig = _configuration.GetSection("Deployment").Get<DeploymentConfiguration>() ?? new DeploymentConfiguration();

            // Create transaction using builder
            var txOptions = new TransactionBuildOptions
            {
                Sender = signerAccount,
                Script = script,
                SystemFee = testResult.GasConsumed,
                NetworkFee = deploymentConfig.DefaultNetworkFee,
                ValidUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset
            };
            var tx = _transactionBuilder.Build(txOptions);

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, signerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            _logger.LogInformation("Transaction sent: {TxId}", txHash);
            return txHash;
        }
        catch (ContractInvocationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send {Method} to {Contract}", method, contractHash);
            throw new ContractInvocationException(contractHash, method, $"Failed to invoke contract method: {ex.Message}");
        }
    }

    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    /// <param name="txHash">Transaction hash</param>
    /// <param name="maxRetries">Maximum retry attempts</param>
    /// <param name="delaySeconds">Delay between retries in seconds</param>
    /// <returns>True if confirmed, false otherwise</returns>
    /// <exception cref="ArgumentNullException">Thrown when txHash is null</exception>
    public async Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5)
    {
        if (txHash == null) throw new ArgumentNullException(nameof(txHash));

        var client = _rpcClientFactory.CreateClient();

        var options = new TransactionConfirmationOptions
        {
            Timeout = TimeSpan.FromSeconds(maxRetries * delaySeconds),
            PollingInterval = TimeSpan.FromSeconds(delaySeconds),
            MaxAttempts = maxRetries
        };

        return await _confirmationService.WaitForConfirmationAsync(client, txHash, options);
    }

    private T? ConvertStackItem<T>(Neo.VM.Types.StackItem item)
    {
        var type = typeof(T);

        if (type == typeof(bool))
            return (T)(object)item.GetBoolean();

        if (type == typeof(BigInteger))
            return (T)(object)item.GetInteger();

        if (type == typeof(long))
            return (T)(object)(long)item.GetInteger();

        if (type == typeof(int))
            return (T)(object)(int)item.GetInteger();

        if (type == typeof(string))
            return (T)(object)item.GetString()!;

        if (type == typeof(byte[]))
            return (T)(object)item.GetSpan().ToArray();

        if (type == typeof(UInt160))
            return (T)(object)new UInt160(item.GetSpan());

        if (type == typeof(UInt256))
            return (T)(object)new UInt256(item.GetSpan());

        // Handle object type (return raw stack item)
        if (type == typeof(object))
            return (T)(object)item;

        throw new NotSupportedException($"Type {type} is not supported for stack item conversion");
    }
}
