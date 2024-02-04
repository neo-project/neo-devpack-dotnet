using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Params
    {
        [TestMethod]
        public void Test_Params()
        {
            TestEngine testengine = new();
            testengine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Params.cs");
            var result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(15, result.Pop());
        }
    }
}
