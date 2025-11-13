// Copyright (C) 2015-2025 The Neo Project.
//
// LinqUsageAnalyzer.cs file belongs to the neo project and is free
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
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LinqUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4011";
        public const string MissingFrameworkUsingDiagnosticId = "NC4032";
        internal const string FrameworkLinqNamespace = "Neo.SmartContract.Framework.Linq";
        private static readonly string Title = "LINQ usage";
        private static readonly string MessageFormat = "Use Neo.SmartContract.Framework.Linq instead of System.Linq: {0}";
        private static readonly string Description = "System.Linq is not supported in neo smart contract. Use Neo.SmartContract.Framework.Linq instead.";
        private const string Category = "Usage";
        private static readonly string MissingUsingMessage = "Add 'using Neo.SmartContract.Framework.Linq;' to enable query expressions";
        private static readonly string MissingUsingDescription = "Add 'using Neo.SmartContract.Framework.Linq;' to enable query expressions.";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        private static readonly DiagnosticDescriptor MissingUsingRule = new(
            MissingFrameworkUsingDiagnosticId,
            Title,
            MissingUsingMessage,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: MissingUsingDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule, MissingUsingRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
            context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
        {
            var usingDirective = (UsingDirectiveSyntax)context.Node;
            if (usingDirective.Name!.ToString() == "System.Linq")
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), usingDirective.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            if (HasFrameworkLinqUsing(context.Node.SyntaxTree, context.CancellationToken))
                return;

            var diagnostic = Diagnostic.Create(MissingUsingRule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static bool HasFrameworkLinqUsing(SyntaxTree syntaxTree, CancellationToken cancellationToken)
        {
            if (syntaxTree == null) return false;
            if (syntaxTree.TryGetRoot(out var rootObj) && rootObj is CompilationUnitSyntax root)
                return root.Usings.Any(IsFrameworkLinqUsing);

            var rootSync = (CompilationUnitSyntax)syntaxTree.GetRoot(cancellationToken);
            return rootSync.Usings.Any(IsFrameworkLinqUsing);
        }

        private static bool IsFrameworkLinqUsing(UsingDirectiveSyntax usingDirective)
        {
            var name = usingDirective.Name?.ToString();
            return string.Equals(name, FrameworkLinqNamespace, System.StringComparison.Ordinal);
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LinqUsageCodeFixProvider)), Shared]
    public class LinqUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            LinqUsageAnalyzer.DiagnosticId,
            LinqUsageAnalyzer.MissingFrameworkUsingDiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            switch (diagnostic.Id)
            {
                case LinqUsageAnalyzer.DiagnosticId:
                    var usingDirective = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().First();
                    if (usingDirective != null)
                    {
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: "Use Neo.SmartContract.Framework.Linq",
                                createChangedDocument: c => FixSystemLinqUsage(context.Document, usingDirective, c),
                                equivalenceKey: "Use Neo.SmartContract.Framework.Linq"),
                            diagnostic);
                    }
                    break;
                case LinqUsageAnalyzer.MissingFrameworkUsingDiagnosticId:
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add using Neo.SmartContract.Framework.Linq",
                            createChangedDocument: c => AddFrameworkUsingAsync(context.Document, c),
                            equivalenceKey: "AddFrameworkLinqUsing"),
                        diagnostic);
                    break;
            }
        }

        private static async Task<Document> FixSystemLinqUsage(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newUsingDirective = usingDirective.WithName(SyntaxFactory.ParseName("Neo.SmartContract.Framework.Linq"));
            var newRoot = root!.ReplaceNode(usingDirective, newUsingDirective);
            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> AddFrameworkUsingAsync(Document document, CancellationToken cancellationToken)
        {
            var root = (CompilationUnitSyntax)(await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
            if (root.Usings.Any(u => u.Name?.ToString() == LinqUsageAnalyzer.FrameworkLinqNamespace))
                return document;

            var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(LinqUsageAnalyzer.FrameworkLinqNamespace));
            var newRoot = root.AddUsings(newUsing);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
