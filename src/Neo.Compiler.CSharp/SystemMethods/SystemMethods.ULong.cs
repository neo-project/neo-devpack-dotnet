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
    private static void HandleULongParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsULongCheck();
    }

    // HandleULongLeadingZeroCount
    private static void HandleULongLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
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
        sb.JmpEq(endLoop); //0 5
        sb.Push1();//0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        endLoop.Instruction = sb.Drop();
        sb.Push(64);
        sb.Swap();
        sb.Sub();
        endTarget.Instruction = sb.Nop();
    }

    // HandleULongCreateChecked
    private static void HandleULongCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsULongCheck();
    }

    // HandleULongCreateSaturating
    private static void HandleULongCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(ulong.MinValue);
        sb.Push(ulong.MaxValue);
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
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ulong RotateLeft(ulong value, int rotateAmount) => (ulong)(value << rotateAmount) | (value >> (64 - rotateAmount));
        var bitWidth = sizeof(ulong) * 8;
        sb.Push(bitWidth - 1);  // Push 63 (64-bit - 1)
        sb.And();    // rotateAmount & 63
        sb.Swap();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.And();
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & 63)
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.And();    // Ensure SHL result is 64-bit
        sb.LdArg0(); // Load value
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.And();
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push 64
        sb.Swap();   // Swap top two elements
        sb.Sub();    // 64 - rotateAmount
        sb.Push(bitWidth - 1);  // Push 63
        sb.And();    // (64 - rotateAmount) & 63
        sb.ShR();    // (ulong)value >> ((64 - rotateAmount) & 63)
        sb.Or();
        sb.Push((BigInteger.One << bitWidth) - 1); // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        sb.And();    // Ensure final result is 64-bit
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
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // public static ulong RotateRight(ulong value, int rotateAmount) => (ulong)(value >> rotateAmount) | (value << (64 - rotateAmount));
        var bitWidth = sizeof(ulong) * 8;
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
