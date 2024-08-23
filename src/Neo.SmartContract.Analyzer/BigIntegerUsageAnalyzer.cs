using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BigIntegerUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4006";

        private readonly string[] _unsupportedBigIntegerMethods = {
            "BitwiseAnd", "BitwiseOr",
            "Exp", "LeftShift",
            "Log10", "OnesComplement",
            "RightShift",
            "Xor"
        };

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
            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol memberSymbol &&
                memberSymbol.ContainingType?.ToString() == "System.Numerics.BigInteger" &&
                _unsupportedBigIntegerMethods.Contains(memberSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), memberSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeMemberAccessExpression(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            if (context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol is IPropertySymbol propertySymbol &&
                propertySymbol.ContainingType?.ToString() == "System.Numerics.BigInteger" &&
                _unsupportedBigIntegerMethods.Contains(propertySymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation(), propertySymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
