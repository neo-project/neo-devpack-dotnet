// Copyright (C) 2015-2025 The Neo Project.
//
// ReturnValueTypeAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReturnValueTypeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4033";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Return type must match declaration",
        "Return statements for '{0}' must return '{0}' values, but the expression has type '{1}'. Convert explicitly to '{0}' (e.g., cast or parse the runtime value).",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Smart contract methods should not rely on implicit conversions when returning UInt160, UInt256, or ECPoint values.");

    private static readonly ImmutableArray<string> TargetTypeMetadataNames = ImmutableArray.Create(
        "Neo.SmartContract.Framework.UInt160",
        "Neo.SmartContract.Framework.UInt256",
        "Neo.SmartContract.Framework.ECPoint",
        "Neo.UInt160",
        "Neo.UInt256",
        "Neo.Cryptography.ECC.ECPoint");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeReturn, OperationKind.Return);
    }

    private static void AnalyzeReturn(OperationAnalysisContext context)
    {
        if (context.Operation is not IReturnOperation returnOperation)
            return;

        var returnedValue = returnOperation.ReturnedValue;
        if (returnedValue is null)
            return;

        if (context.ContainingSymbol is not IMethodSymbol methodSymbol)
            return;

        var returnType = methodSymbol.ReturnType;
        if (returnType is null || returnType.SpecialType == SpecialType.System_Void)
            return;

        if (!IsRestrictedReturnType(returnType, context.Compilation))
            return;

        if (returnedValue.ConstantValue.HasValue && returnedValue.ConstantValue.Value is null)
            return;

        if (!HasImplicitMismatchedConversion(returnedValue, returnType))
            return;

        var diagnostic = Diagnostic.Create(
            Rule,
            returnedValue.Syntax.GetLocation(),
            returnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            GetExpressionTypeDisplay(returnedValue));

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsRestrictedReturnType(ITypeSymbol returnType, Compilation compilation)
    {
        foreach (var metadataName in TargetTypeMetadataNames)
        {
            var candidate = compilation.GetTypeByMetadataName(metadataName);
            if (candidate is not null && SymbolEqualityComparer.Default.Equals(returnType, candidate))
                return true;
        }

        return false;
    }

    private static bool HasImplicitMismatchedConversion(IOperation value, ITypeSymbol expectedType)
    {
        if (value is IConversionOperation conversion &&
            conversion.IsImplicit &&
            SymbolEqualityComparer.Default.Equals(conversion.Type, expectedType) &&
            conversion.Operand?.Type is { } operandType &&
            !SymbolEqualityComparer.Default.Equals(operandType, expectedType))
        {
            return true;
        }

        return false;
    }

    private static string GetExpressionTypeDisplay(IOperation operation) =>
        operation switch
        {
            IConversionOperation { Operand.Type: { } operandType } =>
                operandType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            _ => operation.Type?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) ?? "unknown"
        };
}
