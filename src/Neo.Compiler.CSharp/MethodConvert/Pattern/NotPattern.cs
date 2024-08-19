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
    /// Convet a "not" pattern to OpCodes.
    /// </summary>
    /// <remarks>
    /// Negation "not" pattern that matches an expression when the negated pattern doesn't match the expression.
    /// </remarks>
    /// <param name="model">The semantic model providing context and information about "not" pattern.</param>
    /// <param name="pattern">The "not" pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// The following example shows how you can negate a constant null pattern to check if an expression is non-null:
    /// <code>
    /// if (input is not null)
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </example>
    private void ConvertNotPattern(SemanticModel model, UnaryPatternSyntax pattern, byte localIndex)
    {
        ConvertPattern(model, pattern.Pattern, localIndex);
        AddInstruction(OpCode.NOT);
    }
}
