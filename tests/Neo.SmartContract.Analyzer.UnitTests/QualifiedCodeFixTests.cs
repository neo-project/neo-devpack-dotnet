// Copyright (C) 2015-2026 The Neo Project.
//
// QualifiedCodeFixTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory or
// http://www.opensource.org/licenses/mit-license.php for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications are permitted.

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
public class QualifiedCodeFixTests
{
    [TestMethod]
    public Task BigIntegerAliasFix_ResolvesTheGlobalType()
        => VerifyFix<BigIntegerUsingUsageAnalyzer, BigIntegerUsingUsageCodeFixProvider>("NC4022", """
            namespace Application
            {
                using BigInteger = global::System.Int64;
                namespace System.Numerics { public class BigInteger { } }
                class Contract { public BigInteger Read() => 1; }
            }
            """);

    [TestMethod]
    [DataRow("NEP-17", "NC4029")]
    [DataRow("NEP-11", "NC4029")]
    [DataRow("NEP-26", "NC4030")]
    public Task NepFix_ResolvesGlobalTypesDespiteShadowing(string standard, string id)
        => VerifyFix<NepStandardImplementationAnalyzer, NepStandardImplementationCodeFixProvider>(id, $$"""
            namespace Neo.SmartContract.Framework
            {
                public class UInt160 { }
                public class ByteString { }
                public class Map<TKey, TValue> { }
            }
            namespace Neo.SmartContract.Framework.Services { public class Iterator { } }
            namespace Neo.SmartContract.Framework.Interfaces { public interface INEP26 { } }
            namespace Neo.SmartContract.Framework.Attributes
            {
                public class SupportedStandardsAttribute : global::System.Attribute
                {
                    public SupportedStandardsAttribute(params string[] names) { }
                }
                public class SafeAttribute : global::System.Attribute { }
            }
            namespace Application
            {
                public class System { }
                public class Neo { }
                [global::Neo.SmartContract.Framework.Attributes.SupportedStandards("{{standard}}")]
                public class Contract { }
            }
            """);

    private static async Task VerifyFix<TAnalyzer, TProvider>(string id, string source)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TProvider : CodeFixProvider, new()
    {
        using var workspace = new AdhocWorkspace();
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator).Select(path => MetadataReference.CreateFromFile(path));
        var project = workspace.AddProject("QualifiedFix", LanguageNames.CSharp)
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(references);
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        var document = workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
        var compilation = (await document.Project.GetCompilationAsync())!;
        AssertNoErrors(compilation);
        var analyzer = new TAnalyzer();
        var diagnostics = await compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();
        Assert.IsFalse(diagnostics.Any(diagnostic => diagnostic.Id == "AD0001"));
        var diagnostic = diagnostics.Single(item => item.Id == id);
        var actions = new List<CodeAction>();
        await new TProvider().RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic,
            (action, _) => actions.Add(action), CancellationToken.None));
        Assert.AreEqual(1, actions.Count);
        var operation = (await actions.Single().GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
        var fixedDocument = operation.ChangedSolution.GetDocument(document.Id)!;
        var fixedCompilation = (await fixedDocument.Project.GetCompilationAsync())!;
        AssertNoErrors(fixedCompilation);
        var remaining = await fixedCompilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync();
        Assert.AreEqual(0, remaining.Length, string.Join(Environment.NewLine, remaining));
    }

    private static void AssertNoErrors(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.AreEqual(0, errors.Length, string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
    }
}
