using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property_Method
    {
        private TestEngine testEngine;

        [TestInitialize]
        public void Init()
        {
            testEngine = new TestEngine();
            testEngine.AddNoOptimizeEntryScript(Utils.Extensions.TestContractRoot + "Contract_PropertyMethod.cs");
        }

        [TestMethod]
        public void TestPropertyMethod()
        {
            testEngine.Reset();
            var res = testEngine.ExecuteTestCaseStandard("testProperty");
            Assert.AreEqual(testEngine.State, VMState.HALT);
            var arr = (Array)res.Pop();
            Assert.AreEqual(arr[0].GetString(), "NEO3");
            Assert.AreEqual(arr[1].GetInteger(), 10);
        }

        [TestMethod]
        public void TestPropertyMethod2()
        {
            testEngine.Reset();
            var res = testEngine.ExecuteTestCaseStandard("testProperty2");
            Assert.AreEqual(testEngine.State, VMState.HALT);
        }
    }
}
