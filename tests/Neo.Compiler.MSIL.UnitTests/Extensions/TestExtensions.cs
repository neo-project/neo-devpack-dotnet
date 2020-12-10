using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Ledger;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Compiler.MSIL.Extensions
{
    public static class TestExtensions
    {
        public static void ContractAdd(this StoreView snapshot, ContractState contract)
        {
            var key = new KeyBuilder(0, 8).Add(contract.Hash);
            snapshot.Storages.Add(key, new StorageItem(contract));
        }

        public static void DeployNativeContracts(this StoreView snapshot)
        {
            var method = typeof(SmartContract.Native.ManagementContract).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot);
            method.Invoke(SmartContract.Native.NativeContract.Management, new object[] { engine });
        }
    }
}
