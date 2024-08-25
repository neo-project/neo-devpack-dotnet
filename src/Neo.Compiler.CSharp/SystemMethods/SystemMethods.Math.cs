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

internal static partial class SystemMethods
{
    // Handler for Math.Abs methods
    private static void HandleMathAbs(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Abs();
    }

    // Handler for Math.Sign methods
    private static void HandleMathSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Sign();
    }

    // Handler for Math.Max methods
    private static void HandleMathMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Max();
    }

    // Handler for Math.Min methods
    private static void HandleMathMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Min();
    }

    private static void HandleMathByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.Dup();
        sb.IsByte();
        sb.Jump(OpCode.JMPIF, endTarget);
        sb.Throw();
        sb.SetTarget(endTarget);
        sb.Pack(2);
    }

    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.Dup();
        sb.IsSByte();
        sb.JmpIf(endTarget);
        sb.Throw();
        sb.SetTarget(endTarget);
        sb.Pack(2);
    }

    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.Dup();
        sb.IsShort();
        sb.JmpIf(endTarget);
        sb.Throw();
        sb.SetTarget(endTarget);
        sb.Pack(2);
    }

    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.Dup();
        sb.Push(ushort.MinValue);
        sb.Push(new BigInteger(ushort.MaxValue) + 1);
        sb.AddInstruction(OpCode.WITHIN);
        sb.Jump(OpCode.JMPIF, endTarget);
        sb.AddInstruction(OpCode.THROW);
        sb.SetTarget(endTarget);
        sb.Pack(2);
    }

    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.IsIntCheck();
        sb.Pack(2);
    }

    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.IsUIntCheck();
        sb.Pack(2);
    }

    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.IsLongCheck();
        sb.Pack(2);
    }

    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.IsULongCheck();
        sb.Pack(2);
    }

    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        sb.Dup();// 5 0 10 10
        sb.Rot();// 5 10 10 0
        sb.Dup();// 5 10 10 0 0
        sb.Rot();// 5 10 0 0 10
        sb.JmpLt(exceptionTarget);// 5 10 0
        sb.Throw();
        sb.SetTarget(exceptionTarget);
        sb.Rot();// 10 0 5
        sb.Dup();// 10 0 5 5
        sb.Rot();// 10 5 5 0
        sb.Dup();// 10 5 5 0 0
        sb.Rot();// 10 5 0 0 5
        sb.JmpGt(minTarget);// 10 5 0
        sb.Drop();// 10 5
        sb.Dup();// 10 5 5
        sb.Rot();// 5 5 10
        sb.Dup();// 5 5 10 10
        sb.Rot();// 5 10 10 5
        sb.JmpLt(maxTarget);// 5 10
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(minTarget);
        sb.Reverse3();
        sb.Drop();
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(maxTarget);
        sb.Swap();
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(endTarget);
    }

    private static void HandleMathBigMul(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Mul();
        sb.IsLongCheck();
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
