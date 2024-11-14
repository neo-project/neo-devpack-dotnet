using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StoredPropertyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4028";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Invalid StoredAttribute usage",
            "{0}",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "StoredAttribute can only be applied to static properties.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Attribute);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;

            // Check if it's a StoredAttribute
            var attributeName = attributeSyntax.Name.ToString();
            if (!attributeName.Contains("Stored"))
                return;

            // Get the parent property declaration
            var propertyDecl = attributeSyntax.Parent?.Parent as PropertyDeclarationSyntax;
            if (propertyDecl == null)
            {
                // Attribute is not on a property
                var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(),
                    "StoredAttribute can only be applied to properties.");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            // Check if the property is static
            if (!propertyDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(),
                    "StoredAttribute can only be applied to static properties.");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StoredPropertyCodeFixProvider)), Shared]
    public class StoredPropertyCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(StoredPropertyAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() =>
            WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var attribute = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeSyntax>().First();
            var propertyDecl = attribute.Parent?.Parent as PropertyDeclarationSyntax;

            if (propertyDecl == null)
            {
                // If attribute is not on a property, offer to remove it
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Remove StoredAttribute",
                        createChangedDocument: c => RemoveAttributeAsync(context.Document, attribute, c),
                        equivalenceKey: "RemoveStoredAttribute"),
                    diagnostic);
            }
            else if (!propertyDecl.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                // If property is not static, offer to make it static
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Make property static",
                        createChangedDocument: c => MakePropertyStaticAsync(context.Document, propertyDecl, c),
                        equivalenceKey: "MakePropertyStatic"),
                    diagnostic);
            }
        }

        private async Task<Document> RemoveAttributeAsync(Document document, AttributeSyntax attribute, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root.RemoveNode(attribute.Parent, SyntaxRemoveOptions.KeepNoTrivia);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> MakePropertyStaticAsync(Document document, PropertyDeclarationSyntax propertyDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newProperty = propertyDecl.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            var newRoot = root.ReplaceNode(propertyDecl, newProperty);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
