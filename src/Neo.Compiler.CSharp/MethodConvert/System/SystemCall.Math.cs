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
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    // Handler for Math.Abs methods
    private static void HandleMathAbs(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.ABS);
    }

    // Handler for Math.Sign methods
    private static void HandleMathSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
    }

    // Handler for Math.Max methods
    private static void HandleMathMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MAX);
    }

    // Handler for Math.Min methods
    private static void HandleMathMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MIN);
    }

    private static void HandleMathByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(byte.MinValue);
        methodConvert.Push(new BigInteger(byte.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(new BigInteger(sbyte.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(new BigInteger(short.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ushort.MinValue);
        methodConvert.Push(new BigInteger(ushort.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(int.MinValue);
        methodConvert.Push(new BigInteger(int.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(uint.MinValue);
        methodConvert.Push(new BigInteger(uint.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(new BigInteger(ulong.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        methodConvert.AddInstruction(OpCode.DUP);// 5 0 10 10
        methodConvert.AddInstruction(OpCode.ROT);// 5 10 10 0
        methodConvert.AddInstruction(OpCode.DUP);// 5 10 10 0 0
        methodConvert.AddInstruction(OpCode.ROT);// 5 10 0 0 10
        methodConvert.Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
        methodConvert.AddInstruction(OpCode.THROW);
        exceptionTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.ROT);// 10 0 5
        methodConvert.AddInstruction(OpCode.DUP);// 10 0 5 5
        methodConvert.AddInstruction(OpCode.ROT);// 10 5 5 0
        methodConvert.AddInstruction(OpCode.DUP);// 10 5 5 0 0
        methodConvert.AddInstruction(OpCode.ROT);// 10 5 0 0 5
        methodConvert.Jump(OpCode.JMPGT, minTarget);// 10 5 0
        methodConvert.AddInstruction(OpCode.DROP);// 10 5
        methodConvert.AddInstruction(OpCode.DUP);// 10 5 5
        methodConvert.AddInstruction(OpCode.ROT);// 5 5 10
        methodConvert.AddInstruction(OpCode.DUP);// 5 5 10 10
        methodConvert.AddInstruction(OpCode.ROT);// 5 10 10 5
        methodConvert.Jump(OpCode.JMPLT, maxTarget);// 5 10
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endTarget);
        minTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.REVERSE3);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endTarget);
        maxTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endTarget);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleMathBigMul(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // RegisterHandler((double x) => Math.Ceiling(x), HandleMathCeiling);
    // RegisterHandler((double x) => Math.Floor(x), HandleMathFloor);
    // RegisterHandler((double x) => Math.Round(x), HandleMathRound);
    // RegisterHandler((double x) => Math.Truncate(x), HandleMathTruncate);
    // RegisterHandler((double x, double y) => Math.Pow(x, y), HandleMathPow);
    // RegisterHandler((double x) => Math.Sqrt(x), HandleMathSqrt);
    // RegisterHandler((double x) => Math.Log(x), HandleMathLog);
    // RegisterHandler((double x, double y) => Math.Log(x, y), HandleMathLogBase);
}
