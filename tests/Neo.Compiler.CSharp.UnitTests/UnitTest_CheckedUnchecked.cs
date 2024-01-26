using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CheckedUnchecked
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            Assert.IsTrue(_engine.AddEntryScript(typeof(Contract_CheckedUnchecked)).Success);
        }

        [TestMethod]
        public void TestAddChecked()
        {
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("addChecked", int.MaxValue, 1);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("addUnchecked", int.MaxValue, 1);
            Assert.AreEqual(int.MinValue, result.Peek().GetInteger());
        }

        [TestMethod]
        public void TestCastChecked()
        {
            _engine.Reset();
            _engine.ExecuteTestCaseStandard("castChecked", -1);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("castUnchecked", -1);
            Assert.AreEqual(uint.MaxValue, result.Peek().GetInteger());
        }
    }
}
