using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ByteArrayAssignment
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            Assert.IsTrue(_engine.AddEntryScript<Contract_ByteArrayAssignment>().Success);
        }

        [TestMethod]
        public void Test_ByteArrayAssignment()
        {
            var result = _engine.ExecuteTestCaseStandard("testAssignment").Pop();
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x04 }, result.GetSpan().ToArray());
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOutOfBounds()
        {
            _engine.ExecuteTestCaseStandard("testAssignmentOutOfBounds");
            Assert.AreEqual(VM.VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentOverflow()
        {
            var result = _engine.ExecuteTestCaseStandard("testAssignmentOverflow").Pop();
            CollectionAssert.AreEqual(new byte[] { 0xff, 0x02, 0x03 }, result.GetSpan().ToArray());
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentWrongCasting()
        {
            _engine.ExecuteTestCaseStandard("testAssignmentWrongCasting");
            Assert.AreEqual(VM.VMState.FAULT, _engine.State);
        }

        [TestMethod]
        public void Test_ByteArrayAssignmentDynamic()
        {
            var result = _engine.ExecuteTestCaseStandard("testAssignmentDynamic", 10);
            Assert.AreEqual(VM.VMState.HALT, _engine.State);
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x0a }, result.Pop().GetSpan().ToArray());
        }
    }
}
