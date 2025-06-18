// Copyright (C) 2015-2025 The Neo Project.
//
// UnaryExpression.PostfixUnary.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
    /// Converts the postfix operator into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the postfix operator.</param>
    /// <param name="expression">The syntax representation of the postfix operator being converted.</param>
    /// <example>
    /// The result of x++ is the value of x before the operation, as the following example shows:
    /// <code>
    /// int i = 3;
    /// Runtime.Log(i.ToString());
    /// Runtime.Log(i++.ToString());
    /// Runtime.Log(i.ToString());
    /// </code>
    /// output: 3、3、4
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators#postfix-increment-operator">Postfix increment operator</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving">! (null-forgiving) operator</seealso>
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
                throw CompilationException.UnsupportedSyntax(expression.OperatorToken, $"Postfix unary operator '{expression.OperatorToken.ValueText}' is not supported. Supported operators are: ++, --, and ! (null-forgiving).");
        }
    }

    private void ConvertPostIncrementOrDecrementExpression(SemanticModel model, PostfixUnaryExpressionSyntax expression)
    {
        switch (expression.Operand)
        {
            case ElementAccessExpressionSyntax operand:
                ConvertElementAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case IdentifierNameSyntax operand:
                ConvertIdentifierNamePostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            case MemberAccessExpressionSyntax operand:
                ConvertMemberAccessPostIncrementOrDecrementExpression(model, expression.OperatorToken, operand);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression, $"Postfix increment/decrement can only be applied to element access, identifiers, or member access expressions. Found: {expression.Operand.GetType().Name}");
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
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            EmitIncrementOrDecrement(operatorToken, property.Type);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.StdCall);
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

    private void ConvertIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IdentifierNameSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(operatorToken, field);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNamePostIncrementOrDecrementExpression(operatorToken, local);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(operatorToken, parameter);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(model, operatorToken, property);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(operand, $"Postfix increment/decrement cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertFieldIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
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
        LdLocSlot(symbol);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        StLocSlot(symbol);
    }

    private void ConvertParameterIdentifierNamePostIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
    {
        LdArgSlot(symbol);
        AddInstruction(OpCode.DUP);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        StArgSlot(symbol);
    }

    private void ConvertPropertyIdentifierNamePostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
    {
        if (!NeedInstanceConstructor(symbol.GetMethod!))
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand)
    {
        ISymbol symbol = model.GetSymbolInfo(operand).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessPostIncrementOrDecrementExpression(model, operatorToken, operand, property);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(operand, $"Postfix increment/decrement cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertFieldMemberAccessPostIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
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
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.DUP);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            AddInstruction(OpCode.TUCK);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }
}
