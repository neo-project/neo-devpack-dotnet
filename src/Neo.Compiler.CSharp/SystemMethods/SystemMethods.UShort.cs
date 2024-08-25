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

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    private static void HandleUShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.Dup();
        sb.IsUShort();
        sb.JmpIf(endTarget);
        sb.Throw("Not a valid ushort value.");
        endTarget.Instruction = sb.Nop();
    }

    // HandleUShortLeadingZeroCount
    private static void HandleUShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
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
        sb.Push(0);// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push1(); //0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        endLoop.Instruction = sb.Drop();
        sb.Push(16);
        sb.Swap();
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleUShortCreateChecked
    private static void HandleUShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUShortCheck();
    }

    // HandleUShortCreateSaturating
    private static void HandleUShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(ushort.MinValue);
        sb.Push(ushort.MaxValue);
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
        sb.Drop();
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

    // implement HandleUShortRotateLeft
    private static void HandleUShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ushort RotateLeft(ushort value, int rotateAmount) => (ushort)((value << (rotateAmount & 15)) | (value >> ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(ushort) * 8;
        sb.Push(bitWidth - 1);  // Push 31 (32-bit - 1)
        sb.And();    // rotateAmount & 31
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 31)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();    // Ensure SHL result is 32-bit
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.And();
        sb.Swap();
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

    // HandleUShortRotateRight
    private static void HandleUShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ushort RotateRight(ushort value, int rotateAmount) => (ushort)((value >> (rotateAmount & 15)) | ((ushort)value << ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(ushort) * 8;
        sb.Push(bitWidth - 1);  // Push (bitWidth - 1)
        sb.And();    // rotateAmount & (bitWidth - 1)
        sb.ShR();    // value >> (rotateAmount & (bitWidth - 1))
        sb.Dup(); // Load value again
        sb.Push(bitWidth);  // Push bitWidth
        sb.Swap(); // Load rotateAmount
        sb.Sub();    // bitWidth - rotateAmount
        sb.Push(bitWidth - 1);  // Push (bitWidth - 1)
        sb.And();    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShL();    // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();     // Combine the results with OR
        sb.Push((BigInteger.One << bitWidth) - 1);  // Push (2^bitWidth - 1) as bitmask
        sb.And();    // Ensure final result is bitWidth-bit
    }
}
