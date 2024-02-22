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
    private void ConvertBinaryPattern(SemanticModel model, BinaryPatternSyntax pattern, byte localIndex)
    {
        switch (pattern.OperatorToken.ValueText)
        {
            case "and":
                ConvertAndPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            case "or":
                ConvertOrPattern(model, pattern.Left, pattern.Right, localIndex);
                break;
            default:
                throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }

    private void ConvertAndPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertPattern(model, left, localIndex);
        Jump(OpCode.JMPIF_L, rightTarget);
        Push(false);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertOrPattern(SemanticModel model, PatternSyntax left, PatternSyntax right, byte localIndex)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertPattern(model, left, localIndex);
        Jump(OpCode.JMPIFNOT_L, rightTarget);
        Push(true);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertPattern(model, right, localIndex);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
