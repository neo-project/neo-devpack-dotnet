// Copyright (C) 2015-2025 The Neo Project.
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
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BitOperationsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4043";

        // BitOperations methods that are not supported in Neo smart contracts
        // Based on the methods implemented in src\Neo.Compiler.CSharp\MethodConvert\System\SystemCall.Register.cs
        private readonly string[] _unsupportedBitOperationsMethods = {
            "TrailingZeroCount",
            "IsPow2",
            "RotateRight"
            // Supported BitOperations methods (not in this list):
            // - LeadingZeroCount
            // - Log2
            // - PopCount
            // - RotateLeft
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported BitOperations method is used",
            "Unsupported BitOperations method: {0}",
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

            // Check if it's a BitOperations method
            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Expression is IdentifierNameSyntax identifier &&
                identifier.Identifier.Text == "BitOperations")
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (_unsupportedBitOperationsMethods.Contains(methodName))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), methodName);
                    context.ReportDiagnostic(diagnostic);
                }
                return;
            }

            // Check if it's a method from System.Numerics.BitOperations
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (methodSymbol != null &&
                methodSymbol.ContainingType?.ToString() == "System.Numerics.BitOperations" &&
                _unsupportedBitOperationsMethods.Contains(methodSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
