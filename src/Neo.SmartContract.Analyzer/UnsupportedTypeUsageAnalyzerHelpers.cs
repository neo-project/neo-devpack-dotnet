// Copyright (C) 2015-2026 The Neo Project.
//
// UnsupportedTypeUsageAnalyzerHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace Neo.SmartContract.Analyzer;

internal static class UnsupportedTypeUsageAnalyzerHelpers
{
    internal static bool IsUnsupportedType(ITypeSymbol? type, SpecialType unsupportedType) =>
        type?.SpecialType == unsupportedType;

    internal static void AnalyzeMethodDeclaration(
        SyntaxNodeAnalysisContext context,
        SpecialType specialType,
        DiagnosticDescriptor rule,
        Func<ITypeSymbol, object?[]> getMessageArgs)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration) return;

        var type = context.SemanticModel.GetTypeInfo(methodDeclaration.ReturnType, context.CancellationToken).Type;
        ReportIfUnsupportedType(context, methodDeclaration.ReturnType.GetLocation(), type, specialType, rule, getMessageArgs);
    }

    internal static void AnalyzeParameter(
        SyntaxNodeAnalysisContext context,
        SpecialType specialType,
        DiagnosticDescriptor rule,
        Func<ITypeSymbol, object?[]> getMessageArgs)
    {
        if (context.Node is not ParameterSyntax parameter || parameter.Type is null) return;

        var type = (context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken) as IParameterSymbol)?.Type;
        ReportIfUnsupportedType(context, parameter.Type.GetLocation(), type, specialType, rule, getMessageArgs);
    }

    private static void ReportIfUnsupportedType(
        SyntaxNodeAnalysisContext context,
        Location location,
        ITypeSymbol? type,
        SpecialType unsupportedType,
        DiagnosticDescriptor rule,
        Func<ITypeSymbol, object?[]> getMessageArgs)
    {
        if (!IsUnsupportedType(type, unsupportedType)) return;

        var diagnostic = Diagnostic.Create(rule, location, getMessageArgs(type!));
        context.ReportDiagnostic(diagnostic);
    }
}
