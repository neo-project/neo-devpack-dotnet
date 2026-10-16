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
using System.Globalization;
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

    [TestMethod]
    public async Task AnalyzerSuite_ProducesStableDiagnosticsAcrossCultures()
    {
        const string source = """
            using System;
            using System.Globalization;

            class Contract
            {
                public DateTime ReadClock() => DateTime.Now;
                public float ReadFloat(float value) => value;
                public string Format(int value) => value.ToString(CultureInfo.CurrentCulture);
            }
            """;
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUICulture = CultureInfo.CurrentUICulture;
        string[] expected = Array.Empty<string>();
        bool hasExpected = false;

        try
        {
            foreach (string cultureName in new[] { CultureInfo.InvariantCulture.Name, "en-US", "tr-TR", "zh-CN" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
                var diagnostics = await AnalyzeAsync(CreateCompilation(source));
                Assert.IsTrue(diagnostics.Any(diagnostic => diagnostic.Id == "NC4058"), "Missing platform API diagnostic.");
                Assert.IsTrue(diagnostics.Any(diagnostic => diagnostic.Id == "NC4002"), "Missing floating-point diagnostic.");
                Assert.IsFalse(diagnostics.Any(diagnostic => diagnostic.Id == "AD0001"), "An analyzer failed.");
                string[] actual = FormatDiagnostics(diagnostics);
                if (!hasExpected)
                {
                    expected = actual;
                    hasExpected = true;
                }
                CollectionAssert.AreEqual(expected, actual, $"Diagnostics changed under {cultureName}.");
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
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
                diagnostic.Location.SourceSpan.Length,
                diagnostic.GetMessage()))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
}
