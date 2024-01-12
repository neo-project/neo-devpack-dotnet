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
