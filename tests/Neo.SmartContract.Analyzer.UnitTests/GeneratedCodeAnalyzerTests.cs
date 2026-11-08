// Copyright (C) 2015-2026 The Neo Project.
//
// GeneratedCodeAnalyzerTests.cs file belongs to the neo project and is free
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
public class GeneratedCodeAnalyzerTests
{
    [TestMethod]
    public async Task AnalyzerSuite_SkipsGeneratedSource()
    {
        const string source = """
            using System;

            class GeneratedContract
            {
                public DateTime ReadClock() => DateTime.Now;
            }
            """;

        var generatedDiagnostics = await AnalyzeAsync(source, "Contract.g.cs");
        var regularDiagnostics = await AnalyzeAsync(source, "Contract.cs");

        Assert.AreEqual(0, generatedDiagnostics.Length, FormatDiagnostics(generatedDiagnostics));
        CollectionAssert.Contains(
            regularDiagnostics.Select(diagnostic => diagnostic.Id).ToArray(),
            "NC4058");
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source, string path)
    {
        var trustedPlatformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = trustedPlatformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            "GeneratedCodeAnalysis",
            [CSharpSyntaxTree.ParseText(source, path: path)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return await compilation
            .WithAnalyzers(NeoAnalyzerSuite.Create())
            .GetAnalyzerDiagnosticsAsync();
    }

    private static string FormatDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        => string.Join(Environment.NewLine, diagnostics.Select(diagnostic => diagnostic.ToString()));
}
