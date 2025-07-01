using Microsoft.Extensions.Logging;
using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.VM;
using Neo.VM.Types;
using System.Text;

namespace Neo.SmartContract.Deploy.Utilities
{
    /// <summary>
    /// Helper utilities for debugging smart contracts
    /// </summary>
    public static class DebugHelper
    {
        /// <summary>
        /// Print detailed transaction information
        /// </summary>
        public static void PrintTransaction(ILogger logger, Transaction tx)
        {
            logger.LogInformation("=== Transaction Details ===");
            logger.LogInformation("Hash: {Hash}", tx.Hash);
            logger.LogInformation("System Fee: {Fee} GAS", DeploymentHelpers.FormatGasAmount(tx.SystemFee));
            logger.LogInformation("Network Fee: {Fee} GAS", DeploymentHelpers.FormatGasAmount(tx.NetworkFee));
            logger.LogInformation("Valid Until Block: {Block}", tx.ValidUntilBlock);
            logger.LogInformation("Script Length: {Length} bytes", tx.Script.Length);
            
            if (tx.Signers?.Length > 0)
            {
                logger.LogInformation("Signers:");
                foreach (var signer in tx.Signers)
                {
                    logger.LogInformation("  - Account: {Account}, Scope: {Scope}", 
                        signer.Account, signer.Scopes);
                }
            }
        }

        /// <summary>
        /// Print invoke result details
        /// </summary>
        public static void PrintInvokeResult(ILogger logger, RpcInvokeResult result)
        {
            logger.LogInformation("=== Invoke Result ===");
            logger.LogInformation("State: {State}", result.State);
            logger.LogInformation("GAS Consumed: {Gas}", DeploymentHelpers.FormatGasAmount(result.GasConsumed));
            
            if (!string.IsNullOrEmpty(result.Exception))
            {
                logger.LogError("Exception: {Exception}", result.Exception);
            }

            if (result.Stack?.Length > 0)
            {
                logger.LogInformation("Stack:");
                for (int i = 0; i < result.Stack.Length; i++)
                {
                    logger.LogInformation("  [{Index}] {Value}", i, FormatStackItem(result.Stack[i]));
                }
            }

            // Notifications are not available in RpcInvokeResult
            // They would be available in the transaction result after sending
        }

        /// <summary>
        /// Print contract storage
        /// </summary>
        public static async Task PrintStorageAsync(
            ILogger logger,
            IBlockchainService blockchain,
            UInt160 contractHash,
            params byte[][] keys)
        {
            logger.LogInformation("=== Contract Storage ===");
            logger.LogInformation("Contract: {Hash}", contractHash);

            var client = blockchain.GetRpcClient();

            foreach (var key in keys)
            {
                try
                {
                    var value = await client.GetStorageAsync(contractHash.ToString(), Convert.ToBase64String(key));
                    if (!string.IsNullOrEmpty(value))
                    {
                        var bytes = Convert.FromBase64String(value);
                        logger.LogInformation("  Key 0x{Key}: {Value} (hex: {Hex})", 
                            Convert.ToHexString(key), 
                            TryDecodeValue(bytes),
                            Convert.ToHexString(bytes));
                    }
                    else
                    {
                        logger.LogInformation("  Key 0x{Key}: <empty>", Convert.ToHexString(key));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("  Key 0x{Key}: Error reading - {Error}", 
                        Convert.ToHexString(key), ex.Message);
                }
            }
        }

        /// <summary>
        /// Create a debug checkpoint
        /// </summary>
        public static async Task<string?> CreateDebugCheckpointAsync(
            ILogger logger,
            INeoExpressService neoExpress,
            string name,
            string description)
        {
            try
            {
                var checkpointName = $"debug_{name}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                logger.LogInformation("Creating debug checkpoint: {Name} - {Description}", 
                    checkpointName, description);
                
                return await neoExpress.CreateCheckpointAsync(checkpointName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create debug checkpoint");
                return null;
            }
        }

        /// <summary>
        /// Format a stack item for display
        /// </summary>
        private static string FormatStackItem(StackItem item)
        {
            return item.Type switch
            {
                StackItemType.Boolean => item.GetBoolean().ToString(),
                StackItemType.Integer => item.GetInteger().ToString(),
                StackItemType.ByteString => TryDecodeValue(item.GetSpan().ToArray()),
                StackItemType.Array or StackItemType.Struct => 
                    $"[{string.Join(", ", ((VM.Types.Array)item).Select(FormatStackItem))}]",
                StackItemType.Map => "Map{...}",
                _ => $"{item.Type}({item})"
            };
        }

        /// <summary>
        /// Try to decode a byte array value
        /// </summary>
        private static string TryDecodeValue(byte[] bytes)
        {
            if (bytes.Length == 0) return "<empty>";
            
            // Try as UTF8 string
            try
            {
                var str = Encoding.UTF8.GetString(bytes);
                if (str.All(c => !char.IsControl(c) || char.IsWhiteSpace(c)))
                {
                    return $"\"{str}\"";
                }
            }
            catch { }

            // Try as integer
            if (bytes.Length <= 32)
            {
                try
                {
                    var integer = new System.Numerics.BigInteger(bytes);
                    return integer.ToString();
                }
                catch { }
            }

            // Try as address
            if (bytes.Length == 20)
            {
                return new UInt160(bytes).ToAddress(ProtocolSettings.Default.AddressVersion);
            }

            // Default to hex
            return $"0x{Convert.ToHexString(bytes)}";
        }
    }
}