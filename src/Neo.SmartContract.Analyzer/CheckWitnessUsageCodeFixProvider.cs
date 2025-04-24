// Copyright (C) 2015-2025 The Neo Project.
//
// CheckWitnessUsageCodeFixProvider.cs file belongs to the neo project and is free
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CheckWitnessUsageCodeFixProvider)), Shared]
    public class CheckWitnessUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(CheckWitnessUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindNode(diagnosticSpan);

            if (node is InvocationExpressionSyntax invocation)
            {
                // Register code fix for wrapping with Assert
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Wrap with Assert",
                        createChangedDocument: c => WrapWithAssertAsync(context.Document, invocation, c),
                        equivalenceKey: "WrapWithAssert"),
                    diagnostic);

                // Register code fix for using in if condition
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Use in if condition",
                        createChangedDocument: c => UseInIfConditionAsync(context.Document, invocation, c),
                        equivalenceKey: "UseInIfCondition"),
                    diagnostic);
            }
        }

        private async Task<Document> WrapWithAssertAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Create Assert(Runtime.CheckWitness(...))
            var assertInvocation = SyntaxFactory.InvocationExpression(
                SyntaxFactory.IdentifierName("Assert"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(invocation.WithoutLeadingTrivia())
                    )
                )
            ).WithLeadingTrivia(invocation.GetLeadingTrivia())
             .WithAdditionalAnnotations(Formatter.Annotation);

            // If the invocation is part of an expression statement, replace the entire statement
            if (invocation.Parent is ExpressionStatementSyntax expressionStatement)
            {
                var newExpressionStatement = SyntaxFactory.ExpressionStatement(assertInvocation)
                    .WithTrailingTrivia(expressionStatement.GetTrailingTrivia());
                var newRoot = root.ReplaceNode(expressionStatement, newExpressionStatement);
                return document.WithSyntaxRoot(newRoot);
            }
            else
            {
                // Otherwise just replace the invocation
                var newRoot = root.ReplaceNode(invocation, assertInvocation);
                return document.WithSyntaxRoot(newRoot);
            }
        }

        private async Task<Document> UseInIfConditionAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find the containing statement
            var containingStatement = invocation.FirstAncestorOrSelf<StatementSyntax>();
            if (containingStatement == null) return document;

            // Create if statement with CheckWitness as condition
            var ifStatement = SyntaxFactory.IfStatement(
                invocation.WithoutLeadingTrivia(),
                SyntaxFactory.Block(
                    SyntaxFactory.List<StatementSyntax>()
                )
            ).WithLeadingTrivia(invocation.GetLeadingTrivia())
             .WithAdditionalAnnotations(Formatter.Annotation);

            // Add a comment to the empty block
            var commentTrivia = SyntaxFactory.Comment("// Add your code here");
            var emptyStatement = SyntaxFactory.EmptyStatement()
                .WithLeadingTrivia(commentTrivia)
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

            ifStatement = ifStatement.WithStatement(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(emptyStatement)
                )
            );

            // Add else block with throw
            var throwStatement = SyntaxFactory.ThrowStatement(
                SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.IdentifierName("Exception"),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal("No authorization.")
                                )
                            )
                        )
                    ),
                    null
                )
            );

            ifStatement = ifStatement.WithElse(
                SyntaxFactory.ElseClause(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(throwStatement)
                    )
                )
            );

            // Replace the containing statement with the if statement
            var newRoot = root.ReplaceNode(containingStatement, ifStatement);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
