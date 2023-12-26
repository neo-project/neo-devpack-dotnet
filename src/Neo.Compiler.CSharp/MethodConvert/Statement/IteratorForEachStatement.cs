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
using Neo.SmartContract;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertIteratorForEachStatement(SemanticModel model, ForEachStatementSyntax syntax)
        {
            ILocalSymbol elementSymbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget startTarget = new();
            JumpTarget continueTarget = new();
            JumpTarget breakTarget = new();
            byte iteratorIndex = AddAnonymousVariable();
            byte elementIndex = AddLocalVariable(elementSymbol);
            PushContinueTarget(continueTarget);
            PushBreakTarget(breakTarget);
            using (InsertSequencePoint(syntax.ForEachKeyword))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, iteratorIndex);
                Jump(OpCode.JMP_L, continueTarget);
            }
            using (InsertSequencePoint(syntax.Identifier))
            {
                startTarget.Instruction = AccessSlot(OpCode.LDLOC, iteratorIndex);
                Call(ApplicationEngine.System_Iterator_Value);
                AccessSlot(OpCode.STLOC, elementIndex);
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
            RemoveLocalVariable(elementSymbol);
            PopContinueTarget();
            PopBreakTarget();
        }


    }
}
