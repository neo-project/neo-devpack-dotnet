using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BigIntegerUsingUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4022";
        private const string Title = "Incorrect BigInteger using usage";
        private const string MessageFormat = "Use 'using BigInteger = System.Numerics.BigInteger;' instead of '{0}'";
        private const string Description = "Ensures that 'using BigInteger = System.Numerics.BigInteger;' is used for BigInteger.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, Microsoft.CodeAnalysis.CSharp.SyntaxKind.UsingDirective);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;

            if (usingDirective is { Alias: not null, Name: not null } && usingDirective.Alias.Name.ToString() == "BigInteger")
            {
                var symbolInfo = ModelExtensions.GetSymbolInfo(context.SemanticModel, usingDirective.Name);
                var symbol = symbolInfo.Symbol;

                if (symbol != null && symbol.ToString() != "System.Numerics.BigInteger")
                {
                    var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), usingDirective.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BigIntegerUsingUsageCodeFixProvider)), Shared]
    public class BigIntegerUsingUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(BigIntegerUsingUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root!.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use 'using BigInteger = System.Numerics.BigInteger;'",
                    createChangedDocument: c => UseSystemNumericsBigInteger(context.Document, declaration, c),
                    equivalenceKey: "Use 'using BigInteger = System.Numerics.BigInteger;'"),
                diagnostic);
        }

        private async Task<Document> UseSystemNumericsBigInteger(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newUsingDirective = SyntaxFactory.UsingDirective(
                SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("BigInteger")),
                SyntaxFactory.ParseName("System.Numerics.BigInteger"));

            var newRoot = root!.ReplaceNode(usingDirective, newUsingDirective);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
