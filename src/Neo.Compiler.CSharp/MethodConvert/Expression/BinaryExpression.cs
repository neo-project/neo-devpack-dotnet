// Copyright (C) 2015-2026 The Neo Project.
//
// BinaryExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using System.Linq;
using System.Numerics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// The conditional logical OR operator ||, also known as the "short-circuiting" logical OR operator, computes the logical OR of its operands.
    /// The result of x || y is true if either x or y evaluates to true.
    /// Otherwise, the result is false. If x evaluates to true, y isn't evaluated.
    ///
    /// The conditional logical AND operator &&, also known as the "short-circuiting" logical AND operator, computes the logical AND of its operands.
    /// The result of x && y is true if both x and y evaluate to true.
    /// Otherwise, the result is false. If x evaluates to false, y isn't evaluated.
    ///
    /// The is operator checks if the run-time type of an expression result is compatible with a given type. The is operator also tests an expression result against a pattern.
    ///
    /// The as operator explicitly converts the result of an expression to a given reference or nullable value type. If the conversion isn't possible, the as operator returns null. Unlike a cast expression, the as operator never throws an exception.
    ///
    /// The null-coalescing operator ?? returns the value of its left-hand operand if it isn't null;
    /// otherwise, it evaluates the right-hand operand and returns its result.
    /// The ?? operator doesn't evaluate its right-hand operand if the left-hand operand evaluates to non-null.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about binary expression.</param>
    /// <param name="expression">The syntax representation of the binary expression statement being converted.</param>
    /// <exception cref="CompilationException">If an unsupported operator is encountered</exception>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators">Boolean logical operators - AND, OR</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast">Type-testing operators and cast expressions - is, as</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator">?? operators - the null-coalescing operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/bitwise-and-shift-operators">Bitwise and shift operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators">Arithmetic operators</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/boolean-logical-operators">Boolean logical operators - AND, OR, NOT, XOR</seealso>
    private void ConvertBinaryExpression(SemanticModel model, BinaryExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "||":
                ConvertLogicalOrExpression(model, expression.Left, expression.Right);
                return;
            case "&&":
                ConvertLogicalAndExpression(model, expression.Left, expression.Right);
                return;
            case "is":
                ConvertIsExpression(model, expression.Left, expression.Right);
                return;
            case "as":
                ConvertAsExpression(model, expression.Left, expression.Right);
                return;
            case "??":
                ConvertCoalesceExpression(model, expression.Left, expression.Right);
                return;
        }
        IMethodSymbol? symbol = (IMethodSymbol?)model.GetSymbolInfo(expression).Symbol;
        if (symbol is not null && TryProcessSystemOperators(model, symbol, expression.Left, expression.Right))
            return;
        ConvertExpression(model, expression.Left);
        ConvertExpression(model, expression.Right);
        var (opcode, checkResult) = expression.OperatorToken.ValueText switch
        {
            "+" => (OpCode.ADD, true),
            "-" => (OpCode.SUB, true),
            "*" => (OpCode.MUL, true),
            "/" => (OpCode.DIV, false),
            "%" => (OpCode.MOD, false),
            "<<" => (OpCode.SHL, true),
            ">>" => (OpCode.SHR, false),
            "|" => (OpCode.OR, false),
            "&" => (OpCode.AND, false),
            "^" => (OpCode.XOR, false),
            "==" => (OpCode.EQUAL, false),
            "!=" => (OpCode.NOTEQUAL, false),
            "<" => (OpCode.LT, false),
            "<=" => (OpCode.LE, false),
            ">" => (OpCode.GT, false),
            ">=" => (OpCode.GE, false),
            _ => throw CompilationException.UnsupportedSyntax(expression.OperatorToken, $"Unsupported binary operator '{expression.OperatorToken.ValueText}'. Supported operators: +, -, *, /, %, <<, >>, |, &, ^, ==, !=, <, <=, >, >=, &&, ||")
        };

        if (expression.OperatorToken.ValueText == "/")
        {
            CheckDivideOverflow(model.GetTypeInfo(expression).Type);
        }

        if (expression.OperatorToken.ValueText == "<<")
        {
            CheckShiftOverflow(model, expression);
        }

        AddInstruction(opcode);
        if (checkResult)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            EnsureIntegerInRange(type);
        }
    }

    /// <summary>
    /// Checks for division overflow in checked context.
    /// Division overflow occurs when dividing the minimum value of a signed integer type by -1,
    /// as the result would exceed the maximum value of that type.
    /// For example: int.MinValue / -1 would be 2147483648, which exceeds int.MaxValue.
    /// </summary>
    /// <param name="type">The result type of the division expression.</param>
    /// <remarks>
    /// Overflow check is needed for:
    /// - Int32 (int): int.MinValue / -1 overflows
    /// - Int64 (long): long.MinValue / -1 overflows
    ///
    /// Overflow check is NOT needed for:
    /// - Smaller types (sbyte, byte, short, ushort, char): promoted to int in division
    /// - Unsigned types (uint, ulong): no negative values, no overflow possible
    /// - BigInteger: arbitrary precision, no overflow possible
    /// </remarks>
    private void CheckDivideOverflow(ITypeSymbol? type)
    {
        if (type is null) return;
        while (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            // Supporting nullable integer like `byte?`
            type = ((INamedTypeSymbol)type).TypeArguments.First();
        }

        // Check division overflow if checked statement.
        // In C#, division overflow is checked or not in `unchecked` statement depends on the implementation.
        if (!_checkedStack.Peek()) return;

        // Determine the minimum value based on the type
        // NOTE: short / short -> int, ushort / ushort -> int, char / char -> int,
        // sbyte / sbyte -> int, byte / byte -> int, so overflow check is not needed for small types.
        // Unsigned types (uint, ulong, nuint) cannot overflow in division.
        // BigInteger has arbitrary precision, so no overflow is possible.
        var minValue = type.Name switch
        {
            "Int32" => (System.Numerics.BigInteger)int.MinValue,
            "Int64" => (System.Numerics.BigInteger)long.MinValue,
            _ => (System.Numerics.BigInteger?)null
        };

        // Skip if type doesn't need overflow check
        if (minValue is null) return;

        var endTarget = new JumpTarget();

        AddInstruction(OpCode.DUP);
        Push(-1);
        Jump(OpCode.JMPNE_L, endTarget);

        AddInstruction(OpCode.OVER);
        Push(minValue.Value);
        Jump(OpCode.JMPNE_L, endTarget);

        AddInstruction(OpCode.THROW);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    /// <summary>
    /// Checks for left shift overflow in checked context.
    /// Validates that the shift amount is non-negative and within the bit width of the left operand type.
    /// </summary>
    /// <param name="model">The semantic model.</param>
    /// <param name="expression">The binary expression containing the shift operation.</param>
    private void CheckShiftOverflow(SemanticModel model, BinaryExpressionSyntax expression)
    {
        // Only check overflow in checked context
        if (!_checkedStack.Peek()) return;

        var leftType = model.GetTypeInfo(expression.Left).Type;
        if (leftType is null) return;

        while (leftType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            leftType = ((INamedTypeSymbol)leftType).TypeArguments.First();
        }

        // Determine the bit width based on the type
        // Note: In NEO, BigInteger is Int256 (256-bit integer)
        var maxShift = leftType.Name switch
        {
            "SByte" or "Byte" => 8,
            "Int16" or "UInt16" or "Char" => 16,
            "Int32" or "UInt32" => 32,
            "Int64" or "UInt64" => 64,
            "BigInteger" => 256, // In NEO, BigInteger is Int256
            _ => 32 // Default to 32 for unknown types
        };

        var endTarget = new JumpTarget();
        var checkUpperTarget = new JumpTarget();

        // Check if shift amount is negative (top of stack is shift amount)
        AddInstruction(OpCode.DUP);
        Push(0);
        Jump(OpCode.JMPGE_L, checkUpperTarget);
        AddInstruction(OpCode.THROW);

        // Check if shift amount exceeds type bit width
        checkUpperTarget.Instruction = AddInstruction(OpCode.DUP);
        Push(maxShift);
        Jump(OpCode.JMPLT_L, endTarget);
        AddInstruction(OpCode.THROW);

        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertLogicalOrExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        Jump(OpCode.JMPIFNOT_L, rightTarget);
        Push(true);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertLogicalAndExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget rightTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        Jump(OpCode.JMPIF_L, rightTarget);
        Push(false);
        Jump(OpCode.JMP_L, endTarget);
        rightTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertIsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        ITypeSymbol type = model.GetTypeInfo(right).Type!;
        ConvertExpression(model, left);
        IsType(type.GetPatternType());
    }

    private void ConvertAsExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        ITypeSymbol type = model.GetTypeInfo(right).Type!;
        ConvertExpression(model, left);
        AddInstruction(OpCode.DUP);
        IsType(type.GetPatternType());
        Jump(OpCode.JMPIF_L, endTarget);
        AddInstruction(OpCode.DROP);
        Push((object?)null);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertCoalesceExpression(SemanticModel model, ExpressionSyntax left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        ConvertExpression(model, left);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIFNOT_L, endTarget);
        AddInstruction(OpCode.DROP);
        ConvertExpression(model, right);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
