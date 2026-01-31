// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SymbolicSecurity.cs file belongs to the neo project and is free
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
    public class UnitTest_SymbolicSecurity
    {
        [TestMethod]
        public void Test_Unguarded_Update_Is_Detected()
        {
            var context = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_SymbolicSecurity));
            var warnings = SymbolicExecutionAnalyzer.Analyze(context.CreateExecutable(), context.CreateManifest(), context.CreateDebugInformation());
            Assert.IsTrue(warnings.HasUnguardedUpdate);
        }

        [TestMethod]
        public void Test_Verify_Write_Is_Detected()
        {
            var context = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_SymbolicSecurity));
            var warnings = SymbolicExecutionAnalyzer.Analyze(context.CreateExecutable(), context.CreateManifest(), context.CreateDebugInformation());
            Assert.IsTrue(warnings.HasVerifyStorageWrite);
        }
    }
}
