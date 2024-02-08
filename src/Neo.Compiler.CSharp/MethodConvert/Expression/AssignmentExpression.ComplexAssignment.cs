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
                throw new CompilationException(expression.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment expression: {expression}");
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
            Call(model, property.GetMethod!, CallingConvention.StdCall);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            Call(model, property.SetMethod!, CallingConvention.Cdecl);
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
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
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
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IFieldSymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            byte index = context.AddStaticField(left);
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
        byte index = _localVariables[left];
        AccessSlot(OpCode.LDLOC, index);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        AccessSlot(OpCode.STLOC, index);
    }

    private void ConvertParameterIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IParameterSymbol left, ExpressionSyntax right)
    {
        byte index = _parameters[left];
        AccessSlot(OpCode.LDARG, index);
        ConvertExpression(model, right);
        EmitComplexAssignmentOperator(type, operatorToken);
        AddInstruction(OpCode.DUP);
        AccessSlot(OpCode.STARG, index);
    }

    private void ConvertPropertyIdentifierNameComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, IPropertySymbol left, ExpressionSyntax right)
    {
        if (left.IsStatic)
        {
            Call(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            Call(model, left.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            AddInstruction(OpCode.DUP);
            Call(model, left.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Call(model, left.SetMethod!, CallingConvention.StdCall);
        }
    }

    private void ConvertFieldMemberAccessComplexAssignment(SemanticModel model, ITypeSymbol type, SyntaxToken operatorToken, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
    {
        if (field.IsStatic)
        {
            byte index = context.AddStaticField(field);
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
            Call(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.DUP);
            Call(model, property.SetMethod!);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            Call(model, property.GetMethod!);
            ConvertExpression(model, right);
            EmitComplexAssignmentOperator(type, operatorToken);
            AddInstruction(OpCode.TUCK);
            Call(model, property.SetMethod!, CallingConvention.StdCall);
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
            "|=" => isBoolean ? (OpCode.BOOLOR, false) : (OpCode.OR, true),
            "<<=" => (OpCode.SHL, true),
            ">>=" => (OpCode.SHR, true),
            _ => throw new CompilationException(operatorToken, DiagnosticId.SyntaxNotSupported, $"Unsupported operator: {operatorToken}")
        };
        AddInstruction(opcode);
        if (isString) ChangeType(VM.Types.StackItemType.ByteString);
        if (checkResult) EnsureIntegerInRange(type);
    }
}
