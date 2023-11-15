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
        private void ConvertCastExpression(SemanticModel model, CastExpressionSyntax expression)
        {
            ITypeSymbol sType = model.GetTypeInfo(expression.Expression).Type!;
            ITypeSymbol tType = model.GetTypeInfo(expression.Type).Type!;
            IMethodSymbol method = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
            if (method is not null)
            {
                Call(model, method, null, expression.Expression);
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
                    {
                        EnsureIntegerInRange(tType);
                    }
                    break;
            }
        }

    }
}
