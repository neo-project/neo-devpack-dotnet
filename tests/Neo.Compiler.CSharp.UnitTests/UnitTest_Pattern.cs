using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Pattern
    {
        [TestMethod]
        public void Between_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Pattern.cs");

            var result = testengine.ExecuteTestCaseStandard("between", 50);
            var value = result.Pop().GetBoolean();
            Assert.AreEqual(true, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between", 1);

            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between", 100);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between", 200);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void Between2_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Pattern.cs");

            var result = testengine.ExecuteTestCaseStandard("between2", 50);
            var value = result.Pop().GetBoolean();
            Assert.AreEqual(true, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between2", 1);

            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between2", 100);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between2", 200);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void Between3_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Pattern.cs");

            var result = testengine.ExecuteTestCaseStandard("between3", 50);
            var value = result.Pop().GetBoolean();
            Assert.AreEqual(true, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between3", 1);

            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between3", 100);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("between3", 200);
            value = result.Pop().GetBoolean();
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestTypePattern_Test()
        {
            using var testengine = new TestEngine();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Pattern.cs");

            var result = testengine.ExecuteTestCaseStandard("testTypePattern", 1);
            var value = result.Pop().GetInteger();
            Assert.AreEqual(4, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testTypePattern", "1");

            value = result.Pop().GetInteger();
            Assert.AreEqual(1, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testTypePattern", new ByteString(new byte[] { 1 }));
            value = result.Pop().GetInteger();
            Assert.AreEqual(1, value);

            testengine.Reset();
            result = testengine.ExecuteTestCaseStandard("testTypePattern", new byte[] { 1 });
            value = result.Pop().GetInteger();
            Assert.AreEqual(1, value);
        }
    }
}
