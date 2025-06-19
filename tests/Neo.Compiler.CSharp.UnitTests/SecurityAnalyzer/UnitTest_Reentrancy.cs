// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Reentrancy.cs file belongs to the neo project and is free
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

        [TestMethod]
        public void Test_ReentrancyWithEnhancedDiagnostics()
        {
            // Test enhanced diagnostic messages without debug info (fallback behavior)
            ReEntrancyAnalyzer.ReEntrancyVulnerabilityPair v =
                ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(NefFile, Manifest, null);
            Assert.AreEqual(v.vulnerabilityPairs.Count, 3);

            // Test that warning message contains enhanced diagnostic information
            string warningInfo = v.GetWarningInfo(print: false);

            // Verify enhanced diagnostic format
            Assert.IsTrue(warningInfo.Contains("[SEC] Potential Re-entrancy vulnerability detected"));
            Assert.IsTrue(warningInfo.Contains("External contract calls:"));
            Assert.IsTrue(warningInfo.Contains("Storage writes that occur after external calls:"));
            Assert.IsTrue(warningInfo.Contains("Recommendation:"));
            Assert.IsTrue(warningInfo.Contains("allowing potential re-entrancy attacks"));
            Assert.IsTrue(warningInfo.Contains("reentrancy guards"));

            // Message should be more detailed than just addresses
            Assert.IsTrue(warningInfo.Length > 300, "Enhanced diagnostic message should be more detailed than simple address listing");
        }
    }
}
