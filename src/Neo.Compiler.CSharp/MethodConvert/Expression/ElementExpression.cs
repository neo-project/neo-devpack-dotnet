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
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts array element access expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The element access expression syntax node</param>
    /// <remarks>
    /// Handles array access or indexer calls like:
    ///
    /// data[i]
    /// vector[x, y]
    ///
    /// Emits code to evaluate array instance and index expression, perform
    /// bounds checking, and access the element value.
    /// </remarks>
    private void ConvertElementAccessExpression(SemanticModel model, ElementAccessExpressionSyntax expression)
    {
        if (expression.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
        if (model.GetSymbolInfo(expression).Symbol is IPropertySymbol property)
        {
            Call(model, property.GetMethod!, expression.Expression, expression.ArgumentList.Arguments.ToArray());
        }
        else
        {
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            ConvertExpression(model, expression.Expression);
            ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
        }
    }

    private void ConvertElementBindingExpression(SemanticModel model, ElementBindingExpressionSyntax expression)
    {
        if (expression.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
    }

    private void ConvertIndexOrRange(SemanticModel model, ITypeSymbol type, ExpressionSyntax indexOrRange)
    {
        if (indexOrRange is RangeExpressionSyntax range)
        {
            if (range.RightOperand is null)
            {
                AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.SIZE);
            }
            else
            {
                ConvertExpression(model, range.RightOperand);
            }
            AddInstruction(OpCode.SWAP);
            if (range.LeftOperand is null)
            {
                Push(0);
            }
            else
            {
                ConvertExpression(model, range.LeftOperand);
            }
            AddInstruction(OpCode.ROT);
            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.SUB);
            switch (type.ToString())
            {
                case "byte[]":
                    AddInstruction(OpCode.SUBSTR);
                    break;
                case "string":
                    AddInstruction(OpCode.SUBSTR);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    break;
                default:
                    throw new CompilationException(indexOrRange, DiagnosticId.ArrayRange, $"The type {type} does not support range access.");
            }
        }
        else
        {
            ConvertExpression(model, indexOrRange);
            AddInstruction(OpCode.PICKITEM);
        }
    }
}
