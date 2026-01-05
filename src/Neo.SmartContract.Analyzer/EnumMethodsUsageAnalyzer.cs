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
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumMethodsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4025";

        private readonly string[] _unsupportedEnumMethods = {
            "Format",
            "GetUnderlyingType"
        };

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

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (methodSymbol is not { ContainingType.SpecialType: SpecialType.System_Enum } ||
                !_unsupportedEnumMethods.Contains(methodSymbol.Name)) return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
