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

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertIfStatement(SemanticModel model, IfStatementSyntax syntax)
    {
        JumpTarget elseTarget = new();
        using (InsertSequencePoint(syntax.Condition))
            ConvertExpression(model, syntax.Condition);
        Jump(OpCode.JMPIFNOT_L, elseTarget);
        ConvertStatement(model, syntax.Statement);
        if (syntax.Else is null)
        {
            elseTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        else
        {
            JumpTarget endTarget = new();
            Jump(OpCode.JMP_L, endTarget);
            elseTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Else.Statement);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
    }
}
