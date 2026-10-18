// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class CollectionMemberDiagnosticsTests
{
    [TestMethod]
    [DataRow("public static List<int> first, second;", "List<int>")]
    [DataRow("public Dictionary<string, List<int>>[] values;", "Dictionary<string, List<int>>[]")]
    [DataRow("public List<int> this[int index] => null;", "List<int>")]
    [DataRow("public event Action<List<int>> Changed;", "Action<List<int>>")]
    [DataRow("public event Action<List<int>> Changed { add { } remove { } }", "Action<List<int>>")]
    public async Task UnsupportedMemberTypesAreDiagnosedOnce(string member, string expectedSpan)
    {
        var compilation = Compile("using System; using System.Collections.Generic; class Contract { " + member + " }");
        var diagnostics = await compilation.WithAnalyzers(NeoAnalyzerSuite.Create()).GetAnalyzerDiagnosticsAsync();
        Assert.IsFalse(diagnostics.Any(d => d.Id == "AD0001"));
        var matches = diagnostics.Where(d => d.Id == CollectionTypesUsageAnalyzer.DiagnosticId).ToArray();
        Assert.HasCount(1, matches, string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(expectedSpan, matches[0].Location.SourceTree!.GetText().ToString(matches[0].Location.SourceSpan));
        Assert.AreEqual(DiagnosticSeverity.Error, matches[0].Severity);
    }

    [TestMethod]
    public async Task UserDefinedCollectionsAndOrdinaryMembersRemainAllowed()
    {
        var compilation = Compile("""
            using System;
            class List<T> { }
            class Contract
            {
                public List<int> values;
                public byte[] bytes;
                public List<int> this[int index] => null;
                public event Action<List<int>> Changed;
            }
            """);
        var diagnostics = await compilation.WithAnalyzers(NeoAnalyzerSuite.Create()).GetAnalyzerDiagnosticsAsync();
        Assert.IsFalse(diagnostics.Any(d => d.Id is "AD0001" or CollectionTypesUsageAnalyzer.DiagnosticId));
    }

    private static CSharpCompilation Compile(string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create("CollectionMembers", [CSharpSyntaxTree.ParseText(source)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        Assert.IsFalse(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error),
            string.Join(Environment.NewLine, compilation.GetDiagnostics()));
        return compilation;
    }
}
