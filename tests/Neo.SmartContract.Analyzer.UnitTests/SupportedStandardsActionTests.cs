// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

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
public class SupportedStandardsActionTests
{
    [TestMethod]
    [DataRow("\"NEP11\", \"NEP17\"", false, true)]
    [DataRow("\"NEP11\"", true, true)]
    [DataRow("\"NEP11\", \"CUSTOM\"", false, false)]
    [DataRow("\"NEP11\", \"NEP24\"", false, true)]
    public async Task SingleActionPreservesValidAttribute(string arguments, bool shadow, bool canFix)
    {
        var source = $$"""
            namespace Neo.SmartContract.Framework
            {
                public enum NepStandard { Nep11, Nep17, Nep24 }
            }
            namespace Neo.SmartContract.Framework.Attributes
            {
                public class SupportedStandardsAttribute : global::System.Attribute
                {
                    public SupportedStandardsAttribute(params string[] values) { }
                    public SupportedStandardsAttribute(params global::Neo.SmartContract.Framework.NepStandard[] values) { }
                }
            }
            namespace Application
            {
                using Neo.SmartContract.Framework.Attributes;
                {{(shadow ? "class NepStandard { }" : "")}}
                [SupportedStandards({{arguments}})]
                class Contract { }
            }
            """;
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("Standards", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                .Split(Path.PathSeparator).Select(path => MetadataReference.CreateFromFile(path)));
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        var document = workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
        var compilation = (await document.Project.GetCompilationAsync())!;
        AssertNoErrors(compilation);
        var analyzer = new SupportedStandardsAnalyzer();
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();
        Assert.IsFalse(diagnostics.Any(d => d.Id == "AD0001"));
        var diagnostic = diagnostics.First(d => d.Properties.ContainsKey("Standard"));
        var actions = new List<CodeAction>();
        await new SupportedStandardsCodeFixProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic,
            (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(canFix ? 1 : 0, actions.Count);
        if (!canFix) return;
        var operation = (await actions.Single().GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
        var fixedDocument = operation.ChangedSolution.GetDocument(document.Id)!;
        var fixedCompilation = (await fixedDocument.Project.GetCompilationAsync())!;
        AssertNoErrors(fixedCompilation);
        var remaining = await fixedCompilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(0, remaining.Length);
        var contract = fixedCompilation.GetTypeByMetadataName("Application.Contract")!;
        var values = contract.GetAttributes().Single().ConstructorArguments.Single().Values;
        Assert.AreEqual(arguments.Split(',').Length, values.Length);
        Assert.AreEqual(0, values[0].Value);
        if (values.Length == 2) Assert.AreEqual(arguments.Contains("NEP24") ? 2 : 1, values[1].Value);
    }

    private static void AssertNoErrors(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.AreEqual(0, errors.Length, string.Join(Environment.NewLine, errors.Select(d => d.ToString())));
    }
}
