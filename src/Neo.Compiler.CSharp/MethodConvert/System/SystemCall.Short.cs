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
    private static void HandleShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(short.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleShortLeadingZeroCount

    private static void HandleShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        if (symbol.ToString() == "short.LeadingZeroCount(short)")
        {
            methodConvert.AddInstruction(OpCode.DUP); // a a
            methodConvert.AddInstruction(OpCode.PUSH0);// a a 0
            JumpTarget notNegative = new();
            methodConvert.Jump(OpCode.JMPGE, notNegative); //a
            methodConvert.AddInstruction(OpCode.DROP);
            methodConvert.AddInstruction(OpCode.PUSH0);
            methodConvert.Jump(OpCode.JMP, endTarget);
            notNegative.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        }
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
        methodConvert.Push(16);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleShortCopySign
    private static void HandleShortCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        JumpTarget noOverflowTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(short.MaxValue);
        methodConvert.Jump(OpCode.JMPLE, noOverflowTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        noOverflowTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleShortCreateChecked
    private static void HandleShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(new BigInteger(short.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleShortCreateSaturating
    private static void HandleShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(short.MaxValue);
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

    // implement HandleShortRotateLeft
    private static void HandleShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateLeft(short value, int rotateAmount) => (short)((value << (rotateAmount & 15)) | ((ushort)value >> ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        methodConvert.Push(bitWidth - 1);  // Push 15 (16-bit - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & 15
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 15)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure SHL result is 16-bit
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);  // Push 16
        methodConvert.AddInstruction(OpCode.SWAP);   // Swap top two elements
        methodConvert.AddInstruction(OpCode.SUB);    // 16 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 15
        methodConvert.AddInstruction(OpCode.AND);    // (16 - rotateAmount) & 15
        methodConvert.AddInstruction(OpCode.SHR);    // (ushort)value >> ((16 - rotateAmount) & 15)
        methodConvert.AddInstruction(OpCode.OR);
        methodConvert.AddInstruction(OpCode.DUP);    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleShortRotateRight
    private static void HandleShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static short RotateRight(short value, int rotateAmount) => (short)((value >> (rotateAmount & 15)) | ((ushort)value << ((16 - rotateAmount) & 15)));
        var bitWidth = sizeof(short) * 8;
        methodConvert.Push(bitWidth - 1);  // Push 15 (16-bit - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & 15
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 15)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure SHL result is 16-bit
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFF (16-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.Push(bitWidth);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.Push(bitWidth);  // Push 16
        methodConvert.AddInstruction(OpCode.SWAP);   // Swap top two elements
        methodConvert.AddInstruction(OpCode.SUB);    // 16 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 15
        methodConvert.AddInstruction(OpCode.AND);    // (16 - rotateAmount) & 15
        methodConvert.AddInstruction(OpCode.SHR);    // (ushort)value >> ((16 - rotateAmount) & 15)
        methodConvert.AddInstruction(OpCode.OR);
        methodConvert.AddInstruction(OpCode.DUP);    // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);
        methodConvert.Push(BigInteger.One << bitWidth); // BigInteger.One << 16 (0x10000)
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleShortPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of int
        var bitWidth = sizeof(short) * 8;

        // Mask to ensure the value is treated as a 16-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // 0xFFFF
        methodConvert.And(); // value = value & 0xFFFF
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
