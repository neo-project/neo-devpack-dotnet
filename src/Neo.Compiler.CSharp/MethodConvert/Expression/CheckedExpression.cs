// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertCheckedExpression(SemanticModel model, CheckedExpressionSyntax expression)
    {
        _checkedStack.Push(expression.Keyword.IsKind(SyntaxKind.CheckedKeyword));
        ConvertExpression(model, expression.Expression);
        _checkedStack.Pop();
    }
}
