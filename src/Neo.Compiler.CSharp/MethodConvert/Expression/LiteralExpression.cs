// Copyright (C) 2015-2025 The Neo Project.
//
// LiteralExpression.cs file belongs to the neo project and is free
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
    /// Convert literal expression to NeoVM instructions
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The literal expression to convert</param>
    /// <example>
    /// <code>
    /// public void ExampleMethod()
    /// {
    ///     int intLiteral = 42;
    ///     string stringLiteral = "Hello, Neo!";
    ///     bool boolLiteral = true;
    ///     object nullLiteral = null;
    ///     int defaultLiteral = default;
    /// }
    /// </code>
    /// </example>
    private void ConvertLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
    {
        if (expression.IsKind(SyntaxKind.DefaultLiteralExpression))
        {
            ConvertDefaultLiteralExpression(model, expression);
        }
        else if (expression.IsKind(SyntaxKind.NullLiteralExpression))
        {
            AddInstruction(OpCode.PUSHNULL);
        }
        else
        {
            throw CompilationException.UnsupportedSyntax(expression, $"Unsupported literal expression type. Use supported literals: numbers, strings, booleans, null, or default expressions.");
        }
    }

    /// <summary>
    /// Convert default literal expression to NeoVM instructions
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The literal expression to convert</param>
    /// <example>
    /// <code>
    /// public void ExampleMethod()
    /// {
    ///     int defaultLiteral = default;
    /// }
    /// </code>
    /// </example>
    private void ConvertDefaultLiteralExpression(SemanticModel model, LiteralExpressionSyntax expression)
    {
        var type = model.GetTypeInfo(expression).Type;
        if (type == null)
        {
            throw CompilationException.UnsupportedSyntax(expression, "Cannot determine type for default expression. Ensure the expression has a clear type context.");
        }

        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
                {
                    AddInstruction(OpCode.PUSHF);
                    break;
                }
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
                AddInstruction(OpCode.PUSH0);
                break;
            case SpecialType.System_String:
            case SpecialType.System_Object:
                AddInstruction(OpCode.PUSHNULL);
                break;
            default:
                if (type.ToString() == "System.Numerics.BigInteger")
                {
                    // BigInteger's default value is 0
                    AddInstruction(OpCode.PUSH0);
                }
                else if (type.IsReferenceType)
                {
                    AddInstruction(OpCode.PUSHNULL);
                }
                else if (type.IsValueType)
                {
                    // For structs and other value types, we need to create a default instance
                    AddInstruction(OpCode.NEWSTRUCT0);
                }
                else
                {
                    throw CompilationException.UnsupportedSyntax(expression, $"Unsupported type '{type}' for default expression. Use supported types: primitives, strings, or custom structs.");
                }
                break;
        }
    }
}
