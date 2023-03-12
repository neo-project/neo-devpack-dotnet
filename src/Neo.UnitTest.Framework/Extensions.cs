// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Reflection;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;

namespace Neo.UnitTest.Framework;

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

        var method = typeof(ContractManagement).GetMethod("OnPersist", BindingFlags.NonPublic | BindingFlags.Instance);
        var engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
        engine.LoadScript(Array.Empty<byte>());
        method.Invoke(NativeContract.ContractManagement, new object[] { engine });
        engine.Snapshot.Commit();

        method = typeof(LedgerContract).GetMethod("PostPersist", BindingFlags.NonPublic | BindingFlags.Instance);
        engine = new TestEngine(TriggerType.OnPersist, null, snapshot, persistingBlock);
        engine.LoadScript(Array.Empty<byte>());
        method.Invoke(NativeContract.Ledger, new object[] { engine });
        engine.Snapshot.Commit();
    }
}
