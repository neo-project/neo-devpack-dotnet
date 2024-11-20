using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Optimizer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class WriteInTryTests : DebugAndTestBase<Contract_TryCatch>
    {
        [TestMethod]
        public void Test_WriteInTry()
        {
            ContractInBasicBlocks contractInBasicBlocks = new(NefFile, Manifest);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 21);

            WriteInTryAnalzyer.WriteInTryVulnerability v =
                WriteInTryAnalzyer.AnalyzeWriteInTry(NefFile, Manifest);
            Assert.AreEqual(v.vulnerabilities.Count, 0);
            v.GetWarningInfo(print: false);
        }
    }
}
