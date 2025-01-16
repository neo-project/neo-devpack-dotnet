// Copyright (C) 2015-2024 The Neo Project.
//
// KeywordUsageAnalyzer.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class KeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4015";
        private static readonly LocalizableString Title = "Restricted keyword usage";
        private static readonly LocalizableString MessageFormat = "Use of '{0}' is not allowed";
        private static readonly LocalizableString Description = "This keyword is restricted in this project.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode,
                SyntaxKind.UnsafeStatement,
                SyntaxKind.LockStatement,
                SyntaxKind.FixedStatement,
                SyntaxKind.StackAllocArrayCreationExpression,
                SyntaxKind.AwaitExpression,
                SyntaxKind.QueryExpression,
                SyntaxKind.YieldReturnStatement,
                SyntaxKind.YieldBreakStatement,
                SyntaxKind.ConversionOperatorDeclaration,
                SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            SyntaxToken keywordToken = context.Node.Kind() switch
            {
                SyntaxKind.UnsafeStatement => ((UnsafeStatementSyntax)context.Node).UnsafeKeyword,
                SyntaxKind.LockStatement => ((LockStatementSyntax)context.Node).LockKeyword,
                SyntaxKind.FixedStatement => ((FixedStatementSyntax)context.Node).FixedKeyword,
                SyntaxKind.StackAllocArrayCreationExpression => ((StackAllocArrayCreationExpressionSyntax)context.Node).StackAllocKeyword,
                SyntaxKind.AwaitExpression => ((AwaitExpressionSyntax)context.Node).AwaitKeyword,
                SyntaxKind.QueryExpression => ((QueryExpressionSyntax)context.Node).FromClause.FromKeyword,
                SyntaxKind.YieldReturnStatement => ((YieldStatementSyntax)context.Node).YieldKeyword,
                SyntaxKind.YieldBreakStatement => ((YieldStatementSyntax)context.Node).YieldKeyword,
                SyntaxKind.ConversionOperatorDeclaration => ((ConversionOperatorDeclarationSyntax)context.Node).ImplicitOrExplicitKeyword,
                SyntaxKind.IdentifierName => ((IdentifierNameSyntax)context.Node).Identifier,
                _ => default
            };

            if (keywordToken != default && IsRestrictedKeyword(keywordToken.ValueText))
            {
                var diagnostic = Diagnostic.Create(Rule, keywordToken.GetLocation(), keywordToken.Text);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsRestrictedKeyword(string keyword)
        {
            return keyword switch
            {
                "unsafe" or "lock" or "fixed" or "stackalloc" or "await" or
                "yield" or "explicit" or "implicit" or "dynamic" => true,
                _ => false
            };
        }
    }
}
