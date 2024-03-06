using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM;

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
            testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_PropertyMethod.cs");
        }

        [TestMethod]
        public void TestPropertyMethod()
        {
            testEngine.Reset();
            var res = testEngine.ExecuteTestCaseStandard("testProperty");
            Assert.AreEqual(testEngine.State, VMState.HALT);
            Assert.AreEqual(res.Pop().GetString(), "NEO3");
        }
    }
}
