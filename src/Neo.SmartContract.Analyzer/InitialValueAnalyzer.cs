// Copyright (C) 2015-2025 The Neo Project.
//
// InitialValueAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InitialValueAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4009";
        private static readonly LocalizableString Title = "Replace InitialValue attribute with direct initialization";
        private static readonly LocalizableString MessageFormat = "Convert '{0}' attribute to Parse-based initialization";
        private static readonly LocalizableString Description = "Prefer direct Parse-based initialization instead of InitialValue-derived attributes.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
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
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                if (variable.Initializer?.Value is LiteralExpressionSyntax literal && literal.Token.ValueText == "default!")
                {
                    var attribute = fieldDeclaration.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .FirstOrDefault(attr => IsTargetAttribute(attr.Name.ToString()));

                    if (attribute != null)
                    {
                        var argumentList = attribute.ArgumentList;
                        if (argumentList != null && argumentList.Arguments.Count > 0)
                        {
                            var argument = argumentList.Arguments[0].ToString();
                            var diagnostic = Diagnostic.Create(Rule, fieldDeclaration.GetLocation(), attribute.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }

        private bool IsTargetAttribute(string attributeName)
        {
            string normalized = TrimAttributeSuffix(attributeName);
            return normalized is "InitialValue" or "Hash160" or "PublicKey" or "Integer" or "String";
        }

        private static string TrimAttributeSuffix(string attributeName)
        {
            const string suffix = "Attribute";
            return attributeName.EndsWith(suffix, StringComparison.Ordinal)
                ? attributeName.Substring(0, attributeName.Length - suffix.Length)
                : attributeName;
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitialValueCodeFixProvider)), Shared]
    public class InitialValueCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InitialValueAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false)!;
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root!.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert to Parse initialization",
                    createChangedDocument: c => ConvertToParseInitializationAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(InitialValueCodeFixProvider)),
                diagnostic);
        }

        private async Task<Document> ConvertToParseInitializationAsync(Document document, FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            AttributeSyntax? attribute = fieldDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(attr => IsTargetAttribute(attr.Name.ToString()));

            if (attribute != null && attribute.ArgumentList?.Arguments.Count > 0)
            {
                var valueExpression = attribute.ArgumentList.Arguments[0].Expression;
                var variable = fieldDeclaration.Declaration.Variables[0];
                var fieldSymbol = semanticModel?.GetDeclaredSymbol(variable, cancellationToken) as IFieldSymbol;
                var initializerExpression = BuildInitializerExpression(fieldSymbol?.Type, valueExpression);
                if (initializerExpression is null)
                    return document;

                var newInitializer = SyntaxFactory.EqualsValueClause(initializerExpression);
                var newVariable = variable.WithInitializer(newInitializer);
                var cleanedField = RemoveInitialValueAttributes(fieldDeclaration);
                cleanedField = cleanedField.WithDeclaration(fieldDeclaration.Declaration.WithVariables(
                    SyntaxFactory.SingletonSeparatedList(newVariable)));

                editor.ReplaceNode(fieldDeclaration, cleanedField);
            }

            return editor.GetChangedDocument();
        }

        private static FieldDeclarationSyntax RemoveInitialValueAttributes(FieldDeclarationSyntax fieldDeclaration)
        {
            var remainingLists = new List<AttributeListSyntax>();
            foreach (var list in fieldDeclaration.AttributeLists)
            {
                var kept = list.Attributes.Where(attr => !IsTargetAttribute(attr.Name.ToString())).ToArray();
                if (kept.Length > 0)
                {
                    remainingLists.Add(list.WithAttributes(SyntaxFactory.SeparatedList(kept)));
                }
            }

            return fieldDeclaration.WithAttributeLists(SyntaxFactory.List(remainingLists));
        }

        private static ExpressionSyntax? BuildInitializerExpression(ITypeSymbol? fieldType, ExpressionSyntax valueExpression)
        {
            string? parseTarget = fieldType?.ToString() switch
            {
                "Neo.SmartContract.Framework.UInt160" => "Neo.SmartContract.Framework.UInt160.Parse",
                "Neo.SmartContract.Framework.UInt256" => "Neo.SmartContract.Framework.UInt256.Parse",
                "Neo.SmartContract.Framework.ECPoint" => "Neo.SmartContract.Framework.ECPoint.Parse",
                "System.Numerics.BigInteger" => "System.Numerics.BigInteger.Parse",
                "string" => null,
                _ => null
            };

            if (parseTarget is null)
            {
                return fieldType?.SpecialType == SpecialType.System_String ? valueExpression : null;
            }

            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.ParseExpression(parseTarget),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(valueExpression))));
        }

        private static bool IsTargetAttribute(string attributeName)
        {
            string normalized = NormalizeAttributeName(attributeName);
            return normalized is "InitialValue" or "Hash160" or "PublicKey" or "Integer" or "String";
        }

        private static string NormalizeAttributeName(string attributeName)
        {
            return TrimAttributeSuffix(attributeName);
        }

        private static string TrimAttributeSuffix(string attributeName)
        {
            const string suffix = "Attribute";
            return attributeName.EndsWith(suffix, StringComparison.Ordinal)
                ? attributeName.Substring(0, attributeName.Length - suffix.Length)
                : attributeName;
        }
    }
}
