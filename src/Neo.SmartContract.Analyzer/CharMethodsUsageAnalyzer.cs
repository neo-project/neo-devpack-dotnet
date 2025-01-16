// Copyright (C) 2015-2024 The Neo Project.
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
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CharMethodsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4012";

        private readonly string[] _unsupportedCharMethods = {
            "CompareTo", "Equals", "GetHashCode",
            "GetType", "GetTypeCode",
            "IsNumber", "IsSeparator", "Parse",
            "ToLowerInvariant",
            "ToUpperInvariant", "TryParse",
            "ConvertFromUtf32", "ConvertToUtf32",
            "GetUnicodeCategory",
            "IsSurrogatePair"
        };

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

            // Get the invoked method symbol
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            // Check if the method symbol belongs to the 'char' type and is in the list of unsupported methods
            if (methodSymbol is not { ContainingType.SpecialType: SpecialType.System_Char } ||
                !_unsupportedCharMethods.Contains(methodSymbol.Name)) return;
            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
