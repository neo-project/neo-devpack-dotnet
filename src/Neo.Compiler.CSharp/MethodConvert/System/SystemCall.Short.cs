// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    private bool HandleShortParse(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        AddInstruction(OpCode.DUP);
        Push(short.MinValue);
        Push(short.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }
}
