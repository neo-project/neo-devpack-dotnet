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
    /// This method converts a ternary conditional expression to OpCodes.
    /// </summary>
    /// /// <param name="model">The semantic model providing context and information about ternary conditional expression.</param>
    /// <param name="expression">The syntax representation of the ternary conditional expression statement being converted.</param>
    /// <example>
    /// The conditional operator ?:, also known as the ternary conditional operator,
    /// evaluates a Boolean expression and returns the result of one of the two expressions,
    /// depending on whether the Boolean expression evaluates to true or false, as the following example shows:
    /// <code>
    /// var index = 10000;
    /// var current = Ledger.CurrentIndex;
    /// var state = current > index ? "start" : "stop";
    /// Runtime.Log(state.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-operator">?: operator - the ternary conditional operator</seealso>
    private void ConvertConditionalExpression(SemanticModel model, ConditionalExpressionSyntax expression)
    {
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, expression.Condition);
        Jump(OpCode.JMPIFNOT_L, falseTarget);
        ConvertExpression(model, expression.WhenTrue);
        Jump(OpCode.JMP_L, endTarget);
        falseTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, expression.WhenFalse);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
