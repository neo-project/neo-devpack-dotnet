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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Neo.VM.Types;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Attempts to process system constructors. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system constructors are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        switch (symbol.ToString())
        {
            //For the BigInteger(byte[]) constructor, prepares method arguments and changes the return type to integer.
            case "System.Numerics.BigInteger.BigInteger(byte[])":
                PrepareArgumentsForMethod(model, symbol, arguments);
                ChangeType(VM.Types.StackItemType.Integer);
                return true;
            //For other constructors, such as List<T>(), return processing failure.
            default:
                return false;
        }
    }

    /// <summary>
    /// Attempts to process system methods. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="instanceExpression">The instance expression representing the instance of method invocation, if any.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system methods are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        //If the method belongs to a delegate and the method name is "Invoke",
        //calls the PrepareArgumentsForMethod method with CallingConvention.Cdecl convention and changes the return type to integer.
        //Example: Func<int, int, int>(privateSum).Invoke(a, b);
        //see ~/tests/Neo.Compiler.CSharp.TestContracts/Contract_Delegate.cs
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
            #region Property of System.Numerics.BigInteger

            //Gets a value that represents the number one (1).
            case "System.Numerics.BigInteger.One.get":
                Push(1);
                return true;
            //Gets a value that represents the number negative one (-1).
            case "System.Numerics.BigInteger.MinusOne.get":
                Push(-1);
                return true;
            //Gets a value that represents the number 0 (zero).
            case "System.Numerics.BigInteger.Zero.get":
                Push(0);
                return true;
            //Indicates whether the value of the current BigInteger object is Zero.
            case "System.Numerics.BigInteger.IsZero.get":
            case "System.Numerics.BigInteger?.IsZero.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                }
            //Indicates whether the value of the current BigInteger object is One.
            case "System.Numerics.BigInteger.IsOne.get":
            case "System.Numerics.BigInteger?.IsOne.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                }
            //Indicates whether the value of the current BigInteger object is an even number.
            case "System.Numerics.BigInteger.IsEven.get":
            case "System.Numerics.BigInteger?.IsEven.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    Push(1);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                }
            //Gets a number that indicates the sign (negative, positive, or zero) of the current BigInteger object.
            case "System.Numerics.BigInteger.Sign.get":
            case "System.Numerics.BigInteger?.Sign.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.SIGN);
                    return true;
                }
            //Missing BigInteger.IsPowerOfTwo Property
            #endregion

            #region Method of System.Numerics.BigInteger
            //Raises a BigInteger value to the power of a specified value.
            case "System.Numerics.BigInteger.Pow(System.Numerics.BigInteger, int)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.POW);
                    return true;
                }
            //Performs modulus division on a number raised to the power of another number.
            case "System.Numerics.BigInteger.ModPow(System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MODPOW);
                    return true;
                }
            //Adds two BigInteger values and returns the result.
            case "System.Numerics.BigInteger.Add(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.ADD);
                    return true;
                }
            //Subtracts one BigInteger value from another and returns the result.
            case "System.Numerics.BigInteger.Subtract(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.SUB);
                    return true;
                }
            //Negates a specified BigInteger value.
            case "System.Numerics.BigInteger.Negate(System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.NEGATE);
                    return true;
                }
            //Returns the product of two BigInteger values.
            case "System.Numerics.BigInteger.Multiply(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MUL);
                    return true;
                }
            //Divides one BigInteger value by another and returns the result.
            case "System.Numerics.BigInteger.Divide(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.DIV);
                    return true;
                }
            //Performs integer division on two BigInteger values and returns the remainder.
            case "System.Numerics.BigInteger.Remainder(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    AddInstruction(OpCode.MOD);
                    return true;
                }
            //Compares two BigInteger values and returns an integer that indicates whether the first value is less than,
            //equal to, or greater than the second value.
            case "System.Numerics.BigInteger.Compare(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    // if left < right return -1;
                    // if left = right return 0;
                    // if left > right return 1;
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.SIGN);
                    return true;
                }
            //Finds the greatest common divisor of two BigInteger values.
            case "System.Numerics.BigInteger.GreatestCommonDivisor(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
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
                }
            //Converts a BigInteger value to a byte array.
            case "System.Numerics.BigInteger.ToByteArray()":
            case "System.Numerics.BigInteger?.ToByteArray()":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    ChangeType(VM.Types.StackItemType.Buffer);
                    return true;
                }
            //Converts the string representation of a number to its BigInteger equivalent.
            case "System.Numerics.BigInteger.Parse(string)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                return true;

            #endregion
            //Defines an explicit conversion of a BigInteger object to a signed 8-bit value.
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
            //Defines an explicit conversion of a BigInteger object to an unsigned byte value.
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
            //Defines an explicit conversion of a BigInteger object to a 16-bit signed integer value.
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
            //Defines an explicit conversion of a BigInteger object to an unsigned 16-bit integer value.
            case "System.Numerics.BigInteger.explicit operator ushort(System.Numerics.BigInteger)":
            case "System.Numerics.BigInteger.explicit operator char(System.Numerics.BigInteger)":
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
            //Defines an explicit conversion of a BigInteger object to a 32-bit signed integer value.
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
                    return true;
                }
            //Defines an explicit conversion of a BigInteger object to an unsigned 32-bit integer value.
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
                    return true;
                }
            //Defines an explicit conversion of a BigInteger object to a 64-bit signed integer value.
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
                    return true;
                }
            //Defines an explicit conversion of a BigInteger object to an unsigned 64-bit integer value.
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
                    return true;
                }
            //Initializes a new instance of the BigInteger structure.
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
            case "System.Math.Abs(sbyte)":
            case "System.Math.Abs(short)":
            case "System.Math.Abs(int)":
            case "System.Math.Abs(long)":
            //Gets the absolute value of a BigInteger object.
            case "System.Numerics.BigInteger.Abs(System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.ABS);
                return true;
            //Returns an integer that indicates the sign of a number.
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
            case "System.Math.Max(object?, object?)":
            //Returns the larger of two BigInteger values.
            case "System.Numerics.BigInteger.Max(System.Numerics.BigInteger, System.Numerics.BigInteger)":
            case "System.Numerics.BigInteger?.Max(System.Numerics.BigInteger, System.Numerics.BigInteger)":
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
            case "System.Math.Min(object?, object?)":
            //Returns the smaller of two BigInteger values.
            case "System.Numerics.BigInteger.Min(System.Numerics.BigInteger, System.Numerics.BigInteger)":
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.MIN);
                return true;
            case "byte?.HasValue.get":
            case "sbyte?.HasValue.get":
            case "short?.HasValue.get":
            case "ushort?.HasValue.get":
            case "int?.HasValue.get":
            case "uint?.HasValue.get":
            case "long?.HasValue.get":
            case "ulong?.HasValue.get":
            case "bool?.HasValue.get":
            case "char?.HasValue.get":
            case "System.Numerics.BigInteger?.HasValue.get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                AddInstruction(OpCode.ISNULL);
                AddInstruction(OpCode.NOT);
                return true;
            case "byte?.Value.get":
            case "sbyte?.Value.get":
            case "short?.Value.get":
            case "ushort?.Value.get":
            case "int?.Value.get":
            case "uint?.Value.get":
            case "long?.Value.get":
            case "ulong?.Value.get":
            case "bool?.Value.get":
            case "char?.Value.get":
            case "System.Numerics.BigInteger?.Value.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMPIFNOT, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "sbyte?.ToString()":
            case "byte?.ToString()":
            case "short?.ToString()":
            case "ushort?.ToString()":
            case "int?.ToString()":
            case "uint?.ToString()":
            case "long?.ToString()":
            case "ulong?.ToString()":
            //Converts the numeric value of the current BigInteger object to its equivalent string representation.
            case "System.Numerics.BigInteger?.ToString()":
                {
                    JumpTarget endTarget = new();
                    JumpTarget endTarget2 = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF, endTarget);
                    CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
                    Jump(OpCode.JMP_L, endTarget2);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP);
                    Push("");
                    endTarget2.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "sbyte.ToString()":
            case "byte.ToString()":
            case "short.ToString()":
            case "ushort.ToString()":
            case "int.ToString()":
            case "uint.ToString()":
            case "long.ToString()":
            case "ulong.ToString()":
            //Converts the numeric value of the current BigInteger object to its equivalent string representation.
            case "System.Numerics.BigInteger.ToString()":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
                    return true;
                }
            // do it for every: byte, sbyte, short, ushort, int, uint, long, ulong, bool, char
            case "byte.Equals(object?)":
            case "sbyte.Equals(object?)":
            case "short.Equals(object?)":
            case "ushort.Equals(object?)":
            case "int.Equals(object?)":
            case "uint.Equals(object?)":
            case "long.Equals(object?)":
            case "ulong.Equals(object?)":
            case "bool.Equals(object?)":
            case "char.Equals(object?)":

            // also do for ? on every type
            case "byte?.Equals(object?)":
            case "sbyte?.Equals(object?)":
            case "short?.Equals(object?)":
            case "ushort?.Equals(object?)":
            case "int?.Equals(object?)":
            case "uint?.Equals(object?)":
            case "long?.Equals(object?)":
            case "ulong?.Equals(object?)":
            case "bool?.Equals(object?)":
            case "char?.Equals(object?)":
            case "System.Numerics.BigInteger.Equals(long)":
            case "System.Numerics.BigInteger?.Equals(long)":
            case "System.Numerics.BigInteger.Equals(ulong)":
            case "System.Numerics.BigInteger?.Equals(ulong)":
            case "System.Numerics.BigInteger?.Equals(object?)":
            case "System.Numerics.BigInteger.Equals(object?)":
            //Returns a value that indicates whether two numeric values are equal.
            case "System.Numerics.BigInteger.Equals(System.Numerics.BigInteger)":
            case "System.Numerics.BigInteger?.Equals(System.Numerics.BigInteger)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                }
            #region Method of string
            //Gets the total number of elements in all the dimensions of the Array.
            case "System.Array.Length.get":
            //Gets the number of characters in the current String object.
            case "string.Length.get":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.SIZE);
                    return true;
                }
            //Converts the string representation of a number to its 8-bit signed integer equivalent.
            case "sbyte.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(sbyte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its Byte equivalent.
            case "byte.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(byte.MinValue);
                    Push(byte.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 16-bit signed integer equivalent.
            case "short.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(short.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 16-bit unsigned integer equivalent.
            case "ushort.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(ushort.MinValue);
                    Push(ushort.MaxValue + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 32-bit signed integer equivalent.
            case "int.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 32-bit unsigned integer equivalent.
            case "uint.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(uint.MinValue);
                    Push(new BigInteger(uint.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 64-bit signed integer equivalent.
            case "long.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Converts the string representation of a number to its 64-bit unsigned integer equivalent.
            case "ulong.Parse(string)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
                    AddInstruction(OpCode.DUP);
                    Push(ulong.MinValue);
                    Push(new BigInteger(ulong.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                return true;
            //Determines whether two object instances are equal.
            case "object.Equals(object?)":
            //Determines whether two String objects have the same value.
            case "string.Equals(string?)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.EQUAL);
                return true;
            //Getting characters in a string by index
            case "string.this[int].get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments);
                AddInstruction(OpCode.PICKITEM);
                return true;
            //Retrieves a substring from this instance.
            //The substring starts at a specified character position and continues to the end of the string.
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
            //Retrieves a substring from this instance.
            //The substring starts at a specified character position and has a specified length.
            case "string.Substring(int, int)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.SUBSTR);
                return true;
            //Converts the value of this instance to its equivalent string representation (either "True" or "False").
            case "bool?.ToString()":
                {
                    JumpTarget trueTarget = new(), nullTarget = new(), endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, nullTarget);
                    Jump(OpCode.JMPIF_L, trueTarget);
                    Push("False");
                    Jump(OpCode.JMP_L, endTarget);
                    trueTarget.Instruction = Push("True");
                    Jump(OpCode.JMP_L, endTarget);
                    nullTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP);
                    Push("");
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
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
                    return true;
                }
            case "char.ToString()":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    ChangeType(StackItemType.ByteString);
                    return true;
                }
            case "char?.ToString()":
                {
                    JumpTarget nullTarget = new(), endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, nullTarget);
                    ChangeType(StackItemType.ByteString);
                    Jump(OpCode.JMP_L, endTarget);
                    nullTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP);
                    Push("");
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "string.ToString()":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    return true;
                }
            case "object.ToString()":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    ChangeType(StackItemType.ByteString);
                    return true;
                }
            #endregion
            //Non-system methods, such as user-defined methods
            default:
                return false;
        }
    }
}
