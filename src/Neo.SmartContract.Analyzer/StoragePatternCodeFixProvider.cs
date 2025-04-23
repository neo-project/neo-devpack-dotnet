// Copyright (C) 2015-2025 The Neo Project.
//
// StoragePatternCodeFixProvider.cs file belongs to the neo project and is free
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StoragePatternCodeFixProvider)), Shared]
    public class StoragePatternCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(
                StoragePatternAnalyzer.DiagnosticIdRepeatedAccess,
                StoragePatternAnalyzer.DiagnosticIdLargeKey,
                StoragePatternAnalyzer.DiagnosticIdStorageInLoop);

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
                switch (diagnostic.Id)
                {
                    case StoragePatternAnalyzer.DiagnosticIdRepeatedAccess:
                        // Register code fix for repeated storage access
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Store value in local variable",
                                createChangedDocument: c => StoreInLocalVariableAsync(context.Document, invocation, c),
                                equivalenceKey: "StoreInLocalVariable"),
                            diagnostic);
                        break;

                    case StoragePatternAnalyzer.DiagnosticIdLargeKey:
                        // Register code fix for large storage key
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Use hash of key",
                                createChangedDocument: c => UseHashOfKeyAsync(context.Document, invocation, c),
                                equivalenceKey: "UseHashOfKey"),
                            diagnostic);
                        break;

                    case StoragePatternAnalyzer.DiagnosticIdStorageInLoop:
                        // Register code fix for storage access in loop
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Move storage access outside loop",
                                createChangedDocument: c => MoveStorageAccessOutsideLoopAsync(context.Document, invocation, c),
                                equivalenceKey: "MoveStorageAccessOutsideLoop"),
                            diagnostic);
                        break;
                }
            }
        }

        private async Task<Document> StoreInLocalVariableAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document;

            // Find the method containing the invocation
            var methodDeclaration = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (methodDeclaration == null) return document;

            // Get the storage key
            string keyName = null;
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                var keyArg = invocation.ArgumentList.Arguments[0].Expression;
                if (keyArg is LiteralExpressionSyntax literal)
                {
                    keyName = literal.Token.ValueText;
                }
                else if (keyArg is IdentifierNameSyntax identifier)
                {
                    keyName = identifier.Identifier.Text;
                }
            }

            if (keyName == null) return document;

            // Create a variable name based on the key
            string variableName = "stored" + char.ToUpperInvariant(keyName[0]) + keyName.Substring(1);

            // Create a local variable declaration
            var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(variableName),
                            null,
                            SyntaxFactory.EqualsValueClause(invocation.WithoutLeadingTrivia())
                        )
                    )
                )
            ).WithLeadingTrivia(invocation.GetLeadingTrivia())
             .WithAdditionalAnnotations(Formatter.Annotation);

            // Replace the invocation with the variable reference
            var variableReference = SyntaxFactory.IdentifierName(variableName)
                .WithAdditionalAnnotations(Formatter.Annotation);

            // Find all identical invocations in the method
            var identicalInvocations = new List<InvocationExpressionSyntax>();
            foreach (var node in methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (AreInvocationsEquivalent(invocation, node, semanticModel))
                {
                    identicalInvocations.Add(node);
                }
            }

            // Check if we found any identical invocations
            if (identicalInvocations.Count == 0)
            {
                // If no identical invocations were found, just use the original invocation
                identicalInvocations.Add(invocation);
            }

            // Find the statement containing the first invocation
            var firstInvocation = identicalInvocations.OrderBy(i => i.SpanStart).First();
            var containingStatement = firstInvocation.FirstAncestorOrSelf<StatementSyntax>();
            if (containingStatement == null) return document;

            // Insert the variable declaration before the statement
            var newRoot = root.InsertNodesBefore(containingStatement, new[] { variableDeclaration });

            // Replace all identical invocations with the variable reference
            var nodesToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var node in identicalInvocations)
            {
                nodesToReplace.Add(node, variableReference);
            }

            newRoot = newRoot.ReplaceNodes(nodesToReplace.Keys, (original, rewritten) => nodesToReplace[original]);

            return document.WithSyntaxRoot(newRoot);
        }

        private bool AreInvocationsEquivalent(InvocationExpressionSyntax invocation1, InvocationExpressionSyntax invocation2, SemanticModel semanticModel)
        {
            // Check if both are calling the same method
            var symbol1 = semanticModel.GetSymbolInfo(invocation1).Symbol;
            var symbol2 = semanticModel.GetSymbolInfo(invocation2).Symbol;
            if (symbol1 == null || symbol2 == null || !symbol1.Equals(symbol2))
                return false;

            // Check if they have the same number of arguments
            if (invocation1.ArgumentList.Arguments.Count != invocation2.ArgumentList.Arguments.Count)
                return false;

            // Check if all arguments are equivalent
            for (int i = 0; i < invocation1.ArgumentList.Arguments.Count; i++)
            {
                var arg1 = invocation1.ArgumentList.Arguments[i].Expression;
                var arg2 = invocation2.ArgumentList.Arguments[i].Expression;

                // For literals, check if they have the same value
                if (arg1 is LiteralExpressionSyntax literal1 && arg2 is LiteralExpressionSyntax literal2)
                {
                    if (!literal1.Token.ValueText.Equals(literal2.Token.ValueText))
                        return false;
                }
                // For identifiers, check if they refer to the same symbol
                else if (arg1 is IdentifierNameSyntax identifier1 && arg2 is IdentifierNameSyntax identifier2)
                {
                    var sym1 = semanticModel.GetSymbolInfo(identifier1).Symbol;
                    var sym2 = semanticModel.GetSymbolInfo(identifier2).Symbol;
                    if (sym1 == null || sym2 == null || !sym1.Equals(sym2))
                        return false;
                }
                else
                {
                    // For other expressions, consider them different
                    return false;
                }
            }

            return true;
        }

        private async Task<Document> UseHashOfKeyAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Check if this is a Storage method with a key argument
            if (invocation.ArgumentList.Arguments.Count == 0)
                return document;

            var keyArg = invocation.ArgumentList.Arguments[0];

            // Create a call to CryptoLib.Sha256 to hash the key
            var hashedKeyExpression = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("CryptoLib"),
                    SyntaxFactory.IdentifierName("Sha256")
                ),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(keyArg)
                )
            );

            // Replace the key argument with the hashed key
            var newKeyArg = keyArg.WithExpression(hashedKeyExpression);
            var newArguments = invocation.ArgumentList.Arguments.Replace(keyArg, newKeyArg);
            var newArgumentList = invocation.ArgumentList.WithArguments(newArguments);
            var newInvocation = invocation.WithArgumentList(newArgumentList)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var newRoot = root.ReplaceNode(invocation, newInvocation);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> MoveStorageAccessOutsideLoopAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            // Find the containing loop
            var loop = invocation.FirstAncestorOrSelf<StatementSyntax>(s =>
                s is ForStatementSyntax ||
                s is ForEachStatementSyntax ||
                s is WhileStatementSyntax ||
                s is DoStatementSyntax);

            if (loop == null) return document;

            // Get the storage key
            string keyName = null;
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                var keyArg = invocation.ArgumentList.Arguments[0].Expression;
                if (keyArg is LiteralExpressionSyntax literal)
                {
                    keyName = literal.Token.ValueText;
                }
                else if (keyArg is IdentifierNameSyntax identifier)
                {
                    keyName = identifier.Identifier.Text;
                }
            }

            if (keyName == null) return document;

            // Create a variable name based on the key
            string variableName = "cached" + char.ToUpperInvariant(keyName[0]) + keyName.Substring(1);

            // Create a local variable declaration
            var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(variableName),
                            null,
                            SyntaxFactory.EqualsValueClause(invocation.WithoutLeadingTrivia())
                        )
                    )
                )
            ).WithAdditionalAnnotations(Formatter.Annotation);

            // Replace the invocation with the variable reference
            var variableReference = SyntaxFactory.IdentifierName(variableName)
                .WithAdditionalAnnotations(Formatter.Annotation);

            // Insert the variable declaration before the loop
            var newRoot = root.InsertNodesBefore(loop, new[] { variableDeclaration });

            // Replace the invocation with the variable reference
            newRoot = newRoot.ReplaceNode(invocation, variableReference);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
