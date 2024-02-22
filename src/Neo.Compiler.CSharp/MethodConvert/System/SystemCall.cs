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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

partial class MethodConvert
{
    private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        switch (symbol.ToString())
        {
            case "System.Numerics.BigInteger.BigInteger(byte[])":
                PrepareArgumentsForMethod(model, symbol, arguments);
                ChangeType(VM.Types.StackItemType.Integer);
                return true;
            default:
                return false;
        }
    }

    private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (symbol.ContainingType.TypeKind == TypeKind.Delegate && symbol.Name == "Invoke")
        {
            if (arguments is not null)
                PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
            ConvertExpression(model, instanceExpression!);
            AddInstruction(OpCode.CALLA);
            return true;
        }
        switch (symbol.ToString())
        {
            case "System.Numerics.BigInteger.One.get":
                Push(1);
                return true;
            case "System.Numerics.BigInteger.MinusOne.get":
                Push(-1);
                return true;
            case "System.Numerics.BigInteger.Zero.get":
                Push(0);
                return true;
            case "System.Numerics.BigInteger.IsZero.get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                Push(0);
                AddInstruction(OpCode.NUMEQUAL);
                return true;
            case "System.Numerics.BigInteger.IsOne.get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                Push(1);
                AddInstruction(OpCode.NUMEQUAL);
                return true;
            case "System.Numerics.BigInteger.IsEven.get":
                if (instanceExpression is not null)
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
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.POW);
                return true;
            case "System.Numerics.BigInteger.ModPow(System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.MODPOW);
                return true;
            case "System.Numerics.BigInteger.Add(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.ADD);
                return true;
            case "System.Numerics.BigInteger.Subtract(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.SUB);
                return true;
            case "System.Numerics.BigInteger.Negate(System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.NEGATE);
                return true;
            case "System.Numerics.BigInteger.Multiply(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.MUL);
                return true;
            case "System.Numerics.BigInteger.Divide(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.DIV);
                return true;
            case "System.Numerics.BigInteger.Remainder(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.MOD);
                return true;
            case "System.Numerics.BigInteger.Compare(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                // if left < right return -1;
                // if left = right return 0;
                // if left > right return 1;
                AddInstruction(OpCode.SUB);
                AddInstruction(OpCode.SIGN);
                return true;
            case "System.Numerics.BigInteger.GreatestCommonDivisor(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                JumpTarget gcdTarget = new()
                {
                    Instruction = AddInstruction(OpCode.DUP)
                };
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
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                ChangeType(VM.Types.StackItemType.Buffer);
                return true;
            case "System.Numerics.BigInteger.explicit operator sbyte(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(sbyte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator byte(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(byte.MinValue);
                    Push(byte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator short(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(short.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator ushort(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(ushort.MinValue);
                    Push(ushort.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator int(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator uint(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(uint.MinValue);
                    Push(new BigInteger(uint.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator long(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.explicit operator ulong(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push(ulong.MinValue);
                    Push(new BigInteger(ulong.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(char)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(sbyte)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(byte)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(short)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ushort)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(int)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(uint)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(long)":
            case "System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(ulong)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                return true;
            case "System.Array.Length.get":
            case "string.Length.get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                AddInstruction(OpCode.SIZE);
                return true;
            case "sbyte.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(sbyte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "byte.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(byte.MinValue);
                    Push(byte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "short.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(short.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "ushort.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(ushort.MinValue);
                    Push(ushort.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "int.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "uint.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(uint.MinValue);
                    Push(new BigInteger(uint.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "long.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "ulong.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(ulong.MinValue);
                    Push(new BigInteger(ulong.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "System.Numerics.BigInteger.Parse(string)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                Call(NativeContract.StdLib.Hash, "atoi", 1, true);
                return true;
            case "System.Math.Abs(sbyte)":
            case "System.Math.Abs(short)":
            case "System.Math.Abs(int)":
            case "System.Math.Abs(long)":
            case "System.Numerics.BigInteger.Abs(System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.ABS);
                return true;
            case "System.Math.Sign(sbyte)":
            case "System.Math.Sign(short)":
            case "System.Math.Sign(int)":
            case "System.Math.Sign(long)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.SIGN);
                return true;
            case "System.Math.Max(byte, byte)":
            case "System.Math.Max(sbyte, sbyte)":
            case "System.Math.Max(short, short)":
            case "System.Math.Max(ushort, ushort)":
            case "System.Math.Max(int, int)":
            case "System.Math.Max(uint, uint)":
            case "System.Math.Max(long, long)":
            case "System.Math.Max(ulong, ulong)":
            case "System.Numerics.BigInteger.Max(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.MAX);
                return true;
            case "System.Math.Min(byte, byte)":
            case "System.Math.Min(sbyte, sbyte)":
            case "System.Math.Min(short, short)":
            case "System.Math.Min(ushort, ushort)":
            case "System.Math.Min(int, int)":
            case "System.Math.Min(uint, uint)":
            case "System.Math.Min(long, long)":
            case "System.Math.Min(ulong, ulong)":
            case "System.Numerics.BigInteger.Min(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.MIN);
                return true;
            case "bool.ToString()":
                {
                    JumpTarget trueTarget = new(), endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Jump(OpCode.JMPIF_L, trueTarget);
                    Push("False");
                    Jump(OpCode.JMP_L, endTarget);
                    trueTarget.Instruction = Push("True");
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            case "sbyte.ToString()":
            case "byte.ToString()":
            case "short.ToString()":
            case "ushort.ToString()":
            case "int.ToString()":
            case "uint.ToString()":
            case "long.ToString()":
            case "ulong.ToString()":
            case "System.Numerics.BigInteger.ToString()":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                Call(NativeContract.StdLib.Hash, "itoa", 1, true);
                return true;
            case "System.Numerics.BigInteger.Equals(long)":
            case "System.Numerics.BigInteger.Equals(ulong)":
            case "System.Numerics.BigInteger.Equals(System.Numerics.BigInteger)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.NUMEQUAL);
                return true;
            case "object.Equals(object?)":
            case "string.Equals(string?)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.EQUAL);
                return true;
            case "string.this[int].get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.PICKITEM);
                return true;
            case "string.Substring(int)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.SIZE);
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.SUB);
                AddInstruction(OpCode.SUBSTR);
                return true;
            case "string.Substring(int, int)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.SUBSTR);
                return true;
            default:
                return false;
        }
    }
}
