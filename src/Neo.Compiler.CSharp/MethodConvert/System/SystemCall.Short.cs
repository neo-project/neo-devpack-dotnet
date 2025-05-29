// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Short.cs file belongs to the neo project and is free
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
    /// Handles the short.Parse method by converting a string to a short value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within short range [-32768, 32767]
    /// </remarks>
    private static void HandleShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.Dup();                                        // Duplicate result for range check
        methodConvert.Within(short.MinValue, new BigInteger(short.MaxValue)); // Check if value is within short range
        methodConvert.Jump(OpCode.JMPIF, endTarget);               // Jump if within range
        methodConvert.Throw();                                     // Throw if out of range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: For negative values returns 0, otherwise counts leading zeros by right-shifting until zero
    /// </remarks>
    private static void HandleShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        if (symbol.ToString() == "short.LeadingZeroCount(short)")
        {
            methodConvert.Dup();                                   // Duplicate value for negative check
            methodConvert.Push0();                                 // Push 0 for comparison
            JumpTarget notNegative = new();
            methodConvert.Jump(OpCode.JMPGE, notNegative);         // Jump if value >= 0
            methodConvert.Drop();                                  // Drop negative value
            methodConvert.Push0();                                 // Return 0 for negative values
            methodConvert.Jump(OpCode.JMP, endTarget);             // Jump to end
            notNegative.Instruction = methodConvert.Nop();         // Target for non-negative values
        }
        methodConvert.Push(0);                                     // Initialize count to 0
        loopStart.Instruction = methodConvert.Swap();              // Swap count and value
        methodConvert.Dup();                                       // Duplicate value for zero check
        methodConvert.Push0();                                     // Push 0 for comparison
        methodConvert.Jump(OpCode.JMPEQ, endLoop);                 // Exit loop if value is 0
        methodConvert.Push1();                                     // Push 1 for right shift
        methodConvert.ShR();                                       // Right shift value by 1
        methodConvert.Swap();                                      // Swap value and count
        methodConvert.Inc();                                       // Increment count
        methodConvert.Jump(OpCode.JMP, loopStart);                 // Continue loop
        endLoop.Instruction = methodConvert.Drop();                // Drop remaining value
        methodConvert.Push(16);                                    // Push 16 (bit width)
        methodConvert.Swap();                                      // Swap 16 and count
        methodConvert.Sub();                                       // Calculate 16 - count
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.CopySign method by copying the sign from one value to another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses BigInteger CopySign then validates result is within short range
    /// </remarks>
    private static void HandleShortCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        JumpTarget noOverflowTarget = new();
        methodConvert.Dup();                                       // Duplicate result for overflow check
        methodConvert.Push(short.MaxValue);                        // Push short maximum value
        methodConvert.Jump(OpCode.JMPLE, noOverflowTarget);        // Jump if <= max value
        methodConvert.Throw();                                     // Throw if overflow
        noOverflowTarget.Instruction = methodConvert.Nop();        // No overflow target
    }

    /// <summary>
    /// Handles the short.CreateChecked method by creating a short with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the input value is within short range [-32768, 32767], throws on overflow
    /// </remarks>
    private static void HandleShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Dup();
        methodConvert.Within(short.MinValue, short.MaxValue);    // Check if value is within short range
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.Throw();                                     // Throw if out of range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.CreateSaturating method by creating a short with saturation on overflow.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Clamps the input value to short range [-32768, 32767] instead of throwing on overflow
    /// </remarks>
    private static void HandleShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(short.MinValue);
        methodConvert.Push(short.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        methodConvert.Dup();                                       // Stack manipulation for clamping logic
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Jump(OpCode.JMPLT, exceptionTarget);         // Jump if value < min
        methodConvert.Throw();                                     // Throw exception for invalid range
        exceptionTarget.Instruction = methodConvert.Nop();         // Exception handling target
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Jump(OpCode.JMPGT, minTarget);               // Jump if value > min threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Jump(OpCode.JMPLT, maxTarget);               // Jump if value < max threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Jump(OpCode.JMP, endTarget);                 // Jump to end
        minTarget.Instruction = methodConvert.Nop();               // Minimum value target
        methodConvert.Reverse3();                                  // Reverse top 3 stack elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Jump(OpCode.JMP, endTarget);                 // Jump to end
        maxTarget.Instruction = methodConvert.Nop();               // Maximum value target
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Jump(OpCode.JMP, endTarget);                 // Jump to end
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.RotateLeft method by rotating bits to the left.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates short bits left by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (short)((value << (rotateAmount & 15)) | ((ushort)value >> ((16 - rotateAmount) & 15)))
        var bitWidth = sizeof(short) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 15 (16-bit - 1)
        methodConvert.And();                                       // rotateAmount & 15
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 15)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 16-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 16 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 15
        methodConvert.And();                                       // (16 - rotateAmount) & 15
        methodConvert.ShR();                                       // (ushort)value >> ((16 - rotateAmount) & 15)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Dup();                                       // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1));      // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);               // Jump if result < 0x8000
        methodConvert.Push(BigInteger.One << bitWidth);            // BigInteger.One << 16 (0x10000)
        methodConvert.Sub();                                       // Apply sign extension
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.RotateRight method by rotating bits to the right.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates short bits right by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (short)((value >> (rotateAmount & 15)) | ((ushort)value << ((16 - rotateAmount) & 15)))
        var bitWidth = sizeof(short) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 15 (16-bit - 1)
        methodConvert.And();                                       // rotateAmount & 15
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Mod();                                       // Modulo operation
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Sub();                                       // Subtraction
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 15)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 16-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFF (16-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Mod();                                       // Modulo operation
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Sub();                                       // Subtraction
        methodConvert.Push(bitWidth);                              // Push 16
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 16 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 15
        methodConvert.And();                                       // (16 - rotateAmount) & 15
        methodConvert.ShR();                                       // (ushort)value >> ((16 - rotateAmount) & 15)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Dup();                                       // Duplicate the result
        methodConvert.Push(BigInteger.One << (bitWidth - 1));      // Push BigInteger.One << 15 (0x8000)
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPLT, endTarget);               // Jump if result < 0x8000
        methodConvert.Push(BigInteger.One << bitWidth);            // BigInteger.One << 16 (0x10000)
        methodConvert.Sub();                                       // Apply sign extension
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the short.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleShortPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // Determine bit width of short
        var bitWidth = sizeof(short) * 8;

        // Mask to ensure the value is treated as a 16-bit unsigned integer
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // 0xFFFF
        methodConvert.And();                                       // value = value & 0xFFFF
        // Initialize count to 0
        methodConvert.Push(0);                                     // value count
        methodConvert.Swap();                                      // count value
        // Loop to count the number of 1 bits
        JumpTarget loopStart = new();
        JumpTarget endLoop = new();
        loopStart.Instruction = methodConvert.Dup();               // count value value
        methodConvert.Push0();                                     // count value value 0
        methodConvert.Jump(OpCode.JMPEQ, endLoop);                 // count value
        methodConvert.Dup();                                       // count value value
        methodConvert.Push1();                                     // count value value 1
        methodConvert.And();                                       // count value (value & 1)
        methodConvert.Rot();                                       // value (value & 1) count
        methodConvert.Add();                                       // value count += (value & 1)
        methodConvert.Swap();                                      // count value
        methodConvert.Push1();                                     // count value 1
        methodConvert.ShR();                                       // count value >>= 1
        methodConvert.Jump(OpCode.JMP, loopStart);                 // Continue loop

        endLoop.Instruction = methodConvert.Drop();                // Drop the remaining value
    }
}
