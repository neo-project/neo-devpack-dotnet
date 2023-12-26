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

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertArrayForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget conditionTarget = new();
            JumpTarget breakTarget = new();
            byte arrayIndex = AddAnonymousVariable();
            byte lengthIndex = AddAnonymousVariable();
            byte iIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AddInstruction(OpCode.DUP);
                AccessSlot(OpCode.STLOC, arrayIndex);
                AddInstruction(OpCode.SIZE);
                AccessSlot(OpCode.STLOC, lengthIndex);
                Push(0);
                AccessSlot(OpCode.STLOC, iIndex);
                Jump(OpCode.JMP_L, conditionTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, arrayIndex);
                AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.PICKITEM);
                AccessSlot(OpCode.STLOC, elementIndex);
            }
            ConvertStatement(model, syntax.Statement);
            using (InsertSequencePoint(syntax.Expression))
            {
                continueTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AddInstruction(OpCode.INC);
                AccessSlot(OpCode.STLOC, iIndex);
                conditionTarget.Instruction = AccessSlot(OpCode.LDLOC, iIndex);
                AccessSlot(OpCode.LDLOC, lengthIndex);
                Jump(OpCode.JMPLT_L, startTarget);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            RemoveAnonymousVariable(arrayIndex);
            RemoveAnonymousVariable(lengthIndex);
            RemoveAnonymousVariable(iIndex);
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
        }


    }
}
