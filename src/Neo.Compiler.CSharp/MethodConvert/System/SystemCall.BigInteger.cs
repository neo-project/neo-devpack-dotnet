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
    private static void HandleBigIntegerOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(1);
    }

    private static void HandleBigIntegerMinusOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(-1);
    }

    private static void HandleBigIntegerZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(0);
    }

    private static void HandleBigIntegerIsZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
    }

    private static void HandleBigIntegerIsOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
    }

    private static void HandleBigIntegerIsEven(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
    }

    private static void HandleBigIntegerSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.SIGN);
    }


    private static void HandleBigIntegerPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.POW);
    }

    private static void HandleBigIntegerModPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MODPOW);
    }

    private static void HandleBigIntegerAdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.ADD);
    }

    private static void HandleBigIntegerSubtract(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.SUB);
    }

    private static void HandleBigIntegerNegate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.NEGATE);
    }

    private static void HandleBigIntegerMultiply(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MUL);
    }

    private static void HandleBigIntegerDivide(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.DIV);
    }

    private static void HandleBigIntegerRemainder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MOD);
    }

    private static void HandleBigIntegerCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // if left < right return -1;
        // if left = right return 0;
        // if left > right return 1;
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SIGN);
    }

    private static void HandleBigIntegerGreatestCommonDivisor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget gcdTarget = new()
        {
            Instruction = methodConvert.AddInstruction(OpCode.DUP)
        };
        methodConvert.AddInstruction(OpCode.REVERSE3);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.PUSH0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMPIFNOT, gcdTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.ABS);
    }

    private static void HandleBigIntegerToByteArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(VM.Types.StackItemType.Buffer);
    }

    private static void HandleBigIntegerParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
    }

    private static void HandleBigIntegerExplicitConversion(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to sbyte
    private static void HandleBigIntegerToSByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to byte
    private static void HandleBigIntegerToByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(byte.MinValue);
        methodConvert.Push(byte.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to short
    private static void HandleBigIntegerToShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(short.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to ushort
    private static void HandleBigIntegerToUShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ushort.MinValue);
        methodConvert.Push(ushort.MaxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to int
    private static void HandleBigIntegerToInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(int.MinValue);
        methodConvert.Push(new BigInteger(int.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to uint
    private static void HandleBigIntegerToUInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(uint.MinValue);
        methodConvert.Push(new BigInteger(uint.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to long
    private static void HandleBigIntegerToLong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(long.MinValue);
        methodConvert.Push(new BigInteger(long.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for explicit conversion of BigInteger to ulong
    private static void HandleBigIntegerToULong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(ulong.MinValue);
        methodConvert.Push(new BigInteger(ulong.MaxValue) + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // Handler for implicit conversion of various types to BigInteger
    private static void HandleToBigInteger(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    private static void HandleBigIntegerMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MAX);
    }

    private static void HandleBigIntegerMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MIN);
    }

    // HandleBigIntegerIsOdd
    private static void HandleBigIntegerIsOdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMNOTEQUAL);
    }

    // HandleBigIntegerIsNegative
    private static void HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.LT);
    }

    // HandleBigIntegerIsPositive
    private static void HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.GE);
    }

    //HandleBigIntegerIsPow2
    private static void HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endFalse = new();
        JumpTarget endTrue = new();
        JumpTarget endTarget = new();
        JumpTarget nonZero = new();
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMPNE, nonZero);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Jump(OpCode.JMP, endFalse);
        nonZero.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.DEC);
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMPIF, endTrue);
        endFalse.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push(false);
        methodConvert.Jump(OpCode.JMP, endTarget);
        endTrue.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push(true);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleBigIntegerLog2
    private static void HandleBigIntegerLog2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endLoop = new();
        JumpTarget negativeInput = new();
        JumpTarget zeroTarget = new();
        methodConvert.AddInstruction(OpCode.DUP);// 5 5
        methodConvert.AddInstruction(OpCode.PUSH0); // 5 5 0
        methodConvert.Jump(OpCode.JMPEQ, zeroTarget); // 5
        methodConvert.AddInstruction(OpCode.DUP);// 5 5
        methodConvert.AddInstruction(OpCode.PUSH0); // 5 5 0
        methodConvert.Jump(OpCode.JMPLT, negativeInput); // 5
        methodConvert.AddInstruction(OpCode.PUSHM1);// 5 -1
        JumpTarget loopStart = new();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.SWAP); // -1 5
        methodConvert.AddInstruction(OpCode.DUP); // -1 5 5
        methodConvert.AddInstruction(OpCode.PUSH0); // -1 5 5 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop);  // -1 5
        methodConvert.AddInstruction(OpCode.PUSH1); // -1 5 1
        methodConvert.AddInstruction(OpCode.SHR); // -1 5>>1
        methodConvert.AddInstruction(OpCode.SWAP); // 5>>1 -1
        methodConvert.AddInstruction(OpCode.INC); // 5>>1 -1+1
        methodConvert.Jump(OpCode.JMP, loopStart);
        endLoop.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // -1
        JumpTarget endMethod = new();
        methodConvert.Jump(OpCode.JMP, endMethod);
        zeroTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        methodConvert.Jump(OpCode.JMP, endMethod);
        negativeInput.Instruction = methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.THROW);
        endMethod.Instruction = methodConvert.AddInstruction(OpCode.NOP);

    }

    // HandleBigIntegerCopySign
    private static void HandleBigIntegerCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget nonZeroTarget = new();
        JumpTarget nonZeroTarget2 = new();
        // a b
        methodConvert.AddInstruction(OpCode.SIGN);         // a 1
        methodConvert.AddInstruction(OpCode.DUP); // a 1 1
        methodConvert.Push(0); // a 1 1 0
        methodConvert.Jump(OpCode.JMPLT, nonZeroTarget); // a 1
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(1); // a 1
        nonZeroTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // a 1
        methodConvert.AddInstruction(OpCode.SWAP);         // 1 a
        methodConvert.AddInstruction(OpCode.DUP);// 1 a a
        methodConvert.AddInstruction(OpCode.SIGN);// 1 a 0
        methodConvert.AddInstruction(OpCode.DUP);// 1 a 0 0
        methodConvert.Push(0); // 1 a 0 0 0
        methodConvert.Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(1);
        nonZeroTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP); // 1 a 1
        methodConvert.AddInstruction(OpCode.ROT);// a 1 1
        methodConvert.AddInstruction(OpCode.EQUAL);// a 1 1
        JumpTarget endTarget = new();
        methodConvert.Jump(OpCode.JMPIF, endTarget); // a
        methodConvert.AddInstruction(OpCode.NEGATE);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleMathBigIntegerDivRem
    private static void HandleMathBigIntegerDivRem(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.TUCK);
        methodConvert.AddInstruction(OpCode.DIV);

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.MUL);
        methodConvert.AddInstruction(OpCode.ROT);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
    }

    //implement HandleBigIntegerLeadingZeroCount
    private static void HandleBigIntegerLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP); // a a
        methodConvert.AddInstruction(OpCode.PUSH0);// a a 0
        JumpTarget notNegative = new();
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
        methodConvert.Push(256);
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.SUB);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleBigIntegerCreatedChecked(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    private static void HandleBigIntegerIsPowerOfTwo(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endTarget = new();
        methodConvert.AddInstruction(OpCode.DUP); // a a
        methodConvert.AddInstruction(OpCode.PUSH0); // a a 0
        methodConvert.Jump(OpCode.JMPLE, endTarget); // a
        methodConvert.AddInstruction(OpCode.DUP); // a a
        methodConvert.AddInstruction(OpCode.DEC); // a a-1
        methodConvert.AddInstruction(OpCode.AND); // a&(a-1)
        methodConvert.AddInstruction(OpCode.PUSH0); // a&(a-1) 0
        methodConvert.Jump(OpCode.JMPEQ, endTarget); // a&(a-1)
        methodConvert.AddInstruction(OpCode.PUSH0); // 0
        methodConvert.Jump(OpCode.JMP, endTarget); // 0
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // NOP
    }

    private static void HandleBigIntegerCreateSaturating(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
    }
    private static void HandleBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
    }
}
