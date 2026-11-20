// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionWithElementUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

// C# 15 collection expression arguments are exposed through experimental Roslyn APIs.
#pragma warning disable RSEXPERIMENTAL006

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CollectionWithElementUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4066";

    private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorFactory.Create(
        DiagnosticId,
        "Collection expression arguments are not supported",
        "Collection expression argument '{0}' is not supported; construct the collection explicitly with its arguments",
        "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Reports collection-expression 'with(...)' elements before they reach unsupported Neo compiler lowering.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeWithElement, SyntaxKind.WithElement);
    }

    private static void AnalyzeWithElement(SyntaxNodeAnalysisContext context)
    {
        var withElement = (WithElementSyntax)context.Node;
        context.ReportDiagnostic(Diagnostic.Create(Rule, withElement.GetLocation(), withElement.ToString()));
    }
}
#pragma warning restore RSEXPERIMENTAL006
