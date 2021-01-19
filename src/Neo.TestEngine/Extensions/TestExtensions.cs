using Neo.TestingEngine;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.TestingEngine
{
    public static class TestExtensions
    {
        public static void ContractAdd(this StoreView snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            snapshot.Storages.Add(key, new StorageItem(contract));
        }
        public static bool ContainsContract(this StoreView snapshot, UInt160 hash)
        {
            var key = new KeyBuilder(0, 8).Add(hash);
            return snapshot.Storages.Contains(key);
        }

        public static void DeleteContract(this StoreView snapshot, UInt160 hash)
        {
            var key = new KeyBuilder(0, 8).Add(hash);
            snapshot.Storages.Delete(key);
        }

        public static void DeployNativeContracts(this StoreView snapshot, Block persistingBlock = null)
        {
            persistingBlock ??= new Block() { Index = 0 };
            var method = typeof(SmartContract.Native.ContractManagement).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            method.Invoke(SmartContract.Native.NativeContract.ContractManagement, new object[] { engine });
        }
        public static bool TryContractAdd(this StoreView snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            if (snapshot.Storages.Contains(key))
            {
                return false;
            }

            snapshot.Storages.Add(key, new StorageItem(contract));
            return true;
        }
    }
}
