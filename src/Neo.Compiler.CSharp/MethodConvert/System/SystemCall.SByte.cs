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

internal partial class MethodConvert
{
    private static void HandleSByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleSByteLeadingZeroCount
    private static void HandleSByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        methodConvert.AddInstruction(OpCode.DUP); // a a
        methodConvert.AddInstruction(OpCode.PUSH0);// a a 0
        methodConvert.Jump(OpCode.JMPGE, notNegative); //a
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.PUSH0);
        methodConvert.Jump(OpCode.JMP, endTarget);
        notNegative.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push(0); // count 5 0
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.SWAP); //0 5
        methodConvert.AddInstruction(OpCode.DUP);//  0 5 5
        methodConvert.AddInstruction(OpCode.PUSH0);// 0 5 5 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop); //0 5
        methodConvert.AddInstruction(OpCode.PUSH1);//0 5 1
        methodConvert.AddInstruction(OpCode.SHR); //0  5>>1
        methodConvert.AddInstruction(OpCode.SWAP);//5>>1 0
        methodConvert.AddInstruction(OpCode.INC);// 5>>1 1
        methodConvert.Jump(OpCode.JMP, loopStart);
        endLoop.Instruction = methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(8);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleSByteCopySign
    private static void HandleSByteCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        JumpTarget noOverflowTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MaxValue);
        methodConvert.Jump(OpCode.JMPLE, noOverflowTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        noOverflowTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleSByteCreateChecked
    private static void HandleSByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(new BigInteger(sbyte.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleSByteCreateSaturating
    private static void HandleSByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue);
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

    // HandleSByteRotateLeft
    private static void HandleSByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static sbyte RotateLeft(sbyte value, int rotateAmount) => (sbyte)((value << (rotateAmount & 7)) | ((byte)value >> ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        methodConvert.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & 7
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 7)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure SHL result is 8-bit
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);  // Push 8
        methodConvert.AddInstruction(OpCode.SWAP);   // Swap top two elements
        methodConvert.AddInstruction(OpCode.SUB);    // 8 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 7
        methodConvert.AddInstruction(OpCode.AND);    // (8 - rotateAmount) & 7
        methodConvert.AddInstruction(OpCode.SHR);    // (byte)value >> ((8 - rotateAmount) & 7)
        methodConvert.AddInstruction(OpCode.OR);
        methodConvert.AddInstruction(OpCode.DUP);    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleSByteRotateRight
    private static void HandleSByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static sbyte RotateRight(sbyte value, int rotateAmount) => (sbyte)(((value & 0xFF) >> (rotateAmount & 7)) | ((value & 0xFF) << ((8 - rotateAmount) & 7)));
        var bitWidth = sizeof(sbyte) * 8;
        methodConvert.Push(bitWidth - 1);  // Push 7 (8-bit - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & 7
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 7)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure SHL result is 8-bit
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFF (8-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push(bitWidth);  // Push 8
        methodConvert.AddInstruction(OpCode.SWAP);   // Swap top two elements
        methodConvert.AddInstruction(OpCode.SUB);    // 8 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 7
        methodConvert.AddInstruction(OpCode.AND);    // (8 - rotateAmount) & 7
        methodConvert.AddInstruction(OpCode.SHR);    // (byte)value >> ((8 - rotateAmount) & 7)
        methodConvert.AddInstruction(OpCode.OR);
        methodConvert.AddInstruction(OpCode.DUP);    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 7 (0x80)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 8 (0x100)
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleSBytePopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of int
        var bitWidth = sizeof(sbyte) * 8;

        // Mask to ensure the value is treated as a 8-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // 0xFF
        methodConvert.And(); // value = value & 0xFF
                             // Initialize count to 0
        methodConvert.Push(0); // value count
        methodConvert.Swap(); // count value
        // Loop to count the number of 1 bits
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        loopStart.Instruction = methodConvert.Dup(); // count value value
        methodConvert.Push0(); // count value value 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop); // count value
        methodConvert.Dup(); // count value value
        methodConvert.Push1(); // count value value 1
        methodConvert.And(); // count value (value & 1)
        methodConvert.Rot(); // value (value & 1) count
        methodConvert.Add(); // value count += (value & 1)
        methodConvert.Swap(); // count value
        methodConvert.Push1(); // count value 1
        methodConvert.ShR(); // count value >>= 1
        methodConvert.Jump(OpCode.JMP, loopStart);

        endLoop.Instruction = methodConvert.Drop(); // Drop the remaining value
    }
}
