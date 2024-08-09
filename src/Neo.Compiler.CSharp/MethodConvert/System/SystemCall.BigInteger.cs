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
    private bool HandleBigIntegerOne(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        Push(1);
        return true;
    }

    private bool HandleBigIntegerMinusOne(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        Push(-1);
        return true;
    }

    private bool HandleBigIntegerZero(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        Push(0);
        return true;
    }

    private bool HandleBigIntegerIsZero(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        Push(0);
        AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private bool HandleBigIntegerIsOne(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        Push(1);
        AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private bool HandleBigIntegerIsEven(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        Push(1);
        AddInstruction(OpCode.AND);
        Push(0);
        AddInstruction(OpCode.NUMEQUAL);
        return true;
    }

    private bool HandleBigIntegerSign(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        AddInstruction(OpCode.SIGN);
        return true;
    }


    private bool HandleBigIntegerPow(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.POW);
        return true;
    }

    private bool HandleBigIntegerModPow(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.MODPOW);
        return true;
    }

    private bool HandleBigIntegerAdd(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.ADD);
        return true;
    }

    private bool HandleBigIntegerSubtract(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.SUB);
        return true;
    }

    private bool HandleBigIntegerNegate(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.NEGATE);
        return true;
    }

    private bool HandleBigIntegerMultiply(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.MUL);
        return true;
    }

    private bool HandleBigIntegerDivide(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.DIV);
        return true;
    }

    private bool HandleBigIntegerRemainder(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        AddInstruction(OpCode.MOD);
        return true;
    }

    private bool HandleBigIntegerCompare(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // if left < right return -1;
        // if left = right return 0;
        // if left > right return 1;
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.SIGN);
        return true;
    }

    private bool HandleBigIntegerGreatestCommonDivisor(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget gcdTarget = new()
        {
            Instruction = AddInstruction(OpCode.DUP)
        };
        AddInstruction(OpCode.REVERSE3);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.MOD);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.PUSH0);
        AddInstruction(OpCode.NUMEQUAL);
        Jump(OpCode.JMPIFNOT, gcdTarget);
        AddInstruction(OpCode.DROP);
        AddInstruction(OpCode.ABS);
        return true;
    }

    private bool HandleBigIntegerToByteArray(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            ConvertExpression(model, instanceExpression);
        ChangeType(VM.Types.StackItemType.Buffer);
        return true;
    }

    private bool HandleBigIntegerParse(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        return true;
    }

    private bool HandleBigIntegerExplicitConversion(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(sbyte.MinValue);
        Push(sbyte.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to sbyte
    private bool HandleBigIntegerToSByte(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(sbyte.MinValue);
        Push(sbyte.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to byte
    private bool HandleBigIntegerToByte(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(byte.MinValue);
        Push(byte.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to short
    private bool HandleBigIntegerToShort(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(short.MinValue);
        Push(short.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to ushort
    private bool HandleBigIntegerToUShort(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(ushort.MinValue);
        Push(ushort.MaxValue + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to int
    private bool HandleBigIntegerToInt(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(int.MinValue);
        Push(new BigInteger(int.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to uint
    private bool HandleBigIntegerToUInt(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(uint.MinValue);
        Push(new BigInteger(uint.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to long
    private bool HandleBigIntegerToLong(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(long.MinValue);
        Push(new BigInteger(long.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for explicit conversion of BigInteger to ulong
    private bool HandleBigIntegerToULong(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(ulong.MinValue);
        Push(new BigInteger(ulong.MaxValue) + 1);
        AddInstruction(OpCode.WITHIN);
        Jump(OpCode.JMPIF, endTarget);
        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
        return true;
    }

    // Handler for implicit conversion of various types to BigInteger
    private bool HandleToBigInteger(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        return true;
    }

    private bool HandleBigIntegerMax(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.MAX);
        return true;
    }

    private bool HandleBigIntegerMin(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        AddInstruction(OpCode.MIN);
        return true;
    }

    private bool HandleBigIntegerDivRem(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.TUCK);
        AddInstruction(OpCode.DIV);

        // Calculate remainder
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.MUL);
        AddInstruction(OpCode.ROT);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.SUB);
        AddInstruction(OpCode.PUSH2);
        AddInstruction(OpCode.PACK);
        return true;
    }

}
