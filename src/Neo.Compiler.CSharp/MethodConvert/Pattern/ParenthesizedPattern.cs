// Copyright (C) 2015-2024 The Neo Project.
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convet a parenthesized pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about parenthesized pattern.</param>
    /// <param name="pattern">The parenthesized pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <remarks>
    /// You can put parentheses around any pattern.
    /// Typically, you do that to emphasize or change the precedence in logical patterns,
    /// as the following example shows:
    /// </remarks>
    /// <example>
    /// <c>return value is (> 1 and < 100);</c>
    /// </example>
    private void ConvertParenthesizedPatternSyntax(SemanticModel model, ParenthesizedPatternSyntax pattern, byte localIndex)
    {
        ConvertPattern(model, pattern.Pattern, localIndex);
    }
}
