// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts a collection expression to the appropriate NeoVM instructions.
    /// Determines if the collection is a byte array or a generic collection and calls the appropriate conversion method.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to convert.</param>
    private void ConvertCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        var typeSymbol = model.GetTypeInfo(expression).ConvertedType;

        // Byte array is considered Buffer type in NeoVM
        if (IsArrayOfBytes(typeSymbol))
        {
            ConvertByteArrayExpression(model, expression);
        }
        else
        {
            ConvertGenericCollectionExpression(model, expression);
        }
    }

    /// <summary>
    /// Determines if the given type symbol represents an array of bytes.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to check.</param>
    /// <returns>True if the type is an array of bytes, false otherwise.</returns>
    private static bool IsArrayOfBytes(ITypeSymbol? typeSymbol)
    {
        return typeSymbol is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte };
    }

    /// <summary>
    /// Converts a byte array expression to NeoVM instructions.
    /// Determines if the array contains constant values or not and calls the appropriate conversion method.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax representing a byte array.</param>
    private void ConvertByteArrayExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        var values = GetConstantValues(model, expression);

        if (values.Any(p => !p.HasValue))
        {
            ConvertNonConstantByteArray(model, expression, values.Length);
        }
        else
        {
            ConvertConstantByteArray(values);
        }
    }

    /// <summary>
    /// Extracts constant values from a collection expression, if possible.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to extract values from.</param>
    /// <returns>An array of optional objects representing the constant values, or default if not constant.</returns>
    private static Optional<object?>[] GetConstantValues(SemanticModel model, CollectionExpressionSyntax expression)
    {
        return expression.Elements
            .Select(p => p is ExpressionElementSyntax exprElement ? model.GetConstantValue(exprElement.Expression) : default)
            .ToArray();
    }

    /// <summary>
    /// Converts a non-constant byte array to NeoVM instructions.
    /// Creates a new buffer and sets each element individually.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax representing a non-constant byte array.</param>
    /// <param name="length">The length of the array.</param>
    private void ConvertNonConstantByteArray(SemanticModel model, CollectionExpressionSyntax expression, int length)
    {
        Push(length);
        AddInstruction(OpCode.NEWBUFFER);
        for (var i = 0; i < expression.Elements.Count; i++)
        {
            AddInstruction(OpCode.DUP);
            Push(i);
            ConvertElement(model, expression.Elements[i]);
            AddInstruction(OpCode.SETITEM);
        }
    }

    /// <summary>
    /// Converts a single element of a collection expression to NeoVM instructions.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="element">The collection element syntax to convert.</param>
    private void ConvertElement(SemanticModel model, CollectionElementSyntax element)
    {
        if (element is ExpressionElementSyntax exprElement)
        {
            ConvertExpression(model, exprElement.Expression);
        }
        else
        {
            throw new NotSupportedException($"Unsupported collection element type: {element.GetType()}");
        }
    }

    /// <summary>
    /// Converts a constant byte array to NeoVM instructions.
    /// Creates a buffer directly from the constant values.
    /// </summary>
    /// <param name="values">An array of optional objects representing the constant byte values.</param>
    private void ConvertConstantByteArray(Optional<object?>[] values)
    {
        var data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
        Push(data);
        ChangeType(VM.Types.StackItemType.Buffer);
    }

    /// <summary>
    /// Converts a generic collection expression to NeoVM instructions.
    /// Creates an array by pushing elements onto the stack and then packing them.
    /// </summary>
    /// <param name="model">The semantic model of the code being analyzed.</param>
    /// <param name="expression">The collection expression syntax to convert.</param>
    private void ConvertGenericCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        for (var i = expression.Elements.Count - 1; i >= 0; i--)
        {
            ConvertElement(model, expression.Elements[i]);
        }
        Push(expression.Elements.Count);
        AddInstruction(OpCode.PACK);
    }
}
