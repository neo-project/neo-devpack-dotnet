// Copyright (C) 2015-2025 The Neo Project.
//
// UnaryExpression.PrefixUnary.cs file belongs to the neo project and is free
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
using System.Linq;

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
                EmitNegativeInteger(model.GetTypeInfo(expression.Operand).Type);
                break;
            case "~":
                ConvertExpression(model, expression.Operand);
                AddInstruction(OpCode.INVERT);
                break;
            case "!":
                ConvertExpression(model, expression.Operand);
                AddInstruction(OpCode.NOT);
                break;
            case "++":
            case "--":
                ConvertPreIncrementOrDecrementExpression(model, expression);
                break;
            case "^":
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.SIZE);
                ConvertExpression(model, expression.Operand);
                AddInstruction(OpCode.SUB);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression.OperatorToken, $"Prefix unary operator '{expression.OperatorToken.ValueText}' is not supported. Supported operators are: +, -, ~, !, ++, --, and ^.");
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
                throw CompilationException.UnsupportedSyntax(expression, $"Prefix increment/decrement can only be applied to element access, identifiers, or member access expressions. Found: {expression.Operand.GetType().Name}");
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
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            EmitIncrementOrDecrement(operatorToken, property.Type);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            ConvertExpression(model, operand.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            EmitIncrementOrDecrement(operatorToken, model.GetTypeInfo(operand).Type);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
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
                throw CompilationException.UnsupportedSyntax(operand, $"Prefix increment/decrement cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertFieldIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertLocalIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, ILocalSymbol symbol)
    {
        LdLocSlot(symbol);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        AddInstruction(OpCode.DUP);
        StLocSlot(symbol);
    }

    private void ConvertParameterIdentifierNamePreIncrementOrDecrementExpression(SyntaxToken operatorToken, IParameterSymbol symbol)
    {
        LdArgSlot(symbol);
        EmitIncrementOrDecrement(operatorToken, symbol.Type);
        AddInstruction(OpCode.DUP);
        StArgSlot(symbol);
    }

    private void ConvertPropertyIdentifierNamePreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.TUCK);
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
                throw CompilationException.UnsupportedSyntax(operand, $"Prefix increment/decrement cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertFieldMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IFieldSymbol symbol)
    {
        if (symbol.IsStatic)
        {
            byte index = _context.AddStaticField(symbol);
            AccessSlot(OpCode.LDSFLD, index);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(symbol.ContainingType.GetFields(), symbol);
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertPropertyMemberAccessPreIncrementOrDecrementExpression(SemanticModel model, SyntaxToken operatorToken, MemberAccessExpressionSyntax operand, IPropertySymbol symbol)
    {
        if (symbol.IsStatic)
        {
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.SetMethod!);
        }
        else
        {
            ConvertExpression(model, operand.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, symbol.GetMethod!);
            EmitIncrementOrDecrement(operatorToken, symbol.Type);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, symbol.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void EmitIncrementOrDecrement(SyntaxToken operatorToken, ITypeSymbol? typeSymbol)
    {
        AddInstruction(operatorToken.ValueText switch
        {
            "++" => OpCode.INC,
            "--" => OpCode.DEC,
            _ => throw CompilationException.UnsupportedSyntax(operatorToken, $"Invalid increment/decrement operator '{operatorToken.ValueText}'. Only '++' and '--' are supported.")
        });
        if (typeSymbol != null) EnsureIntegerInRange(typeSymbol);
    }

    private void EmitNegativeInteger(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol is null) return;
        while (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            // Supporting nullable integer like `byte?`
            typeSymbol = ((INamedTypeSymbol)typeSymbol).TypeArguments.First();
        }

        if (typeSymbol.Name != "Int32" && typeSymbol.Name != "Int64")
        {
            //  -sbyte, -byte, -short, -ushort, -char -> int, -int, -uint -> long
            AddInstruction(OpCode.NEGATE); // Emit NEGATE for other integer types
            return;
        }

        var minValue = typeSymbol.Name == "Int64" ? long.MinValue : int.MinValue; // int32 or int64

        JumpTarget negateTarget = new(), endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(minValue);
        Jump(OpCode.JMPNE_L, negateTarget);

        if (_checkedStack.Peek()) // if `checked` is true, throw exception
        {
            AddInstruction(OpCode.THROW);
        }
        else // -int.MinValue == -int.MinValue, -long.MinValue == -long.MinValue, i.e. same value
        {
            Jump(endTarget);
        }
        negateTarget.Instruction = AddInstruction(OpCode.NEGATE);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }
}
