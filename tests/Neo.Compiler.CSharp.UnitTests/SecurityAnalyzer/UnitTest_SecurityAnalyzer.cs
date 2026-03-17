// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SecurityAnalyzer.cs file belongs to the neo project and is free
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
using System;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.SecurityAnalyzer
{
    [TestClass]
    public class SecurityAnalyzerTests
    {
        [TestMethod]
        public void Test_AnalyzeWithPrint_IncludesMissingCheckWitnessWarnings()
        {
            string output = CaptureAnalyzeWithPrintOutput(
                Contract_MissingCheckWitness.Nef,
                Contract_MissingCheckWitness.Manifest);

            Assert.IsTrue(output.Contains("[SECURITY]"));
            Assert.IsTrue(output.Contains("unsafeUpdate"));
            Assert.IsTrue(output.Contains("unsafeLocalUpdate"));
            Assert.IsTrue(output.Contains("cannot be updated"));
        }

        [TestMethod]
        public void Test_AnalyzeWithPrint_IncludesUnboundedOperationWarnings()
        {
            string output = CaptureAnalyzeWithPrintOutput(
                Contract_UnboundedOperation.Nef,
                Contract_UnboundedOperation.Manifest);

            Assert.IsTrue(output.Contains("[SECURITY]"));
            Assert.IsTrue(output.Contains("Potential unbounded operations"));
            Assert.IsTrue(output.Contains("113"));
        }

        private static string CaptureAnalyzeWithPrintOutput(
            Neo.SmartContract.NefFile nef,
            Neo.SmartContract.Manifest.ContractManifest manifest)
        {
            var stdout = new StringWriter();
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetOut(stdout);
                Neo.Compiler.SecurityAnalyzer.SecurityAnalyzer.AnalyzeWithPrint(nef, manifest, null);
            }
            finally
            {
                Console.SetOut(originalOut);
            }

            return stdout.ToString();
        }
    }
}
