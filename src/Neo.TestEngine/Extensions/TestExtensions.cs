// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Linq;
using Array = Neo.VM.Types.Array;

namespace Neo.TestingEngine
{
    public static class TestExtensions
    {
        private const byte Prefix_NextAvailableId = 15;

        public static StackItem ToStackItem(this ContractParameter parameter, ReferenceCounter referenceCounter)
        {
            var stackItem = parameter.ToStackItem();
            return SetReferenceCounter(stackItem, referenceCounter);
        }

        private static StackItem SetReferenceCounter(StackItem stackItem, ReferenceCounter referenceCounter)
        {
            if (stackItem is CompoundType)
            {
                if (stackItem is Map map)
                {
                    var newStackItem = new Map(referenceCounter);
                    foreach (var (key, value) in map)
                    {
                        newStackItem[key] = value;
                    }
                    stackItem = newStackItem;
                }
                else if (stackItem is Array array)
                {
                    stackItem = new Array(referenceCounter, array.SubItems);
                }
                else if (stackItem is Struct stackStruct)
                {
                    stackItem = new Struct(referenceCounter, stackStruct);
                }
            }

            return stackItem;
        }

        public static void ContractAdd(this DataCache snapshot, ContractState contract)
        {
            var key = new KeyBuilder(-1, 8).Add(contract.Hash);
            snapshot.Add(key, new StorageItem(contract));
            snapshot.InnerCommit();
        }

        public static bool ContainsContract(this DataCache snapshot, UInt160 hash)
        {
            return NativeContract.ContractManagement.GetContract(snapshot, hash) != null;
        }

        public static void DeleteContract(this DataCache snapshot, UInt160 hash)
        {
            var contract = NativeContract.ContractManagement.GetContract(snapshot, hash);
            if (contract != null)
            {
                var key = new KeyBuilder(NativeContract.ContractManagement.Id, 8).Add(hash);
                snapshot.Delete(key);
            }
            snapshot.InnerCommit();
        }

        public static void DeployNativeContracts(this DataCache snapshot, Block? persistingBlock = null)
        {
            persistingBlock ??= new NeoSystem(ProtocolSettings.Default).GenesisBlock;

            var method = typeof(ContractManagement).GetMethod("OnPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(System.Array.Empty<byte>());
            method.Invoke(NativeContract.ContractManagement, new object[] { engine });
            engine.Snapshot.InnerCommit();

            var method2 = typeof(LedgerContract).GetMethod("PostPersist", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
            engine.LoadScript(System.Array.Empty<byte>());
            method2.Invoke(NativeContract.Ledger, new object[] { engine });
            engine.Snapshot.InnerCommit();
        }

        public static bool TryContractAdd(this DataCache snapshot, ContractState contract)
        {
            var key = NativeContract.ContractManagement.CreateStorageKey(8, contract.Hash);
            if (snapshot.Contains(key))
            {
                return false;
            }

            snapshot.Add(key, new StorageItem(contract));
            snapshot.InnerCommit();
            return true;
        }
        public static int GetNextAvailableId(this DataCache snapshot)
        {
            StorageItem item = snapshot.GetAndChange(NativeContract.ContractManagement.CreateStorageKey(Prefix_NextAvailableId));
            item.Add(1);
            return NativeContract.ContractManagement.ListContracts(snapshot).ToList().Where(state => state.Id >= 0).Count();
        }
    }
}
