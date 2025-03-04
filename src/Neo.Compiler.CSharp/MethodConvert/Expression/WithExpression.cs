// Copyright (C) 2015-2025 The Neo Project.
//
// WithExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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

internal partial class MethodConvert
{
    /// <summary>
    /// Convert record with expression: record with{...InitializerExpression}
    /// </summary>
    /// <param name="model"></param>
    /// <param name="expression"></param>
    private void ConvertWithExpressionSyntax(SemanticModel model, WithExpressionSyntax expression)
    {
        //load record
        ConvertExpression(model, expression.Expression);
        //clone record struct
        AddInstruction(new Instruction { OpCode = OpCode.UNPACK });
        AddInstruction(new Instruction { OpCode = OpCode.PACKSTRUCT });
        //convert InitializerExpression
        ConvertObjectCreationExpressionInitializer(model, expression.Initializer);
    }
}
