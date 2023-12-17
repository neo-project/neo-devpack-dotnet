using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BigIntegerCreationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC3008";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Use of BigInteger constructor",
            "Use of new BigInteger(int) is not allowed, please use BigInteger x = 0;",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            // Check if it is a BigInteger creation
            if (context.SemanticModel.GetTypeInfo(objectCreationExpression).Type?.ToString() == "System.Numerics.BigInteger" &&
                objectCreationExpression.ArgumentList.Arguments.Count == 1 &&
                context.SemanticModel.GetTypeInfo(objectCreationExpression.ArgumentList.Arguments[0].Expression).Type?.SpecialType == SpecialType.System_Int32)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
