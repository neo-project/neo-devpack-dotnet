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
            case "byte.IsEvenInteger(byte)":
            case "sbyte.IsEvenInteger(sbyte)":
            case "short.IsEvenInteger(short)":
            case "ushort.IsEvenInteger(ushort)":
            case "int.IsEvenInteger(int)":
            case "uint.IsEvenInteger(uint)":
            case "long.IsEvenInteger(long)":
            case "ulong.IsEvenInteger(ulong)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push(1);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    return true;
                }
            case "byte.IsOddInteger(byte)":
            case "sbyte.IsOddInteger(sbyte)":
            case "short.IsOddInteger(short)":
            case "ushort.IsOddInteger(ushort)":
            case "int.IsOddInteger(int)":
            case "uint.IsOddInteger(uint)":
            case "long.IsOddInteger(long)":
            case "ulong.IsOddInteger(ulong)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push(1);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMNOTEQUAL);
                    return true;
                }
            case "sbyte.IsNegative(sbyte)":
            case "short.IsNegative(short)":
            case "int.IsNegative(int)":
            case "long.IsNegative(long)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.SIGN);
                    Push(0);
                    AddInstruction(OpCode.LT);
                    return true;
                }
            case "sbyte.IsPositive(sbyte)":
            case "short.IsPositive(short)":
            case "int.IsPositive(int)":
            case "long.IsPositive(long)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.SIGN);
                    Push(0);
                    AddInstruction(OpCode.GE);
                    return true;
                }
            case "byte.IsPow2(byte)":
            case "sbyte.IsPow2(sbyte)":
            case "short.IsPow2(short)":
            case "ushort.IsPow2(ushort)":
            case "int.IsPow2(int)":
            case "uint.IsPow2(uint)":
            case "long.IsPow2(long)":
            case "ulong.IsPow2(ulong)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endFalse = new();
                    JumpTarget endTrue = new();
                    JumpTarget endTarget = new();
                    JumpTarget nonZero = new();
                    AddInstruction(OpCode.DUP);
                    Push(0);
                    Jump(OpCode.JMPNE, nonZero);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endFalse);
                    nonZero.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.DEC);
                    AddInstruction(OpCode.AND);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    Jump(OpCode.JMPIF, endTrue);
                    endFalse.Instruction = AddInstruction(OpCode.NOP);
                    Push(false);
                    Jump(OpCode.JMP, endTarget);
                    endTrue.Instruction = AddInstruction(OpCode.NOP);
                    Push(true);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "int.LeadingZeroCount(int)":
            case "uint.LeadingZeroCount(uint)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endLoop = new();
                    JumpTarget loopStart = new();
                    JumpTarget endTarget = new();
                    if (symbol.ToString() == "int.LeadingZeroCount(int)")
                    {
                        AddInstruction(OpCode.DUP); // a a
                        AddInstruction(OpCode.PUSH0);// a a 0
                        JumpTarget notNegative = new();
                        Jump(OpCode.JMPGE, notNegative); //a
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
                        Jump(OpCode.JMP, endTarget);
                        notNegative.Instruction = AddInstruction(OpCode.NOP);
                    }
                    Push(0); // count 5 0
                    loopStart.Instruction = AddInstruction(OpCode.SWAP); //0 5
                    AddInstruction(OpCode.DUP);//  0 5 5
                    AddInstruction(OpCode.PUSH0);// 0 5 5 0
                    Jump(OpCode.JMPEQ, endLoop); //0 5
                    AddInstruction(OpCode.PUSH1);//0 5 1
                    AddInstruction(OpCode.SHR); //0  5>>1
                    AddInstruction(OpCode.SWAP);//5>>1 0
                    AddInstruction(OpCode.INC);// 5>>1 1
                    Jump(OpCode.JMP, loopStart);
                    endLoop.Instruction = AddInstruction(OpCode.DROP);
                    Push(32);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "byte.LeadingZeroCount(byte)":
            case "sbyte.LeadingZeroCount(sbyte)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endLoop = new();
                    JumpTarget loopStart = new();
                    JumpTarget endTarget = new();
                    JumpTarget notNegative = new();
                    if (symbol.ToString() == "sbyte.LeadingZeroCount(sbyte)")
                    {
                        AddInstruction(OpCode.DUP); // a a
                        AddInstruction(OpCode.PUSH0);// a a 0
                        Jump(OpCode.JMPGE, notNegative); //a
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
                        Jump(OpCode.JMP, endTarget);
                        notNegative.Instruction = AddInstruction(OpCode.NOP);
                    }
                    Push(0); // count 5 0
                    loopStart.Instruction = AddInstruction(OpCode.SWAP); //0 5
                    AddInstruction(OpCode.DUP);//  0 5 5
                    AddInstruction(OpCode.PUSH0);// 0 5 5 0
                    Jump(OpCode.JMPEQ, endLoop); //0 5
                    AddInstruction(OpCode.PUSH1);//0 5 1
                    AddInstruction(OpCode.SHR); //0  5>>1
                    AddInstruction(OpCode.SWAP);//5>>1 0
                    AddInstruction(OpCode.INC);// 5>>1 1
                    Jump(OpCode.JMP, loopStart);
                    endLoop.Instruction = AddInstruction(OpCode.DROP);
                    Push(8);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "short.LeadingZeroCount(short)":
            case "ushort.LeadingZeroCount(ushort)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endLoop = new();
                    JumpTarget loopStart = new();
                    JumpTarget endTarget = new();
                    if (symbol.ToString() == "short.LeadingZeroCount(short)")
                    {
                        AddInstruction(OpCode.DUP); // a a
                        AddInstruction(OpCode.PUSH0);// a a 0
                        JumpTarget notNegative = new();
                        Jump(OpCode.JMPGE, notNegative); //a
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
                        Jump(OpCode.JMP, endTarget);
                        notNegative.Instruction = AddInstruction(OpCode.NOP);
                    }
                    Push(0); // count 5 0
                    loopStart.Instruction = AddInstruction(OpCode.SWAP); //0 5
                    AddInstruction(OpCode.DUP);//  0 5 5
                    AddInstruction(OpCode.PUSH0);// 0 5 5 0
                    Jump(OpCode.JMPEQ, endLoop); //0 5
                    AddInstruction(OpCode.PUSH1);//0 5 1
                    AddInstruction(OpCode.SHR); //0  5>>1
                    AddInstruction(OpCode.SWAP);//5>>1 0
                    AddInstruction(OpCode.INC);// 5>>1 1
                    Jump(OpCode.JMP, loopStart);
                    endLoop.Instruction = AddInstruction(OpCode.DROP);
                    Push(16);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "long.LeadingZeroCount(long)":
            case "ulong.LeadingZeroCount(ulong)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endLoop = new();
                    JumpTarget loopStart = new();
                    JumpTarget endTarget = new();
                    if (symbol.ToString() == "long.LeadingZeroCount(long)")
                    {
                        AddInstruction(OpCode.DUP); // a a
                        AddInstruction(OpCode.PUSH0);// a a 0
                        JumpTarget notNegative = new();
                        Jump(OpCode.JMPGE, notNegative); //a
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
                        Jump(OpCode.JMP, endTarget);
                        notNegative.Instruction = AddInstruction(OpCode.NOP);
                    }
                    Push(0); // count 5 0
                    loopStart.Instruction = AddInstruction(OpCode.SWAP); //0 5
                    AddInstruction(OpCode.DUP);//  0 5 5
                    AddInstruction(OpCode.PUSH0);// 0 5 5 0
                    Jump(OpCode.JMPEQ, endLoop); //0 5
                    AddInstruction(OpCode.PUSH1);//0 5 1
                    AddInstruction(OpCode.SHR); //0  5>>1
                    AddInstruction(OpCode.SWAP);//5>>1 0
                    AddInstruction(OpCode.INC);// 5>>1 1
                    Jump(OpCode.JMP, loopStart);
                    endLoop.Instruction = AddInstruction(OpCode.DROP);
                    Push(64);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "int.Log2(int)":
            case "byte.Log2(byte)":
            case "sbyte.Log2(sbyte)":
            case "short.Log2(short)":
            case "ushort.Log2(ushort)":
            case "uint.Log2(uint)":
            case "long.Log2(long)":
            case "ulong.Log2(ulong)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);

                    JumpTarget endLoop = new();
                    JumpTarget negativeInput = new();
                    JumpTarget zeroTarget = new();
                    AddInstruction(OpCode.DUP);// 5 5
                    AddInstruction(OpCode.PUSH0); // 5 5 0
                    Jump(OpCode.JMPEQ, zeroTarget); // 5
                    AddInstruction(OpCode.DUP);// 5 5
                    AddInstruction(OpCode.PUSH0); // 5 5 0
                    Jump(OpCode.JMPLT, negativeInput); // 5
                    AddInstruction(OpCode.PUSHM1);// 5 -1
                    JumpTarget loopStart = new();
                    loopStart.Instruction = AddInstruction(OpCode.SWAP); // -1 5
                    AddInstruction(OpCode.DUP); // -1 5 5
                    AddInstruction(OpCode.PUSH0); // -1 5 5 0
                    Jump(OpCode.JMPEQ, endLoop);  // -1 5
                    AddInstruction(OpCode.PUSH1); // -1 5 1
                    AddInstruction(OpCode.SHR); // -1 5>>1
                    AddInstruction(OpCode.SWAP); // 5>>1 -1
                    AddInstruction(OpCode.INC); // 5>>1 -1+1
                    Jump(OpCode.JMP, loopStart);
                    endLoop.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP); // -1
                    JumpTarget endMethod = new();
                    Jump(OpCode.JMP, endMethod);
                    zeroTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP);
                    Push(0);
                    Jump(OpCode.JMP, endMethod);
                    negativeInput.Instruction = AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.THROW);
                    endMethod.Instruction = AddInstruction(OpCode.NOP);

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
            case "System.Math.BigMul(int, int)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "System.Math.DivRem(sbyte, sbyte)":
            case "sbyte.DivRem(sbyte, sbyte)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(new BigInteger(sbyte.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(byte, byte)":
            case "byte.DivRem(byte, byte)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(byte.MinValue);
                    Push(new BigInteger(byte.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(short, short)":
            case "short.DivRem(short, short)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(new BigInteger(short.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(ushort, ushort)":
            case "ushort.DivRem(ushort, ushort)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(ushort.MinValue);
                    Push(new BigInteger(ushort.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(int, int)":
            case "int.DivRem(int, int)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(uint, uint)":
            case "uint.DivRem(uint, uint)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(uint.MinValue);
                    Push(new BigInteger(uint.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(long, long)":
            case "long.DivRem(long, long)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.DivRem(ulong, ulong)":
            case "ulong.DivRem(ulong, ulong)":
                {
                    JumpTarget endTarget = new();
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // Perform division
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.TUCK);
                    AddInstruction(OpCode.DIV);

                    // Calculate remainder
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.MUL);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(ulong.MinValue);
                    Push(new BigInteger(ulong.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.PUSH2);
                    AddInstruction(OpCode.PACK);
                    return true;
                }
            case "System.Math.Clamp(byte, byte, byte)":
            case "byte.Clamp(byte, byte, byte)":
            case "System.Math.Clamp(sbyte, sbyte, sbyte)":
            case "sbyte.Clamp(sbyte, sbyte, sbyte)":
            case "System.Math.Clamp(short, short, short)":
            case "short.Clamp(short, short, short)":
            case "System.Math.Clamp(ushort, ushort, ushort)":
            case "ushort.Clamp(ushort, ushort, ushort)":
            case "System.Math.Clamp(int, int, int)":
            case "int.Clamp(int, int, int)":
            case "System.Math.Clamp(uint, uint, uint)":
            case "uint.Clamp(uint, uint, uint)":
            case "System.Math.Clamp(long, long, long)":
            case "long.Clamp(long, long, long)":
            case "System.Math.Clamp(ulong, ulong, ulong)":
            case "ulong.Clamp(ulong, ulong, ulong)":
            case "System.Numerics.BigInteger.Clamp(System.Numerics.BigInteger, System.Numerics.BigInteger, System.Numerics.BigInteger)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "int.CopySign(int, int)":

                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget nonZeroTarget = new();
                    JumpTarget nonZeroTarget2 = new();
                    // a b
                    AddInstruction(OpCode.SIGN);         // a 1
                    AddInstruction(OpCode.DUP); // a 1 1
                    Push(0); // a 1 1 0
                    Jump(OpCode.JMPLT, nonZeroTarget); // a 1
                    AddInstruction(OpCode.DROP);
                    Push(1); // a 1
                    nonZeroTarget.Instruction = AddInstruction(OpCode.NOP); // a 1
                    AddInstruction(OpCode.SWAP);         // 1 a
                    AddInstruction(OpCode.DUP);// 1 a a
                    AddInstruction(OpCode.SIGN);// 1 a 0
                    AddInstruction(OpCode.DUP);// 1 a 0 0
                    Push(0); // 1 a 0 0 0
                    Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
                    AddInstruction(OpCode.DROP);
                    Push(1);
                    nonZeroTarget2.Instruction = AddInstruction(OpCode.NOP); // 1 a 1
                    AddInstruction(OpCode.ROT);// a 1 1
                    AddInstruction(OpCode.EQUAL);// a 1 1
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMPIF, endTarget); // a
                    AddInstruction(OpCode.NEGATE);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);

                    var endTarget2 = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget2);
                    AddInstruction(OpCode.THROW);
                    endTarget2.Instruction = AddInstruction(OpCode.NOP);

                    return true;
                }

            case "sbyte.CopySign(sbyte, sbyte)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget nonZeroTarget = new();
                    JumpTarget nonZeroTarget2 = new();
                    // a b
                    AddInstruction(OpCode.SIGN);         // a 1
                    AddInstruction(OpCode.DUP); // a 1 1
                    Push(0); // a 1 1 0
                    Jump(OpCode.JMPLT, nonZeroTarget); // a 1
                    AddInstruction(OpCode.DROP);
                    Push(1); // a 1
                    nonZeroTarget.Instruction = AddInstruction(OpCode.NOP); // a 1
                    AddInstruction(OpCode.SWAP);         // 1 a
                    AddInstruction(OpCode.DUP);// 1 a a
                    AddInstruction(OpCode.SIGN);// 1 a 0
                    AddInstruction(OpCode.DUP);// 1 a 0 0
                    Push(0); // 1 a 0 0 0
                    Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
                    AddInstruction(OpCode.DROP);
                    Push(1);
                    nonZeroTarget2.Instruction = AddInstruction(OpCode.NOP); // 1 a 1
                    AddInstruction(OpCode.ROT);// a 1 1
                    AddInstruction(OpCode.EQUAL);// a 1 1
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMPIF, endTarget); // a
                    AddInstruction(OpCode.NEGATE);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);

                    var endTarget2 = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(new BigInteger(sbyte.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget2);
                    AddInstruction(OpCode.THROW);
                    endTarget2.Instruction = AddInstruction(OpCode.NOP);

                    return true;
                }
            case "short.CopySign(short, short)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget nonZeroTarget = new();
                    JumpTarget nonZeroTarget2 = new();
                    // a b
                    AddInstruction(OpCode.SIGN);         // a 1
                    AddInstruction(OpCode.DUP); // a 1 1
                    Push(0); // a 1 1 0
                    Jump(OpCode.JMPLT, nonZeroTarget); // a 1
                    AddInstruction(OpCode.DROP);
                    Push(1); // a 1
                    nonZeroTarget.Instruction = AddInstruction(OpCode.NOP); // a 1
                    AddInstruction(OpCode.SWAP);         // 1 a
                    AddInstruction(OpCode.DUP);// 1 a a
                    AddInstruction(OpCode.SIGN);// 1 a 0
                    AddInstruction(OpCode.DUP);// 1 a 0 0
                    Push(0); // 1 a 0 0 0
                    Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
                    AddInstruction(OpCode.DROP);
                    Push(1);
                    nonZeroTarget2.Instruction = AddInstruction(OpCode.NOP); // 1 a 1
                    AddInstruction(OpCode.ROT);// a 1 1
                    AddInstruction(OpCode.EQUAL);// a 1 1
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMPIF, endTarget); // a
                    AddInstruction(OpCode.NEGATE);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);

                    var endTarget2 = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(new BigInteger(short.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget2);
                    AddInstruction(OpCode.THROW);
                    endTarget2.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "long.CopySign(long, long)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget nonZeroTarget = new();
                    JumpTarget nonZeroTarget2 = new();
                    // a b
                    AddInstruction(OpCode.SIGN);         // a 1
                    AddInstruction(OpCode.DUP); // a 1 1
                    Push(0); // a 1 1 0
                    Jump(OpCode.JMPLT, nonZeroTarget); // a 1
                    AddInstruction(OpCode.DROP);
                    Push(1); // a 1
                    nonZeroTarget.Instruction = AddInstruction(OpCode.NOP); // a 1
                    AddInstruction(OpCode.SWAP);         // 1 a
                    AddInstruction(OpCode.DUP);// 1 a a
                    AddInstruction(OpCode.SIGN);// 1 a 0
                    AddInstruction(OpCode.DUP);// 1 a 0 0
                    Push(0); // 1 a 0 0 0
                    Jump(OpCode.JMPLT, nonZeroTarget2); // 1 a 0
                    AddInstruction(OpCode.DROP);
                    Push(1);
                    nonZeroTarget2.Instruction = AddInstruction(OpCode.NOP); // 1 a 1
                    AddInstruction(OpCode.ROT);// a 1 1
                    AddInstruction(OpCode.EQUAL);// a 1 1
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMPIF, endTarget); // a
                    AddInstruction(OpCode.NEGATE);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    var endTarget2 = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget2);
                    AddInstruction(OpCode.THROW);
                    endTarget2.Instruction = AddInstruction(OpCode.NOP);

                    return true;
                }
            case "int.CreateChecked<byte>(byte)":
            case "int.CreateChecked<sbyte>(sbyte)":
            case "int.CreateChecked<short>(short)":
            case "int.CreateChecked<ushort>(ushort)":
            case "int.CreateChecked<int>(int)":
            case "int.CreateChecked<uint>(uint)":
            case "int.CreateChecked<long>(long)":
            case "int.CreateChecked<ulong>(ulong)":
            case "int.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(int.MinValue);
                    Push(new BigInteger(int.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "uint.CreateChecked<byte>(byte)":
            case "uint.CreateChecked<sbyte>(sbyte)":
            case "uint.CreateChecked<short>(short)":
            case "uint.CreateChecked<ushort>(ushort)":
            case "uint.CreateChecked<uint>(uint)":
            case "uint.CreateChecked<int>(int)":
            case "uint.CreateChecked<long>(long)":
            case "uint.CreateChecked<ulong>(ulong)":
            case "uint.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(uint.MinValue);
                    Push(new BigInteger(uint.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "byte.CreateChecked<byte>(byte)":
            case "byte.CreateChecked<sbyte>(sbyte)":
            case "byte.CreateChecked<short>(short)":
            case "byte.CreateChecked<ushort>(ushort)":
            case "byte.CreateChecked<int>(int)":
            case "byte.CreateChecked<uint>(uint)":
            case "byte.CreateChecked<long>(long)":
            case "byte.CreateChecked<ulong>(ulong)":
            case "byte.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(byte.MinValue);
                    Push(new BigInteger(byte.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "sbyte.CreateChecked<sbyte>(sbyte)":
            case "sbyte.CreateChecked<byte>(byte)":
            case "sbyte.CreateChecked<short>(short)":
            case "sbyte.CreateChecked<ushort>(ushort)":
            case "sbyte.CreateChecked<int>(int)":
            case "sbyte.CreateChecked<uint>(uint)":
            case "sbyte.CreateChecked<long>(long)":
            case "sbyte.CreateChecked<ulong>(ulong)":
            case "sbyte.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(sbyte.MinValue);
                    Push(new BigInteger(sbyte.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "short.CreateChecked<byte>(byte)":
            case "short.CreateChecked<sbyte>(sbyte)":
            case "short.CreateChecked<short>(short)":
            case "short.CreateChecked<ushort>(ushort)":
            case "short.CreateChecked<int>(int)":
            case "short.CreateChecked<uint>(uint)":
            case "short.CreateChecked<long>(long)":
            case "short.CreateChecked<ulong>(ulong)":
            case "short.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(short.MinValue);
                    Push(new BigInteger(short.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "ushort.CreateChecked<byte>(byte)":
            case "ushort.CreateChecked<sbyte>(sbyte)":
            case "ushort.CreateChecked<ushort>(ushort)":
            case "ushort.CreateChecked<short>(short)":
            case "ushort.CreateChecked<int>(int)":
            case "ushort.CreateChecked<uint>(uint)":
            case "ushort.CreateChecked<long>(long)":
            case "ushort.CreateChecked<ulong>(ulong)":
            case "ushort.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(ushort.MinValue);
                    Push(new BigInteger(ushort.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "long.CreateChecked<int>(int)":
            case "long.CreateChecked<uint>(uint)":
            case "long.CreateChecked<byte>(byte)":
            case "long.CreateChecked<sbyte>(sbyte)":
            case "long.CreateChecked<short>(short)":
            case "long.CreateChecked<long>(long)":
            case "long.CreateChecked<ushort>(ushort)":
            case "long.CreateChecked<ulong>(ulong)":
            case "long.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(long.MinValue);
                    Push(new BigInteger(long.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "ulong.CreateChecked<int>(int)":
            case "ulong.CreateChecked<uint>(uint)":
            case "ulong.CreateChecked<byte>(byte)":
            case "ulong.CreateChecked<sbyte>(sbyte)":
            case "ulong.CreateChecked<ushort>(ushort)":
            case "ulong.CreateChecked<short>(short)":
            case "ulong.CreateChecked<long>(long)":
            case "ulong.CreateChecked<ulong>(ulong)":
            case "ulong.CreateChecked<char>(char)":
                {
                    JumpTarget endTarget = new();
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push(ulong.MinValue);
                    Push(new BigInteger(ulong.MaxValue) + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "int.CreateSaturating<byte>(byte)":
            case "int.CreateSaturating<sbyte>(sbyte)":
            case "int.CreateSaturating<short>(short)":
            case "int.CreateSaturating<ushort>(ushort)":
            case "int.CreateSaturating<int>(int)":
            case "int.CreateSaturating<uint>(uint)":
            case "int.CreateSaturating<long>(long)":
            case "int.CreateSaturating<ulong>(ulong)":
            case "int.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(int.MinValue);
                    Push(int.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "uint.CreateSaturating<byte>(byte)":
            case "uint.CreateSaturating<sbyte>(sbyte)":
            case "uint.CreateSaturating<short>(short)":
            case "uint.CreateSaturating<ushort>(ushort)":
            case "uint.CreateSaturating<uint>(uint)":
            case "uint.CreateSaturating<int>(int)":
            case "uint.CreateSaturating<long>(long)":
            case "uint.CreateSaturating<ulong>(ulong)":
            case "uint.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(uint.MinValue);
                    Push(uint.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "byte.CreateSaturating<byte>(byte)":
            case "byte.CreateSaturating<sbyte>(sbyte)":
            case "byte.CreateSaturating<short>(short)":
            case "byte.CreateSaturating<ushort>(ushort)":
            case "byte.CreateSaturating<int>(int)":
            case "byte.CreateSaturating<uint>(uint)":
            case "byte.CreateSaturating<long>(long)":
            case "byte.CreateSaturating<ulong>(ulong)":
            case "byte.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(byte.MinValue);
                    Push(byte.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "sbyte.CreateSaturating<sbyte>(sbyte)":
            case "sbyte.CreateSaturating<byte>(byte)":
            case "sbyte.CreateSaturating<short>(short)":
            case "sbyte.CreateSaturating<ushort>(ushort)":
            case "sbyte.CreateSaturating<int>(int)":
            case "sbyte.CreateSaturating<uint>(uint)":
            case "sbyte.CreateSaturating<long>(long)":
            case "sbyte.CreateSaturating<ulong>(ulong)":
            case "sbyte.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(sbyte.MinValue);
                    Push(sbyte.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "short.CreateSaturating<byte>(byte)":
            case "short.CreateSaturating<sbyte>(sbyte)":
            case "short.CreateSaturating<short>(short)":
            case "short.CreateSaturating<ushort>(ushort)":
            case "short.CreateSaturating<int>(int)":
            case "short.CreateSaturating<uint>(uint)":
            case "short.CreateSaturating<long>(long)":
            case "short.CreateSaturating<ulong>(ulong)":
            case "short.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(short.MinValue);
                    Push(short.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "ushort.CreateSaturating<byte>(byte)":
            case "ushort.CreateSaturating<sbyte>(sbyte)":
            case "ushort.CreateSaturating<ushort>(ushort)":
            case "ushort.CreateSaturating<short>(short)":
            case "ushort.CreateSaturating<int>(int)":
            case "ushort.CreateSaturating<uint>(uint)":
            case "ushort.CreateSaturating<long>(long)":
            case "ushort.CreateSaturating<ulong>(ulong)":
            case "ushort.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(ushort.MinValue);
                    Push(ushort.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "long.CreateSaturating<int>(int)":
            case "long.CreateSaturating<uint>(uint)":
            case "long.CreateSaturating<byte>(byte)":
            case "long.CreateSaturating<sbyte>(sbyte)":
            case "long.CreateSaturating<short>(short)":
            case "long.CreateSaturating<long>(long)":
            case "long.CreateSaturating<ushort>(ushort)":
            case "long.CreateSaturating<ulong>(ulong)":
            case "long.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(long.MinValue);
                    Push(long.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "ulong.CreateSaturating<int>(int)":
            case "ulong.CreateSaturating<uint>(uint)":
            case "ulong.CreateSaturating<byte>(byte)":
            case "ulong.CreateSaturating<sbyte>(sbyte)":
            case "ulong.CreateSaturating<ushort>(ushort)":
            case "ulong.CreateSaturating<short>(short)":
            case "ulong.CreateSaturating<long>(long)":
            case "ulong.CreateSaturating<ulong>(ulong)":
            case "ulong.CreateSaturating<char>(char)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    Push(ulong.MinValue);
                    Push(ulong.MaxValue);
                    var endTarget = new JumpTarget();
                    var exceptionTarget = new JumpTarget();
                    var minTarget = new JumpTarget();
                    var maxTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);// 5 0 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 0
                    AddInstruction(OpCode.DUP);// 5 10 10 0 0
                    AddInstruction(OpCode.ROT);// 5 10 0 0 10
                    Jump(OpCode.JMPLT, exceptionTarget);// 5 10 0
                    AddInstruction(OpCode.THROW);
                    exceptionTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.ROT);// 10 0 5
                    AddInstruction(OpCode.DUP);// 10 0 5 5
                    AddInstruction(OpCode.ROT);// 10 5 5 0
                    AddInstruction(OpCode.DUP);// 10 5 5 0 0
                    AddInstruction(OpCode.ROT);// 10 5 0 0 5
                    Jump(OpCode.JMPGT, minTarget);// 10 5 0
                    AddInstruction(OpCode.DROP);// 10 5
                    AddInstruction(OpCode.DUP);// 10 5 5
                    AddInstruction(OpCode.ROT);// 5 5 10
                    AddInstruction(OpCode.DUP);// 5 5 10 10
                    AddInstruction(OpCode.ROT);// 5 10 10 5
                    Jump(OpCode.JMPLT, maxTarget);// 5 10
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    minTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    maxTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
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
            // https://learn.microsoft.com/en-us/dotnet/api/system.string.compare?view=net-8.0
            case "string.Compare(string?, string?)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    // if left < right return -1;
                    // if left = right return 0;
                    // if left > right return 1;
                    // Less than zero	  The first substring precedes the second substring in the sort order.
                    // Zero	              The substrings occur in the same position in the sort order, or length is zero.
                    // Greater than zero  The first substring follows the second substring in the sort order.
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.SIGN);
                    return true;
                }
            case "string.IsNullOrEmpty(string?)":
                {
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                    JumpTarget endTarget = new();
                    JumpTarget nullOrEmptyTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF, nullOrEmptyTarget);
                    AddInstruction(OpCode.SIZE);
                    Push(0);
                    AddInstruction(OpCode.NUMEQUAL);
                    Jump(OpCode.JMP, endTarget);
                    nullOrEmptyTarget.Instruction = AddInstruction(OpCode.DROP); // drop the duped item
                    AddInstruction(OpCode.PUSHT);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            //Retrieves a substring from this instance.
            //The substring starts at a specified character position and has a specified length.
            case "string.Substring(int, int)":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                if (arguments is not null)
                    PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
                AddInstruction(OpCode.SUBSTR);
                return true;
            // Returns a value indicating whether a specified substring occurs within this string.
            case "string.Contains(string)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
                    AddInstruction(OpCode.PUSH0);
                    AddInstruction(OpCode.GE);
                    return true;
                }
            // Determines whether the end of this string instance matches the specified string.
            case "string.EndsWith(string)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    var endTarget = new JumpTarget();
                    var validCountTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    AddInstruction(OpCode.DUP);
                    Push(3);
                    AddInstruction(OpCode.ROLL);
                    AddInstruction(OpCode.SWAP);
                    AddInstruction(OpCode.SUB);
                    AddInstruction(OpCode.DUP);
                    Push(0);
                    Jump(OpCode.JMPGT, validCountTarget);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.PUSHF);
                    Jump(OpCode.JMP, endTarget);
                    validCountTarget.Instruction = AddInstruction(OpCode.NOP);
                    Push(3);
                    AddInstruction(OpCode.ROLL);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.SUBSTR);
                    ChangeType(StackItemType.ByteString);
                    AddInstruction(OpCode.EQUAL);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            // Reports the zero-based index of the first occurrence of the specified string in this instance.
            case "string.IndexOf(string)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    if (instanceExpression is not null)
                        ConvertExpression(model, instanceExpression);
                    CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
                    return true;
                }
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

            #region char methods
            case "char.IsDigit(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push((ushort)'0');
                    Push((ushort)'9' + 1);
                    AddInstruction(OpCode.WITHIN);
                    return true;
                }
            case "char.IsLetter(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'A');
                    Push((ushort)'Z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.SWAP);
                    Push((ushort)'a');
                    Push((ushort)'z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.BOOLOR);
                    return true;
                }
            case "char.IsWhiteSpace(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    var endTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'\t');
                    Push((ushort)'\r' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    Push((ushort)'\n');
                    Push((ushort)' ' + 1);
                    AddInstruction(OpCode.WITHIN);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsLower(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push((ushort)'a');
                    Push((ushort)'z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    return true;
                }
            case "char.ToLower(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'A');
                    Push((ushort)'Z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    var endTarget = new JumpTarget();
                    Jump(OpCode.JMPIFNOT, endTarget);
                    Push((ushort)'A');
                    AddInstruction(OpCode.SUB);
                    Push((ushort)'a');
                    AddInstruction(OpCode.ADD);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsUpper(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push((ushort)'A');
                    Push((ushort)'Z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    return true;
                }
            case "char.ToUpper(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'a');
                    Push((ushort)'z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    var endTarget = new JumpTarget();
                    Jump(OpCode.JMPIFNOT, endTarget);
                    Push((ushort)'a');
                    AddInstruction(OpCode.SUB);
                    Push((ushort)'A');
                    AddInstruction(OpCode.ADD);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsPunctuation(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    var endTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'!');
                    Push((ushort)'/' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)':');
                    Push((ushort)'@' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'[');
                    Push((ushort)'`' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    Push((ushort)'{');
                    Push((ushort)'~' + 1);
                    AddInstruction(OpCode.WITHIN);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsSymbol(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    var endTarget = new JumpTarget();
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'$');
                    Push((ushort)'+' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'<');
                    Push((ushort)'=' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'>');
                    Push((ushort)'@' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'[');
                    Push((ushort)'`' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    Push((ushort)'{');
                    Push((ushort)'~' + 1);
                    AddInstruction(OpCode.WITHIN);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsControl(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'\0');
                    Push((ushort)'\x1F' + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.SWAP);
                    Push((ushort)'\x7F');
                    Push((ushort)'\x9F' + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.BOOLOR);
                    return true;
                }
            case "char.IsSurrogate(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)0xD800);
                    Push((ushort)0xDBFF + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.SWAP);
                    Push((ushort)0xDC00);
                    Push((ushort)0xDFFF + 1);
                    AddInstruction(OpCode.WITHIN);
                    AddInstruction(OpCode.BOOLOR);
                    return true;
                }
            case "char.IsHighSurrogate(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push((ushort)0xD800);
                    Push((ushort)0xDBFF + 1);
                    AddInstruction(OpCode.WITHIN);
                    return true;
                }
            case "char.IsLowSurrogate(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    Push((ushort)0xDC00);
                    Push((ushort)0xDFFF + 1);
                    AddInstruction(OpCode.WITHIN);
                    return true;
                }
            case "char.IsLetterOrDigit(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'0');
                    Push((ushort)'9' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'A');
                    Push((ushort)'Z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, endTarget);
                    Push((ushort)'a');
                    Push((ushort)'z' + 1);
                    AddInstruction(OpCode.WITHIN);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.IsBetween(char, char, char)": //min max
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget validTarget = new();
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ROT);
                    AddInstruction(OpCode.GE);
                    AddInstruction(OpCode.DUP);
                    Jump(OpCode.JMPIFNOT, validTarget);
                    AddInstruction(OpCode.REVERSE3);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.DROP);
                    Jump(OpCode.JMP, endTarget);
                    validTarget.Instruction = AddInstruction(OpCode.NOP);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.LT);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }
            case "char.GetNumericValue(char)":
                {
                    if (arguments is not null)
                        PrepareArgumentsForMethod(model, symbol, arguments);
                    JumpTarget endTarget = new();
                    JumpTarget validTarget = new();
                    AddInstruction(OpCode.DUP);
                    Push((ushort)'0');
                    Push((ushort)'9' + 1);
                    AddInstruction(OpCode.WITHIN);
                    Jump(OpCode.JMPIF, validTarget);
                    AddInstruction(OpCode.DROP);
                    AddInstruction(OpCode.PUSHM1);
                    Jump(OpCode.JMP, endTarget);
                    validTarget.Instruction = AddInstruction(OpCode.NOP);
                    Push((ushort)'0');
                    AddInstruction(OpCode.SUB);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    return true;
                }

            #endregion
            //Non-system methods, such as user-defined methods
            default:
                return false;
        }
    }
}
