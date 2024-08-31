// Copyright (C) 2015-2024 The Neo Project.
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

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the prefix operator into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the prefix operator.</param>
    /// <param name="expression">The syntax representation of the prefix operator being converted.</param>
    /// <example>
    /// The result of ++x is the value of x before the operation, as the following example shows:
    /// <code>
    /// int i = 3;
    /// Runtime.Log(i.ToString());
    /// Runtime.Log(++i.ToString());
    /// Runtime.Log(i.ToString());
    /// </code>
    /// output: 3、4、4
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators#prefix-increment-operator">Prefix increment operator</seealso>
    private void ConvertPrefixUnaryExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "+":
                ConvertExpression(model, expression.Operand);
                break;
            case "-":
                ConvertExpression(model, expression.Operand);
                _instructionsBuilder.Negate();
                break;
            case "~":
                ConvertExpression(model, expression.Operand);
                _instructionsBuilder.Invert();
                break;
            case "!":
                ConvertExpression(model, expression.Operand);
                _instructionsBuilder.Not();
                break;
            case "++":
            case "--":
                ConvertPreIncrementOrDecrementExpression(model, expression);
                break;
            case "^":
                _instructionsBuilder.Dup();
                _instructionsBuilder.Size();
                ConvertExpression(model, expression.Operand);
                _instructionsBuilder.Sub();
                break;
            default:
                throw new CompilationException(expression.OperatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {expression.OperatorToken}");
        }
    }

    private void ConvertPreIncrementOrDecrementExpression(SemanticModel model, PrefixUnaryExpressionSyntax expression)
    {
        switch (expression.Operand)
        {
            case ElementAccessExpressionSyntax operand:
                ConvertElementAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case IdentifierNameSyntax operand:
                ConvertIdentifierNamePreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case MemberAccessExpressionSyntax operand:
                ConvertMemberAccessPreIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported postfix unary expression: {expression}");
        }
    }

    private void ConvertElementAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, ElementAccessExpressionSyntax operand)
    {
        if (operand.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(operand.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {operand.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(operand).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            _instructionsBuilder.Over();
            _instructionsBuilder.Over();
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            EmitIncrementOrDecrement(operatorToken, property.Type);
            _instructionsBuilder.Dup();
            _instructionsBuilder.Reverse4();
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            _instructionsBuilder.Over();
            _instructionsBuilder.Over();
            _instructionsBuilder.PickItem();
            EmitIncrementOrDecrement(operatorToken, model.GetTypeInfo(operand).Type);
            _instructionsBuilder.Dup();
            _instructionsBuilder.Reverse4();
            _instructionsBuilder.Reverse3();
            _instructionsBuilder.SetItem();
        }
    }

    private void ConvertIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(operatorToken, field);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(operatorToken, local);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(operatorToken, parameter);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(model, operatorToken, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = Context.AddStaticField(symbol);
            _instructionsBuilder.LdSFld(index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Dup();
            _instructionsBuilder.StSFld(index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            _instructionsBuilder.LdArg0();
            _instructionsBuilder.Dup();
            _instructionsBuilder.PickItem(index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Tuck();
            _instructionsBuilder.Push(index);
            _instructionsBuilder.Swap();
            _instructionsBuilder.SetItem();
        }
    }

    private void ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
    {
        LdLocSlot(symbol);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        _instructionsBuilder.Dup();
        StLocSlot(symbol);
    }

    private void ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
    {
        LdArgSlot(symbol);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        _instructionsBuilder.Dup();
        StArgSlot(symbol);
    }

    private void ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Dup();
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            _instructionsBuilder.LdArg0();
            _instructionsBuilder.Dup();
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Tuck();
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(model, operatorToken, operand, property);
                break;
            default:
                throw new CompilationException(operand, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = Context.AddStaticField(symbol);
            _instructionsBuilder.LdSFld(index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Dup();
            _instructionsBuilder.StSFld(index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            ConvertExpression(model, operand.Expression);
            _instructionsBuilder.Dup();
            _instructionsBuilder.PickItem(index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Tuck();
            _instructionsBuilder.Push(index);
            _instructionsBuilder.Swap();
            _instructionsBuilder.SetItem();
        }
    }

    private void ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Dup();
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            _instructionsBuilder.Dup();
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            _instructionsBuilder.Tuck();
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void EmitIncrementOrDecrement(SyntaxToken operatorToken, ITypeSymbol? typeSymbol)
    {
        _instructionsBuilder.AddInstruction(operatorToken.ValueText switch
        {
            "++" => OpCode.INC,
            "--" => OpCode.DEC,
            _ => throw new CompilationException(operatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {operatorToken}")
        });
        if (typeSymbol != null) EnsureIntegerInRange(typeSymbol);
    }
}
