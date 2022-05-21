using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using System;

namespace Neo.Compiler.CSharp.UnitTests.Utils
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
            persistingBlock ??= new NeoSystem(ProtocolSettings.Default).GenesisBlock;

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
    }
}
