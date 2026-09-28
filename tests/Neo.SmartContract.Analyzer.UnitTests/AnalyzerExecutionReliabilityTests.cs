// Copyright (C) 2015-2026 The Neo Project.
//
// AnalyzerExecutionReliabilityTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class AnalyzerExecutionReliabilityTests
{
    [TestMethod]
    public async Task AnalyzerSuite_ProducesStableDiagnosticsUnderConcurrentExecution()
    {
        const string source = """
            using System;

            class Contract
            {
                public DateTime ReadClock() => DateTime.Now;
            }
            """;
        var compilation = CreateCompilation(source);

        var runs = await Task.WhenAll(
            Enumerable.Range(0, 8).Select(_ => AnalyzeAsync(compilation)));
        var expected = FormatDiagnostics(runs[0]);

        foreach (var run in runs.Skip(1))
        {
            CollectionAssert.AreEqual(expected, FormatDiagnostics(run));
        }

        Assert.IsTrue(
            expected.Any(value => value.StartsWith("NC4058|Error|", StringComparison.Ordinal)),
            string.Join(Environment.NewLine, expected));
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = trustedPlatformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        return CSharpCompilation.Create(
            "AnalyzerExecutionReliability",
            [CSharpSyntaxTree.ParseText(source, path: "Contract.cs")],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(CSharpCompilation compilation)
        => await compilation
            .WithAnalyzers(NeoAnalyzerSuite.Create())
            .GetAnalyzerDiagnosticsAsync();

    private static string[] FormatDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        => diagnostics
            .Select(diagnostic => string.Join(
                "|",
                diagnostic.Id,
                diagnostic.Severity,
                diagnostic.Location.SourceTree?.FilePath,
                diagnostic.Location.SourceSpan.Start,
                diagnostic.GetMessage()))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
}
