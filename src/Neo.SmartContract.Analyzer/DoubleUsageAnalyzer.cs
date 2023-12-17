using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.Operations;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoubleUsageAnalyzer : DiagnosticAnalyzer
    {
        // public const string FloatingPointNumber = "NC2004";
        public const string DiagnosticId = "NC3004";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of double is not allowed in neo contract",
            "Neo contract does not support double data type: {0}",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.VariableDeclaration);
        }

        private static void AnalyzeOperation(OperationAnalysisContext context)
        {
            if (context.Operation is not IVariableDeclarationOperation variableDeclaration) return;
            var variableType = variableDeclaration.GetDeclaredVariables()[0].Type;
            if (variableDeclaration.GetDeclaredVariables().All(p => p.Type.SpecialType != SpecialType.System_Double)) return;

            var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
