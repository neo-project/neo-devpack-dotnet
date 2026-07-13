// Copyright (C) 2015-2026 The Neo Project.
//
// BigIntegerUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class BigIntegerUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4006";

        private const string BigIntegerMetadataName = "System.Numerics.BigInteger";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported BigInteger method is used",
            "Unsupported BigInteger method: {0}",
            "Method",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.InvocationExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case InvocationExpressionSyntax invocationExpression:
                    AnalyzeInvocationExpression(context, invocationExpression);
                    break;
                case MemberAccessExpressionSyntax memberAccessExpression:
                    AnalyzeMemberAccessExpression(context, memberAccessExpression);
                    break;
            }
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol methodSymbol)
                AnalyzeMethod(context, methodSymbol, invocationExpression.GetLocation());
        }

        private void AnalyzeMemberAccessExpression(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            if (memberAccessExpression.Parent is InvocationExpressionSyntax invocationExpression &&
                invocationExpression.Expression == memberAccessExpression)
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is IMethodSymbol methodSymbol)
                AnalyzeMethod(context, methodSymbol, memberAccessExpression.GetLocation());
        }

        private static void AnalyzeMethod(
            SyntaxNodeAnalysisContext context,
            IMethodSymbol method,
            Location location)
        {
            var bigIntegerType = context.Compilation.GetTypeByMetadataName(BigIntegerMetadataName);
            if (bigIntegerType is null ||
                !SymbolEqualityComparer.Default.Equals(method.ContainingType, bigIntegerType) ||
                IsSupportedBigIntegerMethod(method, bigIntegerType))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, location, method.Name));
        }

        private static bool IsSupportedBigIntegerMethod(IMethodSymbol method, ITypeSymbol bigIntegerType)
        {
            if (!method.IsStatic)
            {
                return method.Name switch
                {
                    "ToByteArray" or "ToString" => method.Parameters.Length == 0,
                    "Equals" => method.Parameters.Length == 1 &&
                                (IsParameter(method, 0, bigIntegerType) ||
                                 IsSpecialParameter(method, 0, SpecialType.System_Int64) ||
                                 IsSpecialParameter(method, 0, SpecialType.System_UInt64) ||
                                 IsSpecialParameter(method, 0, SpecialType.System_Object)),
                    _ => false
                };
            }

            if (method.IsGenericMethod)
                return IsSupportedCreateMethod(method, bigIntegerType);

            return method.Name switch
            {
                "Pow" => HasParameters(method, bigIntegerType, SpecialType.System_Int32),
                "ModPow" or "Clamp" => HasParameters(method, bigIntegerType, bigIntegerType, bigIntegerType),
                "Add" or "Subtract" or "Multiply" or "Divide" or "Remainder" or "Compare" or
                    "GreatestCommonDivisor" or "DivRem" or "CopySign" or "Max" or "Min" =>
                    HasParameters(method, bigIntegerType, bigIntegerType),
                "Negate" or "IsEvenInteger" or "IsOddInteger" or "IsNegative" or "IsPositive" or
                    "IsPow2" or "LeadingZeroCount" or "Log2" or "PopCount" or "Abs" =>
                    HasParameters(method, bigIntegerType),
                "Parse" => HasParameters(method, SpecialType.System_String),
                "TryParse" => method.Parameters.Length == 2 &&
                              IsSpecialParameter(method, 0, SpecialType.System_String) &&
                              IsParameter(method, 1, bigIntegerType, RefKind.Out),
                _ => false
            };
        }

        private static bool IsSupportedCreateMethod(IMethodSymbol method, ITypeSymbol bigIntegerType)
        {
            if (method.TypeArguments.Length != 1 || method.Parameters.Length != 1)
                return false;

            var sourceType = method.TypeArguments[0];
            if (!IsParameter(method, 0, sourceType))
                return false;

            return method.Name switch
            {
                "CreateChecked" or "CreateTruncating" => IsSupportedCreateSource(sourceType, bigIntegerType),
                "CreateSaturating" => SymbolEqualityComparer.Default.Equals(sourceType, bigIntegerType),
                _ => false
            };
        }

        private static bool IsSupportedCreateSource(ITypeSymbol sourceType, ITypeSymbol bigIntegerType)
        {
            return SymbolEqualityComparer.Default.Equals(sourceType, bigIntegerType) ||
                   sourceType.SpecialType is SpecialType.System_Byte or
                       SpecialType.System_SByte or
                       SpecialType.System_Int16 or
                       SpecialType.System_UInt16 or
                       SpecialType.System_Int32 or
                       SpecialType.System_UInt32 or
                       SpecialType.System_Int64 or
                       SpecialType.System_UInt64 or
                       SpecialType.System_Char;
        }

        private static bool HasParameters(IMethodSymbol method, params ITypeSymbol[] parameterTypes)
        {
            if (method.Parameters.Length != parameterTypes.Length)
                return false;

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                if (!IsParameter(method, i, parameterTypes[i]))
                    return false;
            }

            return true;
        }

        private static bool HasParameters(IMethodSymbol method, ITypeSymbol firstType, SpecialType secondType)
        {
            return method.Parameters.Length == 2 &&
                   IsParameter(method, 0, firstType) &&
                   IsSpecialParameter(method, 1, secondType);
        }

        private static bool HasParameters(IMethodSymbol method, SpecialType parameterType)
        {
            return method.Parameters.Length == 1 &&
                   IsSpecialParameter(method, 0, parameterType);
        }

        private static bool IsSpecialParameter(
            IMethodSymbol method,
            int index,
            SpecialType parameterType,
            RefKind refKind = RefKind.None)
        {
            return index < method.Parameters.Length &&
                   method.Parameters[index].RefKind == refKind &&
                   method.Parameters[index].Type.SpecialType == parameterType;
        }

        private static bool IsParameter(
            IMethodSymbol method,
            int index,
            ITypeSymbol parameterType,
            RefKind refKind = RefKind.None)
        {
            return index < method.Parameters.Length &&
                   method.Parameters[index].RefKind == refKind &&
                   SymbolEqualityComparer.Default.Equals(method.Parameters[index].Type, parameterType);
        }
    }
}
