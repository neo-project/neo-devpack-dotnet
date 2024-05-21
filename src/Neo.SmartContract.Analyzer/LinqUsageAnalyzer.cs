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
    public class LinqUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4011";
        private static readonly string Title = "LINQ usage";
        private static readonly string MessageFormat = "Use Neo.SmartContract.Framework.Linq instead of System.Linq: {0}";
        private static readonly string Description = "System.Linq is not supported in neo smart contract. Use Neo.SmartContract.Framework.Linq instead.";
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
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;
            if (usingDirective.Name!.ToString() == "System.Linq")
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), usingDirective.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LinqUsageCodeFixProvider)), Shared]
    public class LinqUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(LinqUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var usingDirective = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().First();
            if (usingDirective != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Use Neo.SmartContract.Framework.Linq",
                        createChangedDocument: c => FixSystemLinqUsage(context.Document, usingDirective, c),
                        equivalenceKey: "Use Neo.SmartContract.Framework.Linq"),
                    diagnostic);
            }
        }

        private static async Task<Document> FixSystemLinqUsage(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newUsingDirective = usingDirective.WithName(SyntaxFactory.ParseName("Neo.SmartContract.Framework.Linq"));
            var newRoot = root!.ReplaceNode(usingDirective, newUsingDirective);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
