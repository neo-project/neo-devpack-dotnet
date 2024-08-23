using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

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

        private static readonly ImmutableArray<SyntaxKind> bannedSyntaxKinds = ImmutableArray.Create(
            SyntaxKind.LockKeyword,
            SyntaxKind.FixedKeyword,
            SyntaxKind.UnsafeKeyword,
            SyntaxKind.StackAllocKeyword,
            SyntaxKind.AwaitKeyword,
            SyntaxKind.UnmanagedKeyword,
            SyntaxKind.SelectKeyword,
            SyntaxKind.OrderByKeyword,
            SyntaxKind.ImplicitKeyword,
            SyntaxKind.ExplicitKeyword,
            SyntaxKind.YieldKeyword,
            SyntaxKind.WhereKeyword
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, bannedSyntaxKinds);
            context.RegisterSyntaxNodeAction(AnalyzeDynamicType, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var token = context.Node switch
            {
                LockStatementSyntax lockStmt => lockStmt.LockKeyword,
                FixedStatementSyntax fixedStmt => fixedStmt.FixedKeyword,
                UnsafeStatementSyntax unsafeStmt => unsafeStmt.UnsafeKeyword,
                StackAllocArrayCreationExpressionSyntax stackAlloc => stackAlloc.StackAllocKeyword,
                AwaitExpressionSyntax awaitExpr => awaitExpr.AwaitKeyword,
                QueryExpressionSyntax queryExpr => queryExpr.FromClause.FromKeyword,
                YieldStatementSyntax yieldStmt => yieldStmt.YieldKeyword,
                WhereClauseSyntax whereClause => whereClause.WhereKeyword,
                ConversionOperatorDeclarationSyntax convOp => convOp.ImplicitOrExplicitKeyword,
                _ => default
            };

            if (token != default)
            {
                var diagnostic = Diagnostic.Create(Rule, token.GetLocation(), token.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeDynamicType(SyntaxNodeAnalysisContext context)
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
