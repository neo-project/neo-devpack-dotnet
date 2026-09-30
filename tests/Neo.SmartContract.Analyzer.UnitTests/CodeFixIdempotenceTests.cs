// Copyright (C) 2015-2026 The Neo Project.
//
// CodeFixIdempotenceTests.cs file belongs to the neo project and is free
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
public class CodeFixIdempotenceTests
{
    [TestMethod]
    public Task BigIntegerCreationFix_IsIdempotent()
        => AssertIdempotent<BigIntegerCreationAnalyzer, BigIntegerCreationCodeFixProvider>(
            "NC4008",
            "using System.Numerics; class Contract { BigInteger Read() => new BigInteger(1); }");

    [TestMethod]
    public Task BigIntegerUsingFix_IsIdempotent()
        => AssertIdempotent<BigIntegerUsingUsageAnalyzer, BigIntegerUsingUsageCodeFixProvider>(
            "NC4022",
            "using BigInteger = System.Int64; class Contract { BigInteger Read() => 1; }");

    [TestMethod]
    public Task VolatileRemovalFix_IsIdempotent()
        => AssertIdempotent<VolatileKeywordUsageAnalyzer, VolatileKeywordRemovalCodeFixProvider>(
            "NC4014",
            "class Contract { private volatile int _value; }");

    private static async Task AssertIdempotent<TAnalyzer, TCodeFixProvider>(string diagnosticId, string source)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("CodeFixIdempotence", LanguageNames.CSharp)
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Preview))
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithMetadataReferences(GetMetadataReferences());
        Assert.IsTrue(workspace.TryApplyChanges(project.Solution));
        var document = workspace.AddDocument(project.Id, "Contract.cs", SourceText.From(source));
        var analyzer = new TAnalyzer();
        var initialDiagnostics = await GetDiagnosticsAsync(document, analyzer).ConfigureAwait(false);
        var diagnostic = initialDiagnostics.FirstOrDefault(item => item.Id == diagnosticId);
        Assert.IsNotNull(
            diagnostic,
            $"Expected {diagnosticId}; received: {string.Join(Environment.NewLine, initialDiagnostics.Select(item => item.ToString()))}");

        var actions = new List<CodeAction>();
        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => actions.Add(action),
            CancellationToken.None);
        await new TCodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

        Assert.AreEqual(1, actions.Count, $"Expected one code fix for {diagnosticId}.");
        var operations = await actions[0].GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var applyChanges = operations.OfType<ApplyChangesOperation>().Single();
        Assert.IsTrue(workspace.TryApplyChanges(applyChanges.ChangedSolution));

        var fixedDocument = workspace.CurrentSolution.GetDocument(document.Id)!;
        var remainingDiagnostics = await GetDiagnosticsAsync(fixedDocument, analyzer).ConfigureAwait(false);
        Assert.IsFalse(
            remainingDiagnostics.Any(item => item.Id == diagnosticId),
            $"The code fix for {diagnosticId} produced a diagnostic when analyzed again.");
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        Document document,
        DiagnosticAnalyzer analyzer)
    {
        var compilation = await document.Project.GetCompilationAsync().ConfigureAwait(false);
        Assert.IsNotNull(compilation);
        return await compilation!.WithAnalyzers(
            ImmutableArray.Create(analyzer)).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var assemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        return assemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(System.Numerics.BigInteger).Assembly.Location));
    }
}
