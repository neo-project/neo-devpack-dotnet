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
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts the code for simple assignment expression into OpCodes.
    /// The assignment operator = assigns the value of its right-hand operand to a variable,
    /// a property, or an indexer element given by its left-hand operand.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about simple assignment expression.</param>
    /// <param name="expression">The syntax representation of the simple assignment expression statement being converted.</param>
    /// <exception cref="CompilationException">Thrown when the syntax is not supported.</exception>
    /// <remarks>
    /// The result of an assignment expression is the value assigned to the left-hand operand.
    /// The type of the right-hand operand must be the same as the type of the left-hand operand or implicitly convertible to it.
    /// </remarks>
    /// <example>
    /// The assignment operator = is right-associative, that is, an expression of the form
    /// <c>a = b = c</c> is evaluated as <c>a = (b = c)</c>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/assignment-operator">Assignment operators</seealso>
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
                    byte index = _context.AddStaticField(field);
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
                // Check if the property is within a constructor and is readonly
                // C# document here https://learn.microsoft.com/en-us/dotnet/csharp/properties
                // example of this syntax:
                // public class Person
                // {
                //     public Person(string firstName) => FirstName = firstName;
                //     // Readonly property
                //     public string FirstName { get; }
                // }
                if (property.SetMethod == null)
                {
                    IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
                    fields = fields.Where(p => !p.IsStatic).ToArray();
                    int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                    AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SETITEM);
                }
                else if (property.SetMethod != null)
                {
                    if (!property.IsStatic) AddInstruction(OpCode.LDARG0);
                    Call(model, property.SetMethod, CallingConvention.Cdecl);
                }
                else
                {
                    throw new CompilationException(left, DiagnosticId.SyntaxNotSupported, $"Property is readonly and not within a constructor: {property.Name}");
                }
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
                    byte index = _context.AddStaticField(field);
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
