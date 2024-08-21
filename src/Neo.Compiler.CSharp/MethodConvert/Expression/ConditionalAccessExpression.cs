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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts a null-conditional access expression to OpCodes.
    /// </summary>
    /// /// <param name="model">The semantic model providing context and information about null-conditional access expression.</param>
    /// <param name="expression">The syntax representation of the null-conditional access expression statement being converted.</param>
    /// <remarks>
    /// The method evaluates the expression and checks if it is null.
    /// If the expression is not null, it converts the 'WhenNotNull' part of the expression.
    /// If the resulting type of the expression is 'System.Void', it handles the case differently by dropping the result.
    /// A null-conditional operator applies a member access (?.) or element access (?[]) operation to its operand only if that operand evaluates to non-null;
    /// otherwise, it returns null.
    /// It will jump to <see cref="ConvertMemberBindingExpression"/> and <see cref="ConvertElementBindingExpression"/> to handle the case where the variable or array is not null.
    /// </remarks>
    /// <example>
    /// If Block is not null, get the block's timestamp; otherwise, it returns null.
    /// <code>
    /// var block = Ledger.GetBlock(10000);
    /// var timestamp = block?.Timestamp;
    /// Runtime.Log(timestamp.ToString());
    /// </code>
    /// If array is not null, get the array's element; otherwise, it returns null.
    /// <code>
    /// var a = Ledger.GetBlock(10000);
    /// var b = Ledger.GetBlock(10001);
    /// var array = new[] { a, b };
    /// var firstItem = array?[0];
    /// Runtime.Log(firstItem?.Timestamp.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-">Null-conditional operators ?. and ?[]</seealso>
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
