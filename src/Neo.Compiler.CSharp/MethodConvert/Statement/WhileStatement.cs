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
        /// Converts a 'while' loop statement into a series of instructions for loop control.
        /// This method handles the translation of the 'while' loop's condition and body into an
        /// intermediate language, managing the loop's execution flow.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the while statement.</param>
        /// <param name="syntax">The syntax representation of the while statement being converted.</param>
        /// <remarks>
        /// The method sets up jump targets for the loop's continue and break points. It evaluates the
        /// loop's condition and generates a conditional jump based on this condition. If the condition
        /// is true, the loop's body is executed; otherwise, control jumps to the end of the loop.
        /// The method ensures proper loop continuation and exit behavior.
        /// </remarks>
        /// <example>
        /// Example of a while loop syntax:
        /// <code>
        /// while (condition)
        /// {
        ///     // Loop body code
        /// }
        /// </code>
        /// In this example, the loop continues to execute as long as the 'condition' evaluates to true.
        /// </example>
        private void ConvertWhileStatement(SemanticModel model, WhileStatementSyntax syntax)
        {
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            using (InsertSequencePoint(syntax.Condition))
            {
                ConvertExpression(model, syntax.Condition);
                Jump(OpCode.JMPIFNOT_L, breakTarget);
            }
            ConvertStatement(model, syntax.Statement);
            Jump(OpCode.JMP_L, continueTarget);
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
