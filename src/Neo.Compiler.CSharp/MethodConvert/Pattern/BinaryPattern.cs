// Copyright (C) 2015-2025 The Neo Project.
//
// BinaryPattern.cs file belongs to the neo project and is free
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
    /// Convet a binary pattern to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about binary pattern.</param>
    /// <param name="pattern">The binary pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <see cref="ConvertAndPattern(SemanticModel, PatternSyntax, PatternSyntax, byte)"/>
    /// <see cref="ConvertOrPattern(SemanticModel, PatternSyntax, PatternSyntax, byte)"/>
    private void ConvertBinaryPattern(SemanticModel model, BinaryPatternSyntax pattern, byte localIndex)
    {
        switch (pattern.OperatorToken.ValueText)
        {
            case "and":
                ConvertAndPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            case "or":
                ConvertOrPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(pattern, $"Binary pattern operator '{pattern.OperatorToken.ValueText}' is not supported. Only 'and' and 'or' operators are allowed in binary patterns.");
        }
    }

    /// <summary>
    /// Convet a "and" pattern to OpCodes.
    /// </summary>
    /// <remarks>
    /// Conjunctive "and" pattern that matches an expression when both patterns match the expression.
    /// </remarks>
    /// <param name="model">The semantic model providing context and information about "and" pattern.</param>
    /// <param name="left">The left pattern to be converted.</param>
    /// <param name="right">The right pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// The following example shows how you can combine relational patterns to check if a value is in a certain range:
    /// <code>
    /// public static string Classify(int measurement) => measurement switch
    /// {
    ///     < -40 => "Too low",
    ///     >= -40 and < 0 => "Low",
    ///     >= 0 and < 10 => "Acceptable",
    ///     >= 10 and < 20 => "High",
    ///     >= 20 => "Too high"
    /// };
    /// </code>
    /// </example>
    private void ConvertAndPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        // Define jump targets for the right pattern and the end of the conversion process
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();

        // Convert the left pattern
        ConvertPattern(model, left, localIndex);

        // Jump to the right pattern if the left pattern matches
        Jump(OpCode.JMPIF_L, rightTarget);

        // Push 'false' onto the evaluation stack and jump to the end if the left pattern does not match
        Push(false);
        Jump(OpCode.JMP_L, endTarget);

        // Define an instruction for the right pattern and convert it
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);

        // Define an instruction for the end of the conversion process
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    /// <summary>
    /// Convet a "or" pattern to OpCodes.
    /// </summary>
    /// <remarks>
    /// Disjunctive "or" pattern that matches an expression when either pattern matches the expression.
    /// </remarks>
    /// <param name="model">The semantic model providing context and information about "or" pattern.</param>
    /// <param name="left">The left pattern to be converted.</param>
    /// <param name="right">The right pattern to be converted.</param>
    /// <param name="localIndex">The index of the local variable.</param>
    /// <example>
    /// As the following example shows:
    /// <code>
    /// public static string GetCalendarSeason(int month) => month switch
    /// {
    ///     3 or 4 or 5 => "spring",
    ///     6 or 7 or 8 => "summer",
    ///     9 or 10 or 11 => "autumn",
    ///     12 or 1 or 2 => "winter",
    ///     _ => throw new Exception($"Unexpected month: {month}."),
    /// };
    /// </code>
    /// As the preceding example shows, you can repeatedly use the pattern combinators in a pattern.
    /// </example>
    private void ConvertOrPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        // Define jump targets for the right pattern and the end of the conversion process
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();

        // Convert the left pattern
        ConvertPattern(model, left, localIndex);

        // Jump to the right pattern if the left pattern does not match
        Jump(OpCode.JMPIFNOT_L, rightTarget);

        // Push 'true' onto the evaluation stack and jump to the end if the left pattern matches
        Push(true);
        Jump(OpCode.JMP_L, endTarget);

        // Define an instruction for the right pattern and convert it
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);

        // Define an instruction for the end of the conversion process
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
