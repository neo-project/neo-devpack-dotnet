// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_MissingCheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.SecurityAnalyzer;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class MissingCheckWitnessTests : DebugAndTestBase<Contract_MissingCheckWitness>
    {
        [TestMethod]
        public void Test_MissingCheckWitness()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            // UnsafeUpdate writes storage without CheckWitness - should be flagged
            Assert.IsTrue(result.vulnerableMethodNames.Contains("unsafeUpdate"));
            // SafeUpdate has CheckWitness - should NOT be flagged
            Assert.IsFalse(result.vulnerableMethodNames.Contains("safeUpdate"));
        }

        [TestMethod]
        public void Test_MissingCheckWitness_WarningInfo()
        {
            var result = MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(NefFile, Manifest, null);
            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SEC]"));
            Assert.IsTrue(warning.Contains("unsafeUpdate"));
        }
    }
}
