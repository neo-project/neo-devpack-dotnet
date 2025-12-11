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

        public const string ParseDiagnosticId = "NC4055";
        private static readonly LocalizableString ParseTitle = "Use Parse for UInt initialization";
        private static readonly LocalizableString ParseMessageFormat = "Use '{0}.Parse(...)' instead of implicit initialization from string or ByteString";
        private static readonly LocalizableString ParseDescription = "UInt160/UInt256/ECPoint must be initialized via Parse to ensure validation.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        private static readonly DiagnosticDescriptor ParseRule = new DiagnosticDescriptor(
            ParseDiagnosticId,
            ParseTitle,
            ParseMessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: ParseDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule, ParseRule);

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

                AnalyzeDirectInitialization(context, fieldDeclaration, variable);
            }
        }

        private void AnalyzeDirectInitialization(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax fieldDeclaration, VariableDeclaratorSyntax variable)
        {
            if (variable.Initializer is null)
                return;

            var semanticModel = context.SemanticModel;
            var fieldSymbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken) as IFieldSymbol;
            if (fieldSymbol?.Type is null)
                return;

            string? parseTarget = fieldSymbol.Type.ToString() switch
            {
                "Neo.SmartContract.Framework.UInt160" => "UInt160",
                "Neo.SmartContract.Framework.UInt256" => "UInt256",
                "Neo.SmartContract.Framework.ECPoint" => "ECPoint",
                _ => null
            };

            if (parseTarget is null)
                return;

            var initializer = variable.Initializer.Value;
            if (IsParseInvocation(initializer))
                return;

            var initType = semanticModel.GetTypeInfo(initializer, context.CancellationToken).Type;
            if (initType is null)
                return;

            if (initType.SpecialType == SpecialType.System_String ||
                initType.ToString() == "Neo.SmartContract.Framework.ByteString" ||
                initType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte })
            {
                var diagnostic = Diagnostic.Create(ParseRule, initializer.GetLocation(), parseTarget);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsParseInvocation(ExpressionSyntax initializer)
        {
            if (initializer is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == "Parse")
            {
                return true;
            }

            return false;
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
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InitialValueAnalyzer.DiagnosticId, InitialValueAnalyzer.ParseDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false)!;
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root!.FindToken(diagnosticSpan.Start).Parent!.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            var title = diagnostic.Id == InitialValueAnalyzer.ParseDiagnosticId
                ? "Wrap initializer with Parse"
                : "Convert to Parse initialization";

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => diagnostic.Id == InitialValueAnalyzer.ParseDiagnosticId
                        ? WrapInitializerWithParseAsync(context.Document, declaration, c)
                        : ConvertToParseInitializationAsync(context.Document, declaration, c),
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

        private async Task<Document> WrapInitializerWithParseAsync(Document document, FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var variable = fieldDeclaration.Declaration.Variables[0];
            var fieldSymbol = semanticModel?.GetDeclaredSymbol(variable, cancellationToken) as IFieldSymbol;
            var initializerExpression = variable.Initializer?.Value;
            if (initializerExpression is null)
                return document;

            var wrapped = BuildInitializerExpression(fieldSymbol?.Type, initializerExpression);
            if (wrapped is null)
                return document;

            var newVariable = variable.WithInitializer(SyntaxFactory.EqualsValueClause(wrapped));
            var updatedDeclaration = fieldDeclaration.WithDeclaration(fieldDeclaration.Declaration.WithVariables(
                SyntaxFactory.SingletonSeparatedList(newVariable)));

            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            editor.ReplaceNode(fieldDeclaration, updatedDeclaration);
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
