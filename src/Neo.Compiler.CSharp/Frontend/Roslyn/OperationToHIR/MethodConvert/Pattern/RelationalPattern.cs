// Copyright (C) 2015-2025 The Neo Project.
//
// RelationalPattern.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
    /// Convet relational pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about convert pattern.</param>
    /// <param name="pattern">The convert pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <exception cref="CompilationException"></exception>
    /// <remarks>
    /// In a relational pattern, you can use any of the relational operators <![CDATA[<, >, <=, or >=]]>.
    /// The right-hand part of a relational pattern must be a constant expression.
    /// The constant expression can be of an integer, char, or enum type.
    /// To check if an expression result is in a certain range, match it against a <see cref="ConvertBinaryPattern(SemanticModel, BinaryPatternSyntax, byte)">conjunctive and pattern</see>.
    /// </remarks>
    /// <example>
    /// You use a relational pattern to compare an expression result with a constant,
    /// as the following example shows:
    /// <code>
    /// int a = 1;
    /// var b = a switch
    /// {
    ///     > 1 => true,
    ///     <= 1 => false
    /// };
    /// </code>
    /// <c>> 1</c> and <c><= 1</c> is RelationalPatternSyntax;
    /// </example>
    private void ConvertRelationalPattern(SemanticModel model, RelationalPatternSyntax pattern, byte localIndex)
    {
        AccessSlot(OpCode.LDLOC, localIndex);
        ConvertExpression(model, pattern.Expression);
        AddInstruction(pattern.OperatorToken.ValueText switch
        {
            "<" => OpCode.LT,
            "<=" => OpCode.LE,
            ">" => OpCode.GT,
            ">=" => OpCode.GE,
            _ => throw CompilationException.UnsupportedSyntax(pattern, $"Relational operator '{pattern.OperatorToken.ValueText}' is not supported in patterns. Only <, <=, >, and >= are allowed.")
        });
    }
}
