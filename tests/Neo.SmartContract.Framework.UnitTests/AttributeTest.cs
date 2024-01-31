using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using System;
using System.IO;
using System.Linq;
using Neo.SmartContract.TestEngine;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class AttributeTest
    {
        class DummyVerificable : IVerifiable
        {
            internal UInt160[] Hashes;
            public Witness[] Witnesses { get; set; }
            public int Size => 0;

            public DummyVerificable(params UInt160[] hashes) { Hashes = hashes; }
            public void Deserialize(ref MemoryReader reader) { }
            public void DeserializeUnsigned(ref MemoryReader reader) { }
            public UInt160[] GetScriptHashesForVerifying(DataCache snapshot) => Hashes;
            public void Serialize(BinaryWriter writer) { }
            public void SerializeUnsigned(BinaryWriter writer) { }
        }

        private NeoSystem _system;

        [TestInitialize]
        public void Init()
        {
            _system = TestBlockchain.TheNeoSystem;
        }

        [TestMethod]
        public void attribute_test()
        {
            var verificable = new DummyVerificable(UInt160.Zero);
            var snapshot = _system.GetSnapshot().CreateSnapshot();

            using var testengine = new TestEngine.TestEngine(TriggerType.Application, verificable, snapshot: snapshot);
            Assert.IsTrue(testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Attribute.cs").Success);

            var result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            testengine.Reset();
            verificable.Hashes = Array.Empty<UInt160>();

            result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void reentrant_test()
        {
            var snapshot = _system.GetSnapshot().CreateSnapshot();
            using var testengine = new TestEngine.TestEngine(TriggerType.Application, snapshot: snapshot);

            Assert.IsTrue(testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Attribute.cs").Success);
            snapshot.ContractAdd(new ContractState()
            {
                Id = 123,
                Hash = testengine.EntryScriptHash,
                Nef = testengine.Nef,
                Manifest = testengine.Manifest
            });

            // return in the middle

            testengine.Reset();
            var before = snapshot.GetChangeSet().Count();
            var result = testengine.ExecuteTestCaseStandard("reentrantTest", 0);
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(before, snapshot.GetChangeSet().Count());

            // Method end

            testengine.Reset();
            before = snapshot.GetChangeSet().Count();
            result = testengine.ExecuteTestCaseStandard("reentrantTest", 1);
            Assert.AreEqual(VM.VMState.HALT, testengine.State);
            Assert.AreEqual(before, snapshot.GetChangeSet().Count());

            // Reentrant test

            testengine.Reset();
            before = snapshot.GetChangeSet().Count();
            result = testengine.ExecuteTestCaseStandard("reentrantTest", 123);
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(before + 1, snapshot.GetChangeSet().Count());
            Assert.IsTrue(testengine.FaultException.Message.Contains("Already entered"));
        }
    }
}
