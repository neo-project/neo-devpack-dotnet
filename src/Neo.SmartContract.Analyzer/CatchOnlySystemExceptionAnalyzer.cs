// Copyright (C) 2015-2025 The Neo Project.
//
// CatchOnlySystemExceptionAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Editing;


namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatchOnlySystemExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CatchOnlySystemException";

        private static readonly LocalizableString Title = "Catch System.Exception";
        private static readonly LocalizableString MessageFormat = "Neo smart contract supports catching System.Exception only. The compiler will catch all exeptions even if you want to catch a limited class of exception.";
        private static readonly LocalizableString Description = "This analyzer enforces catching only System.Exception.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;
            var declaration = catchClause.Declaration;

            if (declaration == null) return;

            var type = declaration.Type;
            if (type == null) return;

            var exceptionType = context.SemanticModel.GetTypeInfo(type).Type;
            if (exceptionType?.ToDisplayString() == "System.Exception") return;

            var diagnostic = Diagnostic.Create(Rule, type.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class CatchOnlySystemExceptionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(CatchOnlySystemExceptionAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics[0];
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root?.FndNode(diagnosticSpan) as TypeSyntax;
            if (declaration == null) return;

            context.RegisterCodeFix(
                Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                    title: "Change to System.Exception",
                    createChangedDocument: c => FixCatchTypeAsync(context.Document, declaration, c),
                    equivalenceKey: "ChangeToSystemException"),
                diagnostic);
        }

        private async Task<Document> FixCatchTypeAsync(Document document, TypeSyntax type, System.Threading.CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var newType = SyntaxFactory.ParseTypeName("System.Exception")
                .WithLeadingTrivia(type.GetLeadingTrivia())
                .WithTrailingTrivia(type.GetTrailingTrivia());

            editor.ReplaceNode(type, newType);
            return editor.GetChangedDocument();
        }
    }
}
