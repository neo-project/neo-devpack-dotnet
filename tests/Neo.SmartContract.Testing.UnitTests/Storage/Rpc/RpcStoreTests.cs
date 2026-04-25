// Copyright (C) 2015-2026 The Neo Project.
//
// RpcStoreTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Storage;
using Neo.SmartContract.Testing.Storage.Rpc;
using System;

namespace Neo.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class RpcStoreTests
    {
        [TestMethod]
        public void RpcStoreMutationsExplainReadOnlyBehavior()
        {
            var store = new RpcStore("http://localhost:10332");

            var deleteException = Assert.ThrowsException<NotImplementedException>(() => store.Delete(new byte[] { 1 }));
            StringAssert.Contains(deleteException.Message, "read-only");

            var putException = Assert.ThrowsException<NotImplementedException>(() => store.Put(new byte[] { 1 }, new byte[] { 2 }));
            StringAssert.Contains(putException.Message, "read-only");
        }

        [TestMethod]
        public void RpcSnapshotCommitExplainsReadOnlyBehaviorWhenDirty()
        {
            var store = new RpcStore("http://localhost:10332");
            var snapshot = store.GetSnapshot();
            snapshot.Put(new byte[] { 1 }, new byte[] { 2 });

            var exception = Assert.ThrowsException<NotImplementedException>(() => snapshot.Commit());

            StringAssert.Contains(exception.Message, "read-only");
        }

        [TestMethod]
        public void TestRpcStore()
        {
            var engine = new TestEngine(new EngineStorage(new RpcStore("http://seed2t5.neo.org:20332")), false);

            // check network values

            Assert.AreEqual(100_000_000, engine.Native.NEO.TotalSupply);
            Assert.IsTrue(engine.Native.Ledger.CurrentIndex > 3_510_270);

            // check with Seek (RPC doesn't support Backward, it could be slow)

            Assert.IsTrue(engine.Native.NEO.GasPerBlock > 0, $"Unexpected GasPerBlock: {engine.Native.NEO.GasPerBlock}");

            // check contract state round-trip through RPC-backed storage

            var state = engine.Native.ContractManagement.GetContract(engine.Native.NEO.Hash);
            Assert.IsNotNull(state);
            Assert.AreEqual(engine.Native.NEO.Hash, state!.Hash);
            Assert.AreEqual("NeoToken", state.Manifest.Name);

            var roundTrip = engine.Native.ContractManagement.GetContractById(state.Id);
            Assert.IsNotNull(roundTrip);
            Assert.AreEqual(state.Hash, roundTrip!.Hash);

            Assert.IsTrue(engine.Native.ContractManagement.HasMethod(engine.Native.NEO.Hash, "getCandidateVote", 1));
        }
    }
}
