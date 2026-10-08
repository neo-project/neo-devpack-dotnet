// Copyright (C) 2015-2026 The Neo Project.

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
public class SupportedBooleanApiTests
{
    [TestMethod]
    public async Task BoolTryParseIsAcceptedByTheAnalyzerSuite()
    {
        var source = CSharpSyntaxTree.ParseText("class Contract { static bool Parse(string value) { return bool.TryParse(value, out var result) && result; } }");
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create("BooleanApi", [source], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        Assert.IsFalse(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        var diagnostics = await compilation.WithAnalyzers(NeoAnalyzerSuite.Create().ToImmutableArray()).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(0, diagnostics.Length, string.Join("\n", diagnostics));
    }
}
