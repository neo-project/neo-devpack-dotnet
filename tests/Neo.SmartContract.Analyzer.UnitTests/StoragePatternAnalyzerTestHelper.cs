// Copyright (C) 2015-2025 The Neo Project.
//
// StoragePatternAnalyzerTestHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public static class StoragePatternAnalyzerTestHelper
    {
        public static CSharpAnalyzerTest<TAnalyzer, XUnitVerifier> CreateAnalyzerTest<TAnalyzer>()
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var test = TestHelper.CreateAnalyzerTest<TAnalyzer>();

            // Set up the test to only report diagnostics from our analyzer
            test.CompilerDiagnostics = CompilerDiagnostics.None;
            test.TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;

            // Add Neo.SmartContract.Framework reference
            var projectDir = System.IO.Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            var frameworkPath = System.IO.Path.Combine(projectDir, "src", "Neo.SmartContract.Framework", "bin", "Debug", "net9.0", "Neo.SmartContract.Framework.dll");

            if (System.IO.File.Exists(frameworkPath))
            {
                test.TestState.AdditionalReferences.Add(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(frameworkPath));
            }

            // Add a custom filter to only include diagnostics from our analyzer
            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(
                        GetCompilerDiagnosticIds().Select(id =>
                            new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));

                return solution.WithProjectCompilationOptions(projectId, compilationOptions);
            });

            return test;
        }

        public static CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier> CreateCodeFixTest<TAnalyzer, TCodeFix>()
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider, new()
        {
            var test = TestHelper.CreateCodeFixTest<TAnalyzer, TCodeFix>();

            // Set up the test to only report diagnostics from our analyzer
            test.CompilerDiagnostics = CompilerDiagnostics.None;
            test.TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;

            // Add Neo.SmartContract.Framework reference
            var projectDir = System.IO.Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            var frameworkPath = System.IO.Path.Combine(projectDir, "src", "Neo.SmartContract.Framework", "bin", "Debug", "net9.0", "Neo.SmartContract.Framework.dll");

            if (System.IO.File.Exists(frameworkPath))
            {
                test.TestState.AdditionalReferences.Add(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(frameworkPath));
            }

            // Add a custom filter to only include diagnostics from our analyzer
            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(
                        GetCompilerDiagnosticIds().Select(id =>
                            new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));

                return solution.WithProjectCompilationOptions(projectId, compilationOptions);
            });

            return test;
        }

        // Custom diagnostic filter that only keeps diagnostics from our analyzer
        public class StoragePatternDiagnosticFilter : AnalyzerConfigOptions
        {
            private readonly ImmutableArray<string> _analyzerDiagnosticIds;

            public StoragePatternDiagnosticFilter(ImmutableArray<string> analyzerDiagnosticIds)
            {
                _analyzerDiagnosticIds = analyzerDiagnosticIds;
            }

            public override bool TryGetValue(string key, out string value)
            {
                value = null;
                return false;
            }

            public bool ShouldReportDiagnostic(Diagnostic diagnostic)
            {
                // Only report diagnostics from our analyzer
                return _analyzerDiagnosticIds.Contains(diagnostic.Id);
            }
        }

        private static IEnumerable<string> GetCompilerDiagnosticIds()
        {
            // These are the compiler diagnostics we want to suppress
            return new[]
            {
                // CS errors
                "CS0518", "CS0246", "CS0103", "CS1705", "CS0234", "CS0117", "CS0012",
                // All other CS errors
                "CS"
            };
        }
    }
}
