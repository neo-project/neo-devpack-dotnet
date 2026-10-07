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
public class BigIntegerTargetTypedTests
{
    [TestMethod]
    [DataRow("new(42)", true)]
    [DataRow("new()", true)]
    [DataRow("new(1.5)", true)]
    [DataRow("new(bytes, true, true)", true)]
    [DataRow("new(bytes)", false)]
    [DataRow("new BigInteger(bytes)", false)]
    public async Task ConstructorOverloadsAreChecked(string expression, bool rejected)
    {
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, $"using System.Numerics; class Contract {{ BigInteger Read(byte[] bytes) => {expression}; }}");
        var compilation = (await document.Project.GetCompilationAsync())!;
        Assert.IsFalse(compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new BigIntegerCreationAnalyzer())).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(rejected ? 1 : 0, diagnostics.Length);
        if (rejected) Assert.AreEqual("NC4008", diagnostics[0].Id);
    }

    [TestMethod]
    public async Task IntegralFixPreservesTargetTypeAndCompilesAgain()
    {
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, "using System.Numerics; class Contract { BigInteger Read() => new(/* keep */ 42); }");
        var compilation = (await document.Project.GetCompilationAsync())!;
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new BigIntegerCreationAnalyzer())).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(1, diagnostics.Length);
        List<CodeAction> actions = [];
        await new BigIntegerCreationCodeFixProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(1, actions.Count);
        var change = (await actions[0].GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
        var fixedDocument = change.ChangedSolution.GetDocument(document.Id)!;
        var fixedCompilation = (await fixedDocument.Project.GetCompilationAsync())!;
        Assert.IsFalse(fixedCompilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error));
        StringAssert.Contains((await fixedDocument.GetTextAsync()).ToString(), "/* keep */");
        var model = (await fixedDocument.GetSemanticModelAsync())!;
        var cast = (await fixedDocument.GetSyntaxRootAsync())!.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.CastExpressionSyntax>().Single();
        Assert.AreEqual("System.Numerics.BigInteger", model.GetTypeInfo(cast).Type!.ToDisplayString());
        Assert.AreEqual(0, (await fixedCompilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new BigIntegerCreationAnalyzer())).GetAnalyzerDiagnosticsAsync()).Length);
    }

    [TestMethod]
    [DataRow("new()")]
    [DataRow("new(1.5)")]
    [DataRow("new(bytes, true, true)")]
    public async Task NonIntegralConstructorsDoNotOfferAnUnsafeFix(string expression)
    {
        using var workspace = new AdhocWorkspace();
        var document = CreateDocument(workspace, $"using System.Numerics; class Contract {{ BigInteger Read(byte[] bytes) => {expression}; }}");
        var compilation = (await document.Project.GetCompilationAsync())!;
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new BigIntegerCreationAnalyzer())).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(1, diagnostics.Length);
        List<CodeAction> actions = [];
        await new BigIntegerCreationCodeFixProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(0, actions.Count);
    }

    private static Document CreateDocument(AdhocWorkspace workspace, string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator).Select(path => MetadataReference.CreateFromFile(path));
        var project = workspace.AddProject("Constructors", LanguageNames.CSharp)
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(references);
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        return workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
    }
}
