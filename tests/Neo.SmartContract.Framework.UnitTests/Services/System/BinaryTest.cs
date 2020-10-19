using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.TestingEngine;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.SmartContract.Framework.UnitTests;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.SmartContractFramework.Services.System
{
    [TestClass]
    public class BinaryTest
    {
        private TestEngine _engine;
        private UInt160 scriptHash;

        [TestInitialize]
        public void Init()
        {
            var _ = TestBlockchain.TheNeoSystem;
            var snapshot = Blockchain.Singleton.GetSnapshot().Clone();
            snapshot.BlockHashIndex.Get().Index = 1234;

            _engine = new TestEngine(TriggerType.Application, snapshot: snapshot);
            _engine.AddEntryScript("./TestClasses/Contract_Binary.cs");
            scriptHash = _engine.ScriptEntry.finalNEF.ToScriptHash();

            snapshot.Contracts.Add(scriptHash, new ContractState()
            {
                Script = _engine.ScriptEntry.finalNEF,
                Manifest = ContractManifest.Parse(_engine.ScriptEntry.finalManifest)
            });
        }

        [TestMethod]
        public void base64DecodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base64Decode", "dGVzdA==");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<ByteString>();
            Assert.AreEqual("test", item.GetString());
        }

        [TestMethod]
        public void base64EncodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base64Encode", "test");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<ByteString>();
            Assert.AreEqual("dGVzdA==", item.GetString());
        }

        [TestMethod]
        public void base58DecodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base58Decode", "3yZe7d");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<ByteString>();
            Assert.AreEqual("test", item.GetString());
        }

        [TestMethod]
        public void base58EncodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base58Encode", "test");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<ByteString>();
            Assert.AreEqual("3yZe7d", item.GetString());
        }
    }
}
