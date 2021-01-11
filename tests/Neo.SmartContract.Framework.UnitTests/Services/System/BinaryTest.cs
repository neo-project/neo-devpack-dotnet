using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.SmartContract.Framework.UnitTests;
using Neo.SmartContract.Manifest;
using Neo.VM;

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
            scriptHash = _engine.ScriptEntry.finalNEFScript.ToScriptHash();

            snapshot.ContractAdd(new ContractState()
            {
                Hash = scriptHash,
                Nef = _engine.ScriptEntry.nefFile,
                Manifest = ContractManifest.Parse(_engine.ScriptEntry.finalManifest)
            });
        }

        [TestMethod]
        public void atoiTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("atoi", "-1", 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Integer>();
            Assert.AreEqual(-1, item.GetInteger());
        }

        [TestMethod]
        public void itoaTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("itoa", -1, 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.ByteString>();
            Assert.AreEqual("-1", item.GetString());
        }

        [TestMethod]
        public void base64DecodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base64Decode", "dGVzdA==");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Buffer>();
            Assert.AreEqual("test", item.GetString());
        }

        [TestMethod]
        public void base64EncodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base64Encode", "test");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.ByteString>();
            Assert.AreEqual("dGVzdA==", item.GetString());
        }

        [TestMethod]
        public void base58DecodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base58Decode", "3yZe7d");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Buffer>();
            Assert.AreEqual("test", item.GetString());
        }

        [TestMethod]
        public void base58EncodeTest()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("base58Encode", "test");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.ByteString>();
            Assert.AreEqual("3yZe7d", item.GetString());
        }
    }
}
