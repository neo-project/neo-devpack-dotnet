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
        sb.SetTarget(endTarget);
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
        sb.Swap().SetTarget(loopStart); //0 5
        sb.Dup();//  0 5 5
        sb.Push(0);// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push1(); //0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        sb.Drop().SetTarget(endLoop);
        sb.Push(16);
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
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
        sb.SetTarget(endTarget);
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
        sb.And(bitWidth - 1);    // rotateAmount & 31
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 31)
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFF (32-bit mask)
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 32
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 32 - rotateAmount
        sb.And(bitWidth - 1);  // Push 31
        sb.ShR();    // (uint)value >> ((32 - rotateAmount) & 31)
        sb.Or();
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure final result is 32-bit
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
        sb.AddInstruction(OpCode.AND);    // rotateAmount & (bitWidth - 1)
        sb.AddInstruction(OpCode.SHR);    // value >> (rotateAmount & (bitWidth - 1))
        sb.AddInstruction(OpCode.LDARG0); // Load value again
        sb.Push(bitWidth);  // Push bitWidth
        sb.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        sb.AddInstruction(OpCode.SUB);    // bitWidth - rotateAmount
        sb.Push(bitWidth - 1);  // Push (bitWidth - 1)
        sb.AddInstruction(OpCode.AND);    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.AddInstruction(OpCode.SHL);    // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.AddInstruction(OpCode.OR);     // Combine the results with OR
        sb.Push((BigInteger.One << bitWidth) - 1);  // Push (2^bitWidth - 1) as bitmask
        sb.AddInstruction(OpCode.AND);    // Ensure final result is bitWidth-bit
    }
}
