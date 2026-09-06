// Copyright (C) 2015-2026 The Neo Project.
//
// SystemCall.BigInteger.cs file belongs to the neo project and is free
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
    /// Handles the BigInteger.One property by pushing the value 1 onto the stack.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Simply pushes the constant value 1 onto the evaluation stack
    /// </remarks>
    private static void HandleBigIntegerOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(1);
    }

    /// <summary>
    /// Handles the BigInteger.MinusOne property by pushing the value -1 onto the stack.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Simply pushes the constant value -1 onto the evaluation stack
    /// </remarks>
    private static void HandleBigIntegerMinusOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(-1);
    }

    /// <summary>
    /// Handles the BigInteger.Zero property by pushing the value 0 onto the stack.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Simply pushes the constant value 0 onto the evaluation stack
    /// </remarks>
    private static void HandleBigIntegerZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.Push(0);
    }

    /// <summary>
    /// Handles the BigInteger.IsZero property by checking if the value equals zero.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the BigInteger value with 0 using numeric equality
    /// </remarks>
    private static void HandleBigIntegerIsZero(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(0);
        methodConvert.NumEqual();                                  // Check if value equals 0
    }

    /// <summary>
    /// Handles the BigInteger.IsOne property by checking if the value equals one.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the BigInteger value with 1 using numeric equality
    /// </remarks>
    private static void HandleBigIntegerIsOne(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Push(1);
        methodConvert.NumEqual();                                  // Check if value equals 1
    }

    /// <summary>
    /// Handles the BigInteger.IsEven property by checking if the value is even.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs modulo 2 operation and checks if result is zero (even number)
    /// </remarks>
    private static void HandleBigIntegerIsEven(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(2);
        methodConvert.Mod();                                       // Calculate value % 2
        methodConvert.Not();                                       // BigInteger GetBoolean() => !value.IsZero;
    }

    /// <summary>
    /// Handles the BigInteger.Sign property by returning the sign of the BigInteger.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns -1 for negative, 0 for zero, 1 for positive values
    /// </remarks>
    private static void HandleBigIntegerSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.Sign();                                      // Get sign of value
    }

    /// <summary>
    /// Handles the BigInteger.Pow method by raising a BigInteger to a power.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Calculates base^exponent using the POW VM instruction
    /// </remarks>
    private static void HandleBigIntegerPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Pow();                                       // Calculate base^exponent
    }

    /// <summary>
    /// Handles the BigInteger.ModPow method by computing modular exponentiation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Calculates (base^exponent) % modulus efficiently using the MODPOW VM instruction
    /// </remarks>
    private static void HandleBigIntegerModPow(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.ModPow();                                    // Calculate (base^exponent) % modulus
    }

    /// <summary>
    /// Handles the BigInteger.Add method by adding two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs addition of two BigInteger values using the ADD VM instruction
    /// </remarks>
    private static void HandleBigIntegerAdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Add();                                       // Add two values
    }

    /// <summary>
    /// Handles the BigInteger.Subtract method by subtracting one BigInteger from another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs subtraction of two BigInteger values using the SUB VM instruction
    /// </remarks>
    private static void HandleBigIntegerSubtract(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Sub();                                       // Subtract second from first
    }

    /// <summary>
    /// Handles the BigInteger.Negate method by negating a BigInteger value.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Changes the sign of a BigInteger value using the NEGATE VM instruction
    /// </remarks>
    private static void HandleBigIntegerNegate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Negate();                                    // Negate the value
    }

    /// <summary>
    /// Handles the BigInteger.Multiply method by multiplying two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs multiplication of two BigInteger values using the MUL VM instruction
    /// </remarks>
    private static void HandleBigIntegerMultiply(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Mul();                                       // Multiply two values
    }

    /// <summary>
    /// Handles the BigInteger.Divide method by dividing one BigInteger by another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs integer division of two BigInteger values using the DIV VM instruction
    /// </remarks>
    private static void HandleBigIntegerDivide(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Div();                                       // Divide first by second
    }

    /// <summary>
    /// Handles the BigInteger.Remainder method by computing the remainder of division.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Computes the remainder of dividing one BigInteger by another using the MOD VM instruction
    /// </remarks>
    private static void HandleBigIntegerRemainder(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        methodConvert.Mod();                                       // Calculate remainder
    }

    /// <summary>
    /// Handles the BigInteger.Compare method by comparing two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns -1 if left &lt; right, 0 if left = right, 1 if left &gt; right
    /// </remarks>
    private static void HandleBigIntegerCompare(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        // Stack (StdCall): bottom left, top right. Compare without left-right SUB:
        // (left > right) - (left < right), using only small-integer SUB on 0/1 flags.
        methodConvert.Over();                // L, R, L
        methodConvert.Over();                // L, R, L, R
        methodConvert.Gt();                  // L, R, (L>R as 0/1)
        methodConvert.Reverse3();            // L > R, R, L
        methodConvert.Gt();                  // L > R, R > L (i.e. L < R) as 0/1
        methodConvert.Sub();                 // (L > R) - (L < R) -> -1, 0, or 1
    }

    /// <summary>
    /// Handles the BigInteger.GreatestCommonDivisor method by computing the GCD of two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses Euclidean algorithm to find GCD by repeatedly applying modulo operation
    /// </remarks>
    private static void HandleBigIntegerGreatestCommonDivisor(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        JumpTarget endTarget = new();
        methodConvert.Dup();                                // a, b, b
        methodConvert.JumpIfNot(endTarget);                 // a, b, // if b is 0 (falsy), result is abs(a)
        JumpTarget gcdTarget = new()                        // a, b
        {
            Instruction = methodConvert.Swap()              // b, a
        };
        methodConvert.Over();                               // b, a, b
        methodConvert.Mod();                                // b, a % b
        methodConvert.Dup();                                // b, a % b, a % b
        methodConvert.JumpIfTrue(gcdTarget);                // b, a % b, // if a % b != 0 (truthy), jump to the start
        endTarget.Instruction = methodConvert.Drop();       // b (or a if b is 0)
        methodConvert.Abs();                                // | result |
    }

    /// <summary>
    /// Handles the BigInteger.ToByteArray method by converting BigInteger to byte array.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Converts the BigInteger to its byte array representation using type conversion
    /// </remarks>
    private static void HandleBigIntegerToByteArray(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        var nonZeroTarget = new JumpTarget();
        methodConvert.Dup();
        methodConvert.JumpIfTrue(nonZeroTarget);
        methodConvert.Drop();
        methodConvert.Push([(byte)0x00]);
        nonZeroTarget.Instruction = methodConvert.Dup();
        methodConvert.Size();
        methodConvert.Left(null);
    }

    /// <summary>
    /// Handles the BigInteger.Parse method by converting a string to BigInteger.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Optimizes constant strings or uses StdLib.atoi for runtime parsing
    /// </remarks>
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

    /// <summary>
    /// Shared helper that enforces a BigInteger value lies within the supplied inclusive range.
    /// </summary>
    /// <param name="methodConvert">The method converter instance.</param>
    /// <param name="model">The semantic model.</param>
    /// <param name="symbol">The method symbol.</param>
    /// <param name="instanceExpression">The instance expression (if any).</param>
    /// <param name="arguments">Method arguments.</param>
    /// <param name="minValue">Inclusive minimum accepted value.</param>
    /// <param name="maxValue">Inclusive maximum accepted value.</param>
    private static void HandleBigIntegerRangeCheckedConversion(
        MethodConvert methodConvert,
        SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments,
        BigInteger minValue,
        BigInteger maxValue)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endTarget = new();
        methodConvert.Dup();                                       // Duplicate value for range check
        methodConvert.Within(minValue, maxValue);                  // Check if within configured range
        methodConvert.JumpIfTrue(endTarget);                       // Jump if within range
        methodConvert.Throw();                                     // Throw if out of range
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    private static void HandleBigIntegerSizeCheckedConversion(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments, int sizeofSigned)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget endTarget = new();
        methodConvert.Dup();                              // Duplicate value for range check
        methodConvert.Size();                             // Get the size of the value
        methodConvert.Push(sizeofSigned);                 // Push the size of the type
        methodConvert.JumpIfLessOrEqual(endTarget);       // Compare the size of the value to the size of the type
        methodConvert.Throw();                            // Throw the value if it is out of range
        endTarget.Instruction = methodConvert.Nop();      // End target
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to sbyte with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within sbyte range [-128, 127], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToSByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerSizeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, sizeof(sbyte));
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to byte with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within byte range [0, 255], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToByte(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerRangeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, byte.MinValue, byte.MaxValue);
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to short with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within short range [-32768, 32767], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerSizeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, sizeof(short));
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to ushort with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within ushort range [0, 65535], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToUShort(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerRangeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, ushort.MinValue, ushort.MaxValue);
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to int with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within int range [-2147483648, 2147483647], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerSizeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, sizeof(int));
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to uint with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within uint range [0, 4294967295], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToUInt(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerRangeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, uint.MinValue, uint.MaxValue);
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to long with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within long range [-9223372036854775808, 9223372036854775807], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToLong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerSizeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, sizeof(long));
    }

    /// <summary>
    /// Handles explicit conversion of BigInteger to ulong with range checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates the BigInteger is within ulong range [0, 18446744073709551615], throws on overflow
    /// </remarks>
    private static void HandleBigIntegerToULong(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerRangeCheckedConversion(methodConvert, model, symbol, instanceExpression, arguments, ulong.MinValue, ulong.MaxValue);
    }

    /// <summary>
    /// Handles implicit conversion of various types to BigInteger.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Direct conversion without range checking since all primitive types fit in BigInteger
    /// </remarks>
    private static void HandleToBigInteger(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    /// <summary>
    /// Handles the BigInteger.Max method by returning the larger of two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares two BigInteger values and returns the larger one using the MAX VM instruction
    /// </remarks>
    private static void HandleBigIntegerMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Max();                                       // Return maximum of two values
    }

    /// <summary>
    /// Handles the BigInteger.Min method by returning the smaller of two BigInteger values.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares two BigInteger values and returns the smaller one using the MIN VM instruction
    /// </remarks>
    private static void HandleBigIntegerMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Min();                                       // Return minimum of two values
    }

    /// <summary>
    /// Handles the BigInteger.IsOdd property by checking if the value is odd.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Performs modulo 2 operation and checks if result is non-zero (odd number)
    /// </remarks>
    private static void HandleBigIntegerIsOdd(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(2);
        methodConvert.Mod();                                       // Calculate value % 2
        methodConvert.Nz();                                        // Check if non-zero (odd)
    }

    /// <summary>
    /// Handles the BigInteger.IsNegative property by checking if the value is negative.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the BigInteger value with 0 to check if it's negative
    /// </remarks>
    private static void HandleBigIntegerIsNegative(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(0);
        methodConvert.Lt();                                        // Check if value < 0
    }

    /// <summary>
    /// Handles the BigInteger.IsPositive property by checking if the value is positive or zero.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares the BigInteger value with 0 to check if it's greater than or equal to 0
    /// </remarks>
    private static void HandleBigIntegerIsPositive(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Push(0);
        methodConvert.Ge();                                        // Check if value >= 0
        // GE instead of GT, because C# BigInteger works like that
        // https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs#L4098C13-L4098C37
    }

    /// <summary>
    /// Handles the BigInteger.IsPow2 property by checking if the value is a power of 2.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses the formula (n &amp; (n-1) == 0) and (n != 0) to check if value is a power of 2
    /// </remarks>
    private static void HandleBigIntegerIsPow2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        // (n & (n-1) == 0) and (n != 0)
        JumpTarget endTarget = new();
        JumpTarget greaterZero = new();
        methodConvert.Dup();                                       // Duplicate value for zero check
        methodConvert.Push0();                                     // Push 0 for comparison
        methodConvert.JumpIfGreater(greaterZero);                  // Jump if non-zero
        methodConvert.Drop();                                      // Drop the value if zero
        methodConvert.Push(false);                                 // Return false if value <= 0
        methodConvert.JumpAlways(endTarget);                        // Return false for zero
        greaterZero.Instruction = methodConvert.Nop();             // Non-zero target
        methodConvert.Dup();                                       // Duplicate value
        methodConvert.Dec();                                       // Decrement (n-1)
        methodConvert.And();                                       // Calculate n & (n-1)
        methodConvert.Not();                                       // Calculate !(n & (n-1))
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Handles the BigInteger.Log2 method by calculating the logarithm base 2.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns 0 when value &lt;= 1. Otherwise increments n from 0 until
    /// (value &gt;&gt; n) &lt;= 1, which equals floor(log2(value)). Throws for negative values.
    /// </remarks>
    private static void HandleBigIntegerLog2(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nonNegativeTarget = new();
        JumpTarget loopStart = new();
        JumpTarget doneTarget = new();

        methodConvert.Dup();                                       // [value, value]
        methodConvert.Push0();                                     // [value, value, 0]
        methodConvert.JumpIfGreaterOrEqual(nonNegativeTarget);     // [value]
        methodConvert.Throw();                                     // Throw if negative
        nonNegativeTarget.Instruction = methodConvert.Nop();

        // n starts at 0. If value <= 1, Log2 is 0 (no shift). Otherwise n++ until (value >> n) <= 1.
        methodConvert.Push0();                                     // [value, n]
        methodConvert.Over();                                      // [value, n, value]
        methodConvert.Push1();                                     // [value, n, value, 1]
        methodConvert.JumpIfLessOrEqual(doneTarget);               // [value, n] if value <= 1
        loopStart.Instruction = methodConvert.Nop();
        methodConvert.Inc();                                       // [value, n + 1]
        methodConvert.Over();                                      // [value, n, value]
        methodConvert.Over();                                      // [value, n, value, n]
        methodConvert.ShR();                                       // [value, n, value >> n]
        methodConvert.Push1();                                     // [value, n, value >> n, 1]
        methodConvert.JumpIfGreater(loopStart);                    // continue while (value >> n) > 1
        doneTarget.Instruction = methodConvert.Nip();              // [n]
    }

    /// <summary>
    /// Handles the BigInteger.CopySign method by copying the sign from one value to another.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Returns value with absolute value of first argument and sign of second argument
    /// </remarks>
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
        methodConvert.Push0();                                     // Push 0 for comparison
        methodConvert.JumpIfLess(negativeTarget);          // Jump if b < 0
        methodConvert.Abs();                                       // Return abs(a) if b >= 0
        methodConvert.JumpAlways(endTarget);                 // Jump to end
        negativeTarget.Instruction = methodConvert.Nop();          // Negative target
        methodConvert.Abs();                                       // Get abs(a)
        methodConvert.Negate();                                    // Return -abs(a) if b < 0
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the Math.BigInteger.DivRem method by computing both quotient and remainder.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Computes both quotient and remainder in a single operation, returns as tuple
    /// </remarks>
    private static void HandleMathBigIntegerDivRem(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        EmitDivRem(methodConvert);
    }

    private static void EmitDivRem(MethodConvert methodConvert)
    {
        // algorithm: (left, right) => (left / right, left % right)
        methodConvert.Dup();                                       // r, l, l
        methodConvert.Pick(2);                                     // r, l, l, r
        methodConvert.Div();                                       // r, l, l/r

        // next, compute left % right
        // we need: l % r
        // the stack is: r l (l/r)
        // we want:    (l/r) (l%r)
        methodConvert.Reverse3();                                  // l/r, l, r
        methodConvert.Mod();                                       // l/r, l%r
        methodConvert.Pack(2);                                     // (l/r, l%r) as array
    }

    /// <summary>
    /// Handles the BigInteger.LeadingZeroCount method by counting leading zeros in the binary representation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: For negative values returns 0, otherwise counts leading zeros in the current 32-bit limb
    /// </remarks>
    private static void HandleBigIntegerLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endLoop = new();
        JumpTarget loopStart = new();
        JumpTarget endTarget = new();
        JumpTarget notNegative = new();
        methodConvert.Dup();                                       // Duplicate value for negative check
        methodConvert.Push0();                                     // Push 0 for comparison
        methodConvert.JumpIfGreaterOrEqual(notNegative);           // Jump if value >= 0
        methodConvert.Drop();                                      // Drop negative value
        methodConvert.Push0();                                     // Return 0 for negative values
        methodConvert.JumpAlways(endTarget);                       // Jump to end
        notNegative.Instruction = methodConvert.Nop();             // Target for non-negative values
        methodConvert.Push(0);                                     // Initialize count to 0
        loopStart.Instruction = methodConvert.Swap();              // Swap count and value
        methodConvert.Dup();                                       // Duplicate value for zero check
        methodConvert.JumpIfFalse(endLoop);                        // Exit loop if value is 0(0 is false)
        methodConvert.Push1();                                     // Push 1 for right shift
        methodConvert.ShR();                                       // Right shift value by 1
        methodConvert.Swap();                                      // Swap value and count
        methodConvert.Inc();                                       // Increment count
        methodConvert.JumpAlways(loopStart);                       // Continue loop
        endLoop.Instruction = methodConvert.Drop();                // Drop remaining value
        JumpTarget nonZeroCount = new();
        methodConvert.Dup();                                       // Duplicate count for zero check
        methodConvert.JumpIfTrue(nonZeroCount);                    // Jump if count != 0(non-zero is true)
        methodConvert.Drop();                                      // Drop zero count
        methodConvert.Push(32);                                    // Zero has one empty 32-bit limb
        methodConvert.JumpAlways(endTarget);                       // Jump to end
        nonZeroCount.Instruction = methodConvert.Nop();            // Non-zero count target
        methodConvert.Dup();                                       // Preserve count for final subtraction
        methodConvert.Push(31);                                    // Round bit count up to a 32-bit limb
        methodConvert.Add();                                       // count + 31
        methodConvert.Push(5);                                     // 32-bit limb divisor
        methodConvert.ShR();                                       // (count + 31) / 32(count >> 5 == count / 32 if positive)
        methodConvert.Push(5);                                     // 32-bit limb width
        methodConvert.ShL();                                       // rounded width(count << 5 == count * 32)
        methodConvert.Swap();                                      // Swap width and count
        methodConvert.Sub();                                       // Calculate rounded width - count
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    /// <summary>
    /// Handles the BigInteger.CreateChecked method by creating a BigInteger with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Direct conversion since BigInteger can represent any integer value without overflow
    /// </remarks>
    private static void HandleBigIntegerCreatedChecked(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    /// <summary>
    /// Handles the BigInteger.CreateSaturating method by creating a BigInteger with saturation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Direct conversion since BigInteger can represent any integer value without saturation
    /// </remarks>
    private static void HandleBigIntegerCreateSaturating(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
    }

    /// <summary>
    /// Handles the BigInteger.CreateTruncating method by creating a BigInteger without truncation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Direct conversion since BigInteger can represent any integer value without truncation
    /// </remarks>
    private static void HandleBigIntegerCreateTruncating(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments!);
    }

    /// <summary>
    /// Handles the BigInteger.Equals method by comparing two BigInteger values for equality.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Compares two BigInteger values using numeric equality
    /// </remarks>
    private static void HandleBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.NumEqual();                                  // Check numeric equality
    }

    /// <summary>
    /// Handles the BigInteger.PopCount method by counting the number of set bits.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Counts 1-bits by repeatedly checking LSB and right-shifting until value becomes zero
    /// </remarks>
    private static void HandleBigIntegerPopCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        // Check if the value is within int range
        methodConvert.Dup();
        methodConvert.Size();
        methodConvert.Push(4); // sizeof(int)

        var endIntCheck = new JumpTarget();
        methodConvert.JumpIfGreater(endIntCheck);

        // If within int range, mask with 0xFFFFFFFF
        methodConvert.Push(0xFFFFFFFF);
        methodConvert.And();
        var endMask = new JumpTarget();
        methodConvert.JumpAlways(endMask);

        // If larger than int, throw exception, cause too many check will make the script too long.
        endIntCheck.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.Push("Out of int range");
        methodConvert.Throw();
        endMask.Instruction = methodConvert.AddInstruction(OpCode.NOP);

        methodConvert.EmitPopCountFromMaskedValue();
    }
}
