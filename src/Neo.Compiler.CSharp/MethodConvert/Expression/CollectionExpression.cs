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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertCollectionExpression(SemanticModel model, CollectionExpressionSyntax expression)
    {
        // IArrayTypeSymbol type = (IArrayTypeSymbol) model.GetTypeInfo(expression).ConvertedType!;
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
