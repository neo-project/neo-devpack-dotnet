using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CompoundAssignments
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_ComplexAssign.cs");
        }

        [TestMethod]
        public void Test_AddAssign_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Add_Assign_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_AddAssign_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Add_Assign_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            // Asserting the expected values after overflow
            var values = (Array)result.Pop();
            Assert.AreEqual(0, values[0].GetInteger()); // uint.MaxValue + 1 overflows to 0
            Assert.AreEqual(unchecked(int.MaxValue + 1), values[1].GetInteger()); // int.MaxValue + 1 overflows to int.MinValue
        }

        [TestMethod]
        public void Test_SubAssign_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Sub_Assign_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_SubAssign_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Sub_Assign_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            // Asserting the expected values after underflow
            var values = (Array)result.Pop();
            Assert.AreEqual(uint.MaxValue, values[0].GetInteger()); // uint.MinValue - 1 underflows to uint.MaxValue
            Assert.AreEqual(unchecked(int.MinValue - 1), values[1].GetInteger()); // int.MinValue - 1 underflows to int.MaxValue
        }

        [TestMethod]
        public void Test_MulAssign_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Mul_Assign_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_MulAssign_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Mul_Assign_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            var values = (Array)result.Pop();
            Assert.AreEqual(unchecked(uint.MaxValue * 2), values[0].GetInteger()); // Multiplying by 2 should not change the value
            Assert.AreEqual(unchecked(int.MaxValue * 2), values[1].GetInteger()); // Same here
        }

        [TestMethod]
        public void Test_LeftShiftAssign_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Left_Shift_Assign_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_LeftShiftAssign_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Left_Shift_Assign_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            var values = (Array)result.Pop();
            Assert.AreEqual(unchecked(uint.MaxValue << 1), values[0].GetInteger());
            Assert.AreEqual(unchecked(int.MaxValue << 1), values[1].GetInteger());

        }

        [TestMethod]
        public void Test_RightShiftAssign_Checked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Right_Shift_Assign_Checked");
            Assert.AreEqual(VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_RightShiftAssign_UnChecked()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("unitTest_Right_Shift_Assign_UnChecked");
            Assert.AreEqual(VMState.HALT, _engine.State);
            var values = (Array)result.Pop();
            Assert.AreEqual(unchecked(uint.MinValue >> 1), values[0].GetInteger());
            Assert.AreEqual(unchecked(int.MinValue >> 1), values[1].GetInteger());
        }


    }
}
