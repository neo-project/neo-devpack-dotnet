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
using Microsoft.CodeAnalysis.Simplification;

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

        private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorFactory.Create(
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
                !Enum.TryParse<NepStandard>(standardName, out _))
                return;

            var attribute = attributeArgument.FirstAncestorOrSelf<AttributeSyntax>();
            var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            if (attribute?.ArgumentList is null || model is null ||
                model.GetSymbolInfo(attribute, context.CancellationToken).Symbol is not IMethodSymbol constructor)
                return;

            var enumType = constructor.ContainingType.InstanceConstructors
                .Where(candidate => candidate.Parameters.Length == 1 && candidate.Parameters[0].IsParams)
                .Select(candidate => (candidate.Parameters[0].Type as IArrayTypeSymbol)?.ElementType)
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault(type => type.TypeKind == TypeKind.Enum && type.Name == nameof(NepStandard));
            if (enumType is null) return;

            var replacements = new System.Collections.Generic.Dictionary<AttributeArgumentSyntax, ExpressionSyntax>();
            foreach (var argument in attribute.ArgumentList.Arguments.Where(argument => argument.NameEquals is null))
            {
                var constant = model.GetConstantValue(argument.Expression, context.CancellationToken);
                if (!constant.HasValue || constant.Value is not string value) return;
                var name = value.StartsWith("NEP-", StringComparison.OrdinalIgnoreCase)
                    ? "Nep" + value.Substring(4) : value;
                var field = enumType.GetMembers().OfType<IFieldSymbol>()
                    .FirstOrDefault(member => member.HasConstantValue && member.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (field is null) return;
                replacements.Add(argument, SyntaxFactory.ParseExpression(
                    enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + "." + field.Name)
                    .WithTriviaFrom(argument.Expression).WithAdditionalAnnotations(Simplifier.Annotation));
            }

            // Convert every constructor argument together; a mixture of strings and enums has no overload.
            var replacement = attribute.ReplaceNodes(replacements.Keys,
                (argument, _) => argument.WithExpression(replacements[argument]));
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: async cancellationToken =>
                    {
                        var updated = context.Document.WithSyntaxRoot(root!.ReplaceNode(attribute, replacement));
                        return await Simplifier.ReduceAsync(updated, Simplifier.Annotation, cancellationToken: cancellationToken).ConfigureAwait(false);
                    },
                    equivalenceKey: Title),
                diagnostic);
        }

    }
}
