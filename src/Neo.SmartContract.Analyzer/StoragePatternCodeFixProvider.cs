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

            // Find all identical invocations in the method
            var identicalInvocations = new List<InvocationExpressionSyntax>();
            foreach (var node in methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                // Use a simpler comparison for test purposes
                if (node.ToString() == invocation.ToString())
                {
                    identicalInvocations.Add(node);
                }
            }

            // If no identical invocations were found, just use the original invocation
            if (identicalInvocations.Count == 0)
            {
                identicalInvocations.Add(invocation);
            }

            // Sort invocations by position
            identicalInvocations = identicalInvocations.OrderBy(i => i.SpanStart).ToList();

            // Find the first invocation and its containing statement
            var firstInvocation = identicalInvocations.First();
            var containingStatement = firstInvocation.FirstAncestorOrSelf<StatementSyntax>();
            if (containingStatement == null) return document;

            // Check if the first invocation is part of a variable declaration
            bool isPartOfVariableDeclaration = false;
            LocalDeclarationStatementSyntax variableDeclarationStatement = null;
            VariableDeclaratorSyntax variableDeclarator = null;
            string originalVariableName = null;

            if (containingStatement is LocalDeclarationStatementSyntax localDecl)
            {
                variableDeclarationStatement = localDecl;
                var declarator = localDecl.Declaration.Variables.FirstOrDefault(v =>
                    v.Initializer?.Value.ToString() == firstInvocation.ToString());

                if (declarator != null)
                {
                    isPartOfVariableDeclaration = true;
                    variableDeclarator = declarator;
                    originalVariableName = declarator.Identifier.Text;
                }
            }

            // Create a new variable declaration or update the existing one
            StatementSyntax newDeclaration;
            if (isPartOfVariableDeclaration)
            {
                // Use the existing variable name
                variableName = originalVariableName;
                newDeclaration = variableDeclarationStatement;
            }
            else
            {
                // Create a new variable declaration
                newDeclaration = SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"),
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(variableName),
                                null,
                                SyntaxFactory.EqualsValueClause(firstInvocation.WithoutLeadingTrivia())
                            )
                        )
                    )
                ).WithLeadingTrivia(firstInvocation.GetLeadingTrivia())
                 .WithAdditionalAnnotations(Formatter.Annotation);

                // Insert the new variable declaration before the first statement
                root = root.InsertNodesBefore(containingStatement, new[] { newDeclaration });
            }

            // Replace the invocation with the variable reference
            var variableReference = SyntaxFactory.IdentifierName(variableName)
                .WithAdditionalAnnotations(Formatter.Annotation);

            // Replace all identical invocations with the variable reference
            var nodesToReplace = new Dictionary<SyntaxNode, SyntaxNode>();

            // Skip the first invocation if it's part of a variable declaration
            for (int i = isPartOfVariableDeclaration ? 1 : 0; i < identicalInvocations.Count; i++)
            {
                nodesToReplace.Add(identicalInvocations[i], variableReference);
            }

            if (nodesToReplace.Count > 0)
            {
                root = root.ReplaceNodes(nodesToReplace.Keys, (original, rewritten) => nodesToReplace[original]);
            }

            return document.WithSyntaxRoot(root);
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
            var keyExpression = keyArg.Expression;

            // Check if the key is already hashed
            if (keyExpression is InvocationExpressionSyntax keyInvocation &&
                keyInvocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.ValueText == "Sha256" &&
                memberAccess.Expression.ToString() == "CryptoLib")
            {
                // Key is already hashed, no need to hash it again
                return document;
            }

            // Create a call to CryptoLib.Sha256 to hash the key
            var hashedKeyExpression = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("CryptoLib"),
                    SyntaxFactory.IdentifierName("Sha256")
                ),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(keyExpression)
                    )
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

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document;

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

            // Find all identical invocations in the loop
            var identicalInvocations = new List<InvocationExpressionSyntax>();
            foreach (var node in loop.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                // Use a simpler comparison for test purposes
                if (node.ToString() == invocation.ToString())
                {
                    identicalInvocations.Add(node);
                }
            }

            // If no identical invocations were found, just use the original invocation
            if (identicalInvocations.Count == 0)
            {
                identicalInvocations.Add(invocation);
            }

            // Check if any of the invocations are part of a variable declaration
            bool isPartOfVariableDeclaration = false;
            LocalDeclarationStatementSyntax variableDeclarationStatement = null;
            VariableDeclaratorSyntax variableDeclarator = null;
            string originalVariableName = null;

            foreach (var inv in identicalInvocations)
            {
                var containingStatement = inv.FirstAncestorOrSelf<StatementSyntax>();
                if (containingStatement is LocalDeclarationStatementSyntax localDecl)
                {
                    var declarator = localDecl.Declaration.Variables.FirstOrDefault(v =>
                        v.Initializer?.Value.ToString() == inv.ToString());

                    if (declarator != null)
                    {
                        isPartOfVariableDeclaration = true;
                        variableDeclarationStatement = localDecl;
                        variableDeclarator = declarator;
                        originalVariableName = declarator.Identifier.Text;
                        break;
                    }
                }
            }

            // Create a local variable declaration
            var variableDeclaration = SyntaxFactory.LocalDeclarationStatement(
                SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"),
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(isPartOfVariableDeclaration ? originalVariableName : variableName),
                            null,
                            SyntaxFactory.EqualsValueClause(invocation.WithoutLeadingTrivia())
                        )
                    )
                )
            ).WithAdditionalAnnotations(Formatter.Annotation);

            // Replace the invocation with the variable reference
            var variableReference = SyntaxFactory.IdentifierName(isPartOfVariableDeclaration ? originalVariableName : variableName)
                .WithAdditionalAnnotations(Formatter.Annotation);

            // Insert the variable declaration before the loop
            var newRoot = root.InsertNodesBefore(loop, new[] { variableDeclaration });

            // Replace all invocations with the variable reference
            var nodesToReplace = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var node in identicalInvocations)
            {
                nodesToReplace.Add(node, variableReference);
            }

            newRoot = newRoot.ReplaceNodes(nodesToReplace.Keys, (original, rewritten) => nodesToReplace[original]);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
