// Copyright (C) 2015-2026 The Neo Project.
//
// EnumMethodsUsageAnalyzer.cs file belongs to the neo project and is free
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
using System;
using System.Collections.Immutable;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumMethodsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4025";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported Enum method is used",
            "Unsupported Enum method: {0}",
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
                methodSymbol.ContainingType.SpecialType != SpecialType.System_Enum ||
                IsSupportedEnumMethod(methodSymbol))
                return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsSupportedEnumMethod(IMethodSymbol method)
        {
            if (!method.IsStatic)
            {
                return method.Name switch
                {
                    nameof(Enum.ToString) => method.Parameters.Length == 0,
                    nameof(Enum.HasFlag) => IsSpecialParameter(method, 0, SpecialType.System_Enum) &&
                                 method.Parameters.Length == 1,
                    _ => false
                };
            }

            if (method.IsGenericMethod)
                return IsSupportedGenericMethod(method);

            return method.Name switch
            {
                nameof(Enum.Parse) => IsParseSignature(method),
                nameof(Enum.TryParse) => IsTryParseSignature(method),
                nameof(Enum.GetNames) or nameof(Enum.GetValues) => method.Parameters.Length == 1 && IsSystemTypeParameter(method, 0),
                nameof(Enum.IsDefined) => method.Parameters.Length == 2 &&
                               IsSystemTypeParameter(method, 0) &&
                               (IsSpecialParameter(method, 1, SpecialType.System_Object) ||
                                IsSpecialParameter(method, 1, SpecialType.System_String)),
                nameof(Enum.GetName) => method.Parameters.Length == 2 &&
                             IsSystemTypeParameter(method, 0) &&
                             IsSpecialParameter(method, 1, SpecialType.System_Object),
                _ => false
            };
        }

        private static bool IsSupportedGenericMethod(IMethodSymbol method)
        {
            if (method.TypeArguments.Length != 1)
                return false;

            var enumType = method.TypeArguments[0];
            return method.Name switch
            {
                nameof(Enum.Parse) => IsGenericParseSignature(method),
                nameof(Enum.TryParse) => IsGenericTryParseSignature(method, enumType),
                nameof(Enum.GetNames) or nameof(Enum.GetValues) => method.Parameters.Length == 0,
                nameof(Enum.GetName) => method.Parameters.Length == 1 &&
                             IsParameter(method, 0, enumType),
                _ => false
            };
        }

        private static bool IsParseSignature(IMethodSymbol method)
        {
            return method.Parameters.Length is 2 or 3 &&
                   IsSystemTypeParameter(method, 0) &&
                   IsSpecialParameter(method, 1, SpecialType.System_String) &&
                   (method.Parameters.Length == 2 ||
                    IsSpecialParameter(method, 2, SpecialType.System_Boolean));
        }

        private static bool IsTryParseSignature(IMethodSymbol method)
        {
            return method.Parameters.Length is 3 or 4 &&
                   IsSystemTypeParameter(method, 0) &&
                   IsSpecialParameter(method, 1, SpecialType.System_String) &&
                   (method.Parameters.Length == 3 ||
                    IsSpecialParameter(method, 2, SpecialType.System_Boolean)) &&
                   IsSpecialParameter(
                       method,
                       method.Parameters.Length - 1,
                       SpecialType.System_Object,
                       RefKind.Out);
        }

        private static bool IsGenericParseSignature(IMethodSymbol method)
        {
            return method.Parameters.Length is 1 or 2 &&
                   IsSpecialParameter(method, 0, SpecialType.System_String) &&
                   (method.Parameters.Length == 1 ||
                    IsSpecialParameter(method, 1, SpecialType.System_Boolean));
        }

        private static bool IsGenericTryParseSignature(IMethodSymbol method, ITypeSymbol enumType)
        {
            return method.Parameters.Length is 2 or 3 &&
                   IsSpecialParameter(method, 0, SpecialType.System_String) &&
                   (method.Parameters.Length == 2 ||
                    IsSpecialParameter(method, 1, SpecialType.System_Boolean)) &&
                   IsParameter(method, method.Parameters.Length - 1, enumType, RefKind.Out);
        }

        private static bool IsSystemTypeParameter(IMethodSymbol method, int index)
        {
            if (index >= method.Parameters.Length || method.Parameters[index].RefKind != RefKind.None)
                return false;

            return method.Parameters[index].Type is INamedTypeSymbol { Name: "Type" } type &&
                   type.ContainingNamespace.ToDisplayString() == "System";
        }

        private static bool IsSpecialParameter(
            IMethodSymbol method,
            int index,
            SpecialType type,
            RefKind refKind = RefKind.None)
        {
            return index < method.Parameters.Length &&
                   method.Parameters[index].RefKind == refKind &&
                   method.Parameters[index].Type.SpecialType == type;
        }

        private static bool IsParameter(
            IMethodSymbol method,
            int index,
            ITypeSymbol type,
            RefKind refKind = RefKind.None)
        {
            return index < method.Parameters.Length &&
                   method.Parameters[index].RefKind == refKind &&
                   SymbolEqualityComparer.Default.Equals(method.Parameters[index].Type, type);
        }
    }
}
