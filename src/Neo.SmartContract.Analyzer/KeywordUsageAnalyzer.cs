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
                SyntaxKind.PointerType,
                SyntaxKind.QueryExpression,
                SyntaxKind.ImplicitKeyword,
                SyntaxKind.ExplicitKeyword,
                SyntaxKind.YieldKeyword,
                SyntaxKind.WhereKeyword);
            context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // Logic to check for the presence of the banned keywords
            // and report a diagnostic if found
            var nodeText = context.Node.ToString();
            foreach (var keyword in bannedKeywords)
            {
                if (!nodeText.Contains(keyword)) continue;
                var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), keyword);
                context.ReportDiagnostic(diagnostic);
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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveKeywordsCodeFixProvider))]
    public class RemoveKeywordsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(KeywordUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root?.FindToken(diagnosticSpan.Start).Parent;
            if (node is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Remove restricted keyword",
                    createChangedDocument: c => RemoveKeywordAsync(context.Document, node, c),
                    equivalenceKey: "Remove restricted keyword"),
                diagnostic);
        }

        private static async Task<Document> RemoveKeywordAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
        {
            var editor = new SyntaxEditor(await document.GetSyntaxRootAsync(cancellationToken), document.Project.Solution.Workspace);

            switch (node)
            {
                case LockStatementSyntax lockStatement:
                    editor.ReplaceNode(lockStatement, lockStatement.Statement);
                    break;
                case FixedStatementSyntax fixedStatement:
                    editor.ReplaceNode(fixedStatement, fixedStatement.Statement);
                    break;
                case UnsafeStatementSyntax unsafeStatement:
                    if (unsafeStatement.Parent is BlockSyntax parentBlock)
                    {
                        var newStatements = parentBlock.Statements.ReplaceRange(unsafeStatement, unsafeStatement.Block.Statements);
                        var newParentBlock = parentBlock.WithStatements(newStatements);
                        editor.ReplaceNode(parentBlock, newParentBlock);
                    }
                    break;
                case StackAllocArrayCreationExpressionSyntax stackAllocExpression:
                    editor.RemoveNode(stackAllocExpression);
                    break;
                case LocalFunctionStatementSyntax localFunction:
                    var unmanagedToken = localFunction.Modifiers.FirstOrDefault(m => m.IsKind(SyntaxKind.UnmanagedKeyword));
                    if (unmanagedToken != default)
                    {
                        var newModifiers = localFunction.Modifiers.Remove(unmanagedToken);
                        editor.ReplaceNode(localFunction, localFunction.WithModifiers(newModifiers));
                    }
                    break;
                case QueryExpressionSyntax queryExpression:
                    editor.RemoveNode(queryExpression);
                    break;
                case YieldStatementSyntax yieldStatement:
                    editor.RemoveNode(yieldStatement);
                    break;
                case InvocationExpressionSyntax invocationExpr when invocationExpr.Expression.ToString() == "nameof":
                    if (invocationExpr.ArgumentList.Arguments.Count == 1 &&
                        invocationExpr.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax identifierName)
                    {
                        var nameLiteral = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(identifierName.Identifier.ValueText));
                        editor.ReplaceNode(invocationExpr, nameLiteral);
                    }
                    break;
                case WhereClauseSyntax whereClause:
                    editor.RemoveNode(whereClause);
                    break;
            }
            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
