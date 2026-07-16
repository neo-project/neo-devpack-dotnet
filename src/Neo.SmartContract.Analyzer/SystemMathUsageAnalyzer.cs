// Copyright (C) 2015-2026 The Neo Project.
//
// SystemMathUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class SystemMathUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4005";

        private static readonly ImmutableHashSet<SpecialType> SignedIntegralTypes = ImmutableHashSet.Create(
            SpecialType.System_SByte,
            SpecialType.System_Int16,
            SpecialType.System_Int32,
            SpecialType.System_Int64);

        private static readonly ImmutableHashSet<SpecialType> IntegralTypes = SignedIntegralTypes
            .Add(SpecialType.System_Byte)
            .Add(SpecialType.System_UInt16)
            .Add(SpecialType.System_UInt32)
            .Add(SpecialType.System_UInt64);

        private static readonly ImmutableHashSet<SpecialType> Int32Type =
            ImmutableHashSet.Create(SpecialType.System_Int32);

        private static readonly SymbolDisplayFormat FullyQualifiedFormat =
            SymbolDisplayFormat.FullyQualifiedFormat;

        private static readonly SymbolDisplayFormat DiagnosticDisplayFormat =
            SymbolDisplayFormat.CSharpShortErrorMessageFormat
                .WithParameterOptions(SymbolDisplayParameterOptions.IncludeType |
                                      SymbolDisplayParameterOptions.IncludeParamsRefOut);

        private const string SystemMathTypeName = "global::System.Math";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported Math method or overload is used",
            "Unsupported Math method or overload: {0}",
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
                methodSymbol.ContainingType?.ToDisplayString(FullyQualifiedFormat) != SystemMathTypeName ||
                IsSupportedMathMethod(methodSymbol))
                return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.ToDisplayString(DiagnosticDisplayFormat));

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsSupportedMathMethod(IMethodSymbol method)
        {
            return method.Name switch
            {
                "Abs" or "Sign" => HasUniformParameterType(method, 1, SignedIntegralTypes),
                "Max" or "Min" or "DivRem" => HasUniformParameterType(method, 2, IntegralTypes),
                "Clamp" => HasUniformParameterType(method, 3, IntegralTypes),
                "BigMul" => HasUniformParameterType(method, 2, Int32Type),
                _ => false
            };
        }

        private static bool HasUniformParameterType(
            IMethodSymbol method,
            int parameterCount,
            ImmutableHashSet<SpecialType> supportedTypes)
        {
            if (method.Parameters.Length != parameterCount)
                return false;

            var parameterType = method.Parameters[0].Type.SpecialType;
            if (!supportedTypes.Contains(parameterType))
                return false;

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                if (method.Parameters[i].Type.SpecialType != parameterType ||
                    method.Parameters[i].RefKind != RefKind.None)
                    return false;
            }

            return true;
        }
    }
}
