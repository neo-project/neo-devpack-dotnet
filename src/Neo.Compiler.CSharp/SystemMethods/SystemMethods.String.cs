using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;


namespace Neo.Compiler;

partial class MethodConvert
{

    private bool TryProcessString(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        switch (symbol.ToString())
        {

            #region Method of string
            //Gets the total number of elements in all the dimensions of the Array.
            case "System.Array.Length.get":
            //Gets the number of characters in the current String object.
            case "string.Length.get":
                if (instanceExpression is not null)
                    ConvertExpression(model, instanceExpression);
                AddInstruction(OpCode.SIZE);
                return true;
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
            #endregion

            //Non-system methods, such as user-defined methods
            default:
                return false;
        }
    }
}
