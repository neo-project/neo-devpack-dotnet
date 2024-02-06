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
    private void ConvertAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "=":
                ConvertSimpleAssignmentExpression(model, expression);
                break;
            case "??=":
                ConvertCoalesceAssignmentExpression(model, expression);
                break;
            default:
                ConvertComplexAssignmentExpression(model, expression);
                break;
        }
    }

    private void ConvertSimpleAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        ConvertExpression(model, expression.Right);
        AddInstruction(OpCode.DUP);
        switch (expression.Left)
        {
            case DeclarationExpressionSyntax left:
                ConvertDeclarationAssignment(model, left);
                break;
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessAssignment(model, left);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameAssignment(model, left);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessAssignment(model, left);
                break;
            case TupleExpressionSyntax left:
                ConvertTupleAssignment(model, left);
                break;
            default:
                throw new CompilationException(expression.Left, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment: {expression.Left}");
        }
    }

    private void ConvertDeclarationAssignment(SemanticModel model, DeclarationExpressionSyntax left)
    {
        ITypeSymbol type = model.GetTypeInfo(left).Type!;
        if (!type.IsValueType)
            throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment type: {type}");
        AddInstruction(OpCode.UNPACK);
        AddInstruction(OpCode.DROP);
        foreach (VariableDesignationSyntax variable in ((ParenthesizedVariableDesignationSyntax)left.Designation).Variables)
        {
            switch (variable)
            {
                case SingleVariableDesignationSyntax singleVariableDesignation:
                    ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                    byte index = AddLocalVariable(local);
                    AccessSlot(OpCode.STLOC, index);
                    break;
                case DiscardDesignationSyntax:
                    AddInstruction(OpCode.DROP);
                    break;
                default:
                    throw new CompilationException(variable, DiagnosticId.SyntaxNotSupported, $"Unsupported designation: {variable}");
            }
        }
    }

    private void ConvertElementAccessAssignment(SemanticModel model, ElementAccessExpressionSyntax left)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            ConvertExpression(model, left.Expression);
            Call(model, property.SetMethod!, CallingConvention.Cdecl);
        }
        else
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.ROT);
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertIdentifierNameAssignment(SemanticModel model, IdentifierNameSyntax left)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IDiscardSymbol:
                AddInstruction(OpCode.DROP);
                break;
            case IFieldSymbol field:
                if (field.IsStatic)
                {
                    byte index = context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    AddInstruction(OpCode.LDARG0);
                    Push(index);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                break;
            case ILocalSymbol local:
                AccessSlot(OpCode.STLOC, _localVariables[local]);
                break;
            case IParameterSymbol parameter:
                AccessSlot(OpCode.STARG, _parameters[parameter]);
                break;
            case IPropertySymbol property:
                if (!property.IsStatic) AddInstruction(OpCode.LDARG0);
                Call(model, property.SetMethod!, CallingConvention.Cdecl);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertMemberAccessAssignment(SemanticModel model, MemberAccessExpressionSyntax left)
    {
        ISymbol symbol = model.GetSymbolInfo(left.Name).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsStatic)
                {
                    byte index = context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    ConvertExpression(model, left.Expression);
                    Push(index);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                break;
            case IPropertySymbol property:
                if (!property.IsStatic) ConvertExpression(model, left.Expression);
                Call(model, property.SetMethod!, CallingConvention.Cdecl);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertTupleAssignment(SemanticModel model, TupleExpressionSyntax left)
    {
        AddInstruction(OpCode.UNPACK);
        AddInstruction(OpCode.DROP);
        foreach (ArgumentSyntax argument in left.Arguments)
        {
            switch (argument.Expression)
            {
                case DeclarationExpressionSyntax declaration:
                    switch (declaration.Designation)
                    {
                        case SingleVariableDesignationSyntax singleVariableDesignation:
                            ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(singleVariableDesignation)!;
                            byte index = AddLocalVariable(local);
                            AccessSlot(OpCode.STLOC, index);
                            break;
                        case DiscardDesignationSyntax:
                            AddInstruction(OpCode.DROP);
                            break;
                        default:
                            throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported designation: {argument}");
                    }
                    break;
                case IdentifierNameSyntax identifier:
                    ConvertIdentifierNameAssignment(model, identifier);
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    ConvertMemberAccessAssignment(model, memberAccess);
                    break;
                default:
                    throw new CompilationException(argument, DiagnosticId.SyntaxNotSupported, $"Unsupported assignment: {argument}");
            }
        }
    }

    private void ConvertCoalesceAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.Left)
        {
            case ElementAccessExpressionSyntax left:
                ConvertElementAccessCoalesceAssignment(model, left, expression.Right);
                break;
            case IdentifierNameSyntax left:
                ConvertIdentifierNameCoalesceAssignment(model, left, expression.Right);
                break;
            case MemberAccessExpressionSyntax left:
                ConvertMemberAccessCoalesceAssignment(model, left, expression.Right);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported coalesce assignment: {expression}");
        }
    }

    private void ConvertElementAccessCoalesceAssignment(SemanticModel model, ElementAccessExpressionSyntax left, ExpressionSyntax right)
    {
        if (left.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(left.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {left.ArgumentList.Arguments}");
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (model.GetSymbolInfo(left).Symbol is IPropertySymbol property)
        {
            ConvertExpression(model, left.Expression);
            ConvertExpression(model, left.ArgumentList.Arguments[0].Expression);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            Call(model, property.GetMethod!, CallingConvention.StdCall);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.NIP);
            AddInstruction(OpCode.NIP);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
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
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertIdentifierNameCoalesceAssignment(SemanticModel model, IdentifierNameSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldIdentifierNameCoalesceAssignment(model, field, right);
                break;
            case ILocalSymbol local:
                ConvertLocalIdentifierNameCoalesceAssignment(model, local, right);
                break;
            case IParameterSymbol parameter:
                ConvertParameterIdentifierNameCoalesceAssignment(model, parameter, right);
                break;
            case IPropertySymbol property:
                ConvertPropertyIdentifierNameCoalesceAssignment(model, property, right);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldIdentifierNameCoalesceAssignment(SemanticModel model, IFieldSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (left.IsStatic)
        {
            byte index = context.AddStaticField(left);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDSFLD, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(left.ContainingType.GetFields(), left);
            AddInstruction(OpCode.LDARG0);
            Push(index);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertLocalIdentifierNameCoalesceAssignment(SemanticModel model, ILocalSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        byte index = _localVariables[left];
        AccessSlot(OpCode.LDLOC, index);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, assignmentTarget);
        AccessSlot(OpCode.LDLOC, index);
        Jump(OpCode.JMP_L, endTarget);
        assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        AddInstruction(OpCode.DUP);
        AccessSlot(OpCode.STLOC, index);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertParameterIdentifierNameCoalesceAssignment(SemanticModel model, IParameterSymbol left, ExpressionSyntax right)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        byte index = _parameters[left];
        AccessSlot(OpCode.LDARG, index);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, assignmentTarget);
        AccessSlot(OpCode.LDARG, index);
        Jump(OpCode.JMP_L, endTarget);
        assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertExpression(model, right);
        AddInstruction(OpCode.DUP);
        AccessSlot(OpCode.STARG, index);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertPropertyIdentifierNameCoalesceAssignment(SemanticModel model, IPropertySymbol left, ExpressionSyntax right)
    {
        JumpTarget endTarget = new();
        if (left.IsStatic)
        {
            Call(model, left.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            Call(model, left.SetMethod!);
        }
        else
        {
            AddInstruction(OpCode.LDARG0);
            Call(model, left.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            AddInstruction(OpCode.LDARG0);
            ConvertExpression(model, right);
            AddInstruction(OpCode.TUCK);
            Call(model, left.SetMethod!, CallingConvention.StdCall);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                ConvertFieldMemberAccessCoalesceAssignment(model, left, right, field);
                break;
            case IPropertySymbol property:
                ConvertPropertyMemberAccessCoalesceAssignment(model, left, right, property);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertFieldMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IFieldSymbol field)
    {
        JumpTarget assignmentTarget = new();
        JumpTarget endTarget = new();
        if (field.IsStatic)
        {
            byte index = context.AddStaticField(field);
            AccessSlot(OpCode.LDSFLD, index);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AccessSlot(OpCode.LDSFLD, index);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AccessSlot(OpCode.STSFLD, index);
        }
        else
        {
            int index = Array.IndexOf(field.ContainingType.GetFields(), field);
            ConvertExpression(model, left.Expression);
            Push(index);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.PICKITEM);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.PICKITEM);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.NOP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.REVERSE4);
            AddInstruction(OpCode.REVERSE3);
            AddInstruction(OpCode.SETITEM);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertPropertyMemberAccessCoalesceAssignment(SemanticModel model, MemberAccessExpressionSyntax left, ExpressionSyntax right, IPropertySymbol property)
    {
        JumpTarget endTarget = new();
        if (property.IsStatic)
        {
            Call(model, property.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIFNOT_L, endTarget);
            AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.DUP);
            Call(model, property.SetMethod!);
        }
        else
        {
            JumpTarget assignmentTarget = new();
            ConvertExpression(model, left.Expression);
            AddInstruction(OpCode.DUP);
            Call(model, property.GetMethod!);
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            Jump(OpCode.JMPIF_L, assignmentTarget);
            AddInstruction(OpCode.NIP);
            Jump(OpCode.JMP_L, endTarget);
            assignmentTarget.Instruction = AddInstruction(OpCode.DROP);
            ConvertExpression(model, right);
            AddInstruction(OpCode.TUCK);
            Call(model, property.SetMethod!, CallingConvention.StdCall);
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

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
