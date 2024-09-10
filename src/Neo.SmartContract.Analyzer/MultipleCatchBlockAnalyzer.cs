using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MultipleCatchBlockAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4024";

        private static readonly LocalizableString Title = "Multiple catch blocks are not allowed in Neo smart contracts";
        private static readonly LocalizableString MessageFormat = "Neo smart contracts only support a single catch block: {0}";
        private static readonly LocalizableString Description = "Neo smart contracts are limited to one catch block per try statement.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);
        }

        private static void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            var tryStatement = (TryStatementSyntax)context.Node;

            if (tryStatement.Catches.Count > 1)
            {
                var diagnostic = Diagnostic.Create(Rule, tryStatement.GetLocation(), tryStatement.Catches.Count);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
