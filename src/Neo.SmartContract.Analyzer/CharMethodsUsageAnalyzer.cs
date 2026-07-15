// Copyright (C) 2015-2026 The Neo Project.
//
// CharMethodsUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class CharMethodsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4012";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported Char method is used",
            "Unsupported Char method: {0}",
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

            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol methodSymbol)
                return;

            if (methodSymbol.ContainingType.SpecialType != SpecialType.System_Char ||
                IsSupportedCharMethod(methodSymbol))
                return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsSupportedCharMethod(IMethodSymbol method)
        {
            if (!method.IsStatic)
                return method.Name == "ToString" && HasParameters(method);

            return method.Name switch
            {
                "IsDigit" or
                "IsLetter" or
                "IsWhiteSpace" or
                "IsLower" or
                "ToLower" or
                "IsUpper" or
                "ToUpper" or
                "IsPunctuation" or
                "IsSymbol" or
                "IsControl" or
                "IsSurrogate" or
                "IsHighSurrogate" or
                "IsLowSurrogate" or
                "GetNumericValue" or
                "IsLetterOrDigit" or
                "ToLowerInvariant" or
                "ToUpperInvariant" or
                "IsAscii" or
                "IsAsciiDigit" or
                "IsAsciiLetter" => HasParameters(method, SpecialType.System_Char),
                "IsBetween" => HasParameters(
                    method,
                    SpecialType.System_Char,
                    SpecialType.System_Char,
                    SpecialType.System_Char),
                "Parse" => HasParameters(method, SpecialType.System_String),
                "TryParse" => HasTryParseParameters(method),
                _ => false
            };
        }

        private static bool HasParameters(IMethodSymbol method, params SpecialType[] parameterTypes)
        {
            if (method.Parameters.Length != parameterTypes.Length)
                return false;

            for (var i = 0; i < parameterTypes.Length; i++)
            {
                if (method.Parameters[i].RefKind != RefKind.None ||
                    method.Parameters[i].Type.SpecialType != parameterTypes[i])
                    return false;
            }

            return true;
        }

        private static bool HasTryParseParameters(IMethodSymbol method)
        {
            return method.Parameters.Length == 2 &&
                   method.Parameters[0].RefKind == RefKind.None &&
                   method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
                   method.Parameters[1].RefKind == RefKind.Out &&
                   method.Parameters[1].Type.SpecialType == SpecialType.System_Char;
        }
    }
}
