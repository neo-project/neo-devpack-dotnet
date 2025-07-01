using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Services;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using System.Numerics;
using System.Linq;

namespace Neo.SmartContract.Deploy.Utilities
{
    /// <summary>
    /// Utility for invoking deployed contracts
    /// </summary>
    public interface IContractInvoker
    {
        /// <summary>
        /// Call a contract method without sending a transaction
        /// </summary>
        Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters);

        /// <summary>
        /// Send a transaction to invoke a contract method
        /// </summary>
        Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters);

        /// <summary>
        /// Wait for transaction confirmation
        /// </summary>
        Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5);
    }

    /// <summary>
    /// Implementation of contract invoker
    /// </summary>
    public class ContractInvoker : IContractInvoker
    {
        private readonly ILogger<ContractInvoker> _logger;
        private readonly IBlockchainService _blockchain;
        private readonly IWalletService _wallet;

        public ContractInvoker(
            ILogger<ContractInvoker> logger,
            IBlockchainService blockchain,
            IWalletService wallet)
        {
            _logger = logger;
            _blockchain = blockchain;
            _wallet = wallet;
        }

        public async Task<T?> CallAsync<T>(UInt160 contractHash, string method, params object[] parameters)
        {
            try
            {
                var client = _blockchain.GetRpcClient();
                
                using var sb = new ScriptBuilder();
                // Build parameters array like in Neo.SmartContract.Testing
                ConvertArgs(sb, parameters);
                sb.EmitPush(CallFlags.All);
                sb.EmitPush(method);
                sb.EmitPush(contractHash);
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

        public async Task<UInt256> SendAsync(UInt160 contractHash, string method, params object[] parameters)
        {
            try
            {
                var signer = _wallet.GetDefaultAccount();
                
                using var sb = new ScriptBuilder();
                // Build parameters array like in Neo.SmartContract.Testing
                ConvertArgs(sb, parameters);
                sb.EmitPush(CallFlags.All);
                sb.EmitPush(method);
                sb.EmitPush(contractHash);
                sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
                var script = sb.ToArray();

                var result = await _blockchain.TestInvokeAsync(script, new Signer { Account = signer.ScriptHash, Scopes = WitnessScope.CalledByEntry });
                
                if (result.State != VMState.HALT)
                {
                    throw new Exception($"Script execution failed: {result.Exception}");
                }

                var tx = await _wallet.CreateTransactionAsync(script, signer.ScriptHash, result.GasConsumed);
                await _blockchain.SendTransactionAsync(tx);

                _logger.LogInformation("Transaction sent: {TxId}", tx.Hash);
                return tx.Hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send {Method} to {Contract}", method, contractHash);
                throw;
            }
        }

        public async Task<bool> WaitForConfirmationAsync(UInt256 txHash, int maxRetries = 30, int delaySeconds = 5)
        {
            var client = _blockchain.GetRpcClient();
            
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var tx = await client.GetRawTransactionAsync(txHash.ToString());
                    if (tx != null)
                    {
                        _logger.LogInformation("Transaction {TxId} confirmed in block {Block}", txHash, tx.BlockHash);
                        return true;
                    }
                }
                catch { }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }

            _logger.LogWarning("Transaction {TxId} not confirmed after {Retries} retries", txHash, maxRetries);
            return false;
        }

        private T? ConvertStackItem<T>(StackItem item)
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

        private static void ConvertArgs(ScriptBuilder script, object[] args)
        {
            if (args is null || args.Length == 0)
                script.Emit(OpCode.NEWARRAY0);
            else
            {
                for (int i = args.Length - 1; i >= 0; i--)
                {
                    script.EmitPush(args[i]);
                }
                script.EmitPush(args.Length);
                script.Emit(OpCode.PACK);
            }
        }
    }
}