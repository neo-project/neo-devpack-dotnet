// Copyright (C) 2015-2025 The Neo Project.
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

        AddInstruction(opcode);
        if (checkResult)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            EnsureIntegerInRange(type);
        }
    }

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

        // Only check overflow for int32 and int64
        // NOTE: short / short -> int, ushort / ushort -> int, char / char -> int,
        // sbyte / sbyte -> int, byte / byte -> int, so overflow check is not needed.
        if (type.Name != "Int32" && type.Name != "Int64") return;

        var minValue = type.Name == "Int64" ? long.MinValue : int.MinValue;
        var endTarget = new JumpTarget();

        AddInstruction(OpCode.DUP);
        Push(-1);
        Jump(OpCode.JMPNE_L, endTarget);

        AddInstruction(OpCode.OVER);
        Push(minValue);
        Jump(OpCode.JMPNE_L, endTarget);

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
