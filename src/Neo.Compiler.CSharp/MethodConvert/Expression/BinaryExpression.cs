// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;
partial class MethodConvert
{
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
        (OpCode opcode, bool checkResult) = expression.OperatorToken.ValueText switch
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
            _ => throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}")
        };
        AddInstruction(opcode);
        if (checkResult)
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            EnsureIntegerInRange(type);
        }
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
