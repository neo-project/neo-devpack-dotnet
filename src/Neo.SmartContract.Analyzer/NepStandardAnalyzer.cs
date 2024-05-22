using System;
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
using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SupportedStandardsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4021";
        private static readonly string Title = "Supported Standards";
        private static readonly string MessageFormat = "Standard format suggestion: '{0}'";
        private static readonly string Description = "Checks for the usage of supported NEP standards.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
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
                            var standardValue = literalExpression.Token.ValueText.ToUpper();
                            if (standardValue is "NEP11" or "NEP-11" or "NEP17" or "NEP-17")
                            {
                                var standard = standardValue is "NEP11" or "NEP-11" ? NepStandard.Nep11 : NepStandard.Nep17;
                                var expectedSyntax = SyntaxFactory.AttributeArgument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("NepStandard"),
                                        SyntaxFactory.IdentifierName(standard.ToString())));

                                if (!argumentList.Arguments[0].Expression.IsEquivalentTo(expectedSyntax.Expression))
                                {
                                    var suggestionMessage = $"Consider using [SupportedStandards(NepStandard.{standard})]";
                                    var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(), suggestionMessage);
                                    context.ReportDiagnostic(diagnostic);
                                }
                            }
                            else if (!IsSupportedStandard(standardValue))
                            {
                                var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(), standardValue);
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsSupportedStandard(string value)
        {
            return Enum.TryParse<NepStandard>(value, out _);
        }

        private string GetSuggestionMessage(NepStandard standard)
        {
            return $"Consider using [SupportedStandards(NepStandard.{standard})]";
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
            var attributeSyntax = root?.FindNode(diagnosticSpan)?.AncestorsAndSelf()?.OfType<AttributeSyntax>()?.FirstOrDefault();

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
                        newAttributeSyntax = attributeSyntax.WithArgumentList(SyntaxFactory.AttributeArgumentList().AddArguments(SyntaxFactory.AttributeArgument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("NepStandard"), SyntaxFactory.IdentifierName("Nep11")))));
                    }
                    else if (standardValue == "NEP17")
                    {
                        newAttributeSyntax = attributeSyntax.WithArgumentList(SyntaxFactory.AttributeArgumentList().AddArguments(SyntaxFactory.AttributeArgument(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("NepStandard"), SyntaxFactory.IdentifierName("Nep17")))));
                    }
                }
            }

            var newRoot = root!.ReplaceNode(attributeSyntax, newAttributeSyntax);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
