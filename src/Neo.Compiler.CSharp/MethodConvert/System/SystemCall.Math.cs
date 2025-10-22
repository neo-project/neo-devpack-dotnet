// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Math.cs file belongs to the neo project and is free
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
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Handles the Math.Abs method by returning the absolute value of a number.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses the ABS VM instruction to compute the absolute value
    /// </remarks>
    private static void HandleMathAbs(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Abs();                                       // Get absolute value
    }

    /// <summary>
    /// Handles the Math.Sign method by returning the sign of a number.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses the SIGN VM instruction to determine the sign (-1, 0, or 1)
    /// </remarks>
    private static void HandleMathSign(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Sign();                                      // Get sign of value
    }

    /// <summary>
    /// Handles the Math.Max method by returning the larger of two numbers.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses the MAX VM instruction to compare and return the larger value
    /// </remarks>
    private static void HandleMathMax(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Max();                                       // Return maximum value
    }

    /// <summary>
    /// Handles the Math.Min method by returning the smaller of two numbers.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Uses the MIN VM instruction to compare and return the smaller value
    /// </remarks>
    private static void HandleMathMin(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.Min();                                       // Return minimum value
    }

    /// <summary>
    /// Handles Math.DivRem for byte by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for sbyte by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathSByteDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for short by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for ushort by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathUShortDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for int by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for uint by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathUIntDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for long by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathLongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles Math.DivRem for ulong by delegating to BigInteger implementation.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Delegates to BigInteger DivRem for consistent implementation across integer types
    /// </remarks>
    private static void HandleMathULongDivRem(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleMathBigIntegerDivRem(methodConvert, model, symbol, instanceExpression, arguments);
    }

    /// <summary>
    /// Handles the Math.Clamp method by constraining a value to a specified range.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Validates min &lt;= max, then returns max(min, min(value, max)) to clamp value within bounds
    /// </remarks>
    private static void HandleMathClamp(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);

        var exceptionTarget = new JumpTarget();
        // Evaluation stack: value=5 min=0 max=10 <- top
        methodConvert.Over();                                      // 5 0 10 0
        methodConvert.Over();                                      // 5 0 10 0 10 <- top
        methodConvert.Jump(OpCode.JMPLE, exceptionTarget);         // 5 0 10  // if 0 <= 10, continue execution
        //methodConvert.Push("min>max");
        methodConvert.Throw();                                     // Throw if min > max
        exceptionTarget.Instruction = methodConvert.Nop();         // Exception handling target
        methodConvert.Reverse3();                                  // 10 0 5
        // MAX&MIN costs 1<<3 each; 16 Datoshi more expensive at runtime
        methodConvert.Max();                                       // 10 5
        methodConvert.Min();                                       // 5
        //methodConvert.AddInstruction(OpCode.RET);
        // Alternatively, a slightly cheaper way at runtime; 10 to 16 Datoshi
        //methodConvert.Over();                                    // 10 0 5 0
        //methodConvert.Over();                                    // 10 0 5 0 5
        //methodConvert.Jump(OpCode.JMPGE, minTarget);             // 10 0 5; should return 0 if JMPed
        //methodConvert.Nip();                                     // 10 5
        //methodConvert.Over();                                    // 10 5 10
        //methodConvert.Over();                                    // 10 5 10 5
        //methodConvert.Jump(OpCode.JMPLE, maxTarget);             // 10 5; should return 10 if JMPed
        //methodConvert.Nip();                                     // 5; should return 5
        //methodConvert.Ret();
        //minTarget.Instruction = methodConvert.Nop();             // 10 0 5; should return 0
        //methodConvert.Drop();                                    // 10 0; should return 0
        //methodConvert.Nip();                                     // 0; should return 0
        //methodConvert.Ret();
        //maxTarget.Instruction = methodConvert.Nop();             // 10 5; should return 10
        //methodConvert.Drop();                                    // 10; should return 10
        //methodConvert.Ret();
    }

    /// <summary>
    /// Handles the Math.BigMul method by performing multiplication with overflow checking.
    /// </summary>
    /// <param name="methodConvert">The method converter instance</param>
    /// <param name="model">The semantic model</param>
    /// <param name="symbol">The method symbol</param>
    /// <param name="instanceExpression">The instance expression (if any)</param>
    /// <param name="arguments">The method arguments</param>
    /// <remarks>
    /// Algorithm: Multiplies two values and validates the result is within long range, throws on overflow
    /// </remarks>
    private static void HandleMathBigMul(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        methodConvert.Mul();                                       // Multiply the values
        methodConvert.Dup();                                       // Duplicate result for range check
        methodConvert.Within(long.MinValue, long.MaxValue);     // Check if within long range
        methodConvert.JumpIfTrue( endTarget);            // Jump if within range
        methodConvert.Throw();                                     // Throw if overflow
        endTarget.Instruction = methodConvert.Nop();               // End target
    }

    // RegisterHandler((double x) => Math.Ceiling(x), HandleMathCeiling);
    // RegisterHandler((double x) => Math.Floor(x), HandleMathFloor);
    // RegisterHandler((double x) => Math.Round(x), HandleMathRound);
    // RegisterHandler((double x) => Math.Truncate(x), HandleMathTruncate);
    // RegisterHandler((double x, double y) => Math.Pow(x, y), HandleMathPow);
    // RegisterHandler((double x) => Math.Sqrt(x), HandleMathSqrt);
    // RegisterHandler((double x) => Math.Log(x), HandleMathLog);
    // RegisterHandler((double x, double y) => Math.Log(x, y), HandleMathLogBase);
}
