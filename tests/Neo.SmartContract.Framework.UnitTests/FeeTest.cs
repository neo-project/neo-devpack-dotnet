using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class FeeTest
    {
        private TestEngine.TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            var snapshot = new TestDataCache();

            _engine = new TestEngine.TestEngine(snapshot: snapshot);
            Assert.IsTrue(_engine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Fee.cs").Success);
        }

        [TestMethod]
        public void TestGAS()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testGAS", 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Integer>();
            Assert.AreEqual(1_000_000_000, item.GetInteger());
        }

        [TestMethod]
        public void TestSatoshi()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testSatoshi", 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Integer>();
            Assert.AreEqual(10, item.GetInteger());
        }

        [TestMethod]
        public void TestkSatoshi()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testkSatoshi", 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Integer>();
            Assert.AreEqual(10_000, item.GetInteger());
        }

        [TestMethod]
        public void TestmSatoshi()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testmSatoshi", 10);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop<VM.Types.Integer>();
            Assert.AreEqual(10_000_000, item.GetInteger());
        }
    }
}
