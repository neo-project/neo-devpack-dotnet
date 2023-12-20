// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        #region Convert Integer Handlers

        private bool HandleIntegerConversion(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        {
            return symbol.ToString() switch
            {
                "sbyte.Parse(string)" => HandleParseConversion(model, symbol, arguments, sbyte.MinValue, sbyte.MaxValue),
                "byte.Parse(string)" => HandleParseConversion(model, symbol, arguments, byte.MinValue, byte.MaxValue),
                "short.Parse(string)" => HandleParseConversion(model, symbol, arguments, short.MinValue, short.MaxValue),
                "ushort.Parse(string)" => HandleParseConversion(model, symbol, arguments, ushort.MinValue, ushort.MaxValue),
                "int.Parse(string)" => HandleParseConversion(model, symbol, arguments, int.MinValue, int.MaxValue),
                "uint.Parse(string)" => HandleParseConversion(model, symbol, arguments, uint.MinValue, uint.MaxValue),
                "long.Parse(string)" => HandleParseConversion(model, symbol, arguments, long.MinValue, long.MaxValue),
                "ulong.Parse(string)" => HandleParseConversion(model, symbol, arguments, ulong.MinValue, ulong.MaxValue),
                "sbyte.ToString()" or "byte.ToString()" or "short.ToString()" or "ushort.ToString()" or "int.ToString()" or "uint.ToString()" or "long.ToString()" or "ulong.ToString()" => HandleToStringConversion(model, instanceExpression),
                _ => HandleBigIntegerConversion(model, symbol, instanceExpression, arguments),
            };
        }

        private bool HandleToStringConversion(SemanticModel model, ExpressionSyntax? instanceExpression)
        {
            ConvertExpression(model, instanceExpression);
            Call(NativeContract.StdLib.Hash, "itoa", 1, true);
            return true;
        }
        private bool HandleBigIntegerConversion(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
        {
            switch (symbol.ToString())
            {
                case "System.Numerics.BigInteger.One.get":
                    Push(1); return true;
                case "System.Numerics.BigInteger.MinusOne.get":
                    Push(-1); return true;
                case "System.Numerics.BigInteger.Zero.get": Push(0); return true;
                case "System.Numerics.BigInteger.IsZero.get":
                    ConvertExpression(model, instanceExpression);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.IsOne.get":
                    ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.IsEven.get":
                    ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.Sign.get":
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Numerics.BigInteger.Pow(System.Numerics.BigInteger, int)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.POW);
                    return true;
                case "System.Numerics.BigInteger.ModPow(System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MODPOW);
                    return true;
                case "System.Numerics.BigInteger.Add(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.ADD);
                    return true;
                case "System.Numerics.BigInteger.Subtract(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.SUB);
                    return true;
                case "System.Numerics.BigInteger.Negate(System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.NEGATE);
                    return true;
                case "System.Numerics.BigInteger.Multiply(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MUL);
                    return true;
                case "System.Numerics.BigInteger.Divide(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.DIV);
                    return true;
                case "System.Numerics.BigInteger.Remainder(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MOD);
                    return true;
                case "System.Numerics.BigInteger.Compare(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.SIGN);
                    return true;
                case "System.Numerics.BigInteger.GreatestCommonDivisor(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget gcdTarget = new();
                    gcdTarget.Instruction = AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.MOD);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.PUSH0);
                    AddInstruction(OpCode.NUMEQUAL);
                    Jump(OpCode.JMPIFNOT, gcdTarget);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.ABS);
                    return true;
                case "System.Numerics.BigInteger.ToByteArray()":
                    ConvertExpression(model, instanceExpression);
                    ChangeType(VM.Types.StackItemType.Buffer);
                    return true;
                case "System.Numerics.BigInteger.explicit operator char(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, char.MinValue, char.MaxValue);
                case "System.Numerics.BigInteger.explicit operator sbyte(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, sbyte.MinValue, sbyte.MaxValue);
                case "System.Numerics.BigInteger.explicit operator byte(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, byte.MinValue, byte.MaxValue);
                case "System.Numerics.BigInteger.explicit operator short(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, short.MinValue, short.MaxValue);
                case "System.Numerics.BigInteger.explicit operator ushort(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, ushort.MinValue, ushort.MaxValue);
                case "System.Numerics.BigInteger.explicit operator int(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, int.MinValue, int.MaxValue);
                case "System.Numerics.BigInteger.explicit operator uint(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, uint.MinValue, uint.MaxValue);
                case "System.Numerics.BigInteger.explicit operator long(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, long.MinValue, long.MaxValue);
                case "System.Numerics.BigInteger.explicit operator ulong(System.Numerics.BigInteger)": return HandleExplicitConversion(model, symbol, arguments, ulong.MinValue, ulong.MaxValue);
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(char)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(sbyte)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(byte)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(short)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ushort)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(int)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(uint)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(long)":
                case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ulong)":
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    return true;
                case "System.Numerics.BigInteger.ToString()": return HandleToStringConversion(model, instanceExpression);
                case "System.Numerics.BigInteger.Equals(long)":
                case "System.Numerics.BigInteger.Equals(ulong)":
                case "System.Numerics.BigInteger.Equals(System.Numerics.BigInteger)":
                    ConvertExpression(model, instanceExpression);
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                case "System.Numerics.BigInteger.Abs(System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.ABS);
                    return true;
                case "System.Numerics.BigInteger.Max(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.MAX);
                    return true;
                case "System.Numerics.BigInteger.Min(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                    PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.MIN);
                    return true;
                case "System.Numerics.BigInteger.Parse(string)": return HandleParseConversion(model, symbol, arguments, null, null);
                default:
                    return false;
            }
        }

        private bool HandleExplicitConversion(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments, BigInteger minValue, BigInteger maxValue)
        {
            PrepareArgumentsForMethod(model, symbol, arguments);
            JumpTarget endTarget = new();
            AddInstruction(OpCode.DUP);
            Push(minValue);
            Push(maxValue + 1);
            AddInstruction(OpCode.WITHIN);
            Jump(OpCode.JMPIF, endTarget);
            AddInstruction(OpCode.THROW);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
            return true;
        }

        private bool HandleParseConversion(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments, BigInteger? minValue, BigInteger? maxValue)
        {
            PrepareArgumentsForMethod(model, symbol, arguments);
            Call(NativeContract.StdLib.Hash, "atoi", 1, true);
            if (minValue == null || maxValue == null) return true;
            JumpTarget endTarget = new();
            AddInstruction(OpCode.DUP);
            Push(minValue);
            Push(maxValue + 1);
            AddInstruction(OpCode.WITHIN);
            Jump(OpCode.JMPIF, endTarget);
            AddInstruction(OpCode.THROW);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
            return true;
        }
        #endregion
    }

}
