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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {

        /// <summary>
        /// Converts C# conditional expressions to executable code.
        /// </summary>
        /// <param name="model">The semantic model</param>
        /// <param name="expression">The conditional expression syntax node</param>
        /// <remarks>
        /// This handles conversion of ternary conditional expressions like:
        ///
        /// result = x > 5 ? 10 : 20;
        ///
        /// The condition is first evaluated, followed by a jump to either the true or
        /// false branch based on the condition value. The respective branch expression is
        /// evaluated and leaves the final result on the stack.
        ///
        /// This allows if-then-else conditional logic in expressions using specialized
        /// branching instructions.
        /// </remarks>
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
}
