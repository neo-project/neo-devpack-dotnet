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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using Neo.VM.Types;

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

        if ((expression.IsKind(SyntaxKind.LeftShiftExpression) ||
             expression.IsKind(SyntaxKind.RightShiftExpression)) &&
            HasNullableOperand(model, expression))
        {
            ConvertLiftedShiftExpression(model, expression);
            return;
        }

        ConvertExpression(model, expression.Left);
        ConvertExpression(model, expression.Right);

        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        bool isBoolean = type.GetStackItemType() == StackItemType.Boolean;
        var (opcode, checkResult) = expression.OperatorToken.ValueText switch
        {
            "+" => (OpCode.ADD, true),
            "-" => (OpCode.SUB, true),
            "*" => (OpCode.MUL, true),
            "/" => (OpCode.DIV, false),
            "%" => (OpCode.MOD, false),
            "<<" => (OpCode.SHL, true),
            ">>" => (OpCode.SHR, false),
            "|" => isBoolean ? (OpCode.BOOLOR, false) : (OpCode.OR, false),
            "&" => isBoolean ? (OpCode.BOOLAND, false) : (OpCode.AND, false),
            "^" => (OpCode.XOR, false),
            "==" => (OpCode.EQUAL, false),
            "!=" => (OpCode.NOTEQUAL, false),
            "<" => (OpCode.LT, false),
            "<=" => (OpCode.LE, false),
            ">" => (OpCode.GT, false),
            ">=" => (OpCode.GE, false),
            _ => throw CompilationException.UnsupportedSyntax(expression.OperatorToken, $"Unsupported binary operator '{expression.OperatorToken.ValueText}'. Supported operators: +, -, *, /, %, <<, >>, |, &, ^, ==, !=, <, <=, >, >=, &&, ||")
        };

        if (expression.OperatorToken.ValueText is "/" or "%")
        {
            CheckSignedDivisionOverflow(model, model.GetTypeInfo(expression).Type, expression.Left, expression.Right);
        }
        else if (expression.OperatorToken.ValueText == "<<")
        {
            ITypeSymbol? leftType = model.GetTypeInfo(expression.Left).Type;
            if (!MaskFixedWidthShiftCount(model, leftType, expression.Right))
                CheckLeftShiftOverflow(model, leftType, expression.Right, true);
        }
        else if (expression.OperatorToken.ValueText == ">>")
        {
            MaskFixedWidthShiftCount(model, model.GetTypeInfo(expression.Left).Type, expression.Right);
        }
        AddInstruction(opcode);

        // XOR of two booleans produces an integer (0 or 1).
        // NZ (4 units = 120 datoshi) converts 0 -> false, nonzero -> true,
        // which is correct for boolean results and 2048× cheaper than
        // convert Boolean (8192 units = 245,760 datoshi).
        if (expression.OperatorToken.ValueText == "^" && type.GetStackItemType() == StackItemType.Boolean)
        {
            Nz();
        }

        if (checkResult)
        {
            if (expression.OperatorToken.ValueText == "<<")
                NormalizeShiftResult(type);
            else
                EnsureIntegerInRange(type);
        }
    }

    private static bool HasNullableOperand(SemanticModel model, BinaryExpressionSyntax expression) =>
        IsNullableValueType(model.GetTypeInfo(expression.Left).Type) ||
        IsNullableValueType(model.GetTypeInfo(expression.Right).Type);

    private static bool IsNullableValueType(ITypeSymbol? type) =>
        type is INamedTypeSymbol
        {
            OriginalDefinition.SpecialType: SpecialType.System_Nullable_T
        };

    /// <summary>
    /// Emits a lifted shift while preserving C# null propagation and operand evaluation order.
    /// </summary>
    private void ConvertLiftedShiftExpression(SemanticModel model, BinaryExpressionSyntax expression)
    {
        ITypeSymbol? leftType = model.GetTypeInfo(expression.Left).Type;
        ITypeSymbol? rightType = model.GetTypeInfo(expression.Right).Type;
        ITypeSymbol resultType = model.GetTypeInfo(expression).Type!;
        bool leftNullable = IsNullableValueType(leftType);
        bool rightNullable = IsNullableValueType(rightType);

        JumpTarget? leftNullTarget = leftNullable ? new JumpTarget() : null;
        JumpTarget? rightNullTarget = rightNullable ? new JumpTarget() : null;
        var endTarget = new JumpTarget();

        ConvertExpression(model, expression.Left);
        if (leftNullTarget is not null)
        {
            Dup();
            IsNull();
            JumpIfTrue(leftNullTarget);
        }

        ConvertExpression(model, expression.Right);
        if (rightNullTarget is not null)
        {
            Dup();
            IsNull();
            JumpIfTrue(rightNullTarget);
        }

        if (expression.IsKind(SyntaxKind.LeftShiftExpression))
        {
            if (!MaskFixedWidthShiftCount(model, leftType, expression.Right))
                CheckLeftShiftOverflow(model, leftType, expression.Right, true);
            AddInstruction(OpCode.SHL);
            NormalizeShiftResult(resultType);
        }
        else
        {
            MaskFixedWidthShiftCount(model, leftType, expression.Right);
            AddInstruction(OpCode.SHR);
        }
        Jump(OpCode.JMP_L, endTarget);

        if (rightNullTarget is not null)
        {
            rightNullTarget.Instruction = Nip();
            Jump(OpCode.JMP_L, endTarget);
        }

        if (leftNullTarget is not null)
        {
            leftNullTarget.Instruction = Nop();
            ConvertExpression(model, expression.Right);
            Drop();
        }

        endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Applies the C# shift-count mask for fixed-width integral operands.
    /// </summary>
    /// <remarks>
    /// C# masks the shift count to five bits for operands up to 32 bits and to
    /// six bits for 64-bit operands. Nullable wrappers use their underlying
    /// integral type to select the mask. BigInteger shift counts are not masked.
    /// </remarks>
    private bool MaskFixedWidthShiftCount(SemanticModel model, ITypeSymbol? leftType, ExpressionSyntax rightExpr)
    {
        if (leftType is null) return false;

        if (leftType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType)
        {
            leftType = nullableType.TypeArguments[0];
        }

        int? mask = leftType.SpecialType switch
        {
            SpecialType.System_SByte or
            SpecialType.System_Byte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Char or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 => 31,
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 => 63,
            _ => null
        };

        if (!mask.HasValue) return false;

        if (TryGetIntegerConstant(model, rightExpr, out var shiftAmount))
        {
            if (shiftAmount >= 0 && shiftAmount <= mask.Value) return true;
        }

        Push(mask.Value);
        AddInstruction(OpCode.AND);
        return true;
    }

    private void NormalizeShiftResult(ITypeSymbol type, bool preserveCheckedConversion = false)
    {
        if (preserveCheckedConversion && type.SpecialType is
            SpecialType.System_SByte or
            SpecialType.System_Byte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Char)
        {
            EnsureIntegerInRange(type);
            return;
        }

        _checkedStack.Push(false);
        try
        {
            EnsureIntegerInRange(type);
        }
        finally
        {
            _checkedStack.Pop();
        }
    }

    /// <summary>
    /// Checks for signed division and remainder overflow.
    /// Both operations overflow when the minimum value of a signed integer type is divided by -1.
    /// For example: int.MinValue / -1 would be 2147483648, which exceeds int.MaxValue.
    /// </summary>
    /// <param name="model">The semantic model of the compilation.</param>
    /// <param name="type">The result type of the division or remainder expression.</param>
    /// <param name="leftExpr">The left expression (dividend) of the operation.</param>
    /// <param name="rightExpr">The right expression (divisor) of the operation.</param>
    /// <remarks>
    /// Overflow check is needed for:
    /// - Int32 (int): int.MinValue / -1 overflows
    /// - Int64 (long): long.MinValue / -1 overflows
    ///
    /// Overflow check is NOT needed for:
    /// - Smaller types (sbyte, byte, short, ushort, char): promoted to int in division
    /// - Unsigned types (uint, ulong): no negative values, no overflow possible
    /// - BigInteger: arbitrary precision, no overflow possible
    /// - Constant divisor != -1: overflow only occurs when dividing by -1
    /// - Constant dividend != minValue: overflow only occurs when dividend is the minimum value
    /// </remarks>
    private void CheckSignedDivisionOverflow(SemanticModel model, ITypeSymbol? type, ExpressionSyntax? leftExpr, ExpressionSyntax rightExpr)
    {
        if (type is null) return;
        while (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            // Supporting nullable integer like `byte?`
            type = ((INamedTypeSymbol)type).TypeArguments.First();
        }

        if (leftExpr is not null)
        {
            var dividendType = model.GetTypeInfo(leftExpr).Type?.SpecialType;
            if (dividendType is SpecialType.System_SByte or SpecialType.System_Byte or
                SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Char)
            {
                return;
            }
        }

        if (TryGetIntegerConstant(model, rightExpr, out var rightValue))
        {
            if (rightValue != -1) return; // Just return if right value is not -1.
        }

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

        // If the dividend is a constant other than minValue, overflow is impossible: skip the check.
        if (leftExpr is not null && TryGetIntegerConstant(model, leftExpr, out var dividend) && dividend != minValue.Value)
            return;

        var endTarget = new JumpTarget();
        Dup();
        Push(-1);
        JumpIfNotEqual(endTarget);

        Over();
        Push(minValue.Value);
        JumpIfNotEqual(endTarget);

        RestoreMethodStackDepth();
        Push("Overflow");
        Throw();
        endTarget.Instruction = Nop();
    }

    /// <summary>
    /// Checks for left shift overflow in checked context.
    /// Validates that the shift amount is non-negative and within the bit width of the left operand type.
    /// </summary>
    /// <param name="model">The semantic model of the compilation.</param>
    /// <param name="rightExpr">The right expression of the shift operation.</param>
    /// <param name="leftType">The left type of the shift operation.</param>
    /// <param name="promotedIfSmall">Whether to promote the left type to int if it is a small integer type(less than 32-bits).</param>
    private void CheckLeftShiftOverflow(SemanticModel model, ITypeSymbol? leftType, ExpressionSyntax rightExpr, bool promotedIfSmall)
    {
        // Only check overflow in checked context
        if (!_checkedStack.Peek()) return;

        if (leftType is null) return;

        while (leftType.NullableAnnotation == NullableAnnotation.Annotated)
        {
            leftType = ((INamedTypeSymbol)leftType).TypeArguments.First();
        }

        // Determine the bit width based on the type
        // Note: In NEO, BigInteger is Int256 (256-bit integer)
        var maxShift = leftType.Name switch
        {
            // Integer types that less than 32-bits, it will be promoted to int except compound-assignment operator.
            "SByte" or "Byte" => promotedIfSmall ? 32 : 8,
            "Int16" or "UInt16" or "Char" => promotedIfSmall ? 32 : 16,
            "Int32" or "UInt32" => 32,
            "Int64" or "UInt64" => 64,
            "BigInteger" => 256, // In NEO, BigInteger is Int256
            _ => 32 // Default to 32 for unknown types
        };

        if (TryGetIntegerConstant(model, rightExpr, out var shiftAmount))
        {
            // If shift amount is in range [0, maxShift), no need to check overflow
            if (shiftAmount >= 0 && shiftAmount < maxShift) return;
        }

        var endTarget = new JumpTarget();
        var checkUpperTarget = new JumpTarget();

        // Check if shift amount is negative (top of stack is shift amount)
        Dup();
        Push(0);
        JumpIfGreaterOrEqual(checkUpperTarget);
        Drop(2); // Drop the value and shift-amount
        Push("NegativeShift");
        Throw();

        // Check if shift amount exceeds type bit width
        checkUpperTarget.Instruction = Dup();
        Push(maxShift);
        JumpIfLess(endTarget);
        Drop(2); // Drop the value and shift-amount
        Push("TooLargeShift");
        Throw();

        endTarget.Instruction = Nop();
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
