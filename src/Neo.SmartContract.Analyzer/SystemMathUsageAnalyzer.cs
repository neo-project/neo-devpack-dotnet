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

        private static readonly ImmutableHashSet<string> UnsupportedMathMethods = ImmutableHashSet.Create(
            "Acos", "Asin", "Atan", "Atan2", "Ceiling", "Cos", "Cosh",
            "Exp", "Floor", "IEEERemainder", "Log", "Log10", "Pow", "Round",
            "Sin", "Sinh", "Sqrt", "Tan", "Tanh",
            "Truncate");

        private static readonly SymbolDisplayFormat FullyQualifiedFormat =
            SymbolDisplayFormat.FullyQualifiedFormat;

        private const string SystemMathTypeName = "global::System.Math";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported Math method is used",
            "Unsupported Math method: {0}",
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
                !UnsupportedMathMethods.Contains(methodSymbol.Name))
                return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                methodSymbol.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
