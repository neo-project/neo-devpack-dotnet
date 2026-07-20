// Copyright (C) 2015-2026 The Neo Project.
//
// ArrayRangeUsageAnalyzer.cs file belongs to the neo project and is free
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
public sealed class ArrayRangeUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC2010";
    public const string Message = "Range access is not supported for '{0}'. Use ranges only with byte[] or string receivers.";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Range receiver is not supported",
        Message,
        "Syntax",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Neo supports range access only for byte arrays and strings.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeElementAccess, SyntaxKind.ElementAccessExpression);
    }

    private static void AnalyzeElementAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ElementAccessExpressionSyntax elementAccess ||
            elementAccess.ArgumentList.Arguments.Count != 1 ||
            elementAccess.ArgumentList.Arguments[0].Expression is not RangeExpressionSyntax range)
        {
            return;
        }

        if (context.SemanticModel.GetTypeInfo(elementAccess.Expression, context.CancellationToken).Type is not IArrayTypeSymbol arrayType ||
            arrayType.ElementType.SpecialType == SpecialType.System_Byte)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            range.GetLocation(),
            arrayType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
    }
}
