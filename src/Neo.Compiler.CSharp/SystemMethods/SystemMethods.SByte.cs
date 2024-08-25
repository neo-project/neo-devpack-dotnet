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
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    private static void HandleSByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsSByteCheck();
    }

    // HandleSByteLeadingZeroCount
    private static void HandleSByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
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
        sb.Push0(); // a a 0
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push0();
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push(0); // count 5 0
        sb.Swap().SetTarget(loopStart); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push1();//0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        sb.Drop().SetTarget(endLoop);
        sb.Push(8);
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleSByteCopySign
    private static void HandleSByteCopySign(MethodConvert methodConvert, SemanticModel model,
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
        sb.Push(sbyte.MinValue);
        sb.Push(new BigInteger(sbyte.MaxValue) + 1);
        sb.AddInstruction(OpCode.WITHIN);
        sb.Jump(OpCode.JMPIF, endTarget2);
        sb.AddInstruction(OpCode.THROW);
        endTarget2.Instruction = sb.AddInstruction(OpCode.NOP);
    }

    // HandleSByteCreateChecked
    private static void HandleSByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsSByteCheck();
    }

    // HandleSByteCreateSaturating
    private static void HandleSByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(sbyte.MinValue);
        sb.Push(sbyte.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        sb.AddInstruction(OpCode.DUP);// 5 0 10 10
        sb.AddInstruction(OpCode.ROT);// 5 10 10 0
        sb.AddInstruction(OpCode.DUP);// 5 10 10 0 0
        sb.AddInstruction(OpCode.ROT);// 5 10 0 0 10
        sb.Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
        sb.AddInstruction(OpCode.THROW);
        exceptionTarget.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.ROT);// 10 0 5
        sb.AddInstruction(OpCode.DUP);// 10 0 5 5
        sb.AddInstruction(OpCode.ROT);// 10 5 5 0
        sb.AddInstruction(OpCode.DUP);// 10 5 5 0 0
        sb.AddInstruction(OpCode.ROT);// 10 5 0 0 5
        sb.Jump(OpCode.JMPGT, minTarget);// 10 5 0
        sb.AddInstruction(OpCode.DROP);// 10 5
        sb.AddInstruction(OpCode.DUP);// 10 5 5
        sb.AddInstruction(OpCode.ROT);// 5 5 10
        sb.AddInstruction(OpCode.DUP);// 5 5 10 10
        sb.AddInstruction(OpCode.ROT);// 5 10 10 5
        sb.Jump(OpCode.JMPLT, maxTarget);// 5 10
        sb.AddInstruction(OpCode.DROP);
        sb.Jump(OpCode.JMP, endTarget);
        minTarget.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.REVERSE3);
        sb.AddInstruction(OpCode.DROP);
        sb.AddInstruction(OpCode.DROP);
        sb.Jump(OpCode.JMP, endTarget);
        maxTarget.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.DROP);
        sb.Jump(OpCode.JMP, endTarget);
        endTarget.Instruction = sb.AddInstruction(OpCode.NOP);
    }

    // HandleSByteRotateLeft
    private static void HandleSByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static sbyte RotateLeft(sbyte value, int rotateAmount) => (sbyte)((value << (rotateAmount & 7)) | ((byte)value >> ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        sb.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        sb.And();    // rotateAmount & 7
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.And();
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 7)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.And();    // Ensure SHL result is 8-bit
        sb.LdArg0(); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.And();
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 8
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 8 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 7
        sb.And();    // (8 - rotateAmount) & 7
        sb.ShR();    // (byte)value >> ((8 - rotateAmount) & 7)
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    // HandleSByteRotateRight
    private static void HandleSByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static sbyte RotateRight(sbyte value, int rotateAmount) => (sbyte)(((value & 0xFF) >> (rotateAmount & 7)) | ((value & 0xFF) << ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        sb.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        sb.AddInstruction(OpCode.AND);    // rotateAmount & 7
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.MOD);
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SUB);
        sb.AddInstruction(OpCode.SWAP);
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.AddInstruction(OpCode.AND);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 7)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.AddInstruction(OpCode.AND);    // Ensure SHL result is 8-bit
        sb.AddInstruction(OpCode.LDARG0); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        sb.AddInstruction(OpCode.AND);
        sb.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.MOD);
        sb.Push(bitWidth);
        sb.AddInstruction(OpCode.SWAP);
        sb.AddInstruction(OpCode.SUB);
        sb.Push(bitWidth);  // Push 8
        sb.AddInstruction(OpCode.SWAP);   // Swap top two elements
        sb.AddInstruction(OpCode.SUB);    // 8 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 7
        sb.AddInstruction(OpCode.AND);    // (8 - rotateAmount) & 7
        sb.AddInstruction(OpCode.SHR);    // (byte)value >> ((8 - rotateAmount) & 7)
        sb.AddInstruction(OpCode.OR);
        sb.AddInstruction(OpCode.DUP);    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPLT, endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        sb.AddInstruction(OpCode.SUB);
        endTarget.Instruction = sb.AddInstruction(OpCode.NOP);
    }
}
