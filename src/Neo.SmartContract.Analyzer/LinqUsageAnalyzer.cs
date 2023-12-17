using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LinqUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC3011";
        private static readonly string Title = "LINQ usage";
        private static readonly string MessageFormat = "LINQ method '{0}' is not supported";
        private static readonly string Description = "LINQ methods are not supported in neo smart contract.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,  // Or DiagnosticSeverity.Error
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression, SyntaxKind.QueryExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Check for LINQ method calls
            if (context.Node is InvocationExpressionSyntax invocationExpression &&
                invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpr)
            {
                var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
                if (IsLinqMethod(memberSymbol))
                {
                    var diagnostic = Diagnostic.Create(Rule, memberAccessExpr.Name.GetLocation(), memberAccessExpr.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
            // Check for LINQ query syntax
            else if (context.Node is QueryExpressionSyntax queryExpression)
            {
                var diagnostic = Diagnostic.Create(Rule, queryExpression.GetLocation(), "query");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsLinqMethod(IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                return false;

            var containingNamespace = methodSymbol.ContainingNamespace;
            if (containingNamespace == null)
                return false;

            // Check if the namespace is 'System.Linq'
            return containingNamespace.Name == "Linq" &&
                   containingNamespace.ContainingNamespace != null &&
                   containingNamespace.ContainingNamespace.Name == "System";
        }
    }
}
