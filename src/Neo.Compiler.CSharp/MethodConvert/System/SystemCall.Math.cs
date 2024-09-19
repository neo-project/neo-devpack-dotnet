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
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var exceptionTarget = new JumpTarget();
        // Evaluation stack: value=5 min=0 max=10 <- top
        methodConvert.AddInstruction(OpCode.OVER);  // 5 0 10 0
        methodConvert.AddInstruction(OpCode.OVER);  // 5 0 10 0 10 <- top
        methodConvert.Jump(OpCode.JMPLE, exceptionTarget);  // 5 0 10  // if 0 <= 10, continue execution
        //methodConvert.Push("min>max");
        methodConvert.AddInstruction(OpCode.THROW);
        exceptionTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.REVERSE3);  // 10 0 5
        // MAX&MIN costs 1<<3 each; 16 Datoshi more expensive at runtime
        methodConvert.AddInstruction(OpCode.MAX);  // 10 5
        methodConvert.AddInstruction(OpCode.MIN);  // 5
        //methodConvert.AddInstruction(OpCode.RET);
        // Alternatively, a slightly cheaper way at runtime; 10 to 16 Datoshi
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 0 5 0
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 0 5 0 5
        //methodConvert.Jump(OpCode.JMPGE, minTarget);  // 10 0 5; should return 0 if JMPed
        //methodConvert.AddInstruction(OpCode.NIP);  // 10 5
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 5 10
        //methodConvert.AddInstruction(OpCode.OVER);  // 10 5 10 5
        //methodConvert.Jump(OpCode.JMPLE, maxTarget);  // 10 5; should return 10 if JMPed
        //methodConvert.AddInstruction(OpCode.NIP);  // 5; should return 5
        //methodConvert.AddInstruction(OpCode.RET);
        //minTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);  // 10 0 5; should return 0
        //methodConvert.AddInstruction(OpCode.DROP);  // 10 0; should return 0
        //methodConvert.AddInstruction(OpCode.NIP);  // 0; should return 0
        //methodConvert.AddInstruction(OpCode.RET);
        //maxTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);  // 10 5; should return 10
        //methodConvert.AddInstruction(OpCode.DROP);  // 10; should return 10
        //methodConvert.AddInstruction(OpCode.RET);
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
