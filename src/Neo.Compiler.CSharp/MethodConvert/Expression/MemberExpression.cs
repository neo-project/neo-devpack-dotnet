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

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts member access expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The member access expression syntax</param>
    /// <remarks>
    /// Handles expressions accessing fields, properties etc like:
    ///
    /// obj.Field
    /// Class.StaticProp
    ///
    /// Determines symbol type and emits instructions to load/access
    /// the value appropriately. Handles static vs instance members.
    /// </remarks>
    private void ConvertMemberAccessExpression(SemanticModel model, MemberAccessExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    byte index = context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    ConvertExpression(model, expression.Expression);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case IMethodSymbol method:
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                MethodConvert convert = context.ConvertMethod(model, method);
                Jump(OpCode.PUSHA, convert._startTarget);
                break;
            case IPropertySymbol property:
                ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                Call(model, property.GetMethod!, instanceExpression);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    private void ConvertMemberBindingExpression(SemanticModel model, MemberBindingExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                break;
            case IPropertySymbol property:
                Call(model, property.GetMethod!);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
