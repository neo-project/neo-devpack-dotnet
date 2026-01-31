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
using CompilerSecurityAnalyzer = Neo.Compiler.SecurityAnalyzer.SecurityAnalyzer;
using Neo.SmartContract.Testing;
using System;
using System.IO;

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

        [TestMethod]
        public void Test_SecurityAnalyzer_Prints_Symbolic_Warnings()
        {
            var context = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_SymbolicSecurity));
            string output = CaptureConsole(() => CompilerSecurityAnalyzer.AnalyzeWithPrint(
                context.CreateExecutable(),
                context.CreateManifest(),
                context.CreateDebugInformation()));

            Assert.IsTrue(output.Contains("Symbolic execution detected unguarded contract update", StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(output.Contains("storage writes reachable from Verify", StringComparison.OrdinalIgnoreCase));
        }

        private static string CaptureConsole(Action action)
        {
            var writer = new StringWriter();
            TextWriter original = Console.Out;
            try
            {
                Console.SetOut(writer);
                action();
            }
            finally
            {
                Console.SetOut(original);
            }
            return writer.ToString();
        }
    }
}
