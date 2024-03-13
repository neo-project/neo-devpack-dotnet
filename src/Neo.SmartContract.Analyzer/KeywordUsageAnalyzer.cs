using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class KeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4015";
        private static readonly string Title = "Restricted keyword usage";
        private static readonly string MessageFormat = "Use of '{0}' is not allowed";
        private static readonly string Description = "This keyword is restricted in this project.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        private static readonly string[] bannedKeywords = new[] { "lock", "fixed", "unsafe", "stackalloc", "await", "dynamic", "unmanaged", "select", "orderby", "nameof", "implicit", "explicit", "yield", "where" };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode,
                SyntaxKind.LockStatement,
                SyntaxKind.FixedStatement,
                SyntaxKind.UnsafeStatement,
                SyntaxKind.StackAllocArrayCreationExpression,
                SyntaxKind.AwaitExpression,
                SyntaxKind.QueryExpression,
                SyntaxKind.YieldKeyword,
                SyntaxKind.InvocationExpression,
                SyntaxKind.WhereClause,
                SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.ToString();
            foreach (var keyword in bannedKeywords)
            {
                if (nodeText.Contains(keyword))
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), keyword);
                    context.ReportDiagnostic(diagnostic);
                    break;
                }
            }
        }

        private void AnalyzeType(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IdentifierNameSyntax identifierName &&
                identifierName.Identifier.ValueText == "dynamic")
            {
                var typeInfo = context.SemanticModel.GetTypeInfo(identifierName);
                if (typeInfo.Type != null && typeInfo.Type.TypeKind == TypeKind.Dynamic)
                {
                    var diagnostic = Diagnostic.Create(Rule, identifierName.GetLocation(), "dynamic");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
