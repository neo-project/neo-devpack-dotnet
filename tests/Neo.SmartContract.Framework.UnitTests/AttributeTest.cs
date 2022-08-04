using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.TestingEngine;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using System;
using System.IO;

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
            var verificable = new DummyVerificable(new UInt160(new byte[20]));
            var snapshot = _system.GetSnapshot().CreateSnapshot();

            using var testengine = new TestEngine(TriggerType.Application, verificable, snapshot: snapshot);
            Assert.IsTrue(testengine.AddEntryScript("./TestClasses/Contract_Attribute.cs").Success);

            var result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsTrue(item.GetBoolean());

            testengine.Reset();
            verificable.Hashes = Array.Empty<UInt160>();

            result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
            Assert.AreEqual(0, result.Count);
            testengine.Dispose();
        }
    }
}
