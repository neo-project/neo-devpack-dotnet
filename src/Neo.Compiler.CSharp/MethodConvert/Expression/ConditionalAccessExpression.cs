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

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts C# conditional access expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The conditional access expression syntax node</param>
    /// <remarks>
    /// This handles conversion of syntax like:
    ///
    /// obj?.Prop
    ///
    /// Where null conditional operator allows access to members of potentially null objects.
    ///
    /// The method emits logic to evaluate the target object, check if its null, and either
    /// access the member or propagate null based on that check.
    /// </remarks>
    private void ConvertConditionalAccessExpression(SemanticModel model, ConditionalAccessExpressionSyntax expression)
    {
        ITypeSymbol type = model.GetTypeInfo(expression).Type!;
        JumpTarget nullTarget = new();
        ConvertExpression(model, expression.Expression);
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, nullTarget);
        ConvertExpression(model, expression.WhenNotNull);
        if (type.SpecialType == SpecialType.System_Void)
        {
            JumpTarget endTarget = new();
            Jump(OpCode.JMP_L, endTarget);
            nullTarget.Instruction = AddInstruction(OpCode.DROP);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        else
        {
            nullTarget.Instruction = AddInstruction(OpCode.NOP);
        }
    }
}
