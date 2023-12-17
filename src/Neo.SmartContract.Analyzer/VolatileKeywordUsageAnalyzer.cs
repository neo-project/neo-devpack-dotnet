using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VolatileKeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4014";
        private static readonly string Title = "Volatile keyword usage";
        private static readonly string MessageFormat = "'volatile' keyword used in {0}";
        private static readonly string Description = "Checks for the usage of 'volatile' keywords.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not FieldDeclarationSyntax fieldDeclaration) return;
            foreach (var diagnostic in from variable in fieldDeclaration.Declaration.Variables where fieldDeclaration.Modifiers.Any(SyntaxKind.VolatileKeyword) select Diagnostic.Create(Rule, variable.GetLocation(), "field declaration"))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VolatileKeywordRemovalCodeFixProvider))]
    public class VolatileKeywordRemovalCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(VolatileKeywordUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Remove 'volatile' keyword",
                    createChangedDocument: c => RemoveVolatileKeywordAsync(context.Document, declaration, c),
                    equivalenceKey: "Remove 'volatile' keyword"),
                diagnostic);
        }

        private async Task<Document> RemoveVolatileKeywordAsync(Document document, FieldDeclarationSyntax fieldDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = new SyntaxEditor(root, document.Project.Solution.Workspace);

            var newModifiers = fieldDecl.Modifiers.Where(m => !m.IsKind(SyntaxKind.VolatileKeyword));
            var newFieldDecl = fieldDecl.WithModifiers(SyntaxFactory.TokenList(newModifiers));

            editor.ReplaceNode(fieldDecl, newFieldDecl);

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
