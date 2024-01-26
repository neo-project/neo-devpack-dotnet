using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestClasses;
using Neo.Compiler.CSharp.UnitTests.Utils;
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
            testengine.AddEntryScript(typeof(Contract_Params));
            var result = testengine.ExecuteTestCaseStandard("test");
            Assert.AreEqual(15, result.Pop());
        }
    }
}
