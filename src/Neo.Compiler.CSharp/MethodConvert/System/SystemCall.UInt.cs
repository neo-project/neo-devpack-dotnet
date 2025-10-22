// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.UInt.cs file belongs to the neo project and is free
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
    /// Handles the uint.Parse method by converting a string to a uint value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within uint range [0, 4294967295]
    /// </remarks>
    private static void HandleUIntParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();                               // Duplicate result for range check
                methodConvert.Within(uint.MinValue, uint.MaxValue); // Check if value is within uint range
                methodConvert.Not();                               // Invert to detect invalid range
            },
            () =>
            {
                methodConvert.Throw();
            });
    }

    /// <summary>
    /// Handles the uint.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts leading zeros by right-shifting until zero (no negative check needed for unsigned values)
    /// </remarks>
    private static void HandleUIntLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(0);                                     // Initialize count to 0
        methodConvert.EmitWhileComparisonTrueExit(
            perIterationSetup: () => methodConvert.Swap(),
            comparisonSetup: () =>
            {
                methodConvert.Dup();                               // Duplicate value for zero check
                methodConvert.Push0();                             // Push 0 for comparison
            },
            comparisonOp: OpCode.JMPEQ,
            bodyEmitter: scope =>
            {
                methodConvert.Push1();                             // Push 1 for right shift
                methodConvert.ShR();                               // Right shift value by 1
                methodConvert.Swap();                              // Swap value and count
                methodConvert.Inc();                               // Increment count
            },
            exitEmitter: () => methodConvert.Drop());
        methodConvert.Push(32);                                    // Push 32 (bit width)
        methodConvert.Swap();                                      // Swap 32 and count
        methodConvert.Sub();                                       // Calculate 32 - count
    }

    /// <summary>
    /// Handles the uint.CreateChecked method by creating a uint with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the input value is within uint range [0, 4294967295], throws on overflow
    /// </remarks>
    private static void HandleUIntCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();
                methodConvert.Within(uint.MinValue, uint.MaxValue); // Check if value is within uint range
                methodConvert.Not();                                // Invert to detect invalid range
            },
            () =>
            {
                methodConvert.Throw();
            });
    }

    /// <summary>
    /// Handles the uint.CreateSaturating method by creating a uint with saturation on overflow.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Clamps the input value to uint range [0, 4294967295] instead of throwing on overflow
    /// </remarks>
    private static void HandleUIntCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(uint.MinValue);
        methodConvert.Push(uint.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();
        methodConvert.Dup();                                       // Stack manipulation for clamping logic
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfLess(exceptionTarget);         // Jump if value < min
        methodConvert.Throw();                                     // Throw exception for invalid range
        exceptionTarget.Instruction = methodConvert.Nop();         // Exception handling target
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfGreater(minTarget);               // Jump if value > min threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.Dup();                                       // Duplicate for comparison
        methodConvert.Rot();                                       // Rotate stack elements
        methodConvert.JumpIfLess(maxTarget);               // Jump if value < max threshold
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways(endTarget);                 // Jump to end
        minTarget.Instruction = methodConvert.Nop();               // Minimum value target
        methodConvert.Reverse3();                                  // Reverse top 3 stack elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways(endTarget);                 // Jump to end
        maxTarget.Instruction = methodConvert.Nop();               // Maximum value target
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Drop();                                      // Drop unnecessary value
        methodConvert.JumpAlways(endTarget);                 // Jump to end
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the uint.RotateLeft method by rotating bits to the left.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates uint bits left by specified amount (no sign extension needed for unsigned values)
    /// </remarks>
    private static void HandleUIntRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (uint)(value << rotateAmount) | (value >> (32 - rotateAmount))
        var bitWidth = sizeof(uint) * 8;
        methodConvert.Push(bitWidth - 1);                          // Push 31 (32-bit - 1)
        methodConvert.And();                                       // rotateAmount & 31
        methodConvert.Swap();                                      // Swap elements
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFF (32-bit mask)
        methodConvert.And();                                       // Apply mask to value
        methodConvert.Swap();                                      // Swap elements
        methodConvert.ShL();                                       // value << (rotateAmount & 31)
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFF (32-bit mask)
        methodConvert.And();                                       // Ensure SHL result is 32-bit
        methodConvert.LdArg0();                                    // Load original value
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFF (32-bit mask)
        methodConvert.And();                                       // Apply mask to original value
        methodConvert.LdArg1();                                    // Load rotate amount
        methodConvert.Push(bitWidth);                              // Push 32
        methodConvert.Swap();                                      // Swap top two elements
        methodConvert.Sub();                                       // 32 - rotateAmount
        methodConvert.Push(bitWidth - 1);                          // Push 31
        methodConvert.And();                                       // (32 - rotateAmount) & 31
        methodConvert.ShR();                                       // (uint)value >> ((32 - rotateAmount) & 31)
        methodConvert.Or();                                        // Combine left and right parts
        methodConvert.Push((BigInteger.One << bitWidth) - 1);      // Push 0xFFFFFFFF (32-bit mask)
        methodConvert.And();                                       // Ensure final result is 32-bit
    }

    /// <summary>
    /// Handles the uint.RotateRight method by rotating bits to the right.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates uint bits right by specified amount (no sign extension needed for unsigned values)
    /// </remarks>
    private static void HandleUIntRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Algorithm: (uint)(value >> rotateAmount) | (value << (32 - rotateAmount))
        var bitWidth = sizeof(uint) * 8;
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
    /// Handles the uint.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleUIntPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitPopCountWithMask(sizeof(uint) * 8);
    }
}
