// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Byte.cs file belongs to the neo project and is free
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
    /// Handles the byte.Parse method by converting a string to a byte value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within byte range [0, 255]
    /// </remarks>
    private static void HandleByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.EmitIf(
            conditionEmitter: () =>
            {
                methodConvert.Dup();                                            // Duplicate result for range check
                methodConvert.Within(byte.MinValue, byte.MaxValue);             // Check if value is within byte range
            },
            thenEmitter: () => { },
            elseEmitter: () => methodConvert.Throw(),
            fallThroughElse: true);
    }

    /// <summary>
    /// Handles the byte.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts leading zeros by right-shifting until zero (no negative check needed for unsigned values)
    /// </remarks>
    private static void HandleByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
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
        methodConvert.Push(8);                                     // Push 8 (bit width)
        methodConvert.Swap();                                      // Swap 8 and count
        methodConvert.Sub();                                       // Calculate 8 - count
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the byte.CreateChecked method by creating a byte with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the input value is within byte range [0, 255], throws on overflow
    /// </remarks>
    private static void HandleByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIf(
            conditionEmitter: () =>
            {
                methodConvert.Dup();                                // Duplicate value for range check
                methodConvert.Within(byte.MinValue, byte.MaxValue); // Check if value is within byte range
            },
            thenEmitter: () => { },
            elseEmitter: () => methodConvert.Throw(),               // Throw if out of range
            fallThroughElse: true);
    }

    /// <summary>
    /// Handles the byte.CreateSaturating method by creating a byte with saturation on overflow.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Clamps the input value to byte range [0, 255] instead of throwing on overflow
    /// </remarks>
    private static void HandleByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(byte.MinValue);
        methodConvert.Push(byte.MaxValue);
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
    /// Handles the byte.RotateLeft method by rotating bits to the left.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates byte bits left by specified amount (no sign extension needed for unsigned values)
    /// </remarks>
    private static void HandleByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (byte)((value << (rotateAmount & 7)) | (value >> ((8 - rotateAmount) & 7)))
        var bitWidth = sizeof(byte) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 7 (8-bit - 1)
        methodConvert.And();                                       // rotateAmount & 7
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFF (8-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 7)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFF (8-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 8-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFF (8-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 8
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 8 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 7
        methodConvert.And();                                       // (8 - rotateAmount) & 7
        methodConvert.ShR();                                       // (byte)value >> ((8 - rotateAmount) & 7)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFF (8-bit mask)
        methodConvert.And();                                       // Ensure final result is 8-bit
    }

    /// <summary>
    /// Handles the byte.RotateRight method by rotating bits to the right.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates byte bits right by specified amount (no sign extension needed for unsigned values)
    /// </remarks>
    private static void HandleByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (byte)((value >> (rotateAmount & 7)) | (value << ((8 - rotateAmount) & 7)))
        var bitWidth = sizeof(byte) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push (bitWidth - 1)
        methodConvert.And();                                       // rotateAmount & (bitWidth - 1)
        methodConvert.ShR();                                       // value >> (rotateAmount & (bitWidth - 1))
        methodConvert.LdArg0();                                    // Load value again
        methodConvert.Push(bitWidth);                              // Push bitWidth
        methodConvert.LdArg1();                                    // Load rotateAmount
        methodConvert.Sub();                                       // bitWidth - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push (bitWidth - 1)
        methodConvert.And();                                       // (bitWidth - rotateAmount) & (bitWidth - 1)
        methodConvert.ShL();                                       // value << ((bitWidth - rotateAmount) & (bitWidth - 1))
        methodConvert.Or();                                        // Combine the results with OR
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push (2^bitWidth - 1) as bitmask
        methodConvert.And();                                       // Ensure final result is bitWidth-bit
    }

    /// <summary>
    /// Handles the byte.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleBytePopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of byte
        var bitWidth = sizeof(byte) * 8;

        // Mask to ensure the value is treated as a 8-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // 0xFF
        methodConvert.And();                                       // value = value & 0xFF
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
