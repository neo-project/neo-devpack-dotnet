// Copyright (C) 2015-2025 The Neo Project.
//
// BigIntegerCreationAnalyzer.cs file belongs to the neo project and is free
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
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BigIntegerCreationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4008";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Use of BigInteger constructor",
            "Use of new BigInteger(int) is not allowed, please use BigInteger x = 0;",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)context.Node;

            // Check if it is a BigInteger creation
            if (context.SemanticModel.GetTypeInfo(objectCreationExpression).Type?.ToString() == "System.Numerics.BigInteger" &&
                objectCreationExpression.ArgumentList?.Arguments.Count == 1 &&
                context.SemanticModel.GetTypeInfo(objectCreationExpression.ArgumentList.Arguments[0].Expression).Type?.SpecialType == SpecialType.System_Int32)
            {
                var diagnostic = Diagnostic.Create(Rule, objectCreationExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BigIntegerCreationCodeFixProvider)), Shared]
    public class BigIntegerCreationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(BigIntegerCreationAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().First();
            if (declaration is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with direct assignment",
                    createChangedDocument: c => ReplaceWithDirectAssignment(context.Document, declaration, c),
                    equivalenceKey: "Replace with direct assignment"),
                diagnostic);
        }

        private static async Task<Document> ReplaceWithDirectAssignment(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
        {
            var argument = objectCreation.ArgumentList?.Arguments.First().Expression;
            if (argument is null) return document;

            var newExpression = SyntaxFactory.ParseExpression(argument.ToString())
                                             .WithLeadingTrivia(objectCreation.GetLeadingTrivia())
                                             .WithTrailingTrivia(objectCreation.GetTrailingTrivia());

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root!.ReplaceNode(objectCreation, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
