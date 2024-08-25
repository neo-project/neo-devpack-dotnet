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
    private static void HandleShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsShortCheck();
    }

    // HandleShortLeadingZeroCount

    private static void HandleShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        sb.Dup(); // a a
        sb.Push(0);// a a 0
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push(0);
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push(0); // count 5 0
        loopStart.Instruction = sb.Swap(); //0 5
        sb.Dup();//  0 5 5
        sb.Push(0);// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push(1);//0 5 1
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

    // HandleShortCopySign
    private static void HandleShortCopySign(MethodConvert methodConvert, SemanticModel model,
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
        sb.AddInstruction(OpCode.SIGN);         // a 1
        sb.AddInstruction(OpCode.DUP); // a 1 1
        sb.Push(0); // a 1 1 0
        sb.Jump(OpCode.JMPLT, nonZeroTarget); // a 1
        sb.AddInstruction(OpCode.DROP);
        sb.Push(1); // a 1
        nonZeroTarget.Instruction = sb.AddInstruction(OpCode.NOP); // a 1
        sb.AddInstruction(OpCode.SWAP);         // 1 a
        sb.AddInstruction(OpCode.DUP);// 1 a a
        sb.AddInstruction(OpCode.SIGN);// 1 a 0
        sb.AddInstruction(OpCode.DUP);// 1 a 0 0
        sb.Push(0); // 1 a 0 0 0
        sb.Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
        sb.AddInstruction(OpCode.DROP);
        sb.Push(1);
        nonZeroTarget2.Instruction = sb.AddInstruction(OpCode.NOP); // 1 a 1
        sb.AddInstruction(OpCode.ROT);// a 1 1
        sb.AddInstruction(OpCode.EQUAL);// a 1 1
        JumpTarget endTarget = new();
        sb.Jump(OpCode.JMPIF, endTarget); // a
        sb.AddInstruction(OpCode.NEGATE);
        endTarget.Instruction = sb.AddInstruction(OpCode.NOP);

        var endTarget2 = new JumpTarget();
        sb.AddInstruction(OpCode.DUP);
        sb.Push(short.MinValue);
        sb.Push(new BigInteger(short.MaxValue) + 1);
        sb.AddInstruction(OpCode.WITHIN);
        sb.Jump(OpCode.JMPIF, endTarget2);
        sb.AddInstruction(OpCode.THROW);
        endTarget2.Instruction = sb.AddInstruction(OpCode.NOP);
    }

    // HandleShortCreateChecked
    private static void HandleShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsShortCheck();
    }

    // HandleShortCreateSaturating
    private static void HandleShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(short.MinValue);
        sb.Push(short.MaxValue);
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

    // implement HandleShortRotateLeft
    private static void HandleShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateLeft(short value, int rotateAmount) => (short)((value << (rotateAmount & 15)) | ((ushort)value >> ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        sb.Push(bitWidth - 1);  // Push 15 (16-bit - 1)
        sb.And();    // rotateAmount & 15
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.And();
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 15)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.And();    // Ensure SHL result is 16-bit
        sb.LdArg0(); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.And();
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 16
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 16 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 15
        sb.And();    // (16 - rotateAmount) & 15
        sb.ShR();    // (ushort)value >> ((16 - rotateAmount) & 15)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleShortRotateRight
    private static void HandleShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateRight(short value, int rotateAmount) => (short)((value >> (rotateAmount & 15)) | ((ushort)value << ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        sb.Push(bitWidth - 1);  // Push 15 (16-bit - 1)
        sb.AddInstruction(OpCode.AND);    // rotateAmount & 15
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.MOD);
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SUB);
        sb.AddInstruction(OpCode.SWAP);
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.AddInstruction(OpCode.AND);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 15)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.AddInstruction(OpCode.AND);    // Ensure SHL result is 16-bit
        sb.AddInstruction(OpCode.LDARG0); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        sb.AddInstruction(OpCode.AND);
        sb.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.MOD);
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SUB);
        sb.Push(bitWidth);  // Push 16
        sb.AddInstruction(OpCode.SWAP);   // Swap top two elements
        sb.AddInstruction(OpCode.SUB);    // 16 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 15
        sb.AddInstruction(OpCode.AND);    // (16 - rotateAmount) & 15
        sb.AddInstruction(OpCode.SHR);    // (ushort)value >> ((16 - rotateAmount) & 15)
        sb.AddInstruction(OpCode.OR);
        sb.AddInstruction(OpCode.DUP);    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPLT, endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        sb.AddInstruction(OpCode.SUB);
        endTarget.Instruction = sb.AddInstruction(OpCode.NOP);
    }
}
