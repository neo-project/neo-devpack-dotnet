using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Optimizer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class WriteInTryAnalyzeTryCatchTests : DebugAndTestBase<Contract_TryCatch>
    {
        [TestMethod]
        public void Test_WriteInTryAnalyzeTryCatch()
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

    [TestClass]
    public class WriteInTryTests : DebugAndTestBase<Contract_WriteInTry>
    {
        [TestMethod]
        public void Test_WriteInTry()
        {
            ContractInBasicBlocks contractInBasicBlocks = new(NefFile, Manifest);
            TryCatchFinallyCoverage tryCatchFinallyCoverage = new(contractInBasicBlocks);
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 9);

            WriteInTryAnalzyer.WriteInTryVulnerability v =
                WriteInTryAnalzyer.AnalyzeWriteInTry(NefFile, Manifest);
            // because most try throws or aborts in catch, or has no catch
            Assert.AreEqual(v.vulnerabilities.Count, 1);
            v.GetWarningInfo(print: false);
        }
    }
}
