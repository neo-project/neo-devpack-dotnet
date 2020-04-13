using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_ByteArrayAssignment
    {
        [TestMethod]
        public void Test_ByteArrayAssignment()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArrayAssignment.cs");

            var result = testengine.GetMethod("testAssignment").Run();
            StackItem wantresult = new byte[] { 0x01, 0x02, 0x04 };

            Assert.AreEqual(wantresult.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOutOfBounds()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArrayAssignment.cs");

            testengine.ExecuteTestCaseStandard("testAssignmentOutOfBounds");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflow()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArrayAssignment.cs");

            testengine.ExecuteTestCaseStandard("testAssignmentOverflow");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflowUncheked()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArrayAssignment.cs");

            var result = testengine.GetMethod("testAssignmentOverflowUncheked").Run();
            StackItem wantresult = new byte[] { 0xFF, 0x02, 0x03 };

            Assert.AreEqual(wantresult.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentWrongCasting()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArrayAssignment.cs");

            testengine.ExecuteTestCaseStandard("testAssignmentWrongCasting");
            Assert.AreEqual(VM.VMState.FAULT, testengine.State);
        }
    }
}
