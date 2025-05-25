// Copyright (C) 2015-2025 The Neo Project.
//
// EventRegistrationCodeFixProvider.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventRegistrationCodeFixProvider)), Shared]
    public class EventRegistrationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(EventRegistrationAnalyzer.DiagnosticIdUnregisteredEvent,
                                 EventRegistrationAnalyzer.DiagnosticIdImproperEventDefinition);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            // Handle different diagnostic IDs
            switch (diagnostic.Id)
            {
                case EventRegistrationAnalyzer.DiagnosticIdUnregisteredEvent:
                    if (node.Parent is ArgumentSyntax argument &&
                        argument.Parent is ArgumentListSyntax argumentList &&
                        argumentList.Parent is InvocationExpressionSyntax invocation)
                    {
                        // Register code fix for unregistered event
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Register event delegate",
                                createChangedDocument: c => RegisterEventDelegateAsync(context.Document, invocation, node, c),
                                equivalenceKey: "RegisterEventDelegate"),
                            diagnostic);
                    }
                    break;

                case EventRegistrationAnalyzer.DiagnosticIdImproperEventDefinition:
                    // No longer providing a code fix for making events static since it's not required
                    break;
            }
        }

        private async Task<Document> RegisterEventDelegateAsync(Document document, InvocationExpressionSyntax invocation, SyntaxNode eventNode, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document;

            // Find the containing class
            var classDeclaration = invocation.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null) return document;

            // Create a new delegate declaration
            var eventName = eventNode.ToString();
            var delegateDeclaration = SyntaxFactory.DelegateDeclaration(
                SyntaxFactory.List<AttributeListSyntax>(),
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier(eventName),
                null,
                SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(new[] {
                    SyntaxFactory.Parameter(
                        SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxFactory.TokenList(),
                        SyntaxFactory.IdentifierName("string"),
                        SyntaxFactory.Identifier("name"),
                        null)
                })),
                SyntaxFactory.List<TypeParameterConstraintClauseSyntax>())
                .WithAdditionalAnnotations(Formatter.Annotation);

            // Add the delegate declaration to the class
            var newClassDeclaration = classDeclaration.AddMembers(delegateDeclaration);
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        // MakeEventStaticAsync method removed since events don't need to be static
    }
}
