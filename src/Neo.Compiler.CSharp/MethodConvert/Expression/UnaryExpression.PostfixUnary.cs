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
using System;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

partial class MethodConvert
{

    /// <summary>
    /// Converts postfix unary operators like x++ and x-- to executable code.
    /// </summary>
    /// <remarks>
    /// Handles syntax like:
    ///
    /// count++
    /// total--
    /// array[i]--
    /// </remarks>
    private void ConvertPostfixUnaryExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "++":
            case "--":
                ConvertPostIncrementOrDecrementExpression(model, expression);
                break;
            case "!":
                ConvertExpression(model, expression.Operand);
                break;
            default:
                throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}");
        }
    }

    private void ConvertPostIncrementOrDecrementExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
    {
        switch (expression.Operand)
        {
            case ElementAccessExpressionSyntax operand:
                // Example: array[i]++
                ConvertElementAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case IdentifierNameSyntax operand:
                // Example: x++
                ConvertIdentifierNamePostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case MemberAccessExpressionSyntax operand:
                // Example: obj.Count++
                ConvertMemberAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported postfix unary expression: {expression}");
        }
    }

    private void ConvertElementAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
    {
        if (operand.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(operand.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {operand.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(operand).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            Call(model, property.GetMethod!, CallingConvention.StdCall);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken, property.Type);
            Call(model, property.SetMethod!, CallingConvention.StdCall);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken, model.GetTypeInfo(operand).Type);
            AddInstruction(OpCode.SETITEM);
        }
    }

    /// <summary>
    /// Converts identifier postfix ++/-- based on symbol type.
    /// </summary>
    /// <remarks>
    /// Dispatches handling based on symbol:
    ///
    /// localVar--
    /// item.Count++
    /// </remarks>
    private void ConvertIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                // Example: item.Count--
                ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(operatorToken, field);
                break;
            case ILocalSymbol local:
                // Example: localVar--
                ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                break;
            case IParameterSymbol parameter:
                // Example: arg--
                ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                break;
            case IPropertySymbol property:
                // Example: item.Count++
                ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(model, operatorToken, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
    {
        byte index = _localVariables[symbol];
        AccessSlot(OpCode.LDLOC, index);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        AccessSlot(OpCode.STLOC, index);
    }

    private void ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
    {
        byte index = _parameters[symbol];
        AccessSlot(OpCode.LDARG, index);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        AccessSlot(OpCode.STARG, index);
    }

    private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            Call(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Call(model, symbol.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Call(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Call(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    /// <summary>
    /// Converts member access postfix ++/-- based on symbol.
    /// </summary>
    /// <remarks>
    /// Member could be static field, instance field, property etc:
    ///
    /// Class.Count--
    /// obj.Size++
    /// </remarks>
    private void ConvertMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                // Example: Class.StaticField--
                ConvertFieldMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, field);
                break;
            case IPropertySymbol property:
                // Example: obj.Count++
                ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            Call(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Call(model, symbol.SetMethod!);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            Call(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            Call(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }
}
