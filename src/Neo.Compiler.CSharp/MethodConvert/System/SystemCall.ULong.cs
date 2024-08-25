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
    private static void HandleULongParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(new BigInteger(ulong.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleULongLeadingZeroCount
    private static void HandleULongLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
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
        methodConvert.Push(64);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleULongCreateChecked
    private static void HandleULongCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(new BigInteger(ulong.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleULongCreateSaturating
    private static void HandleULongCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(ulong.MaxValue);
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

    /// <summary>
    /// Handles the ULong.RotateLeft operation by converting it to the appropriate VM instructions.
    /// </summary>
    /// <param name="methodConvert">The MethodConvert instance to add instructions to.</param>
    /// <param name="model">The semantic model of the code being converted.</param>
    /// <param name="symbol">The method symbol representing the RotateLeft operation.</param>
    /// <param name="instanceExpression">The instance expression, if any (null for static methods).</param>
    /// <param name="arguments">The list of arguments passed to the method.</param>
    private static void HandleULongRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ulong RotateLeft(ulong value, int rotateAmount) => (ulong)(value << rotateAmount) | (value >> (64 - rotateAmount));
        var bitWidth = sizeof(ulong) * 8;
        methodConvert.Push(bitWidth - 1);  // Push 63 (64-bit - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & 63
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SHL);    // value << (rotateAmount & 63)
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure SHL result is 64-bit
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.Push(bitWidth);  // Push 64
        methodConvert.AddInstruction(OpCode.SWAP);   // Swap top two elements
        methodConvert.AddInstruction(OpCode.SUB);    // 64 - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push 63
        methodConvert.AddInstruction(OpCode.AND);    // (64 - rotateAmount) & 63
        methodConvert.AddInstruction(OpCode.SHR);    // (ulong)value >> ((64 - rotateAmount) & 63)
        methodConvert.AddInstruction(OpCode.OR);
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.AddInstruction(OpCode.AND);    // Ensure final result is 64-bit
    }

    // HandleULongRotateRight
    /// <summary>
    /// Handles the ULong.RotateRight operation by converting it to the appropriate VM instructions.
    /// </summary>
    /// <param name="methodConvert">The MethodConvert instance to add instructions to.</param>
    /// <param name="model">The semantic model of the code being converted.</param>
    /// <param name="symbol">The method symbol representing the RotateRight operation.</param>
    /// <param name="instanceExpression">The instance expression, if any (null for static methods).</param>
    /// <param name="arguments">The list of arguments passed to the method.</param>
    /// <remark>This method implements the rotation using bitwise operations.</remark>
    private static void HandleULongRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ulong RotateRight(ulong value, int rotateAmount) => (ulong)(value >> rotateAmount) | (value << (64 - rotateAmount));
        var bitWidth = sizeof(ulong) * 8;
        methodConvert.Push(bitWidth - 1);  // Push (bitWidth - 1)
        methodConvert.AddInstruction(OpCode.AND);    // rotateAmount & (bitWidth - 1)
        methodConvert.AddInstruction(OpCode.SHR);    // value >> (rotateAmount & (bitWidth - 1))
        methodConvert.AddInstruction(OpCode.LDARG0); // Load value again
        methodConvert.Push(bitWidth);  // Push bitWidth
        methodConvert.AddInstruction(OpCode.LDARG1); // Load rotateAmount
        methodConvert.AddInstruction(OpCode.SUB);    // bitWidth - rotateAmount
        methodConvert.Push(bitWidth - 1);  // Push (bitWidth - 1)
        methodConvert.AddInstruction(OpCode.AND);    // (bitWidth - rotateAmount) & (bitWidth - 1)
        methodConvert.AddInstruction(OpCode.SHL);    // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        methodConvert.AddInstruction(OpCode.OR);     // Combine the results with OR
        methodConvert.Push((BigInteger.One << bitWidth) - 1);  // Push (2^bitWidth - 1) as bitmask
        methodConvert.AddInstruction(OpCode.AND);    // Ensure final result is bitWidth-bit
    }

    private static void HandleULongPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of int
        var bitWidth = sizeof(ulong) * 8;

        // Mask to ensure the value is treated as a 64-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1); // 0xFFFFFFFFFFFFFFFF
        methodConvert.And(); // value = value & 0xFFFFFFFFFFFFFFFF
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
