using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Inc_Dec
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Inc_Dec.cs");
        }

        [TestMethod]
        public void Test_Property_Inc_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Inc_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Property_Inc_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Inc_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Property_Dec_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Dec_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Property_Dec_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Dec_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(uint.MaxValue - 1, result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Local_Inc_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Inc_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Inc_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Dec_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Dec_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(uint.MaxValue - 1, result.Pop().GetInteger());
        }
        [TestMethod]
        public void Test_Param_Inc_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Inc_Checked", 0);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Inc_UnChecked", 0);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Param_Dec_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Dec_Checked", 0);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Dec_UnChecked", 0);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(uint.MaxValue - 1, result.Pop().GetInteger());
        }
        [TestMethod]
        public void Test_Not_DeadLoop()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Not_DeadLoop");
            Assert.AreEqual(VMState.HALT, _engine.State);
        }
    }
}
