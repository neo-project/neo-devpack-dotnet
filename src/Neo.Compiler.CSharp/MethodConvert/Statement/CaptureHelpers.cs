// Copyright (C) 2015-2026 The Neo Project.
//
// CaptureHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        private void RegisterCapturedSymbols(SemanticModel model)
        {
            if (SyntaxNode is null)
                return;

            SyntaxNode? body = SyntaxNode switch
            {
                BaseMethodDeclarationSyntax method => (SyntaxNode?)method.Body ?? method.ExpressionBody?.Expression,
                AccessorDeclarationSyntax accessor => (SyntaxNode?)accessor.Body ?? accessor.ExpressionBody?.Expression,
                SimpleLambdaExpressionSyntax lambda => lambda.Body,
                ParenthesizedLambdaExpressionSyntax lambda => lambda.Body,
                ArrowExpressionClauseSyntax arrow => arrow.Expression,
                _ => null
            };

            // Register before emitting declarations or branches so every path uses the
            // same storage, even when a branch containing a lambda is skipped.
            if (body is not null)
            {
                foreach (ISymbol symbol in model.AnalyzeDataFlow(body).Captured)
                {
                    if (symbol is ILocalSymbol or IParameterSymbol &&
                        SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, Symbol))
                        _context.GetOrAddCapturedStaticField(symbol);
                }
            }

            // Loop control expressions can refer to symbols captured in the loop body.
            SyntaxNode root = SyntaxNode;
            foreach (SyntaxNode node in root.DescendantNodesAndSelf(descendIntoChildren: node =>
                node == root || node is not LambdaExpressionSyntax and not AnonymousMethodExpressionSyntax and not LocalFunctionStatementSyntax))
            {
                if (node is not WhileStatementSyntax and not DoStatementSyntax and not ForStatementSyntax and
                    not ForEachStatementSyntax and not ForEachVariableStatementSyntax)
                    continue;

                foreach (ISymbol symbol in model.AnalyzeDataFlow(node).Captured)
                {
                    if (symbol is ILocalSymbol or IParameterSymbol)
                        _context.GetOrAddCapturedStaticField(symbol);
                }
            }
        }
    }
}
