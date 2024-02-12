using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
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
            _engine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Inc_Dec.cs");
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
            Assert.AreEqual(unchecked(uint.MaxValue + 2), result.Pop().GetInteger());
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
            Assert.AreEqual(unchecked(uint.MinValue - 2), result.Pop().GetInteger());
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
            Assert.AreEqual(unchecked(uint.MaxValue + 2), result.Pop().GetInteger());
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
            Assert.AreEqual(unchecked(uint.MinValue - 2), result.Pop().GetInteger());
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
            Assert.AreEqual(unchecked(uint.MaxValue + 2), result.Pop().GetInteger());
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
            Assert.AreEqual(unchecked(uint.MinValue - 2), result.Pop().GetInteger());
        }

        // Test Methods for int type
        [TestMethod]
        public void Test_IntProperty_Inc_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Inc_Checked_Int");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_IntProperty_Inc_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Inc_UnChecked_Int");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MaxValue + 2), result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_IntProperty_Dec_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Dec_Checked_Int");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_IntProperty_Dec_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Property_Dec_UnChecked_Int");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MinValue - 2), result.Pop().GetInteger());
        }

        // Local Variable Tests for int
        [TestMethod]
        public void Test_Local_Inc_Checked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Inc_Checked_Int");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Local_Inc_UnChecked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Inc_UnChecked_Int");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MaxValue + 2), result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Local_Dec_Checked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Dec_Checked_Int");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Local_Dec_UnChecked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Local_Dec_UnChecked_Int");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MinValue - 2), result.Pop().GetInteger());
        }

        // Parameter Tests for int
        [TestMethod]
        public void Test_Param_Inc_Checked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Inc_Checked_Int", 0);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Param_Inc_UnChecked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Inc_UnChecked_Int", 0);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MaxValue + 2), result.Pop().GetInteger());
        }

        [TestMethod]
        public void Test_Param_Dec_Checked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Dec_Checked_Int", 0);
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_Param_Dec_UnChecked_Int()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Param_Dec_UnChecked_Int", 0);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(unchecked(int.MinValue - 2), result.Pop().GetInteger());
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
