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

partial class MethodConvert
{
    private static bool HandleBigIntegerOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(1);
        return true;
    }

    private static bool HandleBigIntegerMinusOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(-1);
        return true;
    }

    private static bool HandleBigIntegerZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(0);
        return true;
    }

    private static bool HandleBigIntegerIsZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private static bool HandleBigIntegerIsOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private static bool HandleBigIntegerIsEven(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert. PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert. Push(1);
        methodConvert. AddInstruction(OpCode.AND);
        methodConvert.  Push(0);
        methodConvert.  AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private static bool HandleBigIntegerSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.SIGN);
        return true;
    }


    private static bool HandleBigIntegerPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.POW);
        return true;
    }

    private static bool HandleBigIntegerModPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MODPOW);
        return true;
    }

    private static bool HandleBigIntegerAdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.ADD);
        return true;
    }

    private static bool HandleBigIntegerSubtract(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.SUB);
        return true;
    }

    private static bool HandleBigIntegerNegate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.NEGATE);
        return true;
    }

    private static bool HandleBigIntegerMultiply(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MUL);
        return true;
    }

    private static bool HandleBigIntegerDivide(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.DIV);
        return true;
    }

    private static bool HandleBigIntegerRemainder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.AddInstruction(OpCode.MOD);
        return true;
    }

    private static bool HandleBigIntegerCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // if left < right return -1;
        // if left = right return 0;
        // if left > right return 1;
        methodConvert.AddInstruction(OpCode.SUB);
        methodConvert.AddInstruction(OpCode.SIGN);
        return true;
    }

    private static bool HandleBigIntegerGreatestCommonDivisor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    private static bool HandleBigIntegerToByteArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.ChangeType(VM.Types.StackItemType.Buffer);
        return true;
    }

    private static bool HandleBigIntegerParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        return true;
    }

    private static bool HandleBigIntegerExplicitConversion(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to sbyte
    private static bool HandleBigIntegerToSByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to byte
    private static bool HandleBigIntegerToByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to short
    private static bool HandleBigIntegerToShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to ushort
    private static bool HandleBigIntegerToUShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to int
    private static bool HandleBigIntegerToInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to uint
    private static bool HandleBigIntegerToUInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to long
    private static bool HandleBigIntegerToLong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for explicit conversion of BigInteger to ulong
    private static bool HandleBigIntegerToULong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // Handler for implicit conversion of various types to BigInteger
    private static bool HandleToBigInteger(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        return true;
    }

    private static bool HandleBigIntegerMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MAX);
        return true;
    }

    private static bool HandleBigIntegerMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.MIN);
        return true;
    }

    private static bool HandleBigIntegerDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
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
        return true;
    }

    // HandleBigIntegerIsOdd
    private static bool HandleBigIntegerIsOdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(1);
        methodConvert.AddInstruction(OpCode.AND);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.NUMNOTEQUAL);
        return true;
    }

    // HandleBigIntegerIsNegative
    private static bool HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.LT);
        return true;
    }

    // HandleBigIntegerIsPositive
    private static bool HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.GE);
        return true;
    }

    //HandleBigIntegerIsPow2
    private static bool HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
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
        return true;
    }

    // HandleBigIntegerLog2
    private static bool HandleBigIntegerLog2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
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

        return true;
    }

    // HandleBigIntegerCopySign
    private static bool HandleBigIntegerCopySign(MethodConvert methodConvert, SemanticModel model,
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
        return true;
    }
}
