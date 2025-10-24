// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.SByte.cs file belongs to the neo project and is free
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
    /// Handles the sbyte.Parse method by converting a string to an sbyte value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts string to integer using StdLib.atoi, then validates it's within sbyte range [-128, 127]
    /// </remarks>
    private static void HandleSByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();                                // Duplicate result for range check
                methodConvert.Within(sbyte.MinValue, sbyte.MaxValue); // Check if value is within [-128, 127]
                methodConvert.Not();                                // Invert to detect invalid range
            },
            () =>
            {
                methodConvert.Throw();
            });
    }

    /// <summary>
    /// Handles the sbyte.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: For negative values returns 0, otherwise counts leading zeros by right-shifting until zero
    /// </remarks>
    private static void HandleSByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIfComparison(
            () =>
            {
                methodConvert.Dup();                               // Duplicate for comparison
                methodConvert.Push0();                             // Compare against zero
            },
            OpCode.JMPGE,
            thenEmitter: () =>
            {
                methodConvert.Push(0);                             // value count
                methodConvert.EmitWhileComparisonTrueExit(
                    perIterationSetup: () => methodConvert.Swap(),
                    comparisonSetup: () =>
                    {
                        methodConvert.Dup();                       // count value value
                        methodConvert.Push0();                     // count value value 0
                    },
                    comparisonOp: OpCode.JMPEQ,
                    bodyEmitter: scope =>
                    {
                        methodConvert.Push1();                     // count value 1
                        methodConvert.ShR();                       // count (value >> 1)
                        methodConvert.Swap();                      // (value >> 1) count
                        methodConvert.Inc();                       // (value >> 1) (count + 1)
                    },
                    exitEmitter: () => methodConvert.Drop());
                methodConvert.Push(8);
                methodConvert.Swap();
                methodConvert.Sub();
            },
            elseEmitter: () =>
            {
                methodConvert.Drop();                              // Remove original value
                methodConvert.Push0();                             // Return 0 for negative values
            });
    }

    /// <summary>
    /// Handles the sbyte.CopySign method by copying the sign from one value to another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses BigInteger CopySign then validates result is within sbyte range
    /// </remarks>
    private static void HandleSByteCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        JumpTarget noOverflowTarget = new();
        methodConvert.Dup();
        methodConvert.Push(sbyte.MaxValue);
        methodConvert.JumpIfLessOrEqual(noOverflowTarget);
        methodConvert.Throw();
        noOverflowTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles the sbyte.CreateChecked method by creating an sbyte with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the input value is within sbyte range [-128, 127], throws on overflow
    /// </remarks>
    private static void HandleSByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();
                methodConvert.Within(sbyte.MinValue, sbyte.MaxValue);
                methodConvert.Not();
            },
            () =>
            {
                methodConvert.Throw();
            });
    }

    /// <summary>
    /// Handles the sbyte.CreateSaturating method by creating an sbyte with saturation on overflow.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Clamps the input value to sbyte range [-128, 127] instead of throwing on overflow
    /// </remarks>
    private static void HandleSByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Push(sbyte.MinValue);
        methodConvert.Push(sbyte.MaxValue);
        var endTarget = new JumpTarget();
        var exceptionTarget = new JumpTarget();
        var minTarget = new JumpTarget();
        var maxTarget = new JumpTarget();

        // Stack manipulation for clamping logic
        methodConvert.Dup();                                       // value min max max
        methodConvert.Rot();                                       // value max max min
        methodConvert.Dup();                                       // value max max min min
        methodConvert.Rot();                                       // value max min min max
        methodConvert.JumpIfLess(exceptionTarget);         // value max min
        methodConvert.Throw();

        exceptionTarget.Instruction = methodConvert.Nop();
        methodConvert.Rot();                                       // max min value
        methodConvert.Dup();                                       // max min value value
        methodConvert.Rot();                                       // max value value min
        methodConvert.Dup();                                       // max value value min min
        methodConvert.Rot();                                       // max value min min value
        methodConvert.JumpIfGreater(minTarget);               // max value min
        methodConvert.Drop();                                      // max value
        methodConvert.Dup();                                       // max value value
        methodConvert.Rot();                                       // value value max
        methodConvert.Dup();                                       // value value max max
        methodConvert.Rot();                                       // value max max value
        methodConvert.JumpIfLess(maxTarget);               // value max
        methodConvert.Drop();
        methodConvert.JumpAlways(endTarget);

        minTarget.Instruction = methodConvert.Nop();
        methodConvert.Reverse3();
        methodConvert.Drop();
        methodConvert.Drop();
        methodConvert.JumpAlways(endTarget);

        maxTarget.Instruction = methodConvert.Nop();
        methodConvert.Swap();
        methodConvert.Drop();
        methodConvert.JumpAlways(endTarget);

        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles the sbyte.RotateLeft method by rotating bits to the left.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates sbyte bits left by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleSByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        EmitRotateLeftSigned(methodConvert, sizeof(sbyte) * 8);
    }

    /// <summary>
    /// Handles the sbyte.RotateRight method by rotating bits to the right.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Rotates sbyte bits right by specified amount, handling sign extension properly
    /// </remarks>
    private static void HandleSByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        EmitRotateRightSigned(methodConvert, sizeof(sbyte) * 8);
    }

    /// <summary>
    /// Handles the sbyte.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleSBytePopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        methodConvert.EmitPopCountWithMask(sizeof(sbyte) * 8);
    }
}
