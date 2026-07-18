// Copyright (C) 2015-2026 The Neo Project.
//
// BitOperationsUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BitOperationsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4060";

        private const string BitOperationsTypeName = "global::System.Numerics.BitOperations";

        private static readonly SymbolDisplayFormat FullyQualifiedFormat =
            SymbolDisplayFormat.FullyQualifiedFormat;

        private static readonly SymbolDisplayFormat DiagnosticDisplayFormat =
            SymbolDisplayFormat.CSharpShortErrorMessageFormat
                .WithParameterOptions(SymbolDisplayParameterOptions.IncludeType |
                                      SymbolDisplayParameterOptions.IncludeParamsRefOut);

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported BitOperations method or overload is used",
            "Unsupported BitOperations method or overload: {0}",
            "Method",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpression) return;

            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol methodSymbol ||
                methodSymbol.ContainingType?.ToDisplayString(FullyQualifiedFormat) != BitOperationsTypeName ||
                IsSupportedBitOperationsMethod(methodSymbol))
                return;

            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                invocationExpression.GetLocation(),
                methodSymbol.ToDisplayString(DiagnosticDisplayFormat)));
        }

        private static bool IsSupportedBitOperationsMethod(IMethodSymbol method)
        {
            return method.Name switch
            {
                "Log2" or "PopCount" or "LeadingZeroCount" =>
                    HasParameters(method, SpecialType.System_UInt32) ||
                    HasParameters(method, SpecialType.System_UInt64),
                "RotateLeft" or "RotateRight" =>
                    HasParameters(method, SpecialType.System_UInt32, SpecialType.System_Int32) ||
                    HasParameters(method, SpecialType.System_UInt64, SpecialType.System_Int32),
                _ => false
            };
        }

        private static bool HasParameters(IMethodSymbol method, params SpecialType[] parameterTypes)
        {
            if (method.Parameters.Length != parameterTypes.Length)
                return false;

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                if (method.Parameters[i].Type.SpecialType != parameterTypes[i] ||
                    method.Parameters[i].RefKind != RefKind.None)
                    return false;
            }

            return true;
        }
    }
}
