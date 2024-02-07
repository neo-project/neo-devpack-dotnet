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

namespace Neo.Compiler
{
    partial class MethodConvert
    {

        /// <summary>
        /// Converts a simple assignment expression into executable instructions.
        /// </summary>
        /// <param name="model">The semantic model for resolving types and symbols within the current context.</param>
        /// <param name="expression">The assignment expression syntax node to be converted.</param>
        /// <remarks>
        /// This method processes a simple assignment expression by evaluating the expression on the right-hand side and preparing
        /// it for assignment to the target on the left-hand side. It supports various forms of the left-hand side expressions,
        /// including declarations, element accesses, identifiers, member accesses, and tuples. Depending on the type of the left-hand
        /// side expression, specific conversion methods are invoked.
        ///
        /// Examples for each case:
        /// - Declaration: Handles variable declarations with assignment, e.g., <code>var x = value;</code>
        /// - ElementAccess: Handles assignments to array or collection elements, e.g., <code>array[index] = value;</code>
        /// - IdentifierName: Handles assignments to variables or fields, e.g., <code>variable = value;</code>
        /// - MemberAccess: Handles assignments to object properties, e.g., <code>object.Property = value;</code>
        /// - Tuple: Handles tuple deconstruction assignments, e.g., <code>(x, y) = (1, 2);</code>
        /// </remarks>
        private void ConvertSimpleAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
        {
            ConvertExpression(model, expression.Right);
            AddInstruction(OpCode.DUP);
            switch (expression.Left)
            {
                case DeclarationExpressionSyntax left:
                    // Example: var x = value;
                    // Processes variable declaration and assignment.
                    ConvertDeclarationAssignment(model, left);
                    break;
                case ElementAccessExpressionSyntax left:
                    // Example: array[index] = value;
                    ConvertElementAccessAssignment(model, left);
                    break;
                case IdentifierNameSyntax left:
                    // Example: variable = value;
                    ConvertIdentifierNameAssignment(model, left);
                    break;
                case MemberAccessExpressionSyntax left:
                    // Example: object.Property = value;
                    ConvertMemberAccessAssignment(model, left);
                    break;
                case TupleExpressionSyntax left:
                    // Example: (x, y) = (1, 2);
                    ConvertTupleAssignment(model, left);
                    break;
                default:
                    throw new CompilationException(expression.Left, DiagnosticId.SyntaxNotSupported,
                        $"Unsupported assignment: {expression.Left}");
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
    }
}
