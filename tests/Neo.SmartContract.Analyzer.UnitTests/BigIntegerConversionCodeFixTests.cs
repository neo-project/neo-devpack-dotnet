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
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer.UnitTests;

[TestClass]
public class BigIntegerConversionCodeFixTests
{
    [TestMethod]
    [DataRow("int", false, false)]
    [DataRow("int", true, false)]
    [DataRow("long", false, false)]
    [DataRow("long", true, false)]
    [DataRow("uint", false, false)]
    [DataRow("ulong", true, false)]
    [DataRow("int", false, true)]
    public async Task FixPreservesUserDefinedArgumentConversions(string integralType, bool targetTyped, bool preconverted)
    {
        string argument = preconverted ? $"({integralType})value" : "value";
        string creation = targetTyped ? $"new({argument})" : $"new BigInteger({argument})";
        string source = $$"""
            using System.Numerics;
            public struct Number
            {
                public static int Conversions;
                public static implicit operator {{integralType}}(Number value) { Conversions++; return 7; }
                public static explicit operator BigInteger(Number value) { Conversions += 100; return 99; }
            }
            public class Contract
            {
                public static BigInteger Run()
                {
                    Number.Conversions = 0;
                    Number value = default;
                    BigInteger result = {{creation}};
                    return result * 1000 + Number.Conversions;
                }
            }
            """;
        using var workspace = new AdhocWorkspace();
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
        var project = workspace.AddProject("ConversionFix", LanguageNames.CSharp)
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(references);
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        var document = workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
        var compilation = (await document.Project.GetCompilationAsync())!;
        Assert.AreEqual(new BigInteger(7001), Execute(compilation));
        var castTree = CSharpSyntaxTree.ParseText(source.Replace(creation, $"(BigInteger){argument}"), new CSharpParseOptions(LanguageVersion.Preview));
        Assert.AreEqual(new BigInteger(preconverted ? 7001 : 99100), Execute(compilation.ReplaceSyntaxTree(compilation.SyntaxTrees.Single(), castTree)));
        var analyzer = new BigIntegerCreationAnalyzer();
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();
        var diagnostic = diagnostics.Single(d => d.Id == BigIntegerCreationAnalyzer.DiagnosticId);
        var actions = new List<CodeAction>();
        await new BigIntegerCreationCodeFixProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic,
            (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(preconverted ? 1 : 0, actions.Count, "A direct cast must not bypass a user-defined argument conversion.");
        if (preconverted)
        {
            var operation = (await actions[0].GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
            var fixedDocument = operation.ChangedSolution.GetDocument(document.Id)!;
            var fixedCompilation = (await fixedDocument.Project.GetCompilationAsync())!;
            Assert.AreEqual(new BigInteger(7001), Execute(fixedCompilation));
        }
    }

    private static BigInteger Execute(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitted = compilation.WithAssemblyName(Guid.NewGuid().ToString("N")).Emit(stream);
        Assert.IsTrue(emitted.Success, string.Join(Environment.NewLine, emitted.Diagnostics));
        var assembly = Assembly.Load(stream.ToArray());
        return (BigInteger)assembly.GetType("Contract")!.GetMethod("Run")!.Invoke(null, null)!;
    }
}
