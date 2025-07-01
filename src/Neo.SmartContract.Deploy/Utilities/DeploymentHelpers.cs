using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Numerics;
using Neo.IO;
using System.Linq;
using Neo.VM.Types;

namespace Neo.SmartContract.Deploy.Utilities
{
    /// <summary>
    /// Helper methods for deployment operations
    /// </summary>
    public static class DeploymentHelpers
    {
        /// <summary>
        /// Calculate the expected contract hash
        /// </summary>
        public static UInt160 CalculateContractHash(UInt160 sender, byte[] nefFile, ContractManifest manifest)
        {
            // NEF file has a header, we need to calculate checksum from the NEF structure
            var nef = NefFile.Parse(nefFile);
            return Helper.GetContractHash(sender, nef.CheckSum, manifest.Name);
        }

        /// <summary>
        /// Convert parameters to the correct types for contract invocation
        /// </summary>
        public static object[] ConvertParameters(params object?[] parameters)
        {
            var converted = new List<object>();
            
            foreach (var param in parameters)
            {
                if (param == null)
                {
                    converted.Add(Neo.VM.Types.StackItem.Null);
                }
                else if (param is string s)
                {
                    converted.Add(s);
                }
                else if (param is byte[] bytes)
                {
                    converted.Add(bytes);
                }
                else if (param is bool b)
                {
                    converted.Add(b);
                }
                else if (param is BigInteger bi)
                {
                    converted.Add(bi);
                }
                else if (param is int i)
                {
                    converted.Add(new BigInteger(i));
                }
                else if (param is long l)
                {
                    converted.Add(new BigInteger(l));
                }
                else if (param is UInt160 hash160)
                {
                    converted.Add(hash160);
                }
                else if (param is UInt256 hash256)
                {
                    converted.Add(hash256);
                }
                else if (param is IEnumerable<object> array)
                {
                    converted.Add(array.Select(item => ConvertParameters(item)[0]).ToArray());
                }
                else
                {
                    throw new ArgumentException($"Unsupported parameter type: {param.GetType()}");
                }
            }
            
            return converted.ToArray();
        }

        /// <summary>
        /// Format GAS amount for display
        /// </summary>
        public static string FormatGasAmount(long systemFee)
        {
            var gas = new BigDecimal((BigInteger)systemFee, 8);
            return $"{gas} GAS";
        }

        /// <summary>
        /// Get contract storage values
        /// </summary>
        public static async Task<Dictionary<string, object>> GetContractStorageAsync(
            IBlockchainService blockchain,
            UInt160 contractHash,
            params byte[] prefixes)
        {
            var storage = new Dictionary<string, object>();
            var client = blockchain.GetRpcClient();
            
            foreach (var prefix in prefixes)
            {
                try
                {
                    using var sb = new ScriptBuilder();
                    ConvertArgs(sb, new object[] { prefix });
                    sb.EmitPush(CallFlags.All);
                    sb.EmitPush("getStorage");
                    sb.EmitPush(contractHash);
                    sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
                    var script = sb.ToArray();
                    
                    var result = await client.InvokeScriptAsync(script);
                    if (result.State == VMState.HALT && result.Stack.Length > 0)
                    {
                        storage[$"0x{prefix:X2}"] = result.Stack[0];
                    }
                }
                catch { }
            }
            
            return storage;
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