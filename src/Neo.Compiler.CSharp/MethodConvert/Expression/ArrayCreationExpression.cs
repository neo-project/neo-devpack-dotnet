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

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        private void ConvertArrayCreationExpression(SemanticModel model, ArrayCreationExpressionSyntax expression)
        {
            ArrayRankSpecifierSyntax specifier = expression.Type.RankSpecifiers[0];
            if (specifier.Rank != 1)
                throw new CompilationException(specifier, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {specifier}");
            IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression.Type).Type!;
            if (expression.Initializer is null)
            {
                ConvertExpression(model, specifier.Sizes[0]);
                if (type.ElementType.SpecialType == SpecialType.System_Byte)
                    AddInstruction(OpCode.NEWBUFFER);
                else
                    AddInstruction(new Instruction { OpCode = OpCode.NEWARRAY_T, Operand = new[] { (byte)type.ElementType.GetStackItemType() } });
            }
            else
            {
                ConvertInitializerExpression(model, type, expression.Initializer);
            }
        }
    }
}
