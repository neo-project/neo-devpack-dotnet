// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Long.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
    /// <summary>
    /// Handles the long.Parse method by converting a string to a long value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within long range [-9223372036854775808, 9223372036854775807]
    /// </remarks>
    private static void HandleLongParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();                                // Duplicate result for range check
                methodConvert.Within(long.MinValue, long.MaxValue); // Check if value is within long range
            },
            () => { },
            () => methodConvert.Throw(),
            fallThroughElse: true);
    }

    /// <summary>
    /// Handles the long.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: For negative values returns 0, otherwise counts leading zeros by right-shifting until zero
    /// </remarks>
    private static void HandleLongLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        methodConvert.Dup();                                       // Duplicate value for negative check
        methodConvert.Push0();                                     // Push 0 for comparison
        JumpTarget notNegative = new();
        methodConvert.JumpIfGreaterOrEqual( notNegative);             // Jump if value >= 0
        methodConvert.Drop();                                      // Drop negative value
        methodConvert.Push0();                                     // Return 0 for negative values
        methodConvert.JumpAlways( endTarget);                 // Jump to end
        notNegative.Instruction = methodConvert.Nop();             // Target for non-negative values
        methodConvert.Push(0);                                     // Initialize count to 0
        loopStart.Instruction = methodConvert.Swap();              // Swap count and value
        methodConvert.Dup();                                       // Duplicate value for zero check
        methodConvert.Push0();                                     // Push 0 for comparison
        methodConvert.JumpIfEqual( endLoop);                 // Exit loop if value is 0
        methodConvert.Push1();                                     // Push 1 for right shift
        methodConvert.ShR();                                       // Right shift value by 1
        methodConvert.Swap();                                      // Swap value and count
        methodConvert.Inc();                                       // Increment count
        methodConvert.JumpAlways( loopStart);                 // Continue loop
        endLoop.Instruction = methodConvert.Drop();                // Drop remaining value
        methodConvert.Push(64);                                    // Push 64 (bit width)
        methodConvert.Swap();                                      // Swap 64 and count
        methodConvert.Sub();                                       // Calculate 64 - count
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the long.CopySign method by copying the sign from one value to another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses BigInteger CopySign then validates result is within long range
    /// </remarks>
    private static void HandleLongCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        JumpTarget noOverflowTarget = new();
        methodConvert.Dup();                                       // Duplicate result for overflow check
        methodConvert.Push(long.MaxValue);                         // Push long maximum value
        methodConvert.JumpIfLessOrEqual( noOverflowTarget);        // Jump if <= max value
        methodConvert.Throw();                                     // Throw if overflow
        noOverflowTarget.Instruction = methodConvert.Nop();        // No overflow target
    }

    /// <summary>
    /// Handles the long.CreateChecked method by creating a long with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the input value is within long range [-9223372036854775808, 9223372036854775807], throws on overflow
    /// </remarks>
    private static void HandleLongCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();                                // Duplicate value for range check
                methodConvert.Within(long.MinValue, long.MaxValue); // Check if value is within long range
            },
            () => { },
            () => methodConvert.Throw(),
            fallThroughElse: true);
    }

    /// <summary>
    /// Handles the long.CreateSaturating method by creating a long with saturation on overflow.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Clamps the input value to long range [-9223372036854775808, 9223372036854775807] instead of throwing on overflow
    /// </remarks>
    private static void HandleLongCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(long.MinValue);
        methodConvert.Push(long.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        methodConvert.Dup();                                       // Stack manipulation for clamping logic
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfLess( exceptionTarget);         // Jump if value < min
        methodConvert.Throw();                                     // Throw exception for invalid range
        exceptionTarget.Instruction = methodConvert.Nop();         // Exception handling target
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfGreater( minTarget);               // Jump if value > min threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfLess( maxTarget);               // Jump if value < max threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways( endTarget);                 // Jump to end
        minTarget.Instruction = methodConvert.Nop();               // Minimum value target
        methodConvert.Reverse3();                                  // Reverse top 3 stack elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways( endTarget);                 // Jump to end
        maxTarget.Instruction = methodConvert.Nop();               // Maximum value target
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways( endTarget);                 // Jump to end
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the long.RotateLeft method by rotating bits to the left.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates long bits left by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleLongRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (long)((value << (rotateAmount & 63)) | ((ulong)value >> (64 - (rotateAmount & 63))))
        var bitWidth = sizeof(long) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 63 (64-bit - 1)
        methodConvert.And();                                       // rotateAmount & 63
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 63)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 64-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 64 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 63
        methodConvert.And();                                       // (64 - rotateAmount) & 63
        methodConvert.ShR();                                       // (ulong)value >> ((64 - rotateAmount) & 63)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Dup();                                       // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1));      // Push BigInteger.One << 63 (0x8000000000000000)
        var endTarget = new JumpTarget();
        methodConvert.JumpIfLess( endTarget);               // Jump if result < 0x8000000000000000
        methodConvert.Push(BigInteger.One << bitWidth);            // BigInteger.One << 64 (0x10000000000000000)
        methodConvert.Sub();                                       // Apply sign extension
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the long.RotateRight method by rotating bits to the right.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates long bits right by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleLongRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (long)(((ulong)value >> (rotateAmount & 63)) | ((long)value << (64 - (rotateAmount & 63))))
        var bitWidth = sizeof(long) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 63 (64-bit - 1)
        methodConvert.And();                                       // rotateAmount & 63
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Mod();                                       // Modulo operation
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Sub();                                       // Subtraction
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 63)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 64-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFFFFFFFFFF (64-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Mod();                                       // Modulo operation
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Sub();                                       // Subtraction
        methodConvert.Push(bitWidth);                              // Push 64
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 64 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 63
        methodConvert.And();                                       // (64 - rotateAmount) & 63
        methodConvert.ShR();                                       // (ulong)value >> ((64 - rotateAmount) & 63)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Dup();                                       // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1));      // Push BigInteger.One << 63 (0x8000000000000000)
        var endTarget = new JumpTarget();
        methodConvert.JumpIfLess( endTarget);               // Jump if result < 0x8000000000000000
        methodConvert.Push(BigInteger.One << bitWidth);            // BigInteger.One << 64 (0x10000000000000000)
        methodConvert.Sub();                                       // Apply sign extension
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the long.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleLongPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of long
        var bitWidth = sizeof(long) * 8;

        // Mask to ensure the value is treated as a 64-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // 0xFFFFFFFFFFFFFFFF
        methodConvert.And();                                       // value = value & 0xFFFFFFFFFFFFFFFF
        // Initialize count to 0
        methodConvert.Push(0);                                     // value count
        methodConvert.Swap();                                      // count value
        // Loop to count the number of 1 bits
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        loopStart.Instruction = methodConvert.Dup();               // count value value
        methodConvert.Push0();                                     // count value value 0
        methodConvert.JumpIfEqual( endLoop);                 // count value
        methodConvert.Dup();                                       // count value value
        methodConvert.Push1();                                     // count value value 1
        methodConvert.And();                                       // count value (value & 1)
        methodConvert.Rot();                                       // value (value & 1) count
        methodConvert.Add();                                       // value count += (value & 1)
        methodConvert.Swap();                                      // count value
        methodConvert.Push1();                                     // count value 1
        methodConvert.ShR();                                       // count value >>= 1
        methodConvert.JumpAlways( loopStart);                 // Continue loop

        endLoop.Instruction = methodConvert.Drop();                // Drop the remaining value
    }
}
