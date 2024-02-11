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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'for' loop statement into a sequence of instructions. This method handles the parsing
        /// and translation of the 'for' loop construct, including its initialization, condition, and
        /// increment expressions, as well as the loop body.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'for' loop statement.</param>
        /// <param name="syntax">The syntax representation of the 'for' loop statement being converted.</param>
        /// <remarks>
        /// The method starts by handling variable declarations and initializations. It then sets up
        /// jump targets for managing the loop's control flow: start, continue, condition, and break.
        /// The loop's body, condition, and increment expressions are then converted into appropriate
        /// instructions. The method also ensures any non-void expressions that leave a value on the stack
        /// are appropriately dropped.
        /// </remarks>
        /// <example>
        /// Example of a 'for' loop syntax:
        /// <code>
        /// for (int i = 0; i < 10; i++)
        /// {
        ///     // Loop body
        /// }
        /// </code>
        /// This example shows a 'for' loop where 'i' is initialized to 0, the loop continues as long
        /// as 'i' is less than 10, and 'i' is incremented by 1 after each iteration.
        /// </example>
        private void ConvertForStatement(SemanticModel model, ForStatementSyntax syntax)
        {
            var variables = (syntax.Declaration?.Variables ?? Enumerable.Empty<VariableDeclaratorSyntax>())
                .Select(p => (p, (ILocalSymbol)model.GetDeclaredSymbol(p)!))
                .ToArray();
            foreach (ExpressionSyntax expression in syntax.Initializers)
                using (InsertSequencePoint(expression))
                {
                    ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                    ConvertExpression(model, expression);
                    if (type.SpecialType != SpecialType.System_Void)
                        AddInstruction(OpCode.DROP);
                }
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            foreach (var (variable, symbol) in variables)
            {
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                    using (InsertSequencePoint(variable))
                    {
                        ConvertExpression(model, variable.Initializer.Value);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
            }
            Jump(OpCode.JMP_L, conditionTarget);
            startTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
            continueTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (ExpressionSyntax expression in syntax.Incrementors)
                using (InsertSequencePoint(expression))
                {
                    ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                    ConvertExpression(model, expression);
                    if (type.SpecialType != SpecialType.System_Void)
                        AddInstruction(OpCode.DROP);
                }
            conditionTarget.Instruction = AddInstruction(OpCode.NOP);
            if (syntax.Condition is null)
            {
                Jump(OpCode.JMP_L, startTarget);
            }
            else
            {
                using (InsertSequencePoint(syntax.Condition))
                    ConvertExpression(model, syntax.Condition);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            foreach (var (_, symbol) in variables)
                RemoveLocalVariable(symbol);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
