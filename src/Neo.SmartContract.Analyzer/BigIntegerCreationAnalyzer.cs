// Copyright (C) 2015-2026 The Neo Project.
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
            "BigInteger constructor is not supported. Only BigInteger(byte[]) is supported; use BigInteger.Zero for zero or an implicit conversion for integral values.",
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
            var constructor = context.SemanticModel
                .GetSymbolInfo(objectCreationExpression, context.CancellationToken)
                .Symbol as IMethodSymbol;
            var bigIntegerType = context.Compilation.GetTypeByMetadataName("System.Numerics.BigInteger");
            if (constructor is null ||
                bigIntegerType is null ||
                !SymbolEqualityComparer.Default.Equals(constructor.ContainingType, bigIntegerType) ||
                IsSupportedByteArrayConstructor(constructor))
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreationExpression.GetLocation()));
        }

        private static bool IsSupportedByteArrayConstructor(IMethodSymbol constructor)
        {
            if (constructor.Parameters.Length != 1 ||
                constructor.Parameters[0].Type is not IArrayTypeSymbol arrayType)
            {
                return false;
            }

            return arrayType.ElementType.SpecialType == SpecialType.System_Byte;
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

            var declaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
            if (declaration is null || !await CanReplaceWithIntegralConversionAsync(context.Document, declaration, context.CancellationToken).ConfigureAwait(false)) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with explicit BigInteger conversion",
                    createChangedDocument: c => ReplaceWithExplicitConversion(context.Document, declaration, c),
                    equivalenceKey: "Replace with explicit BigInteger conversion"),
                diagnostic);
        }

        private static async Task<bool> CanReplaceWithIntegralConversionAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var constructor = semanticModel?.GetSymbolInfo(objectCreation, cancellationToken).Symbol as IMethodSymbol;
            if (constructor is null || constructor.Parameters.Length != 1) return false;

            var specialType = constructor.Parameters[0].Type.SpecialType;
            return specialType is SpecialType.System_SByte or
                SpecialType.System_Byte or
                SpecialType.System_Int16 or
                SpecialType.System_UInt16 or
                SpecialType.System_Int32 or
                SpecialType.System_UInt32 or
                SpecialType.System_Int64 or
                SpecialType.System_UInt64;
        }

        private static async Task<Document> ReplaceWithExplicitConversion(Document document, ObjectCreationExpressionSyntax objectCreation, CancellationToken cancellationToken)
        {
            var argumentList = objectCreation.ArgumentList;
            if (argumentList is null || argumentList.Arguments.Count != 1) return document;

            var argument = argumentList.Arguments[0];
            var parenthesizedArgument = SyntaxFactory.ParenthesizedExpression(
                argumentList.OpenParenToken,
                argument.Expression,
                argumentList.CloseParenToken);
            var newExpression = SyntaxFactory.CastExpression(
                    objectCreation.Type.WithoutTrivia(),
                    parenthesizedArgument)
                .WithTriviaFrom(objectCreation);

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root!.ReplaceNode(objectCreation, newExpression);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
