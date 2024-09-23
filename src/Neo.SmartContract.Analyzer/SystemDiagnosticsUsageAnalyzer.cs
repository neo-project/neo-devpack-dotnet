using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemDiagnosticsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4025";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of System.Diagnostics is not allowed in Neo smart contracts",
            "Neo smart contracts do not support System.Diagnostics: {0}",
            "Namespace",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.UsingDirective, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case UsingDirectiveSyntax usingDirective:
                    AnalyzeUsingDirective(context, usingDirective);
                    break;
                case IdentifierNameSyntax identifierName:
                    AnalyzeIdentifierName(context, identifierName);
                    break;
            }
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            var name = usingDirective.Name?.ToString();
            if (name == "System.Diagnostics" || name?.StartsWith("System.Diagnostics.") == true)
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierName)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName);
            var symbol = symbolInfo.Symbol;

            if (symbol != null && symbol.ContainingNamespace != null)
            {
                var namespaceName = symbol.ContainingNamespace.ToDisplayString();
                if (namespaceName == "System.Diagnostics" || namespaceName.StartsWith("System.Diagnostics."))
                {
                    var diagnostic = Diagnostic.Create(Rule, identifierName.GetLocation(), namespaceName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
