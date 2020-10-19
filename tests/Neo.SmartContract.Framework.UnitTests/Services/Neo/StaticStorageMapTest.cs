using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.TestingEngine;
using Neo.Ledger;
using Neo.VM;
using System.Linq;

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
            var snapshot = Blockchain.Singleton.GetSnapshot();

            _engine = new TestEngine(snapshot: snapshot.Clone());
            _engine.AddEntryScript("./TestClasses/Contract_StaticStorageMap.cs");
            Assert.AreEqual(ContractFeatures.HasStorage, _engine.ScriptEntry.converterIL.outModule.attributes
                .Where(u => u.AttributeType.Name == "FeaturesAttribute")
                .Select(u => (ContractFeatures)u.ConstructorArguments.FirstOrDefault().Value)
                .FirstOrDefault());

            _engine.Snapshot.Contracts.Add(_engine.EntryScriptHash, new ContractState()
            {
                Script = _engine.EntryContext.Script,
                Manifest = new Manifest.ContractManifest()
                {
                    Features = Manifest.ContractFeatures.HasStorage
                }
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
