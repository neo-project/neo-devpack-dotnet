// Copyright (C) 2015-2025 The Neo Project.
//
// SmartContractMethodNamingAnalyzer.Underline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractMethodNamingAnalyzerUnderline : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4020";
        private const string Title = "SmartContract method naming violation";
        private const string MessageFormat = "Method names starting with '_' are not allowed except '_deploy' or '_initial'";
        private const string Description = "Ensure method names in SmartContract subclasses follow the naming convention.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning, // or Error based on your preference
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDecl = (MethodDeclarationSyntax)context.Node;
            var methodName = methodDecl.Identifier.ValueText;
            var isStatic = methodDecl.Modifiers.Any(SyntaxKind.StaticKeyword);
            var isPublic = methodDecl.Modifiers.Any(SyntaxKind.PublicKeyword);

            if (isStatic && isPublic && methodName.StartsWith("_") &&
                methodName != "_deploy" && methodName != "_initial")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, methodDecl.Identifier.GetLocation()));
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SmartContractMethodNamingCodeFixProvider))]
    public class SmartContractMethodNamingCodeFixProviderUnderline : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SmartContractMethodNamingAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDecl = root?.FindToken(diagnosticSpan.Start).Parent as MethodDeclarationSyntax;
            if (methodDecl == null) return;

            var methodName = methodDecl.Identifier.ValueText;
            var newMethodName = Char.ToUpper(methodName[1]) + methodName.Substring(2);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Rename to '{newMethodName}'",
                    createChangedDocument: c => RenameMethodAsync(context.Document, methodDecl, newMethodName, c),
                    equivalenceKey: "RenameMethod"),
                diagnostic);
        }

        private static async Task<Document> RenameMethodAsync(Document document, MethodDeclarationSyntax methodDecl, string newMethodName, CancellationToken cancellationToken)
        {
            var identifierToken = methodDecl.Identifier;
            var root = await document.GetSyntaxRootAsync(cancellationToken);
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
            var newRoot = root.ReplaceToken(identifierToken, SyntaxFactory.Identifier(newMethodName));
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.

            return document.WithSyntaxRoot(newRoot!);
        }
    }
}
