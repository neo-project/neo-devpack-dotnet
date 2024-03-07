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
    /// This method converts an element access expression to OpCodes.
    /// An element access expression accesses a single element in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element access expression.</param>
    /// <param name="expression">The syntax representation of the element access expression statement being converted.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// If the accessed element is a property, the method calls the property's getter.
    /// If the accessed element is an array or a collection, the method generates OpCodes to fetch the element.
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

    /// <summary>
    /// This method converts an element binding expression to OpCodes.
    /// An element binding expression is used to access a single element in a collection initializer.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element binding expression.</param>
    /// <param name="expression">The syntax representation of the element binding expression statement being converted.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// The method generates OpCodes to fetch the element based on the given index or range.
    /// </remarks>
    private void ConvertElementBindingExpression(SemanticModel model, ElementBindingExpressionSyntax expression)
    {
        if (expression.ArgumentList.Arguments.Count != 1)
            throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
    }

    /// <summary>
    /// This method converts an index or range expression to OpCodes.
    /// An index or range expression specifies the index or range of elements to access in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing contextual information for the expression.</param>
    /// <param name="type">The type symbol of the array or collection being accessed.</param>
    /// <param name="indexOrRange">The expression representing the index or range.</param>
    /// <exception cref="CompilationException">Only one-dimensional arrays are supported, otherwise an exception is thrown.</exception>
    /// <remarks>
    /// If the expression is a range, it calculates the start and end indices and extracts the relevant sub-array or sub-collection.
    /// If the expression is an index, it generates OpCodes to fetch the element at the specified index.
    /// </remarks>
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
