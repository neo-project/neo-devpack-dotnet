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
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    private static void HandleBigIntegerOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        sb.Push1();
    }

    private static void HandleBigIntegerMinusOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        sb.PushM1();
    }

    private static void HandleBigIntegerZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        sb.Push0();
    }

    private static void HandleBigIntegerIsZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Push0();
        sb.NumEqual();
    }

    private static void HandleBigIntegerIsOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Push1();
        sb.NumEqual();
    }

    private static void HandleBigIntegerIsEven(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Push1();
        sb.And();
        sb.Push0();
        sb.NumEqual();
    }

    private static void HandleBigIntegerSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Sign();
    }


    private static void HandleBigIntegerPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Pow();
    }

    private static void HandleBigIntegerModPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.ModPow();
    }

    private static void HandleBigIntegerAdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Add();
    }

    private static void HandleBigIntegerSubtract(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Sub();
    }

    private static void HandleBigIntegerNegate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Negate();
    }

    private static void HandleBigIntegerMultiply(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Mul();
    }

    private static void HandleBigIntegerDivide(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Div();
    }

    private static void HandleBigIntegerRemainder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Mod();
    }

    private static void HandleBigIntegerCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // if left < right return -1;
        // if left = right return 0;
        // if left > right return 1;
        sb.Sub();
        sb.Sign();
    }

    private static void HandleBigIntegerGreatestCommonDivisor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        JumpTarget gcdTarget = new()
        {
            Instruction = sb.Dup()
        };
        sb.Reverse3();
        sb.Swap();
        sb.Mod();
        sb.Dup();
        sb.Push0();
        sb.NumEqual();
        sb.JmpIfNot(gcdTarget);
        sb.Drop();
        sb.Abs();
    }

    private static void HandleBigIntegerToByteArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.ChangeType(VM.Types.StackItemType.Buffer);
    }

    private static void HandleBigIntegerParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Atoi(methodConvert);
    }

    private static void HandleBigIntegerExplicitConversion(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsSByteCheck();
    }

    // Handler for explicit conversion of BigInteger to sbyte
    private static void HandleBigIntegerToSByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsSByteCheck();
    }

    // Handler for explicit conversion of BigInteger to byte
    private static void HandleBigIntegerToByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsByteCheck();
    }

    // Handler for explicit conversion of BigInteger to short
    private static void HandleBigIntegerToShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsShortCheck();
    }

    // Handler for explicit conversion of BigInteger to ushort
    private static void HandleBigIntegerToUShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUShortCheck();
    }

    // Handler for explicit conversion of BigInteger to int
    private static void HandleBigIntegerToInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsIntCheck();
    }

    // Handler for explicit conversion of BigInteger to uint
    private static void HandleBigIntegerToUInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUIntCheck();
    }

    // Handler for explicit conversion of BigInteger to long
    private static void HandleBigIntegerToLong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsLongCheck();
    }

    // Handler for explicit conversion of BigInteger to ulong
    private static void HandleBigIntegerToULong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsULongCheck();
    }

    // Handler for implicit conversion of various types to BigInteger
    private static void HandleToBigInteger(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    private static void HandleBigIntegerMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Max();
    }

    private static void HandleBigIntegerMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Min();
    }

    // HandleBigIntegerIsOdd
    private static void HandleBigIntegerIsOdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Push1();
        sb.And();
        sb.Push0();
        sb.NumNotEqual();
    }

    // HandleBigIntegerIsNegative
    private static void HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Sign();
        sb.Push0();
        sb.Lt();
    }

    // HandleBigIntegerIsPositive
    private static void HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Sign();
        sb.Push0();
        sb.Ge();
    }

    //HandleBigIntegerIsPow2
    private static void HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endFalse = new();
        JumpTarget endTrue = new();
        JumpTarget endTarget = new();
        JumpTarget nonZero = new();
        sb.Dup();
        sb.Push0();
        sb.Jump(OpCode.JMPNE, nonZero);
        sb.Drop();
        sb.Jump(OpCode.JMP, endFalse);
        nonZero.Instruction = sb.Nop();
        sb.Dup();
        sb.Dec();
        sb.And();
        sb.Push0();
        sb.NumEqual();
        sb.Jump(OpCode.JMPIF, endTrue);
        endFalse.Instruction = sb.Nop();
        sb.Push(false);
        sb.Jump(OpCode.JMP, endTarget);
        endTrue.Instruction = sb.Nop();
        sb.Push(true);
        sb.SetTarget(endTarget);
    }

    // HandleBigIntegerLog2
    private static void HandleBigIntegerLog2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endLoop = new();
        JumpTarget negativeInput = new();
        JumpTarget zeroTarget = new();
        sb.Dup();// 5 5
        sb.Push0(); // 5 5 0
        sb.Jump(OpCode.JMPEQ, zeroTarget); // 5
        sb.Dup();// 5 5
        sb.Push0(); // 5 5 0
        sb.JmpLt(negativeInput); // 5
        sb.PushM1();// 5 -1
        JumpTarget loopStart = new();
        sb.Swap().SetTarget(loopStart); // -1 5
        sb.Dup(); // -1 5 5
        sb.Push0(); // -1 5 5 0
        sb.Jump(OpCode.JMPEQ, endLoop);  // -1 5
        sb.Push1(); // -1 5 1
        sb.ShR(); // -1 5>>1
        sb.Swap(); // 5>>1 -1
        sb.Inc(); // 5>>1 -1+1
        sb.Jmp(loopStart);
        endLoop.Instruction = sb.Nop();
        sb.Drop(); // -1
        JumpTarget endMethod = new();
        sb.Jmp(endMethod);
        zeroTarget.Instruction = sb.Nop();
        sb.Drop();
        sb.Push0();
        sb.Jmp(endMethod);
        negativeInput.Instruction = sb.Drop();
        sb.Throw();
        endMethod.Instruction = sb.Nop();

    }

    // HandleBigIntegerCopySign
    private static void HandleBigIntegerCopySign(MethodConvert methodConvert, SemanticModel model,
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
        sb.Sign();         // a 1
        sb.Dup(); // a 1 1
        sb.Push0(); // a 1 1 0
        sb.JmpLt(nonZeroTarget); // a 1
        sb.Drop();
        sb.Push1(); // a 1
        nonZeroTarget.Instruction = sb.Nop(); // a 1
        sb.Swap();         // 1 a
        sb.Dup();// 1 a a
        sb.Sign();// 1 a 0
        sb.Dup();// 1 a 0 0
        sb.Push0(); // 1 a 0 0 0
        sb.JmpLt(nonZeroTarget2); // 1 a 0
        sb.Drop();
        sb.Push1();
        sb.SetTarget(nonZeroTarget2); // 1 a 1
        sb.Rot();// a 1 1
        sb.Equal();// a 1 1
        JumpTarget endTarget = new();
        sb.JmpIf(endTarget); // a
        sb.Negate();
        sb.SetTarget(endTarget);
    }

    // HandleMathBigIntegerDivRem
    private static void HandleMathBigIntegerDivRem(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Perform division
        sb.Dup();
        sb.Rot();
        sb.Tuck();
        sb.Div();

        // Calculate remainder
        sb.Dup();
        sb.Rot();
        sb.Mul();
        sb.Rot();
        sb.Swap();
        sb.Sub();
        sb.Push2();
        sb.Pack();
    }

    //implement HandleBigIntegerLeadingZeroCount
    private static void HandleBigIntegerLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        sb.Dup(); // a a
        sb.Push0();// a a 0
        JumpTarget notNegative = new();
        sb.JmpGe(notNegative); //a
        sb.Drop();
        sb.Push0();
        sb.Jmp(endTarget);
        notNegative.Instruction = sb.Nop();
        sb.Push0(); // count 5 0
        sb.Swap().SetTarget(loopStart); //0 5
        sb.Dup();//  0 5 5
        sb.Push0();// 0 5 5 0
        sb.JmpEq(endLoop); //0 5
        sb.Push1();//0 5 1
        sb.ShR(); //0  5>>1
        sb.Swap();//5>>1 0
        sb.Inc();// 5>>1 1
        sb.Jmp(loopStart);
        sb.SetTarget(endLoop);
        sb.Drop();
        sb.Push(256);
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    private static void HandleBigIntegerCreatedChecked(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    private static void HandleBigIntegerIsPowerOfTwo(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endTarget = new();
        sb.Dup(); // a a
        sb.Push0(); // a a 0
        sb.JmpLe(endTarget); // a
        sb.Dup(); // a a
        sb.Dec(); // a a-1
        sb.And(); // a&(a-1)
        sb.Push0(); // a&(a-1) 0
        sb.JmpEq(endTarget); // a&(a-1)
        sb.Push0(); // 0
        sb.Jmp(endTarget); // 0
        sb.SetTarget(endTarget);
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
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.NumEqual();
    }

    private static void HandleBigIntegerPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check if the value is within int range
        sb.AddInstruction(OpCode.DUP);
        sb.Within(int.MinValue, int.MaxValue);
        var endIntCheck = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endIntCheck);

        // If within int range, mask with 0xFFFFFFFF
        sb.Push(0xFFFFFFFF);
        sb.AddInstruction(OpCode.AND);
        var endMask = new JumpTarget();
        sb.Jump(OpCode.JMP, endMask);

        // If larger than int, throw exception, cause too many check will make the script too long.
        endIntCheck.Instruction = sb.AddInstruction(OpCode.NOP);
        sb.Push("Value out of range, must be between int.MinValue and int.MaxValue.");
        sb.Throw();
        endMask.Instruction = sb.AddInstruction(OpCode.NOP);

        // Initialize count to 0
        sb.Push(0); // value count
        sb.Swap(); // count value
        // Loop to count the number of 1 bit
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        loopStart.Instruction = sb.Dup(); // count value value
        sb.Push0(); // count value value 0
        sb.Jump(OpCode.JMPEQ, endLoop); // count value
        sb.Dup(); // count value value
        sb.Push1(); // count value value 1
        sb.And(); // count value (value & 1)
        sb.Rot(); // value (value & 1) count
        sb.Add(); // value count += (value & 1)
        sb.Swap(); // count value
        sb.Push1(); // count value 1
        sb.ShR(); // count value >>= 1
        sb.Jump(OpCode.JMP, loopStart);

        endLoop.Instruction = sb.Drop(); // Drop the remaining value
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
