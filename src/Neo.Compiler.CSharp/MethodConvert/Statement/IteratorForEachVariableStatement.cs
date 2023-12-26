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
using Neo.SmartContract;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertIteratorForEachVariableStatement(SemanticModel model, ForEachVariableStatementSyntax syntax)
        {
            ILocalSymbol[] symbols = ((ParenthesizedVariableDesignationSyntax)((DeclarationExpressionSyntax)syntax.Variable).Designation).Variables.Select(p => (ILocalSymbol)model.GetDeclaredSymbol(p)!).ToArray();
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Variable))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
                AddInstruction(OpCode.UNPACK);
                AddInstruction(OpCode.DROP);
                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i] is null)
                    {
                        AddInstruction(OpCode.DROP);
                    }
                    else
                    {
                        byte variableIndex = AddLocalVariable(symbols[i]);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
                }
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Next);
                Jump(OpCode.JMPIF_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(iteratorIndex);
            foreach (ILocalSymbol symbol in symbols)
                if (symbol is not null)
                    RemoveLocalVariable(symbol);
            PopContinueTarget();
            PopBreakTarget();
        }
    }
}
