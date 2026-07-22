// Copyright (C) 2015-2026 The Neo Project.
//
// CapturedForeachVariableAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CapturedForeachVariableAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4063";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Captured foreach variables use Neo-specific semantics",
        "Lambda captures foreach variable '{0}', whose value is shared across iterations on NeoVM",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Warns when a retained lambda can observe the last foreach value instead of C# per-iteration capture semantics.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeLambda,
            SyntaxKind.SimpleLambdaExpression,
            SyntaxKind.ParenthesizedLambdaExpression);
    }

    private static void AnalyzeLambda(SyntaxNodeAnalysisContext context)
    {
        var lambda = (LambdaExpressionSyntax)context.Node;
        var foreachVariables = GetEnclosingForeachVariables(context, lambda);
        if (foreachVariables.Count == 0)
        {
            return;
        }

        var reportedVariables = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        foreach (var identifier in lambda.Body
            .DescendantNodesAndSelf(static node => node is not AnonymousFunctionExpressionSyntax)
            .OfType<IdentifierNameSyntax>())
        {
            if (IsInsideNameof(identifier))
            {
                continue;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;
            if (symbol is not ILocalSymbol local ||
                !foreachVariables.Any(candidate => SymbolEqualityComparer.Default.Equals(candidate, local)) ||
                !reportedVariables.Add(local))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, identifier.GetLocation(), local.Name));
        }
    }

    private static List<ILocalSymbol> GetEnclosingForeachVariables(
        SyntaxNodeAnalysisContext context,
        LambdaExpressionSyntax lambda)
    {
        var variables = new List<ILocalSymbol>();
        foreach (var statement in lambda.Ancestors().OfType<CommonForEachStatementSyntax>())
        {
            switch (statement)
            {
                case ForEachStatementSyntax simple
                    when context.SemanticModel.GetDeclaredSymbol(simple, context.CancellationToken) is { } symbol:
                    variables.Add(symbol);
                    break;
                case ForEachVariableStatementSyntax deconstruction:
                    variables.AddRange(deconstruction.Variable
                        .DescendantNodesAndSelf()
                        .OfType<SingleVariableDesignationSyntax>()
                        .Select(designation => context.SemanticModel.GetDeclaredSymbol(
                            designation,
                            context.CancellationToken))
                        .OfType<ILocalSymbol>());
                    break;
            }
        }

        return variables;
    }

    private static bool IsInsideNameof(SyntaxNode node) =>
        node.Ancestors().OfType<InvocationExpressionSyntax>().Any(static invocation =>
            invocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" });
}
