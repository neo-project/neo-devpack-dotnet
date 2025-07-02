using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Deploy.Services;

/// <summary>
/// Contract invocation service implementation
/// </summary>
public class ContractInvokerService : IContractInvoker
{
    private readonly ILogger<ContractInvokerService> _logger;
    private readonly IWalletManager _walletManager;
    private readonly IConfiguration _configuration;

    public ContractInvokerService(ILogger<ContractInvokerService> logger, IWalletManager walletManager, IConfiguration configuration)
    {
        _logger = logger;
        _walletManager = walletManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Call a contract method without sending a transaction
    /// </summary>
    public async Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters)
    {
        try
        {
            var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
            if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
            {
                throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
            }

            var client = new RpcClient(new Uri(networkConfig.RpcUrl));

            using var sb = new ScriptBuilder();

            // Build parameters array
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = parameters.Length - 1; i >= 0; i--)
                {
                    sb.EmitPush(parameters[i]);
                }
                sb.EmitPush(parameters.Length);
                sb.Emit(OpCode.PACK);
            }
            else
            {
                sb.Emit(OpCode.NEWARRAY0);
            }

            // Call contract method
            sb.EmitPush(method);
            sb.EmitPush(contractHash);
            sb.EmitPush(CallFlags.All);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

            var script = sb.ToArray();
            var result = await client.InvokeScriptAsync(script);

            if (result.State != VMState.HALT)
            {
                throw new Exception($"Script execution failed: {result.Exception}");
            }

            if (result.Stack.Length == 0)
            {
                return default;
            }

            return ConvertStackItem<T>(result.Stack[0]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call {Method} on {Contract}", method, contractHash);
            throw;
        }
    }

    /// <summary>
    /// Send a transaction to invoke a contract method
    /// </summary>
    public async Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters)
    {
        try
        {
            var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
            if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
            {
                throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
            }

            var client = new RpcClient(new Uri(networkConfig.RpcUrl));

            var signerAccount = _walletManager.GetAccount();

            using var sb = new ScriptBuilder();

            // Build parameters array
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = parameters.Length - 1; i >= 0; i--)
                {
                    sb.EmitPush(parameters[i]);
                }
                sb.EmitPush(parameters.Length);
                sb.Emit(OpCode.PACK);
            }
            else
            {
                sb.Emit(OpCode.NEWARRAY0);
            }

            // Call contract method
            sb.EmitPush(method);
            sb.EmitPush(contractHash);
            sb.EmitPush(CallFlags.All);
            sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

            var script = sb.ToArray();

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
                throw new Exception($"Contract invocation test failed: {testResult.Exception}");
            }

            // Get current block count
            var blockCount = await client.GetBlockCountAsync();

            // Get deployment configuration
            var deploymentConfig = _configuration.GetSection("Deployment").Get<DeploymentConfiguration>() ?? new DeploymentConfiguration();

            // Create transaction
            var gasConsumed = testResult.GasConsumed;
            var networkFee = deploymentConfig.DefaultNetworkFee;
            var validUntilBlock = blockCount + deploymentConfig.ValidUntilBlockOffset;

            var tx = new Transaction
            {
                Version = 0,
                Nonce = (uint)new Random().Next(),
                SystemFee = gasConsumed,
                NetworkFee = networkFee,
                ValidUntilBlock = validUntilBlock,
                Signers = new[] { signer },
                Attributes = Array.Empty<TransactionAttribute>(),
                Script = script,
                Witnesses = Array.Empty<Witness>()
            };

            // Sign transaction using wallet manager
            await _walletManager.SignTransactionAsync(tx, signerAccount);

            // Send transaction
            var txHash = await client.SendRawTransactionAsync(tx);

            _logger.LogInformation("Transaction sent: {TxId}", txHash);
            return txHash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send {Method} to {Contract}", method, contractHash);
            throw;
        }
    }

    /// <summary>
    /// Wait for transaction confirmation
    /// </summary>
    public async Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5)
    {
        var networkConfig = _configuration.GetSection("Network").Get<NetworkConfiguration>();
        if (networkConfig == null || string.IsNullOrEmpty(networkConfig.RpcUrl))
        {
            throw new InvalidOperationException("Network configuration not found. Please check appsettings.json");
        }

        var client = new RpcClient(new Uri(networkConfig.RpcUrl));

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var tx = await client.GetRawTransactionAsync(txHash.ToString());
                if (tx != null)
                {
                    _logger.LogInformation("Transaction {TxId} confirmed", txHash);
                    return true;
                }
            }
            catch { }

            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }

        _logger.LogWarning("Transaction {TxId} not confirmed after {Retries} retries", txHash, maxRetries);
        return false;
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

        throw new NotSupportedException($"Type {type} is not supported");
    }
}
