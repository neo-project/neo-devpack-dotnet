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
