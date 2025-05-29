// Copyright (C) 2015-2025 The Neo Project.
//
// ContractAttributeCodeFixProvider.cs file belongs to the neo project and is free
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ContractAttributeCodeFixProvider)), Shared]
    public class ContractAttributeCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(ContractAttributeAnalyzer.DiagnosticIdMissingAttributes,
                                 ContractAttributeAnalyzer.DiagnosticIdInvalidAttribute);

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
                case ContractAttributeAnalyzer.DiagnosticIdMissingAttributes:
                    if (node is IdentifierNameSyntax identifier &&
                        node.Parent is ClassDeclarationSyntax classDeclaration)
                    {
                        // Get the missing attributes from the diagnostic message
                        var message = diagnostic.GetMessage();
                        var startIndex = message.IndexOf(':') + 1;
                        var missingAttributesString = message.Substring(startIndex).Trim();
                        var missingAttributes = missingAttributesString.Split(',').Select(a => a.Trim()).ToList();

                        // Register code fix for missing attributes
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Add missing contract attributes",
                                createChangedDocument: c => AddMissingAttributesAsync(context.Document, classDeclaration, missingAttributes, c),
                                equivalenceKey: "AddMissingAttributes"),
                            diagnostic);
                    }
                    break;

                case ContractAttributeAnalyzer.DiagnosticIdInvalidAttribute:
                    if (node is AttributeSyntax attribute)
                    {
                        // Register code fix for invalid attribute
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Fix attribute usage",
                                createChangedDocument: c => FixAttributeUsageAsync(context.Document, attribute, c),
                                equivalenceKey: "FixAttributeUsage"),
                            diagnostic);
                    }
                    break;
            }
        }

        private async Task<Document> AddMissingAttributesAsync(Document document, ClassDeclarationSyntax classDeclaration, List<string> missingAttributes, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var newAttributeLists = classDeclaration.AttributeLists.ToList();

            foreach (var attributeName in missingAttributes)
            {
                AttributeSyntax newAttribute;

                switch (attributeName)
                {
                    case "DisplayName":
                        newAttribute = CreateAttributeWithStringArgument("DisplayName", classDeclaration.Identifier.Text);
                        break;
                    case "ContractDescription":
                        newAttribute = CreateAttributeWithStringArgument("ContractDescription", "Description of " + classDeclaration.Identifier.Text);
                        break;
                    case "ContractVersion":
                        newAttribute = CreateAttributeWithStringArgument("ContractVersion", "1.0.0");
                        break;
                    case "ContractPermission":
                        newAttribute = SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName("ContractPermission"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(new[] {
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Permission"),
                                            SyntaxFactory.IdentifierName("Any"))),
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Method"),
                                            SyntaxFactory.IdentifierName("Any")))
                                })
                            )
                        );
                        break;
                    default:
                        continue;
                }

                var newAttributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(newAttribute))
                    .WithAdditionalAnnotations(Formatter.Annotation);
                newAttributeLists.Add(newAttributeList);
            }

            var newClassDeclaration = classDeclaration.WithAttributeLists(SyntaxFactory.List(newAttributeLists));
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private AttributeSyntax CreateAttributeWithStringArgument(string attributeName, string argumentValue)
        {
            return SyntaxFactory.Attribute(
                SyntaxFactory.IdentifierName(attributeName),
                SyntaxFactory.AttributeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.AttributeArgument(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal(argumentValue)
                            )
                        )
                    )
                )
            );
        }

        private async Task<Document> FixAttributeUsageAsync(Document document, AttributeSyntax attribute, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root == null) return document;

            var attributeName = attribute.Name.ToString();
            if (attributeName.EndsWith("Attribute"))
                attributeName = attributeName.Substring(0, attributeName.Length - "Attribute".Length);

            AttributeSyntax newAttribute;

            switch (attributeName)
            {
                case "DisplayName":
                case "ContractDescription":
                case "ContractVersion":
                    // Fix string argument attributes
                    if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
                    {
                        var defaultValue = attributeName == "DisplayName" ? "MyContract" :
                                          attributeName == "ContractDescription" ? "Contract Description" : "1.0.0";

                        newAttribute = CreateAttributeWithStringArgument(attributeName, defaultValue);
                    }
                    else
                    {
                        // Ensure the argument is a string literal
                        var argument = attribute.ArgumentList.Arguments[0];
                        if (argument.Expression is not LiteralExpressionSyntax literalExpression ||
                            literalExpression.Kind() != SyntaxKind.StringLiteralExpression)
                        {
                            newAttribute = CreateAttributeWithStringArgument(attributeName, "DefaultValue");
                        }
                        else
                        {
                            return document; // Already correct
                        }
                    }
                    break;

                case "ContractPermission":
                    // Fix ContractPermission attribute
                    if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
                    {
                        newAttribute = SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName("ContractPermission"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(new[] {
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Permission"),
                                            SyntaxFactory.IdentifierName("Any"))),
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("Method"),
                                            SyntaxFactory.IdentifierName("Any")))
                                })
                            )
                        );
                    }
                    else
                    {
                        return document; // Already has arguments, assume they're correct
                    }
                    break;

                case "SupportedStandards":
                    // Fix SupportedStandards attribute
                    if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
                    {
                        newAttribute = SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName("SupportedStandards"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SingletonSeparatedList(
                                    SyntaxFactory.AttributeArgument(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName("NepStandard"),
                                            SyntaxFactory.IdentifierName("Nep17"))
                                    )
                                )
                            )
                        );
                    }
                    else
                    {
                        return document; // Already has arguments, assume they're correct
                    }
                    break;

                default:
                    return document; // Can't fix unknown attribute
            }

            var newRoot = root.ReplaceNode(attribute, newAttribute.WithAdditionalAnnotations(Formatter.Annotation));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
