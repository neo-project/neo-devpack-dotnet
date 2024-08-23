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
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts initialization of array fields into OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about initialization of array fields expression.</param>
    /// <param name="expression">The syntax representation of the initialization of array fields expression statement being converted.</param>
    /// <example>
    /// The following 4 static fields will each be converted in this method.
    /// <code>
    /// static string[] A = { "BTC", "NEO", "GAS" };
    /// static int[] B = { 1, 2 };
    /// static byte[] C = { 1, 2 };
    /// static UInt160 D = UInt160.Zero;
    ///
    /// public static void MyMethod()
    /// {
    ///     Runtime.Log(A[0]);
    ///     Runtime.Log(B[0]);
    ///     Runtime.Log(C[0]);
    ///     Runtime.Log(D.ToAddress());
    /// }
    /// </code>
    /// </example>
    private void ConvertInitializerExpression(SemanticModel model, InitializerExpressionSyntax expression)
    {
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression).ConvertedType!;
        ConvertInitializerExpression(model, type, expression);
    }

    private void ConvertInitializerExpression(SemanticModel model, IArrayTypeSymbol type, InitializerExpressionSyntax expression)
    {
        if (type.ElementType.SpecialType == SpecialType.System_Byte)
        {
            Optional<object?>[] values = expression.Expressions.Select(p => model.GetConstantValue(p)).ToArray();
            if (values.Any(p => !p.HasValue))
            {
                Push(values.Length);
                AddInstruction(OpCode.NEWBUFFER);
                for (int i = 0; i < expression.Expressions.Count; i++)
                {
                    AddInstruction(OpCode.DUP);
                    Push(i);
                    ConvertExpression(model, expression.Expressions[i]);
                    AddInstruction(OpCode.SETITEM);
                }
            }
            else
            {
                byte[] data = values.Select(p => (byte)System.Convert.ChangeType(p.Value, typeof(byte))!).ToArray();
                Push(data);
                ChangeType(VM.Types.StackItemType.Buffer);
            }
        }
        else
        {
            for (int i = expression.Expressions.Count - 1; i >= 0; i--)
                ConvertExpression(model, expression.Expressions[i]);
            Push(expression.Expressions.Count);
            AddInstruction(OpCode.PACK);
        }
    }
}
