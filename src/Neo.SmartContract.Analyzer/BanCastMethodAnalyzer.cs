using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BanCastMethodAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4017";
        private const string Title = "Do not use .Cast<T>() method";
        private const string MessageFormat = "Use of .Cast<T>() method is not allowed";
        private const string Description = "Replace .Cast<T>() with direct type casting.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            if (invocationExpr.Expression is MemberAccessExpressionSyntax memberAccessExpr &&
                memberAccessExpr.Name.Identifier.ValueText == "Cast" &&
                memberAccessExpr.Expression != null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccessExpr.Name.GetLocation()));
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BanCastMethodCodeFixProvider))]
    public class BanCastMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(BanCastMethodAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocationExpr = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();
            if (invocationExpr is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with direct type casting",
                    createChangedDocument: c => ReplaceWithDirectCastAsync(context.Document, invocationExpr, c),
                    equivalenceKey: "Replace with direct type casting"),
                diagnostic);
        }

        private static async Task<Document> ReplaceWithDirectCastAsync(Document document, InvocationExpressionSyntax invocationExpr, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeArgument = invocationExpr.DescendantNodes().OfType<TypeArgumentListSyntax>().FirstOrDefault()?.Arguments.FirstOrDefault();
            if (typeArgument is null) return document;

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            if (root is null) return document;

            var newExpr = SyntaxFactory.ParenthesizedExpression(
                SyntaxFactory.CastExpression(typeArgument, invocationExpr.Expression)
            );

            var newRoot = root.ReplaceNode(invocationExpr, newExpr);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
