using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.Wallets;
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
            public void Deserialize(BinaryReader reader) { }
            public void DeserializeUnsigned(BinaryReader reader) { }
            public UInt160[] GetScriptHashesForVerifying(DataCache snapshot) => Hashes;
            public void Serialize(BinaryWriter writer) { }
            public void SerializeUnsigned(BinaryWriter writer) { }
        }

        [TestMethod]
        public void attribute_test()
        {
            var verificable = new DummyVerificable(new UInt160(new byte[20]));

            using var testengine = new TestEngine(TriggerType.Application, verificable);
            testengine.AddEntryScript("./TestClasses/Contract_Attribute.cs");

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
