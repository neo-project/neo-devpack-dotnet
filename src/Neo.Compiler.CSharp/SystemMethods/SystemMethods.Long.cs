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
    private static void HandleLongParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsLongCheck();
    }

    // HandleLongLeadingZeroCount
    private static void HandleLongLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        sb.Dup(); // a a
        sb.Push0();// a a 0
        JumpTarget notNegative = new();
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push0();
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push0(); // count 5 0
        sb.Swap().SetTarget(loopStart); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.ShR(1); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        sb.Drop().SetTarget(endLoop);
        sb.Push(64);
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleLongCopySign
    private static void HandleLongCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget nonZeroTarget = new();
        JumpTarget nonZeroTarget2 = new();
        // a b
        sb.Sign();         // a 1
        sb.Dup(); // a 1 1
        sb.Push0(); // a 1 1 0
        sb.JmpLt(nonZeroTarget); // a 1
        sb.Drop();
        sb.Push1(); // a 1
        sb.SetTarget(nonZeroTarget); // nonZeroTarget.Instruction = sb.Nop(); // a 1
        sb.Swap();         // 1 a
        sb.Dup();// 1 a a
        sb.Sign();// 1 a 0
        sb.Dup();// 1 a 0 0
        sb.Push0(); // 1 a 0 0 0
        sb.JmpLt(nonZeroTarget2); // 1 a 0
        sb.Drop();
        sb.Push1();
        sb.SetTarget(nonZeroTarget2); // 1 a 1
        sb.Rot();// a 1 1
        sb.Equal();// a 1 1
        JumpTarget endTarget = new();
        sb.JmpIf(endTarget); // a
        sb.Negate();
        sb.SetTarget(endTarget);
        sb.IsLongCheck();
    }

    // HandleLongCreateChecked
    private static void HandleLongCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsLongCheck();
    }

    // HandleLongCreateSaturating
    private static void HandleLongCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(long.MinValue);
        sb.Push(long.MaxValue);
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
        sb.Drop(2);
        sb.Jmp(endTarget);
        sb.SetTarget(maxTarget);
        sb.Swap();
        sb.Drop();
        sb.SetTarget(endTarget);
    }

    // HandleLongRotateLeft
    private static void HandleLongRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static long RotateLeft(long value, int rotateAmount) => (long)((value << (rotateAmount & 63)) | ((ulong)value >> (64 - (rotateAmount & 63))));
        var bitWidth = sizeof(long) * 8;
        sb.And(bitWidth - 1);    // rotateAmount & 63
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 63)
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure SHL result is 64-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 64
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 64 - rotateAmount
        sb.And(bitWidth - 1);    // (64 - rotateAmount) & 63
        sb.ShR();    // (ulong)value >> ((64 - rotateAmount) & 63)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 63 (0x8000000000000000)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 64 (0x10000000000000000)
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleLongRotateRight
    private static void HandleLongRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static long RotateRight(long value, int rotateAmount) => (long)(((ulong)value >> (rotateAmount & 63)) | ((long)value << (64 - (rotateAmount & 63))));
        var bitWidth = sizeof(long) * 8;
        sb.And(bitWidth - 1);    // rotateAmount & 63
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1);
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 63)
        sb.And((BigInteger.One << bitWidth) - 1);    // Ensure SHL result is 64-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1);
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Push(bitWidth);  // Push 64
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 64 - rotateAmount
        sb.And(bitWidth - 1);    // (64 - rotateAmount) & 63
        sb.ShR();    // (ulong)value >> ((64 - rotateAmount) & 63)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 63 (0x8000000000000000)
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPLT, endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 64 (0x10000000000000000)
        sb.Sub();
        sb.SetTarget(endTarget);
    }
}
