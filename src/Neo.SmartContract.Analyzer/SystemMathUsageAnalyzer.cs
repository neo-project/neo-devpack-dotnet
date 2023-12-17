using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemMathUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC3005";

        private readonly string[] _unsupportedMathMethods =
        {
            "Abs", "Acos", "Asin", "Atan", "Atan2",
            "BigMul", "Ceiling", "Cos", "Cosh", "DivRem",
            "Exp", "Floor", "IEEERemainder", "Log", "Log10", "Pow", "Round",
            "Sin", "Sinh", "Sqrt", "Tan", "Tanh",
            "Truncate"
        };

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

            if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccess ||
                memberAccess.Expression is not IdentifierNameSyntax identifier ||
                !identifier.Identifier.Text.Equals("Math") ||
                !_unsupportedMathMethods.Contains(memberAccess.Name.Identifier.Text)) return;

            var diagnostic = Diagnostic.Create(Rule,
                invocationExpression.GetLocation(),
                memberAccess.Name.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
