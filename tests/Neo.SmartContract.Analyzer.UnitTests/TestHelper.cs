// Copyright (C) 2015-2025 The Neo Project.
//
// TestHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests
{
    public static class TestHelper
    {
        private static readonly string[] NeoAssemblies = new[]
        {
            "Neo.SmartContract.Framework.dll"
        };

        public static Task<Compilation> CreateCompilationAsync(string source)
        {
            return CreateCompilationAsync(source, new string[] { });
        }



        public static async Task<Compilation> CreateCompilationAsync(string source, string[] additionalReferences)
        {
            var references = new List<MetadataReference>();

            // Add basic .NET references
            var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            foreach (var path in trustedAssemblies)
            {
                try
                {
                    references.Add(MetadataReference.CreateFromFile(path));
                }
                catch
                {
                    // Ignore any assemblies that can't be loaded
                }
            }

            // Add Neo.SmartContract.Framework reference
            foreach (var assembly in NeoAssemblies)
            {
                var neoAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly);
                if (File.Exists(neoAssemblyPath))
                {
                    references.Add(MetadataReference.CreateFromFile(neoAssemblyPath));
                }
                else
                {
                    // Try to find the assembly in the build output
                    var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                    var frameworkPath = Path.Combine(projectDir, "src", "Neo.SmartContract.Framework", "bin", "Debug", "net9.0", assembly);

                    if (File.Exists(frameworkPath))
                    {
                        references.Add(MetadataReference.CreateFromFile(frameworkPath));
                    }
                    else
                    {
                        throw new FileNotFoundException($"Could not find {assembly}. Make sure it's built and available.");
                    }
                }
            }

            // Add additional references
            foreach (var reference in additionalReferences)
            {
                references.Add(MetadataReference.CreateFromFile(reference));
            }

            var compilation = CSharpCompilation.Create(
                "TestCompilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation;
        }

        public static CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> CreateCodeFixTest<TAnalyzer, TCodeFix>()
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider, new()
        {
            var test = new CustomCodeFixTest<TAnalyzer, TCodeFix>();
            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler().Select(id => new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));

                return solution.WithProjectCompilationOptions(projectId, compilationOptions);
            });
            return test;
        }

        public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest<TAnalyzer>()
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            var test = new CustomAnalyzerTest<TAnalyzer>();
            test.SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler().Select(id => new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));

                return solution.WithProjectCompilationOptions(projectId, compilationOptions);
            });
            return test;
        }

        private static IEnumerable<string> GetNullableWarningsFromCompiler()
        {
            // These are the compiler diagnostics we want to suppress
            return new[]
            {
                // CS errors
                "CS0518", "CS0246", "CS0103", "CS1705",
            };
        }

        private class CustomCodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider, new()
        {
            public CustomCodeFixTest()
            {
                TestState.AdditionalReferences.Add(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

                // Add Neo.SmartContract.Framework reference
                foreach (var assembly in NeoAssemblies)
                {
                    var neoAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly);
                    if (File.Exists(neoAssemblyPath))
                    {
                        TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(neoAssemblyPath));
                    }
                    else
                    {
                        // Try to find the assembly in the build output
                        var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                        var frameworkPath = Path.Combine(projectDir, "src", "Neo.SmartContract.Framework", "bin", "Debug", "net9.0", assembly);

                        if (File.Exists(frameworkPath))
                        {
                            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(frameworkPath));
                        }
                    }
                }

                // Add System.Numerics for BigInteger
                var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
                var systemNumerics = trustedAssemblies.FirstOrDefault(a => a.EndsWith("System.Numerics.dll"));
                if (systemNumerics != null)
                {
                    TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(systemNumerics));
                }

                // Configure to ignore compiler diagnostics
                TestState.AnalyzerConfigFiles.Add(("/.editorconfig", "root = true\n\n[*.cs]\ndotnet_diagnostic.CS*.severity = none\n"));

                // Suppress compiler diagnostics
                TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;
            }

            protected override CompilationOptions CreateCompilationOptions()
            {
                var compilationOptions = base.CreateCompilationOptions();
                return compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler().Select(id => new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));
            }

            private static IEnumerable<string> GetNullableWarningsFromCompiler()
            {
                // These are the compiler diagnostics we want to suppress
                return new[]
                {
                    // CS errors
                    "CS0518", "CS0246", "CS0103", "CS1705",
                };
            }
        }

        private class CustomAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
            where TAnalyzer : DiagnosticAnalyzer, new()
        {
            public CustomAnalyzerTest()
            {
                TestState.AdditionalReferences.Add(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

                // Add Neo.SmartContract.Framework reference
                foreach (var assembly in NeoAssemblies)
                {
                    var neoAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly);
                    if (File.Exists(neoAssemblyPath))
                    {
                        TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(neoAssemblyPath));
                    }
                    else
                    {
                        // Try to find the assembly in the build output
                        var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                        var frameworkPath = Path.Combine(projectDir, "src", "Neo.SmartContract.Framework", "bin", "Debug", "net9.0", assembly);

                        if (File.Exists(frameworkPath))
                        {
                            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(frameworkPath));
                        }
                    }
                }

                // Add System.Numerics for BigInteger
                var trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
                var systemNumerics = trustedAssemblies.FirstOrDefault(a => a.EndsWith("System.Numerics.dll"));
                if (systemNumerics != null)
                {
                    TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(systemNumerics));
                }

                // Configure to ignore compiler diagnostics
                TestState.AnalyzerConfigFiles.Add(("/.editorconfig", "root = true\n\n[*.cs]\ndotnet_diagnostic.CS*.severity = none\n"));

                // Suppress compiler diagnostics
                TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck;
            }

            protected override CompilationOptions CreateCompilationOptions()
            {
                var compilationOptions = base.CreateCompilationOptions();
                return compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler().Select(id => new KeyValuePair<string, ReportDiagnostic>(id, ReportDiagnostic.Suppress))));
            }

            private static IEnumerable<string> GetNullableWarningsFromCompiler()
            {
                // These are the compiler diagnostics we want to suppress
                return new[]
                {
                    // CS errors
                    "CS0518", "CS0246", "CS0103", "CS1705",
                };
            }
        }
    }
}
