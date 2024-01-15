using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DecimalUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4003";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of decimal is not allowed in neo contract",
            "Neo contract does not support the decimal data type: {0}",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.PredefinedType);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var predefinedTypeSyntax = (PredefinedTypeSyntax)context.Node;

            if (predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.DecimalKeyword))
            {
                var diagnostic = Diagnostic.Create(Rule, predefinedTypeSyntax.GetLocation(), predefinedTypeSyntax.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
