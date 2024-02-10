using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Optimize
    {
        [TestMethod]
        public void Test_Optimize()
        {
            TestEngine testEngine = new();
            TestEngine testEngine2 = new();
            testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Optimize.cs");
            testEngine2.AddNoOptimizeEntryScript(Utils.Extensions.TestContractRoot + "Contract_Optimize.cs");

            var result = testEngine.ExecuteTestCaseStandard("unitTest_001");
            var result2 = testEngine2.ExecuteTestCaseStandard("unitTest_001");
            Assert.AreNotEqual(testEngine.Manifest.ToJson(), testEngine2.Manifest.ToJson());
            Assert.AreEqual(result.Count, result2.Count);
            Assert.IsTrue(result.Pop().GetSpan().SequenceEqual(result2.Pop().GetSpan()));
        }
    }
}
