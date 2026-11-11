// Copyright (C) 2015-2026 The Neo Project.
//
// LoopCaptureHelpers.cs file belongs to the neo project and is free
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
        private void RegisterCapturedSymbolsUsedByLoops(SemanticModel model)
        {
            if (SyntaxNode is null)
                return;

            // Register before emitting declarations or branches so every path uses the
            // same storage, including paths that skip the loop entirely.
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
