using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Compiler.MSIL.Extensions
{
    public static class TestExtensions
    {
        public static void ContractAdd(this StoreView snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            snapshot.Storages.Add(key, new StorageItem(contract));
        }

        public static void DeployNativeContracts(this StoreView snapshot, Block persistingBlock = null)
        {
            persistingBlock ??= new Block() { Index = 0 };
            var method = typeof(SmartContract.Native.ContractManagement).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            method.Invoke(SmartContract.Native.NativeContract.ContractManagement, new object[] { engine });
        }
    }
}
