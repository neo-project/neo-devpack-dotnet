// Copyright (C) 2015-2026 The Neo Project.
//
// IfStatement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts an 'if' statement into a series of jump instructions and conditional logic.
        /// This method handles the translation of the 'if' condition and its corresponding 'else'
        /// branch (if present) into executable instructions.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'if' statement.</param>
        /// <param name="syntax">The syntax representation of the 'if' statement being converted.</param>
        /// <remarks>
        /// The method first evaluates the condition of the 'if' statement. Based on the condition,
        /// it either executes the 'if' block or jumps to the 'else' block (if it exists). In the case
        /// of an 'else if' chain, this logic is applied recursively. The method adds appropriate jump
        /// instructions to ensure correct control flow between the 'if' and 'else' blocks.
        /// </remarks>
        /// <example>
        /// Example of an 'if-else' statement syntax:
        /// <code>
        /// if (condition)
        /// {
        ///     // Code to execute if the condition is true
        /// }
        /// else
        /// {
        ///     // Code to execute if the condition is false
        /// }
        /// </code>
        /// This example shows an 'if' statement with a corresponding 'else' block. Depending on
        /// the evaluation of 'condition', either the 'if' block or the 'else' block will be executed.
        /// </example>
        private void ConvertIfStatement(SemanticModel model, IfStatementSyntax syntax)
        {
            JumpTarget elseTarget = new();

            using (InsertSequencePoint(syntax))
            {
                if (TryProcessZeroOrFalseCondition(model, syntax.Condition, SyntaxKind.EqualsExpression))
                {
                    Jump(OpCode.JMPIF_L, elseTarget);
                }
                else if (TryProcessZeroOrFalseCondition(model, syntax.Condition, SyntaxKind.NotEqualsExpression))
                {
                    Jump(OpCode.JMPIFNOT_L, elseTarget);
                }
                else
                {
                    ConvertExpression(model, syntax.Condition);
                    Jump(OpCode.JMPIFNOT_L, elseTarget);
                }

                ConvertStatement(model, syntax.Statement);

                if (syntax.Else is null)
                {
                    elseTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                else
                {
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMP_L, endTarget);

                    using (InsertSequencePoint(syntax.Else.Statement))
                    {
                        elseTarget.Instruction = AddInstruction(OpCode.NOP);
                        ConvertStatement(model, syntax.Else.Statement);
                    }

                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
            }
        }

        private bool TryProcessZeroOrFalseCondition(SemanticModel model, ExpressionSyntax syntax, SyntaxKind syntaxKind)
        {
            if (!IsPreEvaluatedInstanceExpression(syntax, out var _) &&
                syntax is BinaryExpressionSyntax expr && expr.Kind() == syntaxKind)
            {
                if (HasNullableOperand(model, expr)) return false;

                var type = model.GetTypeInfo(expr).Type!.GetStackItemType();
                if (type != StackItemType.Integer && type != StackItemType.Boolean) return false;

                var leftIs0 = TryGetIntegerConstant(model, expr.Left, out var left) && left.IsZero;
                var rightIs0 = TryGetIntegerConstant(model, expr.Right, out var right) && right.IsZero;
                if (leftIs0 || rightIs0)
                {
                    ConvertExpression(model, leftIs0 ? expr.Right : expr.Left);
                    return true;
                }

                var leftIsFalse = TryGetBoolConstant(model, expr.Left, out bool leftBool) && !leftBool;
                var rightIsFalse = TryGetBoolConstant(model, expr.Right, out bool rightBool) && !rightBool;
                if (leftIsFalse || rightIsFalse)
                {
                    ConvertExpression(model, leftIsFalse ? expr.Right : expr.Left);
                    return true;
                }
            }
            return false;
        }
    }
}
