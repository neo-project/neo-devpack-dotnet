using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Partial()
        {
            var testengine = new TestEngine();
            var context = testengine.AddEntryScript(typeof(Contract_Partial));
            Assert.IsTrue(context.Success);
            var result = testengine.ExecuteTestCaseStandard("test1").Pop();
            Assert.AreEqual(1, result.GetInteger());
            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("test2").Pop();
            Assert.AreEqual(2, result.GetInteger());
        }

        [TestMethod]
        public void Test_MultipleContracts()
        {
            var testengine = new TestEngine();
            var context = testengine.AddEntryScript(typeof(Contract_Multiple));
            Assert.IsFalse(context.Success);
            Assert.IsTrue(context.Diagnostics.Any(u => u.Id == DiagnosticId.MultiplyContracts));
        }

        [TestMethod]
        public void Test_InvalidNameMethodContracts()
        {
            var testengine = new TestEngine();
            var context = testengine.AddEntryScript(typeof(Contract_InvalidName));
            Assert.IsFalse(context.Success);
            Assert.IsTrue(context.Diagnostics.Any(u => u.Id == DiagnosticId.InvalidMethodName));
        }

        [TestMethod]
        public void Test_DuplicateDisplayNames()
        {
            var testengine = new TestEngine();
            var context = testengine.AddEntryScript(typeof(Contract_DuplicateNames));
            Assert.IsFalse(context.Success);
            Assert.IsTrue(context.Diagnostics.Any(u => u.Id == DiagnosticId.EventNameConflict));
        }

        [TestMethod]
        public void Test_PrivateMethod()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            Assert.IsTrue(Encoding.ASCII.GetString(testengine.Nef.Script.Span).Contains("NEO3"));
        }

        [TestMethod]
        public void Test_ByteArray_New()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("unitTest_001").Pop();
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, result.GetSpan().ToArray());
        }

        [TestMethod]
        public void Test_testArgs1()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("testArgs1", 4).Pop();
            Assert.AreEqual(new byte[] { 1, 2, 3, 4 }.ToHexString(), result.GetSpan().ToHexString());
        }

        [TestMethod]
        public void Test_testArgs2()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("testArgs2", new byte[] { 1, 2, 3 }).Pop();
            Assert.AreEqual(new byte[] { 1, 2, 3 }.ToHexString(), result.GetSpan().ToHexString());
        }

        [TestMethod]
        public void Test_testArgs3()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("testArgs3", 1, 2);
            Assert.AreEqual(testengine.State, VM.VMState.HALT);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_testArgs4()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("testArgs4", 1, 2).Pop();
            Assert.AreEqual(testengine.State, VM.VMState.HALT);
            Assert.AreEqual(5, result.GetInteger());
        }

        [TestMethod]
        public void Test_testVoid()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract1));
            var result = testengine.ExecuteTestCaseStandard("testVoid");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_ByteArrayPick()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript(typeof(Contract2));

            var result = testengine.ExecuteTestCaseStandard("unitTest_002", "hello", 1);

            Assert.AreEqual(3, result.Pop().GetInteger());
        }
    }
}
