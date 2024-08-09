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

partial class MethodConvert
{
    private void ConvertCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        var typeSymbol = model.GetTypeInfo(expression).ConvertedType;

        if (typeSymbol is IArrayTypeSymbol arrayType && arrayType.ElementType.SpecialType == SpecialType.System_Byte)
        {
            Optional<object?>[] values = expression.Elements
                .Select(p => p is ExpressionElementSyntax exprElement ? model.GetConstantValue(exprElement.Expression) : default)
                .ToArray();

            if (values.Any(p => !p.HasValue))
            {
                Push(values.Length);
                AddInstruction(OpCode.NEWBUFFER);
                for (int i = 0; i < expression.Elements.Count; i++)
                {
                    AddInstruction(OpCode.DUP);
                    Push(i);
                    if (expression.Elements[i] is ExpressionElementSyntax exprElement)
                    {
                        ConvertExpression(model, exprElement.Expression);
                    }
                    else
                    {
                        throw new NotSupportedException($"Unsupported collection element type: {expression.Elements[i].GetType()}");
                    }
                    AddInstruction(OpCode.SETITEM);
                }
            }
            else
            {
                byte[] data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
                Push(data);
                ChangeType(VM.Types.StackItemType.Buffer);
            }

            return;
        }

        for (int i = expression.Elements.Count - 1; i >= 0; i--)
        {
            var element = expression.Elements[i];
            switch (element)
            {
                case ExpressionElementSyntax expressionElementSyntax:
                    ConvertExpression(model, expressionElementSyntax.Expression);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported collection element type: {element.GetType()}");
            }
        }
        Push(expression.Elements.Count);
        AddInstruction(OpCode.PACK);
    }
}
