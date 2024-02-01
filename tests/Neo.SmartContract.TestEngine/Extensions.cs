using System.Text;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.VM;
using Neo.VM.Types;
using Array = System.Array;

namespace Neo.SmartContract.TestEngine
{
    public static class Extensions
    {
        public static void ContractAdd(this DataCache snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            snapshot.Add(key, new StorageItem(contract));
        }

        public static void DeployNativeContracts(this DataCache snapshot, Block persistingBlock = null)
        {
            persistingBlock ??= new NeoSystem(TestProtocolSettings.Default).GenesisBlock;

            var method = typeof(SmartContract.Native.ContractManagement).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(Array.Empty<byte>());
            method.Invoke(SmartContract.Native.NativeContract.ContractManagement, new object[] { engine });
            engine.Snapshot.Commit();

            method = typeof(SmartContract.Native.LedgerContract).GetMethod("PostPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(Array.Empty<byte>());
            method.Invoke(SmartContract.Native.NativeContract.Ledger, new object[] { engine });
            engine.Snapshot.Commit();
        }

        public static bool TryGetString(this byte[] byteArray, out string? value)
        {
            try
            {
                value = Utility.StrictUTF8.GetString(byteArray);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static string Print(this EvaluationStack stack)
        {
            return $"[{string.Join(", ", stack.Select(p =>
                {
                    var value = p.Type switch
                    {
                        StackItemType.Pointer => $"({((Pointer)p).Position})",
                        StackItemType.Boolean => $"({p.GetBoolean()})",
                        StackItemType.Integer => $"({p.GetInteger()})",
                        // If the bytestring is not a valid UTF-8 string, we'll just print the base64 representation
                        StackItemType.ByteString => p.GetSpan().ToArray().TryGetString(out var str) ? $"(\"{str}\")" : $"(\"Base64: {Convert.ToBase64String(p.GetSpan())}\")",
                        StackItemType.Array
                            or StackItemType.Map
                            or StackItemType.Struct => $"({((CompoundType)p).Count})",
                        _ => ""
                    };
                    return $"{p.Type.ToString()}{value}";
                }
            ))}]";
        }
    }
}
