// Copyright (C) 2015-2025 The Neo Project.
//
// AssignmentExpression.ComplexAssignment.cs file belongs to the neo project and is free
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
    /// Converts the code for complex assignment (or compound assignment) expression into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about complex assignment expression.</param>
    /// <param name="expression">The syntax representation of the complex assignment expression statement being converted.</param>
    /// <exception cref="CompilationException">Thrown when the syntax is not supported.</exception>
    /// <remarks>
    /// For a binary operator op, a compound assignment expression of the form "x op= y" is equivalent to "x = x op y" except that x is only evaluated once.
    /// </remarks>
    /// <example>
    /// The following example demonstrates the usage of compound assignment with arithmetic operators:
    /// The corresponding code branch is "ConvertComplexAssignmentExpression"
    /// <code>
    /// int a = 5;
    /// a += 9;
    /// Runtime.Log(a.ToString());
    /// a -= 4;
    /// Runtime.Log(a.ToString());
    /// a *= 2;
    /// Runtime.Log(a.ToString());
    /// a /= 4;
    /// Runtime.Log(a.ToString());
    /// a %= 3;
    /// Runtime.Log(a.ToString());
    /// </code>
    /// output: 14, 10, 20, 5, 2
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/assignment-operator#compound-assignment">Compound assignment</seealso>
    private void ConvertComplexAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        switch (expression.Left)
        {
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessComplexAssignment(model, type, expression.OperatorToken, left, expression.Right);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression.Left, $"Complex assignment operators (+=, -=, etc.) can only be used with element access, identifiers, or member access expressions. Found: {expression.Left.GetType().Name}");
        }
    }

    private void ConvertElementAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ElementAccessExpressionSyntax left, ExpressionSyntax right)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            CallMethodWithConvention(model, property.GetMethod!, CallingConvention.StdCall);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IdentifierNameSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNameComplexAssignment(model, type, operatorToken, field, right);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNameComplexAssignment(model, type, operatorToken, local, right);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNameComplexAssignment(model, type, operatorToken, parameter, right);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNameComplexAssignment(model, type, operatorToken, property, right);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(left, $"Complex assignment operators cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessComplexAssignment(model, type, operatorToken, left, right, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessComplexAssignment(model, type, operatorToken, left, right, property);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(left, $"Complex assignment operators cannot be applied to symbol type '{symbol.GetType().Name}'. Only fields, locals, parameters, and properties are supported.");
        }
    }

    private void ConvertFieldIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IFieldSymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            byte index = _context.AddStaticField(left);
            AccessSlot(OpCode.LDSFLD, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(left.ContainingType.GetFields(), left);
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertLocalIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, ILocalSymbol left, ExpressionSyntax right)
    {
        LdLocSlot(left);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        StLocSlot(left);
    }

    private void ConvertParameterIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IParameterSymbol left, ExpressionSyntax right)
    {
        LdArgSlot(left);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        StArgSlot(left);
    }

    private void ConvertPropertyIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IPropertySymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            CallMethodWithConvention(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, left.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, left.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertFieldMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
    {
        if (field.IsStatic)
        {
            byte index = _context.AddStaticField(field);
            AccessSlot(OpCode.LDSFLD, index);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(field.ContainingType.GetFields(), field);
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            Push(index);
            AddInstruction(OpCode.PICKITEM);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Push(index);
            AddInstruction(OpCode.SWAP);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertPropertyMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
    {
        if (property.IsStatic)
        {
            CallMethodWithConvention(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.SetMethod!);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            CallMethodWithConvention(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            CallMethodWithConvention(model, property.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void EmitComplexAssignmentOperator(ITypeSymbol type, SyntaxToken operatorToken)
    {
        var itemType = type.GetStackItemType();
        bool isBoolean = itemType == VM.Types.StackItemType.Boolean;
        bool isString = itemType == VM.Types.StackItemType.ByteString;

        var (opcode, checkResult) = operatorToken.ValueText switch
        {
            "+=" => isString ? (OpCode.CAT, false) : (OpCode.ADD, true),
            "-=" => (OpCode.SUB, true),
            "*=" => (OpCode.MUL, true),
            "/=" => (OpCode.DIV, true),
            "%=" => (OpCode.MOD, true),
            "&=" => isBoolean ? (OpCode.BOOLAND, false) : (OpCode.AND, true),
            "^=" when !isBoolean => (OpCode.XOR, true),
            "^=" when isBoolean => (OpCode.XOR, false),
            "|=" => isBoolean ? (OpCode.BOOLOR, false) : (OpCode.OR, true),
            "<<=" => (OpCode.SHL, true),
            ">>=" => (OpCode.SHR, true),
            _ => throw CompilationException.UnsupportedSyntax(operatorToken, $"Complex assignment operator '{operatorToken.ValueText}' is not supported. Supported operators are: +=, -=, *=, /=, %=, &=, |=, ^=, <<=, and >>=.")
        };
        AddInstruction(opcode);
        if (opcode == OpCode.XOR && isBoolean)
            ChangeType(VM.Types.StackItemType.Boolean);
        if (isString) ChangeType(VM.Types.StackItemType.ByteString);
        if (checkResult) EnsureIntegerInRange(type);
    }
}
