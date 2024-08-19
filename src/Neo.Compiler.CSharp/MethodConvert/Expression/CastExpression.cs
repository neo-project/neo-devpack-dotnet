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
    /// This method converts a cast expression to OpCodes.
    /// A cast expression of the form (T)E performs an explicit conversion of the result of expression E to type T.
    /// If no explicit conversion exists from the type of E to type T, a compile-time error occurs.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about cast expression.</param>
    /// <param name="expression">The syntax representation of the cast expression statement being converted.</param>
    /// <remarks>
    /// This method determines the source type and the target type of the cast expression.
    /// If the cast can be resolved to a method symbol, it calls the corresponding method.
    /// Otherwise, it generates OpCodes based on the types involved in the cast operation.
    /// </remarks>
    /// <example>
    /// This code is cast a ByteString type to an ECPoint type,
    /// where the source type is ByteString and the target type is ECPoint.
    /// <code>
    /// ByteString bytes = ByteString.Empty;
    /// ECPoint point = (ECPoint)bytes;
    /// Runtime.Log(point.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression">Cast expression</seealso>
    private void ConvertCastExpression(SemanticModel model, CastExpressionSyntax expression)
    {
        ITypeSymbol sType = model.GetTypeInfo(expression.Expression).Type!;
        ITypeSymbol tType = model.GetTypeInfo(expression.Type).Type!;
        IMethodSymbol method = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        if (method is not null)
        {
            CallMethodWithInstanceExpression(model, method, null, expression.Expression);
            return;
        }
        ConvertExpression(model, expression.Expression);
        switch ((sType.Name, tType.Name))
        {
            case ("ByteString", "ECPoint"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(33);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt160"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(20);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt256"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(32);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("SByte", "Byte"):
            case ("SByte", "UInt16"):
            case ("SByte", "UInt32"):
            case ("SByte", "UInt64"):
            case ("Int16", "SByte"):
            case ("Int16", "Byte"):
            case ("Int16", "UInt16"):
            case ("Int16", "UInt32"):
            case ("Int16", "UInt64"):
            case ("Int32", "SByte"):
            case ("Int32", "Int16"):
            case ("Int32", "Byte"):
            case ("Int32", "UInt16"):
            case ("Int32", "UInt32"):
            case ("Int32", "UInt64"):
            case ("Int64", "SByte"):
            case ("Int64", "Int16"):
            case ("Int64", "Int32"):
            case ("Int64", "Byte"):
            case ("Int64", "UInt16"):
            case ("Int64", "UInt32"):
            case ("Int64", "UInt64"):
            case ("Byte", "SByte"):
            case ("UInt16", "SByte"):
            case ("UInt16", "Int16"):
            case ("UInt16", "Byte"):
            case ("UInt32", "SByte"):
            case ("UInt32", "Int16"):
            case ("UInt32", "Int32"):
            case ("UInt32", "Byte"):
            case ("UInt32", "UInt16"):
            case ("UInt64", "SByte"):
            case ("UInt64", "Int16"):
            case ("UInt64", "Int32"):
            case ("UInt64", "Int64"):
            case ("UInt64", "Byte"):
            case ("UInt64", "UInt16"):
            case ("UInt64", "UInt32"):
            case ("Char", "SByte"):
            case ("Char", "Byte"):
            case ("Char", "Int16"):
            case ("Char", "UInt16"):
            case ("Char", "Int32"):
            case ("Char", "UInt32"):
            case ("Char", "Int64"):
            case ("Char", "UInt64"):
            case ("SByte", "Char"):
            case ("Byte", "Char"):
            case ("Int16", "Char"):
            case ("UInt16", "Char"):
            case ("Int32", "Char"):
            case ("UInt32", "Char"):
            case ("Int64", "Char"):
            case ("UInt64", "Char"):
                {
                    EnsureIntegerInRange(tType);
                }
                break;
        }
    }
}
