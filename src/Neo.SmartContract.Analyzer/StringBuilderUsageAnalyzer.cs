// Copyright (C) 2015-2025 The Neo Project.
//
// StringBuilderUsageAnalyzer.cs file belongs to the neo project and is free
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
public sealed class StringBuilderUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4031";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Unsupported StringBuilder member",
        "Unsupported StringBuilder member: {0}",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly SymbolDisplayFormat DiagnosticDisplayFormat =
        SymbolDisplayFormat.CSharpShortErrorMessageFormat
            .WithParameterOptions(SymbolDisplayParameterOptions.IncludeType);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
            return;

        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
            return;

        if (!IsStringBuilder(methodSymbol.ContainingType))
            return;

        if (IsSupportedInvocation(methodSymbol))
            return;

        var diagnostic = Diagnostic.Create(
            Rule,
            invocation.GetLocation(),
            methodSymbol.ToDisplayString(DiagnosticDisplayFormat));

        context.ReportDiagnostic(diagnostic);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AssignmentExpressionSyntax assignment)
            return;

        var symbol = context.SemanticModel.GetSymbolInfo(assignment.Left).Symbol as IPropertySymbol;
        if (symbol is null || !IsStringBuilder(symbol.ContainingType))
            return;

        if (symbol.Name != "Length")
            return;

        var diagnostic = Diagnostic.Create(
            Rule,
            assignment.GetLocation(),
            $"{symbol.Name} (set)");

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsSupportedInvocation(IMethodSymbol methodSymbol)
    {
        return methodSymbol.Name switch
        {
            "Append" => methodSymbol.Parameters.Length == 1 && IsSupportedAppendParameter(methodSymbol.Parameters[0].Type),
            "AppendLine" => methodSymbol.Parameters.Length == 0 ||
                            (methodSymbol.Parameters.Length == 1 && IsString(methodSymbol.Parameters[0].Type)),
            "ToString" => methodSymbol.Parameters.Length == 0,
            "Clear" => methodSymbol.Parameters.Length == 0,
            _ => false
        };
    }

    private static bool IsSupportedAppendParameter(ITypeSymbol parameterType)
    {
        if (IsString(parameterType))
            return true;

        if (parameterType.SpecialType == SpecialType.System_Char ||
            parameterType.SpecialType == SpecialType.System_Object)
            return true;

        return IsStringBuilder(parameterType);
    }

    private static bool IsString(ITypeSymbol type) =>
        type.SpecialType == SpecialType.System_String;

    private static bool IsStringBuilder(ITypeSymbol? type) =>
        type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Text.StringBuilder";
}
