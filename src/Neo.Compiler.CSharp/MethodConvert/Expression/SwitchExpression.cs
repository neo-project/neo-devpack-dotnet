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
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertSwitchExpression(SemanticModel model, SwitchExpressionSyntax expression)
    {
        var arms = expression.Arms.Select(p => (p, new JumpTarget())).ToArray();
        JumpTarget breakTarget = new();
        byte anonymousIndex = AddAnonymousVariable();
        ConvertExpression(model, expression.GoverningExpression);
        AccessSlot(OpCode.STLOC, anonymousIndex);
        foreach (var (arm, nextTarget) in arms)
        {
            ConvertPattern(model, arm.Pattern, anonymousIndex);
            Jump(OpCode.JMPIFNOT_L, nextTarget);
            if (arm.WhenClause is not null)
            {
                ConvertExpression(model, arm.WhenClause.Condition);
                Jump(OpCode.JMPIFNOT_L, nextTarget);
            }
            ConvertExpression(model, arm.Expression);
            Jump(OpCode.JMP_L, breakTarget);
            nextTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        AccessSlot(OpCode.LDLOC, anonymousIndex);
        AddInstruction(OpCode.THROW);
        breakTarget.Instruction = AddInstruction(OpCode.NOP);
        RemoveAnonymousVariable(anonymousIndex);
    }
}
