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
        methodConvert.Push(2);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.AddInstruction(OpCode.NOT);  // BigInteger GetBoolean() => !value.IsZero;
        //methodConvert.Push(1);
        //methodConvert.AddInstruction(OpCode.AND);
        //methodConvert.Push(0);
        //methodConvert.AddInstruction(OpCode.NUMEQUAL);
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
        {
            if (arguments.Count == 1 && arguments[0] is ArgumentSyntax { NameColon: null } arg)
            {
                // Optimize call when is a constant string

                Optional<object?> constant = model.GetConstantValue(arg.Expression);

                if (constant.HasValue && constant.Value is string strValue && BigInteger.TryParse(strValue, out var bi))
                {
                    // Insert a sequence point for debugging purposes
                    using var sequence = methodConvert.InsertSequencePoint(arg.Expression);
                    methodConvert.Push(bi);
                    return;
                }
            }

            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        }

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
        methodConvert.Push(2);
        methodConvert.AddInstruction(OpCode.MOD);
        methodConvert.AddInstruction(OpCode.NZ);
        //methodConvert.Push(1);
        //methodConvert.AddInstruction(OpCode.AND);
        //methodConvert.Push(0);
        //methodConvert.AddInstruction(OpCode.NUMNOTEQUAL);
    }

    // HandleBigIntegerIsNegative
    private static void HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        //methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.LT);
        // The following is cheaper than methodConvert.AddInstruction(OpCode.LT);
        //JumpTarget isNegative = new();
        //JumpTarget end = new();
        //methodConvert.Jump(OpCode.JMPLT, isNegative);
        //methodConvert.AddInstruction(OpCode.PUSHF);
        //methodConvert.Jump(OpCode.JMP, end);
        //isNegative.Instruction = methodConvert.AddInstruction(OpCode.PUSHT);
        //end.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    // HandleBigIntegerIsPositive
    private static void HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        //methodConvert.AddInstruction(OpCode.SIGN);
        methodConvert.Push(0);
        methodConvert.AddInstruction(OpCode.GE);
        // The following is cheaper than methodConvert.AddInstruction(OpCode.GE);
        //JumpTarget isPositive = new();
        //JumpTarget end = new();
        //methodConvert.Jump(OpCode.JMPGE, isPositive);
        //methodConvert.AddInstruction(OpCode.PUSHF);
        //methodConvert.Jump(OpCode.JMP, end);
        //isPositive.Instruction = methodConvert.AddInstruction(OpCode.PUSHT);
        //end.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        // GE instead of GT, because C# BigInteger works like that
        // https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L4098C13-L4098C37
    }

    //HandleBigIntegerIsPow2
    private static void HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // (n & (n-1) == 0) and (n != 0)
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
        methodConvert.Jump(OpCode.JMPEQ, endTrue);
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

        JumpTarget nonNegativeTarget = new();
        JumpTarget endMethod = new();
        methodConvert.AddInstruction(OpCode.DUP);// 5 5
        methodConvert.AddInstruction(OpCode.PUSH0); // 5 5 0
        methodConvert.Jump(OpCode.JMPGE, nonNegativeTarget); // 5
        methodConvert.AddInstruction(OpCode.THROW);
        nonNegativeTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DUP);// 5 5
        methodConvert.AddInstruction(OpCode.PUSH0); // 5 5 0
        methodConvert.Jump(OpCode.JMPEQ, endMethod); // 0  // return 0 when input is 0
        methodConvert.AddInstruction(OpCode.PUSH0);// 5 0
        //input = 5 > 0; result = 0; 
        //do
        //  result += 1
        //while (input >> result) > 0
        //result -= 1
        JumpTarget loopStart = new();
        loopStart.Instruction = methodConvert.AddInstruction(OpCode.NOP); // 5 0
        methodConvert.AddInstruction(OpCode.INC);// 5 1
        methodConvert.AddInstruction(OpCode.OVER);// 5 1 5
        methodConvert.AddInstruction(OpCode.OVER);// 5 1 5 1
        methodConvert.AddInstruction(OpCode.SHR); // 5 1 5>>1
        methodConvert.AddInstruction(OpCode.PUSH0); // 5 1 5>>1 0
        methodConvert.Jump(OpCode.JMPGT, loopStart); // 5 1
        methodConvert.AddInstruction(OpCode.NIP); // 1
        methodConvert.AddInstruction(OpCode.DEC); // 0
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
        JumpTarget negativeTarget = new();
        JumpTarget endTarget = new();
        // a b
        // if a==0 return 0
        // if b==0 return abs(a)
        // return value has abs(value)==abs(a), sign(value)==sign(b)
        methodConvert.AddInstruction(OpCode.PUSH0); // a b 0
        methodConvert.Jump(OpCode.JMPLT, negativeTarget); // a
        methodConvert.AddInstruction(OpCode.ABS);   // abs(a)
        methodConvert.Jump(OpCode.JMP, endTarget);  // abs(a)
        negativeTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.ABS);   // abs(a)
        methodConvert.AddInstruction(OpCode.NEGATE);// -abs(a)
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
        // r, l -> l/r, l%r
        // Perform division
        methodConvert.AddInstruction(OpCode.DUP); // r, l, l
        methodConvert.Push(2);
        methodConvert.AddInstruction(OpCode.PICK);// r, l, l, r
        methodConvert.AddInstruction(OpCode.DIV);  // r, l, l/r
        // For types that is restricted by range, there should be l/r <= MaxValue
        // However it's only possible to get l/r == MaxValue + 1 when l/r > MaxValue
        // and it's impossible to get l/r < MinValue
        // Therefore we ignore this case; l/r <= MaxValue is not checked

        // Calculate remainder
        methodConvert.AddInstruction(OpCode.REVERSE3);  // l/r, l, r
        methodConvert.AddInstruction(OpCode.MOD);  // l/r, l%r
        methodConvert.AddInstruction(OpCode.PUSH2);
        methodConvert.AddInstruction(OpCode.PACK);
        // It's impossible to get l%r out of range
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

    private static void HandleBigIntegerPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check if the value is within int range
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Within(int.MinValue, int.MaxValue);
        var endIntCheck = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endIntCheck);

        // If within int range, mask with 0xFFFFFFFF
        methodConvert.Push(0xFFFFFFFF);
        methodConvert.AddInstruction(OpCode.AND);
        var endMask = new JumpTarget();
        methodConvert.Jump(OpCode.JMP, endMask);

        // If larger than int, throw exception, cause too many check will make the script too long.
        endIntCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push("Value out of range, must be between int.MinValue and int.MaxValue.");
        methodConvert.Throw();
        endMask.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        // Initialize count to 0
        methodConvert.Push(0); // value count
        methodConvert.Swap(); // count value
        // Loop to count the number of 1 bit
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
