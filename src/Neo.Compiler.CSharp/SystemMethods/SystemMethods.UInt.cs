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
    private static void HandleUIntParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Atoi(methodConvert);
        sb.IsUIntCheck();
    }

    // HandleUIntLeadingZeroCount
    private static void HandleUIntLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        sb.Push(0); // count 5 0
        loopStart.Instruction = sb.Swap(); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.Jump(OpCode.JMPEQ, endLoop); //0 5
        sb.Push1();//0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jump(OpCode.JMP, loopStart);
        endLoop.Instruction = sb.Drop();
        sb.Push(32);
        sb.Swap();
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleUIntCreateChecked
    private static void HandleUIntCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUIntCheck();
    }

    // HandleUIntCreateSaturating
    private static void HandleUIntCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(uint.MinValue);
        sb.Push(uint.MaxValue);
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
        exceptionTarget.Instruction = sb.Nop();
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
        sb.Drop();// 5 10
        sb.Jmp(endTarget);
        minTarget.Instruction = sb.Nop();
        sb.Reverse3();
        sb.Drop();
        sb.Drop();
        sb.Jmp(endTarget);
        maxTarget.Instruction = sb.Nop();
        sb.Swap();
        sb.Drop();
        sb.Jmp(endTarget);
        endTarget.Instruction = sb.Nop();
    }

    // HandleUIntRotateLeft
    private static void HandleUIntRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static uint RotateLeft(uint value, int rotateAmount) => (uint)(value << rotateAmount) | (value >> (32 - rotateAmount));
        var bitWidth = sizeof(uint) * 8;
        sb.Push(bitWidth - 1);  // Push 31 (32-bit - 1)
        sb.And();    // rotateAmount & 31
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 31)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();    // Ensure SHL result is 32-bit
        sb.LdArg0(); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 32
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 32 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 31
        sb.And();    // (32 - rotateAmount) & 31
        sb.ShR();    // (uint)value >> ((32 - rotateAmount) & 31)
        sb.Or();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();    // Ensure final result is 32-bit
    }

    // HandleUIntRotateRight
    private static void HandleUIntRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static uint RotateRight(uint value, int rotateAmount) => (uint)(value >> rotateAmount) | (value << (32 - rotateAmount));
        var bitWidth = sizeof(uint) * 8;
        sb.Push(bitWidth - 1);  // Push (bitWidth - 1)
        sb.And();    // rotateAmount & (bitWidth - 1)
        sb.ShR();    // value >> (rotateAmount & (bitWidth - 1))
        sb.LdArg0(); // Load value again
        sb.Push(bitWidth);  // Push bitWidth
        sb.LdArg1(); // Load rotateAmount
        sb.Sub();    // bitWidth - rotateAmount
        sb.Push(bitWidth - 1);  // Push (bitWidth - 1)
        sb.And();    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShL();    // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();     // Combine the results with OR
        sb.Push((BigInteger.One << bitWidth) - 1);  // Push (2^bitWidth - 1) as bitmask
        sb.And();    // Ensure final result is bitWidth-bit
    }
}