// Copyright (C) 2015-2026 The Neo Project.
//
// AnalyzerFuzzTests.cs file belongs to the neo project and is free
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
public class AnalyzerFuzzTests
{
    private static readonly string[] SourceTemplates =
    [
        "using System; class Contract { public DateTime Read() => DateTime.Now; }",
        "using System; class Contract { public Guid Read() => Guid.NewGuid(); }",
        "using System.Numerics; class Contract { public BigInteger Read(int value) => new BigInteger(value); }",
        "using System.Linq; class Contract { public int Read(int[] values) => values.Sum(); }",
        "using System.Text; class Contract { public string Read() { var builder = new StringBuilder(); builder.Append(\"x\"); return builder.ToString(); } }",
        "using System.Threading.Tasks; class Contract { public Task<int> Read() => Task.FromResult(1); }",
        "using System.Collections.Generic; class Contract { public List<int> Read() => new List<int>(); }",
        "using System; class Contract { public object Read() => typeof(DateTime); }",
        "using System; class Contract { public string Read(string value) => value.Trim(); }",
        "class Contract { public int Read(int value) => value > 0 ? value : 0; }",
        "class Contract { public int Read(int[] values) => values.Length == 0 ? 0 : values[0]; }",
        "using System; class Contract { public bool Read(bool value) => value && !false; }"
    ];

    [TestMethod]
    public async Task GeneratedSources_ShouldProduceDeterministicDiagnosticsWithoutAnalyzerFailures()
    {
        var results = await Task.WhenAll(Enumerable.Range(0, 64).Select(async seed =>
        {
            var source = BuildSource(seed);
            var first = FormatDiagnostics(await AnalyzeAsync(source, $"Generated{seed}.cs"));
            var second = FormatDiagnostics(await AnalyzeAsync(source, $"Generated{seed}.cs"));

            CollectionAssert.AreEqual(first, second, $"Analyzer output changed for seed {seed}.");
            CollectionAssert.DoesNotContain(first, "AD0001", $"Analyzer failure reported for seed {seed}.");
            return first;
        }));

        Assert.IsTrue(results.Any(result => result.Length > 0), "The generated corpus did not exercise any analyzer rule.");
    }

    private static string BuildSource(int seed)
    {
        var template = SourceTemplates[seed % SourceTemplates.Length];
        return template.Replace("class Contract", $"class Contract{seed}", StringComparison.Ordinal);
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source, string path)
    {
        var trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = trustedPlatformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            $"AnalyzerFuzz{path}",
            [CSharpSyntaxTree.ParseText(source, path: path)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation
            .WithAnalyzers(NeoAnalyzerSuite.Create())
            .GetAnalyzerDiagnosticsAsync();
    }

    private static string[] FormatDiagnostics(ImmutableArray<Diagnostic> diagnostics) => diagnostics
        .OrderBy(diagnostic => diagnostic.Location.SourceSpan.Start)
        .ThenBy(diagnostic => diagnostic.Id, StringComparer.Ordinal)
        .ThenBy(diagnostic => diagnostic.GetMessage(), StringComparer.Ordinal)
        .Select(diagnostic => $"{diagnostic.Id}:{diagnostic.Location.SourceSpan.Start}:{diagnostic.GetMessage()}")
        .ToArray();
}
