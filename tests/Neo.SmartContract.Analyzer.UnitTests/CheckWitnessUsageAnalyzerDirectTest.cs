// Copyright (C) 2015-2025 The Neo Project.
//
// CheckWitnessUsageAnalyzerDirectTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    [TestClass]
    public class CheckWitnessUsageAnalyzerDirectTest
    {
        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_UnverifiedResult_ShouldReportDiagnostic()
        {
            const string sourceCode = """
                                       using Neo.SmartContract.Framework;
                                       using Neo.SmartContract.Framework.Services;

                                       public class TestContract : SmartContract
                                       {
                                           public static void Main(UInt160 owner)
                                           {
                                               // Unverified CheckWitness result
                                               Runtime.CheckWitness(owner);
                                           }
                                       }
                                       """;

            var compilation = await TestHelper.CreateCompilationAsync(sourceCode);
            var analyzer = new CheckWitnessUsageAnalyzer();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            // Filter to only our analyzer diagnostics
            var analyzerDiagnostics = diagnostics.Where(d => d.Id == CheckWitnessUsageAnalyzer.DiagnosticId).ToArray();

            Assert.AreEqual(1, analyzerDiagnostics.Length, "Expected exactly one diagnostic from CheckWitnessUsageAnalyzer");
            Assert.AreEqual(CheckWitnessUsageAnalyzer.DiagnosticId, analyzerDiagnostics[0].Id);
            Assert.IsTrue(analyzerDiagnostics[0].GetMessage().Contains("CheckWitness"), "Diagnostic message should mention CheckWitness");
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithAssert_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                       using Neo.SmartContract.Framework;
                                       using Neo.SmartContract.Framework.Services;

                                       public class TestContract : SmartContract
                                       {
                                           public static void Main(UInt160 owner)
                                           {
                                               // Verified CheckWitness result with Assert
                                               Assert(Runtime.CheckWitness(owner));
                                           }
                                       }
                                       """;

            var compilation = await TestHelper.CreateCompilationAsync(sourceCode);
            var analyzer = new CheckWitnessUsageAnalyzer();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            // Filter to only our analyzer diagnostics
            var analyzerDiagnostics = diagnostics.Where(d => d.Id == CheckWitnessUsageAnalyzer.DiagnosticId).ToArray();

            Assert.AreEqual(0, analyzerDiagnostics.Length, "Expected no diagnostics when CheckWitness is used with Assert");
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithIfCondition_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                       using Neo.SmartContract.Framework;
                                       using Neo.SmartContract.Framework.Services;

                                       public class TestContract : SmartContract
                                       {
                                           public static void Main(UInt160 owner)
                                           {
                                               // Verified CheckWitness result in if condition
                                               if (Runtime.CheckWitness(owner))
                                               {
                                                   // Do something
                                               }
                                           }
                                       }
                                       """;

            var compilation = await TestHelper.CreateCompilationAsync(sourceCode);
            var analyzer = new CheckWitnessUsageAnalyzer();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            // Filter to only our analyzer diagnostics
            var analyzerDiagnostics = diagnostics.Where(d => d.Id == CheckWitnessUsageAnalyzer.DiagnosticId).ToArray();

            Assert.AreEqual(0, analyzerDiagnostics.Length, "Expected no diagnostics when CheckWitness is used in if condition");
        }

        [TestMethod]
        public async Task CheckWitnessUsageAnalyzer_VerifiedWithVariableAssignment_ShouldNotReportDiagnostic()
        {
            const string sourceCode = """
                                       using Neo.SmartContract.Framework;
                                       using Neo.SmartContract.Framework.Services;

                                       public class TestContract : SmartContract
                                       {
                                           public static void Main(UInt160 owner)
                                           {
                                               // Verified CheckWitness result assigned to variable
                                               bool isAuthorized = Runtime.CheckWitness(owner);
                                               if (isAuthorized)
                                               {
                                                   // Do something
                                               }
                                           }
                                       }
                                       """;

            var compilation = await TestHelper.CreateCompilationAsync(sourceCode);
            var analyzer = new CheckWitnessUsageAnalyzer();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            // Filter to only our analyzer diagnostics
            var analyzerDiagnostics = diagnostics.Where(d => d.Id == CheckWitnessUsageAnalyzer.DiagnosticId).ToArray();

            Assert.AreEqual(0, analyzerDiagnostics.Length, "Expected no diagnostics when CheckWitness result is assigned to variable");
        }
    }
}
