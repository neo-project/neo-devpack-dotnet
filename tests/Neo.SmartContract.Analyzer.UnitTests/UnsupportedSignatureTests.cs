// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class UnsupportedSignatureTests
{
    public static IEnumerable<object[]> UnsupportedMembers()
    {
        foreach (var (type, id) in new[] { ("float", FloatUsageAnalyzer.DiagnosticId), ("double", DoubleUsageAnalyzer.DiagnosticId), ("decimal", DecimalUsageAnalyzer.DiagnosticId), ("List<int>", CollectionTypesUsageAnalyzer.DiagnosticId) })
        {
            foreach (string member in new[] {
                "public delegate TYPE Factory();",
                "public static TYPE operator +(Contract value) => default;" })
                yield return [member.Replace("TYPE", type), type, id];
            if (id == "NC4013") continue;
            yield return [$"public {type} this[int index] => default;", type, id];
            yield return [$"public event Action<{type}> Changed {{ add {{ }} remove {{ }} }}", $"Action<{type}>", id];
            yield return [$"public event Action<{type}> Changed;", $"Action<{type}>", id];
            yield return [$"public {type} this[int index] => ({type})0;", type, id];
        }
    }

    [TestMethod]
    [DynamicData(nameof(UnsupportedMembers), DynamicDataSourceType.Method)]
    public async Task UnsupportedMemberSignaturesAreReportedOnce(string member, string expectedSpan, string id)
    {
        var diagnostics = await Analyze("using System; using System.Collections.Generic; class Contract { " + member + " }");
        Assert.IsFalse(diagnostics.Any(d => d.Id == "AD0001"));
        var matches = diagnostics.Where(d => d.Id == id).ToArray();
        Assert.AreEqual(1, matches.Length, string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString())));
        Assert.AreEqual(expectedSpan, matches[0].Location.SourceTree!.GetText().ToString(matches[0].Location.SourceSpan));
        Assert.AreEqual(DiagnosticSeverity.Error, matches[0].Severity);
    }

    [TestMethod]
    public async Task UserTypesAndSupportedSignaturesRemainAllowed()
    {
        var diagnostics = await Analyze("""
            using System;
            class Task { }
            class List<T> { }
            class Contract
            {
                public delegate Task Factory();
                public Task this[int index] => null;
                public event Action<Task> Changed { add { } remove { } }
                public static List<int> operator +(Contract value) => null;
                public delegate int IntegerFactory();
            }
            """);
        Assert.IsFalse(diagnostics.Any(d => d.Id is "AD0001" or "NC4002" or "NC4003" or "NC4004" or "NC4057" or "NC4013"));
    }

    private static async Task<System.Collections.Immutable.ImmutableArray<Diagnostic>> Analyze(string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create("Signatures", [CSharpSyntaxTree.ParseText(source)], references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.AreEqual(0, errors.Length, string.Join(Environment.NewLine, errors.Select(d => d.ToString())));
        return await compilation.WithAnalyzers(NeoAnalyzerSuite.Create()).GetAnalyzerDiagnosticsAsync();
    }
}
