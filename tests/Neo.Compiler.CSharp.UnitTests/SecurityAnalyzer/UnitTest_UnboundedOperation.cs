// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UnboundedOperation.cs file belongs to the neo project and is free
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
    public class UnboundedOperationTests : DebugAndTestBase<Contract_UnboundedOperation>
    {
        [TestMethod]
        public void Test_UnboundedOperation()
        {
            var result = UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(NefFile, Manifest, null);
            // The for loop in Sum should produce at least one backward jump
            Assert.IsTrue(result.backwardJumpAddresses.Count > 0);
        }

        [TestMethod]
        public void Test_UnboundedOperation_WarningInfo()
        {
            var result = UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(NefFile, Manifest, null);
            string warning = result.GetWarningInfo(print: false);
            Assert.IsTrue(warning.Contains("[SEC]"));
            Assert.IsTrue(warning.Contains("backward jump"));
        }
    }
}
