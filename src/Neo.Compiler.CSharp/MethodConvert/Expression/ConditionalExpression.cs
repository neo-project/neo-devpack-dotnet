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

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertConditionalExpression(SemanticModel model, ConditionalExpressionSyntax expression)
        {
            JumpTarget falseTarget = new();
            JumpTarget endTarget = new();
            ConvertExpression(model, expression.Condition);
            Jump(OpCode.JMPIFNOT_L, falseTarget);
            ConvertExpression(model, expression.WhenTrue);
            Jump(OpCode.JMP_L, endTarget);
            falseTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, expression.WhenFalse);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
    }
}
