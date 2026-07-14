// Copyright (C) 2015-2026 The Neo Project.
//
// NepStandardAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SupportedStandardsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4021";
        internal const string StandardPropertyName = "Standard";
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
                    if (argumentList != null)
                    {
                        foreach (var attributeArgument in argumentList.Arguments)
                        {
                            if (attributeArgument.Expression is not LiteralExpressionSyntax literalExpression ||
                                !literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
                            {
                                continue;
                            }

                            var standardValue = literalExpression.Token.ValueText.ToUpperInvariant();
                            if (standardValue is "NEP11" or "NEP-11" or "NEP17" or "NEP-17")
                            {
                                var standard = standardValue is "NEP11" or "NEP-11" ? NepStandard.Nep11 : NepStandard.Nep17;
                                var suggestionMessage = $"Consider using [SupportedStandards(NepStandard.{standard})]";
                                var properties = ImmutableDictionary<string, string?>.Empty
                                    .Add(StandardPropertyName, standard.ToString());
                                var diagnostic = Diagnostic.Create(
                                    Rule,
                                    attributeArgument.Expression.GetLocation(),
                                    properties,
                                    suggestionMessage);
                                context.ReportDiagnostic(diagnostic);
                            }
                            else if (!IsSupportedStandard(standardValue))
                            {
                                var diagnostic = Diagnostic.Create(Rule, attributeArgument.Expression.GetLocation(), standardValue);
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsSupportedStandard(string value)
        {
            return Enum.TryParse<NepStandard>(value, ignoreCase: true, out _);
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
            var attributeArgument = root?.FindNode(diagnosticSpan, getInnermostNodeForTie: true)
                .AncestorsAndSelf()
                .OfType<AttributeArgumentSyntax>()
                .FirstOrDefault();

            if (attributeArgument is null ||
                !diagnostic.Properties.TryGetValue(SupportedStandardsAnalyzer.StandardPropertyName, out var standardName) ||
                !Enum.TryParse(standardName, out NepStandard standard))
                return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => UpdateSupportedStandards(context.Document, attributeArgument, standard, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> UpdateSupportedStandards(
            Document document,
            AttributeArgumentSyntax attributeArgument,
            NepStandard standard,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var replacementExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("NepStandard"),
                    SyntaxFactory.IdentifierName(standard.ToString()))
                .WithTriviaFrom(attributeArgument.Expression);
            var newArgument = attributeArgument.WithExpression(replacementExpression);
            var newRoot = root!.ReplaceNode(attributeArgument, newArgument);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
