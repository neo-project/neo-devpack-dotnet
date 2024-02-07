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
    /// Converts C# null-coalescing assignment expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The assignment expression syntax node</param>
    /// <remarks>
    /// This handles conversion of null-coalescing assignment syntax like:
    ///
    /// array[0] ??= 10;
    ///
    /// obj.Prop ??= 20;
    ///
    /// Where if the left hand side is null, it is assigned the right hand value.
    ///
    /// The method detects the kind of left hand assignment (array element, property etc)
    /// and emits specialized instructions to implement the null check and assignment.
    ///
    /// For example array element coalescing has specialized logic compared to property
    /// coalescing assignments.
    /// </remarks>
    private void ConvertCoalesceAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.Left)
        {
            case ElementAccessExpressionSyntax left:
                // Example：array[index] ??= "Array Element Default";
                ConvertElementAccessCoalesceAssignment(model, left, expression.Right);
                break;
            case IdentifierNameSyntax left:
                // Example：localVariable ??= "Local Variable Default";
                ConvertIdentifierNameCoalesceAssignment(model, left, expression.Right);
                break;
            case MemberAccessExpressionSyntax left:
                // Example：instance.Property ??= "Property Default";
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

    /// <summary>
    /// Converts identifier name null-coalescing assignments to executable code.
    /// </summary>
    /// <param name="model">The semantic model used for type and symbol resolution.</param>
    /// <param name="left">The identifier name syntax node representing the variable or property on the left hand side of the assignment.</param>
    /// <param name="right">The expression syntax node representing the right hand side of the assignment.</param>
    /// <remarks>
    /// This method facilitates the conversion of null-coalescing assignments involving identifier names, covering various scenarios such as:
    ///
    /// localVariable ??= expression;
    /// this.Property ??= expression;
    /// parameter ??= expression;
    /// field ??= expression;
    ///
    /// Examples:
    ///
    /// 1. Converting a field access:
    ///    int? field = null;
    ///    field ??= 20;
    ///
    /// 2. Converting a local variable:
    ///    int? localVariable = null;
    ///    localVariable ??= 5;
    ///
    /// 3. Converting a parameter:
    ///    void Method(int? parameter = null)
    ///    {
    ///        parameter ??= 30;
    ///    }
    ///
    /// 4. Converting a property access:
    ///    public int? Property { get; set; }
    ///    Property ??= 10;
    ///
    /// In each case, the method checks if the identifier (local variable, property, field, or parameter) is null.
    /// If it is null, the right-hand side value is assigned. Otherwise, the existing value is preserved.
    /// </remarks>
    private void ConvertIdentifierNameCoalesceAssignment(SemanticModel model, IdentifierNameSyntax left, ExpressionSyntax right)
    {
        ISymbol symbol = model.GetSymbolInfo(left).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                // Example：MyClass.MyField ??= "Default Value";
                ConvertFieldIdentifierNameCoalesceAssignment(model, field, right);
                break;
            case ILocalSymbol local:
                // Example：string? localVariable = null; localVariable ??= "Local Default";
                ConvertLocalIdentifierNameCoalesceAssignment(model, local, right);
                break;
            case IParameterSymbol parameter:
                // Example：public void MyMethod(string? parameter = null) { parameter ??= "Parameter Default"; }
                ConvertParameterIdentifierNameCoalesceAssignment(model, parameter, right);
                break;
            case IPropertySymbol property:
                // Example：public string? MyProperty { get; set; }; MyProperty ??= "Property Default";
                ConvertPropertyIdentifierNameCoalesceAssignment(model, property, right);
                break;
            default:
                throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
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
}
