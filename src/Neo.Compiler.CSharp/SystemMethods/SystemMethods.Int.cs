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
    private static void HandleIntParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Atoi(methodConvert);
        sb.IsIntCheck();
    }

    // HandleIntLeadingZeroCount
    private static void HandleIntLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        sb.Dup();
        sb.Push0();
        JumpTarget notNegative = new();
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push0();
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push0(); // count 5 0
        loopStart.Instruction = sb.Swap(); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push1();//0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        endLoop.Instruction = sb.Drop();
        sb.Push(32);
        sb.Swap();
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleIntCopySign
    private static void HandleIntCopySign(MethodConvert methodConvert, SemanticModel model,
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
        nonZeroTarget.Instruction = sb.Nop(); // a 1
        sb.Swap();         // 1 a
        sb.Dup();// 1 a a
        sb.Sign();// 1 a 0
        sb.Dup();// 1 a 0 0
        sb.Push0(); // 1 a 0 0 0
        sb.JmpLt(nonZeroTarget2); // 1 a 0
        sb.Drop();
        sb.Push1();
        nonZeroTarget2.Instruction = sb.Nop(); // 1 a 1
        sb.Rot();// a 1 1
        sb.Equal();// a 1 1
        JumpTarget endTarget = new();
        sb.JmpIf(endTarget); // a
        sb.Negate();
        endTarget.Instruction = sb.Nop();
        sb.IsIntCheck();
    }

    // HandleIntCreateChecked
    private static void HandleIntCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsIntCheck();
    }

    // HandleIntCreateSaturating
    private static void HandleIntCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(int.MinValue);
        sb.Push(int.MaxValue);
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

    // HandleIntRotateLeft
    private static void HandleIntRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static int RotateLeft(int value, int rotateAmount) => (int)((value << (rotateAmount & 31)) | ((uint)value >> ((32 - rotateAmount) & 31)));
        var bitWidth = sizeof(int) * 8;
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
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 31 (0x80000000)
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPLT, endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 32 (0x100000000)
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleIntRotateRight
    private static void HandleIntRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static int RotateRight(int value, int rotateAmount) => (int)(((uint)value >> (rotateAmount & 31)) | (value << ((32 - rotateAmount) & 31)));
        var bitWidth = sizeof(int) * 8;
        sb.Push(bitWidth - 1);  // Push 31 (32-bit - 1)
        sb.And();    // rotateAmount & 31
        sb.Push(bitWidth);
        sb.Mod();
        sb.Swap();
        sb.Sub();
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
        sb.Push(bitWidth);
        sb.Mod();
        sb.Swap();
        sb.Sub();
        sb.Push(bitWidth);  // Push 32
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 32 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 31
        sb.And();    // (32 - rotateAmount) & 31
        sb.ShR();    // (uint)value >> ((32 - rotateAmount) & 31)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 31 (0x80000000)
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPLT, endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 32 (0x100000000)
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }
}
