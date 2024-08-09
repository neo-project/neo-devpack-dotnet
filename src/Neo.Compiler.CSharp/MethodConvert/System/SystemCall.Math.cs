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
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{

    // Handler for Math.Abs methods
    private bool HandleMathAbs(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.ABS);
        return true;
    }

    // Handler for Math.Sign methods
    private bool HandleMathSign(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.SIGN);
        return true;
    }

    // Handler for Math.Max methods
    private bool HandleMathMax(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.MAX);
        return true;
    }

    // Handler for Math.Min methods
    private bool HandleMathMin(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.MIN);
        return true;
    }

    private bool HandleMathByteDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(byte.MinValue);
        Push(new BigInteger(byte.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathSByteDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(sbyte.MinValue);
        Push(new BigInteger(sbyte.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathShortDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(short.MinValue);
        Push(new BigInteger(short.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathUShortDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(ushort.MinValue);
        Push(new BigInteger(ushort.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathIntDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(int.MinValue);
        Push(new BigInteger(int.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathUIntDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(uint.MinValue);
        Push(new BigInteger(uint.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathLongDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(long.MinValue);
        Push(new BigInteger(long.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathULongDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.DUP);
        Push(ulong.MinValue);
        Push(new BigInteger(ulong.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

    private bool HandleMathClamp(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);

        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        AddInstruction(OpCode.REVERSE3);// 5 0 10
        AddInstruction(OpCode.DUP);// 5 0 10 10
        AddInstruction(OpCode.ROT);// 5 10 10 0
        AddInstruction(OpCode.DUP);// 5 10 10 0 0
        AddInstruction(OpCode.ROT);// 5 10 0 0 10
        Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
        AddInstruction(OpCode.THROW);
        exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.ROT);// 10 0 5
        AddInstruction(OpCode.DUP);// 10 0 5 5
        AddInstruction(OpCode.ROT);// 10 5 5 0
        AddInstruction(OpCode.DUP);// 10 5 5 0 0
        AddInstruction(OpCode.ROT);// 10 5 0 0 5
        Jump(OpCode.JMPGT, minTarget);// 10 5 0
        AddInstruction(OpCode.DROP);// 10 5
        AddInstruction(OpCode.DUP);// 10 5 5
        AddInstruction(OpCode.ROT);// 5 5 10
        AddInstruction(OpCode.DUP);// 5 5 10 10
        AddInstruction(OpCode.ROT);// 5 10 10 5
        Jump(OpCode.JMPLT, maxTarget);// 5 10
        AddInstruction(OpCode.DROP);
        Jump(OpCode.JMP, endTarget);
        minTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.REVERSE3);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.DROP);
        Jump(OpCode.JMP, endTarget);
        maxTarget.Instruction = AddInstruction(OpCode.NOP);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.DROP);
        Jump(OpCode.JMP, endTarget);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    private bool HandleMathBigMul(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.DUP);
        Push(int.MinValue);
        Push(new BigInteger(int.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }
}
