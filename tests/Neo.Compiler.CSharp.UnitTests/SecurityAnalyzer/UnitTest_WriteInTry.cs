// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_WriteInTry.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.Json;
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
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 22);

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
            Assert.AreEqual(tryCatchFinallyCoverage.allTry.Count, 14);

            WriteInTryAnalzyer.WriteInTryVulnerability v =
                WriteInTryAnalzyer.AnalyzeWriteInTry(NefFile, Manifest);
            // because most try throws or aborts in catch, or has no catch, or throws or aborts in finally
            Assert.AreEqual(v.vulnerabilities.Count, 2);
            v.GetWarningInfo(print: false);
        }

        [TestMethod]
        public void Test_WriteInTryWithEnhancedDiagnostics()
        {
            // Test enhanced diagnostic messages without debug info (fallback behavior)
            WriteInTryAnalzyer.WriteInTryVulnerability v =
                WriteInTryAnalzyer.AnalyzeWriteInTry(NefFile, Manifest, null);
            Assert.AreEqual(v.vulnerabilities.Count, 2);

            // Test that warning message contains enhanced diagnostic information
            string warningInfo = v.GetWarningInfo(print: false);

            // Verify enhanced diagnostic format
            Assert.IsTrue(warningInfo.Contains("[SEC] Writing storage in `try` block is risky"));
            Assert.IsTrue(warningInfo.Contains("Recommendation:"));
            Assert.IsTrue(warningInfo.Contains("writes may not be properly reverted on exceptions"));
            Assert.IsTrue(warningInfo.Contains("Try block addresses:"));
            Assert.IsTrue(warningInfo.Contains("Write instruction addresses:"));

            // Message should be more detailed than just addresses
            Assert.IsTrue(warningInfo.Length > 150, "Enhanced diagnostic message should be more detailed than simple address listing");
        }
    }
}
