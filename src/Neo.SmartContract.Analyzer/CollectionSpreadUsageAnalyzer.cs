// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionSpreadUsageAnalyzer.cs file belongs to the neo project and is free
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

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CollectionSpreadUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4062";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Collection spread elements are not supported",
        "Collection spread element '{0}' is not supported; construct the collection explicitly",
        "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Reports collection-expression spread elements before they reach unsupported Neo compiler lowering.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSpreadElement, SyntaxKind.SpreadElement);
    }

    private static void AnalyzeSpreadElement(SyntaxNodeAnalysisContext context)
    {
        var spread = (SpreadElementSyntax)context.Node;
        context.ReportDiagnostic(Diagnostic.Create(Rule, spread.GetLocation(), spread.ToString()));
    }
}
