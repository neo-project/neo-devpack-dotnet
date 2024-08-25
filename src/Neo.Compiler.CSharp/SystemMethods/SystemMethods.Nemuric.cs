// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal static partial class SystemMethods
{

    private static void HandleCopySign<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, Action<InstructionsBuilder> typeCheck)
        where T : struct
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
        sb.SetTarget(nonZeroTarget); // a 1
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
        typeCheck(sb);
    }

    private static void HandleCreateSaturating<T>(MethodConvert methodConvert, SemanticModel model,
    IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
    IReadOnlyList<SyntaxNode>? arguments, T minValue, T maxValue)
    where T : struct, IComparable<T>
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.Push(minValue);
        sb.Push(maxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        sb.Dup();
        sb.Rot();
        sb.Dup();
        sb.Rot();
        sb.JmpLt(exceptionTarget);
        sb.Throw();
        sb.SetTarget(exceptionTarget);
        sb.Rot();
        sb.Dup();
        sb.Rot();
        sb.Dup();
        sb.Rot();
        sb.JmpGt(minTarget);
        sb.Drop();
        sb.Dup();
        sb.Rot();
        sb.Dup();
        sb.Rot();
        sb.JmpLt(maxTarget);
        sb.Drop();
        sb.Jmp(endTarget);
        sb.SetTarget(minTarget);
        sb.Reverse3();
        sb.Drop(2);
        sb.Jmp(endTarget);
        sb.SetTarget(maxTarget);
        sb.Swap();
        sb.Drop();
        sb.SetTarget(endTarget);
    }

    private static void HandleUnsignedRotateLeft<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.And(bitWidth - 1);    // rotateAmount & (bitWidth - 1)
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & (bitWidth - 1))
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push bitWidth
        sb.Swap();   // Swap top two elements
        sb.Sub();    // bitWidth - rotateAmount
        sb.And(bitWidth - 1);  // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShR();    // value >> ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure final result is bitWidth-bit
    }

    private static void HandleSignedRotateLeft<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.And(bitWidth - 1);    // rotateAmount & (bitWidth - 1)
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & (bitWidth - 1))
        sb.And((BigInteger.One << bitWidth) - 1); // Ensure SHL result is bitWidth-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);  // Push bitWidth
        sb.Swap();   // Swap top two elements
        sb.Sub();    // bitWidth - rotateAmount
        sb.And(bitWidth - 1);    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShR();    // (ushort)value >> ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << (bitWidth - 1)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << bitWidth
        sb.Sub();
        sb.SetTarget(endTarget);
    }
    private static void HandleUnsignedRotateRight<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.And(bitWidth - 1);    // rotateAmount & (bitWidth - 1)
        sb.ShR();    // value >> (rotateAmount & (bitWidth - 1))
        sb.LdArg0(); // Load value again
        sb.Push(bitWidth);  // Push bitWidth
        sb.LdArg1(); // Load rotateAmount
        sb.Sub();    // bitWidth - rotateAmount
        sb.And(bitWidth - 1);    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShL();    // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();     // Combine the results with OR
        sb.And((BigInteger.One << bitWidth) - 1);  // Ensure final result is bitWidth-bit
    }

    private static void HandleSignedRotateRight<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        sb.And(bitWidth - 1);    // rotateAmount & (bitWidth - 1)
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Swap();
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.Swap();
        sb.ShL();    // value << (rotateAmount & (bitWidth - 1))
        sb.Push((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.And();    // Ensure SHL result is bitWidth-bit
        sb.LdArg0(); // Load value
        sb.And((BigInteger.One << bitWidth) - 1); // Push bitWidth-bit mask
        sb.LdArg1(); // Load rotateAmount
        sb.Push(bitWidth);
        sb.Mod();
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.Push(bitWidth);  // Push bitWidth
        sb.Swap();   // Swap top two elements
        sb.Sub();    // bitWidth - rotateAmount
        sb.And(bitWidth - 1);    // (bitWidth - rotateAmount) & (bitWidth - 1)
        sb.ShR();    // value >> ((bitWidth - rotateAmount) & (bitWidth - 1))
        sb.Or();
        sb.Dup();    // Duplicate the result
        sb.Push(BigInteger.One << (bitWidth - 1)); // Push BigInteger.One << (bitWidth - 1)
        var endTarget = new JumpTarget();
        sb.JmpLt(endTarget);
        sb.Push(BigInteger.One << bitWidth); // BigInteger.One << bitWidth
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    private static void HandleLeadingZeroCount<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth, bool isSigned)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();

        if (isSigned)
        {
            JumpTarget notNegative = new();
            sb.Dup(); // a a
            sb.Push0(); // a a 0
            sb.JmpGe(notNegative); // a
            sb.Drop();
            sb.Push0();
            sb.Jmp(endTarget);
            notNegative.Instruction = sb.Nop();
        }

        sb.Push0(); // count 0
        sb.Swap().SetTarget(loopStart); // 0 a
        sb.Dup(); // 0 a a
        sb.Push0(); // 0 a a 0
        sb.JmpEq(endLoop); // 0 a
        sb.ShR(1); // 0 a>>1
        sb.Swap(); // a>>1 0
        sb.Inc(); // a>>1 1
        sb.Jmp(loopStart);
        sb.Drop().SetTarget(endLoop);
        sb.Push(bitWidth);
        sb.Swap();
        sb.Sub();
        sb.SetTarget(endTarget);
    }

    private static void HandlePopCount<T>(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int bitWidth)
        where T : struct
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Mask to ensure the value is treated as an unsigned integer of the given bit width
        sb.And((BigInteger.One << bitWidth) - 1); // value = value & mask

        // Initialize count to 0
        sb.Push(0); // value count
        sb.Swap(); // count value

        // Loop to count the number of 1 bits
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        sb.Dup().SetTarget(loopStart); // count value value
        sb.Push0(); // count value value 0
        sb.JmpEq(endLoop); // count value
        sb.Dup(); // count value value
        sb.And(1); // count value (value & 1)
        sb.Rot(); // value (value & 1) count
        sb.Add(); // value count += (value & 1)
        sb.Swap(); // count value
        sb.ShR(1); // count value >>= 1
        sb.Jmp(loopStart);

        sb.Drop().SetTarget(endLoop); // Drop the remaining value
    }
}
