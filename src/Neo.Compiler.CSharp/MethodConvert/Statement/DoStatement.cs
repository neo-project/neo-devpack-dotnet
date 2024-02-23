// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'do-while' loop statement into a corresponding set of instructions.
        /// This method handles the parsing and translation of the 'do-while' loop construct,
        /// creating the necessary control flow for the loop's execution.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'do-while' statement.</param>
        /// <param name="syntax">The syntax representation of the 'do-while' statement being converted.</param>
        /// <remarks>
        /// The method sets up jump targets for the start, continue, and break points of the loop.
        /// It then converts the loop's statement body and condition. The loop continues if the condition
        /// is true, jumping back to the start. If the condition is false, the loop exits, and the control
        /// flow moves to the break target.
        /// </remarks>
        /// <example>
        /// Example of a 'do-while' loop syntax:
        /// <code>
        /// do
        /// {
        ///     // Loop body
        /// }
        /// while (condition);
        /// </code>
        /// This example shows a 'do-while' loop where the loop body is executed at least once
        /// before evaluating the condition.
        /// </example>
        private void ConvertDoStatement(SemanticModel model, DoStatementSyntax syntax)
        {
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, syntax.Condition);
            Jump(OpCode.JMPIF_L, startTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
