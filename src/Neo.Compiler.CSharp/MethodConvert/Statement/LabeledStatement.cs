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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertLabeledStatement(SemanticModel model, LabeledStatementSyntax syntax)
    {
        ILabelSymbol symbol = model.GetDeclaredSymbol(syntax)!;
        JumpTarget target = AddLabel(symbol, true);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            foreach (Instruction instruction in result.PendingGotoStatments)
                if (instruction.Target == target)
                    instruction.OpCode = OpCode.JMP_L;
        target.Instruction = AddInstruction(OpCode.NOP);
        ConvertStatement(model, syntax.Statement);
    }
}
