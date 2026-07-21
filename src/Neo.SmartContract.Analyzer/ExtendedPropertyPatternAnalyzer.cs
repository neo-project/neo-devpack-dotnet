// Copyright (C) 2015-2026 The Neo Project.
//
// ExtendedPropertyPatternAnalyzer.cs file belongs to the neo project and is free
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
public sealed class ExtendedPropertyPatternAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4061";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Extended property patterns are not supported",
        "Extended property pattern '{0}' is not supported; use nested property patterns instead",
        "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Reports dotted property names in recursive patterns before they reach unsupported Neo compiler lowering.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSubpattern, SyntaxKind.Subpattern);
    }

    private static void AnalyzeSubpattern(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not SubpatternSyntax
            {
                ExpressionColon: { Expression: MemberAccessExpressionSyntax expression }
            })
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, expression.GetLocation(), expression.ToString()));
    }
}
