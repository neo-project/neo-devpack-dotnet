// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class ParseInitializerTests
{
    private const string Framework = "namespace Neo.SmartContract.Framework { public sealed class UInt160 { public static UInt160 Parse(string value) => new(); public static implicit operator UInt160(string value) => new(); } }";

    [TestMethod]
    [DataRow("Helper.Parse()", 1)]
    [DataRow("(Helper.Parse())", 1)]
    [DataRow("global::Helper.Parse()", 1)]
    [DataRow("Helper.Read()", 0)]
    [DataRow("global::Neo.SmartContract.Framework.UInt160.Parse(\"value\")", 0)]
    public async Task ParseNameDoesNotSuppressStringInitialization(string initializer, int expected)
    {
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, Framework + $" class Helper {{ public static string Parse() => \"value\"; public static Neo.SmartContract.Framework.UInt160 Read() => Neo.SmartContract.Framework.UInt160.Parse(\"value\"); }} class Contract {{ static Neo.SmartContract.Framework.UInt160 value = {initializer}; }}");
        var compilation = (await document.Project.GetCompilationAsync())!;
        Assert.IsFalse(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new InitialValueAnalyzer())).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(expected, diagnostics.Length);
        if (expected > 0) Assert.AreEqual("NC4055", diagnostics[0].Id);
    }

    [TestMethod]
    public async Task ParseFixUsesGlobalQualificationWhenNeoIsShadowed()
    {
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, Framework + " namespace Contracts { class Neo {} class Contract { static global::Neo.SmartContract.Framework.UInt160 value = \"value\"; } }");
        var compilation = (await document.Project.GetCompilationAsync())!;
        Assert.IsFalse(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new InitialValueAnalyzer())).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(1, diagnostics.Length);
        List<CodeAction> actions = [];
        await new InitialValueCodeFixProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(1, actions.Count);
        var change = (await actions[0].GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
        var fixedDocument = change.ChangedSolution.GetDocument(document.Id)!;
        var fixedCompilation = (await fixedDocument.Project.GetCompilationAsync())!;
        Assert.IsFalse(fixedCompilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error), string.Join("\n", fixedCompilation.GetDiagnostics()));
        Assert.AreEqual(0, (await fixedCompilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new InitialValueAnalyzer())).GetAnalyzerDiagnosticsAsync()).Length);
    }

    private static Document CreateDocument(AdhocWorkspace workspace, string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator).Select(path => MetadataReference.CreateFromFile(path));
        var project = workspace.AddProject("Initializers", LanguageNames.CSharp)
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(references);
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        return workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
    }
}
