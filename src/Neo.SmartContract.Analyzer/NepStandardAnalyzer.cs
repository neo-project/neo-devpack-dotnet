using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Neo.SmartContract.Analyzer
{
    enum NEPStandard
    {
        NEP11,
        NEP17,
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SupportedStandardsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4021";
        private static readonly string Title = "Supported Standards";
        private static readonly string MessageFormat = "Unsupported standard '{0}' used";
        private static readonly string Description = "Checks for the usage of unsupported NEP standards.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error, // Set the severity to Error
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Attribute);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is AttributeSyntax attributeSyntax)
            {
                var attributeName = attributeSyntax.Name.ToString();
                if (attributeName == "SupportedStandards")
                {
                    var argumentList = attributeSyntax.ArgumentList;
                    if (argumentList != null && argumentList.Arguments.Count > 0)
                    {
                        var argument = argumentList.Arguments[0].Expression;
                        if (argument is LiteralExpressionSyntax literalExpression)
                        {
                            var standardValue = literalExpression.Token.ValueText;
                            if (!IsSupportedStandard(standardValue))
                            {
                                var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(), standardValue);
                                context.ReportDiagnostic(diagnostic);
                            }
                            else if (standardValue == "NEP11" || standardValue == "NEP17")
                            {
                                var standard = standardValue == "NEP11" ? NEPStandard.NEP11 : NEPStandard.NEP17;
                                var suggestionMessage = GetSuggestionMessage(standard);
                                var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(), suggestionMessage);
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private bool IsSupportedStandard(string value)
        {
            return value == "NEP-11" || value == "NEP11" || value == "NEP-17" || value == "NEP17";
        }

        private string GetSuggestionMessage(NEPStandard standard)
        {
            return standard == NEPStandard.NEP11 ? "Consider using [SupportedStandards(NEPStandard.NEP11)]" : "Consider using [SupportedStandards(NEPStandard.NEP17)]";
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SupportedStandardsCodeFixProvider)), Shared]
    public class SupportedStandardsCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Update Supported Standards";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SupportedStandardsAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var attributeSyntax = root.FindNode(diagnosticSpan).AncestorsAndSelf().OfType<AttributeSyntax>().FirstOrDefault();

            if (attributeSyntax != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Title,
                        createChangedDocument: c => UpdateSupportedStandards(context.Document, attributeSyntax, c),
                        equivalenceKey: Title),
                    diagnostic);
            }
        }

        private async Task<Document> UpdateSupportedStandards(Document document, AttributeSyntax attributeSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newAttributeSyntax = attributeSyntax;
            var argumentList = attributeSyntax.ArgumentList;
            if (argumentList != null && argumentList.Arguments.Count > 0)
            {
                var argument = argumentList.Arguments[0].Expression;
                if (argument is LiteralExpressionSyntax literalExpression)
                {
                    var standardValue = literalExpression.Token.ValueText;
                    if (standardValue == "NEP11")
                    {
                        newAttributeSyntax = attributeSyntax.WithArgumentList(SyntaxFactory.AttributeArgumentList().AddArguments(SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("NEP-11")))));
                    }
                    else if (standardValue == "NEP17")
                    {
                        newAttributeSyntax = attributeSyntax.WithArgumentList(SyntaxFactory.AttributeArgumentList().AddArguments(SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("NEP-17")))));
                    }
                }
            }

            var newRoot = root.ReplaceNode(attributeSyntax, newAttributeSyntax);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
