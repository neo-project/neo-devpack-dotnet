extern alias scfx;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Extensions;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.Ledger;
using Neo.VM;

namespace Neo.SmartContract.Framework.UnitTests.Services.Neo
{
    [TestClass]
    public class StaticStorageMapTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var _ = TestBlockchain.TheNeoSystem;
            var snapshot = Blockchain.Singleton.GetSnapshot().Clone();

            _engine = new TestEngine(snapshot: snapshot);
            _engine.AddEntryScript("./TestClasses/Contract_StaticStorageMap.cs");
            snapshot.ContractAdd(new ContractState()
            {
                Hash = _engine.EntryScriptHash,
                Nef = _engine.ScriptEntry.nefFile,
                Manifest = new Manifest.ContractManifest()
            });
        }

        [TestMethod]
        public void Test_Storage()
        {
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("put2", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("get2", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(3, result.Pop());
        }

        [TestMethod]
        public void Test_StaticStorageMap()
        {
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("put", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("get", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop());

            _engine.Reset();
            _engine.ExecuteTestCaseStandard("putReadonly", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("getReadonly", "a");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(2, result.Pop());
        }
    }
}
