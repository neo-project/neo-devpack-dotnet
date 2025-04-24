// Copyright (C) 2015-2025 The Neo Project.
//
// CheckWitnessUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class CheckWitnessUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4029";

        private static readonly LocalizableString Title = "CheckWitness result should be used";
        private static readonly LocalizableString MessageFormat = "The result of CheckWitness should be used: {0}";
        private static readonly LocalizableString Description = "CheckWitness results should be used to ensure proper authentication.";
        private const string Category = "Security";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            // Check if this is a call to Runtime.CheckWitness
            if (IsCheckWitnessCall(invocation, context.SemanticModel))
            {
                // Check if the result is being used properly
                if (!IsResultVerified(invocation))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), "The result of Runtime.CheckWitness(...) should be used in a condition, assigned to a variable, or otherwise utilized");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private bool IsCheckWitnessCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (symbol == null) return false;

                // Check if it's Runtime.CheckWitness
                if (symbol.Name == "CheckWitness" &&
                    symbol.ContainingType.Name == "Runtime" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsResultVerified(InvocationExpressionSyntax invocation)
        {
            // Get the parent of the invocation
            var parent = invocation.Parent;

            // The only case where the result is not being used is when the invocation
            // is directly used as a statement without capturing the result
            if (parent is ExpressionStatementSyntax)
            {
                return false;
            }

            // In all other cases, the result is being used in some way
            // (as a condition, in an expression, assigned to a variable, etc.)
            return true;
        }
    }
}
