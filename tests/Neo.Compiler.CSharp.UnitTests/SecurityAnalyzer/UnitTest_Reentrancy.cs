using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Optimizer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class ReentrancyTests : DebugAndTestBase<Contract_Reentrancy>
    {
        [TestMethod]
        public void Test_HasReentrancy()
        {
            ReEntrancyAnalyzer.ReEntrancyVulnerabilityPair v =
                ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(NefFile, Manifest);
            Assert.AreEqual(v.vulnerabilityPairs.Count, 3);
            foreach (BasicBlock b in v.vulnerabilityPairs.Keys)
                // basic blocks calling contract
                Assert.IsTrue(b.startAddr < NefFile.Size * 0.66);
            v.GetWarningInfo(print: false);
        }
    }
}
