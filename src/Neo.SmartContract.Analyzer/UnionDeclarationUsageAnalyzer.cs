// Copyright (C) 2015-2026 The Neo Project.
//
// UnionDeclarationUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

// C# 15 union declarations are exposed through experimental Roslyn APIs.
#pragma warning disable RSEXPERIMENTAL006

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnionDeclarationUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4067";

    private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorFactory.Create(
        DiagnosticId,
        "Union declarations are not supported",
        "Union declaration '{0}' is not supported; model the closed set of values with an explicit type hierarchy or a tagged struct instead",
        "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Reports C# 15 union declarations before they reach unsupported Neo compiler lowering.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeUnionDeclaration, SyntaxKind.UnionDeclaration);
    }

    // Roslyn 5.9 delivers UnionDeclarationSyntax nodes to syntax actions twice because the
    // experimental kind participates in both the member-declaration and type-declaration
    // walks. Track reported nodes weakly so each union is reported exactly once.
    private static readonly ConditionalWeakTable<SyntaxNode, object> ReportedNodes = new();

    private static void AnalyzeUnionDeclaration(SyntaxNodeAnalysisContext context)
    {
        var union = (UnionDeclarationSyntax)context.Node;
        // ConditionalWeakTable (netstandard2.0) has no TryAdd; Add throws when the key exists.
        if (ReportedNodes.TryGetValue(union, out _))
            return;
        try
        {
            ReportedNodes.Add(union, Boxed.True);
        }
        catch (System.ArgumentException)
        {
            return;
        }
        context.ReportDiagnostic(Diagnostic.Create(Rule, union.GetLocation(), union.Identifier.ValueText));
    }

    private static class Boxed
    {
        internal static readonly object True = new();
    }
}
#pragma warning restore RSEXPERIMENTAL006
