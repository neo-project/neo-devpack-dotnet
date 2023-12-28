using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringMethodUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4007";

        // Add string method names to this array as needed
        private readonly string[] _unsupportedStringMethods =
        {
            "Clone", "Compare", "CompareOrdinal", "CompareTo",
            "Concat", "Contains", "Copy", "CopyTo",
            "EndsWith", "Equals", "Format", "GetEnumerator",
            "GetHashCode", "GetType", "GetTypeCode", "IndexOf",
            "IndexOfAny", "Insert", "Intern", "IsInterned",
            "IsNormalized", "Join", "LastIndexOf", "LastIndexOfAny",
            "Normalize", "PadLeft", "PadRight", "Remove",
            "Replace", "Split", "StartsWith", "Substring",
            "ToCharArray", "ToLower", "ToLowerInvariant", "ToString",
            "ToUpper", "ToUpperInvariant", "Trim", "TrimEnd",
            "TrimStart"
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported string method is used",
            "Unsupported string method: {0}",
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

            // Check if the method belongs to String class or is an Object method listed in _unsupportedStringMethods
            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol memberSymbol ||
                (memberSymbol.ContainingType?.SpecialType != SpecialType.System_String &&
                    memberSymbol.ContainingType?.SpecialType != SpecialType.System_Object) ||
                !_unsupportedStringMethods.Contains(memberSymbol.Name)) return;

            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), memberSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
