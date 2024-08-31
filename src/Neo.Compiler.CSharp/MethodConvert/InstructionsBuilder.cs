// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Buffer = System.Buffer;
using Neo.SmartContract.Native;

namespace Neo.Compiler
{
    internal class InstructionsBuilder
    {
        internal List<Instruction> Instructions { get; } = [];

        internal bool IsEmpty => Instructions.Count == 0
                               || Instructions is [{ OpCode: OpCode.RET }]
                               || Instructions is [{ OpCode: OpCode.INITSLOT }, { OpCode: OpCode.RET }];


        internal Instruction AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
            return instruction;
        }

        internal Instruction AddInstruction(OpCode opcode)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode
            });
        }

        #region Constants

        internal Instruction JumpLong(OpCode opcode, JumpTarget target)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Target = target
            });
        }

        internal Instruction Jmp(JumpTarget target)
        {
            return Jump(OpCode.JMP, target);
        }

        internal Instruction JmpL(JumpTarget target)
        {
            return JumpLong(OpCode.JMP_L, target);
        }

        internal Instruction JmpIf(JumpTarget target)
        {
            return Jump(OpCode.JMPIF, target);
        }

        internal Instruction JmpIfL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPIF_L, target);
        }

        internal Instruction JmpIfNot(JumpTarget target)
        {
            return Jump(OpCode.JMPIFNOT, target);
        }

        internal Instruction JmpIfNotL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPIFNOT_L, target);
        }

        internal Instruction JmpEq(JumpTarget target)
        {
            return Jump(OpCode.JMPEQ, target);
        }

        internal Instruction JmpEqL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPEQ_L, target);
        }

        internal Instruction JmpNe(JumpTarget target)
        {
            return Jump(OpCode.JMPNE, target);
        }

        internal Instruction JmpNeL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPNE_L, target);
        }

        internal Instruction JmpGt(JumpTarget target)
        {
            return Jump(OpCode.JMPGT, target);
        }

        internal Instruction JmpGtL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPGT_L, target);
        }

        internal Instruction JmpGe(JumpTarget target)
        {
            return Jump(OpCode.JMPGE, target);
        }

        internal Instruction JmpGeL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPGE_L, target);
        }

        internal Instruction JmpLt(JumpTarget target)
        {
            return Jump(OpCode.JMPLT, target);
        }

        internal Instruction JmpLtL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPLT_L, target);
        }

        internal Instruction JmpLe(JumpTarget target)
        {
            return Jump(OpCode.JMPLE, target);
        }

        internal Instruction JmpLeL(JumpTarget target)
        {
            return JumpLong(OpCode.JMPLE_L, target);
        }

        internal Instruction Abort() => AddInstruction(OpCode.ABORT);

        internal Instruction Assert() => AddInstruction(OpCode.ASSERT);

        internal Instruction Debug(string message)
        {
            PushTrue();
            Push(message);
            return AddInstruction(OpCode.ASSERTMSG);
        }

        internal Instruction Nop() => AddInstruction(OpCode.NOP);

        internal Instruction Throw(string? message = null)
        {
            if (message is not null)
                Push(message);
            return AddInstruction(OpCode.THROW);
        }

        #endregion

        #region Stack

        internal Instruction Depth() => AddInstruction(OpCode.DEPTH);

        internal Instruction Drop(int count = 1)
        {
            for (var i = 0; i < count - 1; i++)
            {
                AddInstruction(OpCode.DROP);
            }

            return AddInstruction(OpCode.DROP);
        }

        internal Instruction Nip() => AddInstruction(OpCode.NIP);

        internal void AddTarget(JumpTarget target)
        {
            target.Instruction = AddInstruction(OpCode.NOP);
        }

        /// <summary>
        /// Remove n items from the stack.
        /// Try to use <see cref="OpCode.DROP"/> as much as possible,
        /// it is more efficient.
        /// </summary>
        /// <param name="count">Number of stack items to be removed.</param>
        /// <returns>The instruction of XDrop.</returns>
        internal Instruction XDrop(int? count = null)
        {
            if (count.HasValue)
                Push(count.Value);
            return AddInstruction(OpCode.XDROP);
        }

        internal Instruction Clear() => AddInstruction(OpCode.CLEAR);

        internal Instruction Dup() => AddInstruction(OpCode.DUP);

        internal Instruction Over() => AddInstruction(OpCode.OVER);

        internal Instruction Pick(int? index = null)
        {
            if (index.HasValue)
                Push(index.Value);
            return AddInstruction(OpCode.PICK);
        }

        internal Instruction Tuck() => AddInstruction(OpCode.TUCK);

        internal Instruction Swap() => AddInstruction(OpCode.SWAP);

        internal Instruction Rot() => AddInstruction(OpCode.ROT);

        internal Instruction Roll(int? index = null)
        {
            if (index.HasValue)
                Push(index.Value);
            return AddInstruction(OpCode.ROLL);
        }

        internal Instruction Reverse3() => AddInstruction(OpCode.REVERSE3);

        internal Instruction Reverse4() => AddInstruction(OpCode.REVERSE4);

        /// <summary>
        /// Reverse the order of the top N items on the stack.
        /// stack bottom: item0 item1 ... itemN -> stack: itemN-1 ... item0
        /// </summary>
        /// <param name="count">The number of items to reverse.</param>
        /// <returns>The instruction of ReverseN.</returns>
        internal Instruction ReverseN(int? count)
        {
            if (count.HasValue)
                Push(count.Value);
            return AddInstruction(OpCode.REVERSEN);
        }

        #endregion

        #region Splice

        /// <summary>
        /// Create a new buffer.
        /// stack bottom: size -> stack: sized-buffer
        /// </summary>
        /// <param name="size">The size of the buffer to create.</param>
        /// <returns>The instruction of NewBuffer.</returns>
        internal Instruction NewBuffer(int? size = null)
        {
            if (size.HasValue)
                Push(size.Value);
            return AddInstruction(OpCode.NEWBUFFER);
        }

        internal Instruction Memcpy() => AddInstruction(OpCode.MEMCPY);

        internal Instruction Cat() => AddInstruction(OpCode.CAT);

        internal Instruction SubStr() => AddInstruction(OpCode.SUBSTR);

        internal Instruction Left(int? length)
        {
            if (length.HasValue)
                Push(length.Value);
            return AddInstruction(OpCode.LEFT);
        }

        internal Instruction Right(int? length)
        {
            if (length.HasValue)
                Push(length.Value);
            return AddInstruction(OpCode.RIGHT);
        }

        #endregion

        #region Bitwise logic

        internal Instruction Invert() => AddInstruction(OpCode.INVERT);

        internal Instruction And(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.AND);
        }

        internal Instruction Or(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.OR);
        }

        internal Instruction Xor(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.XOR);
        }

        internal Instruction Equal(string? value = null)
        {
            if (value is not null)
                Push(value);
            return AddInstruction(OpCode.EQUAL);
        }

        internal Instruction NotEqual(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.NOTEQUAL);
        }

        #endregion

        #region Arithmetic

        internal Instruction Sign() => AddInstruction(OpCode.SIGN);

        internal Instruction Abs() => AddInstruction(OpCode.ABS);

        internal Instruction Negate() => AddInstruction(OpCode.NEGATE);

        internal Instruction Inc() => AddInstruction(OpCode.INC);

        internal Instruction Dec() => AddInstruction(OpCode.DEC);

        internal Instruction Add(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.ADD);
        }

        internal Instruction Sub(BigInteger? value = null)
        {
            // stack bottom: a b -> stack: a-b
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.SUB);
        }

        /// <summary>
        /// Multiply two values.
        /// stack bottom: a b -> stack: a*b
        /// </summary>
        /// <param name="value">The value to multiply with.</param>
        /// <returns>The instruction of Mul.</returns>
        internal Instruction Mul(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.MUL);
        }

        /// <summary>
        /// Divide two values.
        /// stack bottom: a b -> stack: a/b
        /// </summary>
        /// <param name="value">The value to divide by.</param>
        /// <returns>The instruction of Div.</returns>  
        internal Instruction Div(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.DIV);
        }

        /// <summary>
        /// Calculate the remainder of two values.
        /// stack bottom: a b -> stack: a%b
        /// </summary>
        /// <param name="value">The value to divide by.</param>
        /// <returns>The instruction of Mod.</returns>  
        internal Instruction Mod(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.MOD);
        }

        internal Instruction Pow() => AddInstruction(OpCode.POW);

        internal Instruction Sqrt() => AddInstruction(OpCode.SQRT);

        internal Instruction ModMul() => AddInstruction(OpCode.MODMUL);

        internal Instruction ModPow() => AddInstruction(OpCode.MODPOW);

        /// <summary>
        /// Calculate the result of bitwise left shift operation.
        /// stack bottom: value count -> stack: value << count
        /// </summary>
        /// <param name="count">The number of bits to shift.</param>
        /// <returns>The instruction of ShL.</returns>
        internal Instruction ShL(BigInteger? count = null)
        {
            if (count.HasValue)
                Push(count.Value);
            return AddInstruction(OpCode.SHL);
        }

        /// <summary>
        /// Calculate the result of bitwise right shift operation.
        /// stack bottom: value count -> stack: value >> count
        /// </summary>
        /// <param name="count">The number of bits to shift.</param>
        /// <returns>The instruction of ShR.</returns>
        internal Instruction ShR(BigInteger? count = null)
        {
            if (count.HasValue)
                Push(count.Value);
            return AddInstruction(OpCode.SHR);
        }

        internal Instruction Not() => AddInstruction(OpCode.NOT);

        internal Instruction BoolAnd() => AddInstruction(OpCode.BOOLAND);

        internal Instruction BoolOr() => AddInstruction(OpCode.BOOLOR);

        internal Instruction Nz() => AddInstruction(OpCode.NZ);

        /// <summary>
        /// Compare two values for equality.
        /// stack bottom: a b -> stack: a == b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of NumEqual.</returns>
        internal Instruction NumEqual(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.NUMEQUAL);
        }

        /// <summary>
        /// Compare two values for inequality.
        /// stack bottom: a b -> stack: a != b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of NumNotEqual.</returns>
        internal Instruction NumNotEqual(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.NUMNOTEQUAL);
        }

        /// <summary>
        /// Compare two values for less than.
        /// stack bottom: a b -> stack: a < b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of Lt.</returns>
        internal Instruction Lt(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.LT);
        }

        /// <summary>
        /// Compare two values for less than or equal to.
        /// stack bottom: a b -> stack: a <= b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of Le.</returns>    
        internal Instruction Le(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.LE);
        }

        /// <summary>
        /// Compare two values for greater than.
        /// stack bottom: a b -> stack: a > b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of Gt.</returns>
        internal Instruction Gt(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.GT);
        }

        /// <summary>
        /// Compare two values for greater than or equal to.
        /// stack bottom: a b -> stack: a >= b
        /// </summary>
        /// <param name="value">The value to compare with.</param>
        /// <returns>The instruction of Ge.</returns>
        internal Instruction Ge(BigInteger? value = null)
        {
            if (value.HasValue)
                Push(value.Value);
            return AddInstruction(OpCode.GE);
        }

        /// <summary>
        /// Compare two values and return the smaller one.
        /// stack bottom: a b -> stack: min(a, b)
        /// </summary>
        /// <returns>The instruction of Min.</returns>
        internal Instruction Min() => AddInstruction(OpCode.MIN);

        /// <summary>
        /// Compare two values and return the larger one.
        /// stack bottom: a b -> stack: max(a, b)
        /// </summary>
        /// <returns>The instruction of Max.</returns>
        internal Instruction Max() => AddInstruction(OpCode.MAX);

        /// <summary>
        /// Check if a value is within a range, inclusive.
        /// stack bottom: value min max -> stack: result of the comparison
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The instruction of Within.</returns>
        internal Instruction Within(BigInteger min, BigInteger max)
        {
            Push(min);
            Push(max + 1);
            return AddInstruction(OpCode.WITHIN);
        }

        /// <summary>
        /// Check if a value is a valid byte.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsByte() => Within(byte.MinValue, byte.MaxValue);

        /// <summary>
        /// Check if a value is a valid signed byte.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsSByte() => Within(sbyte.MinValue, sbyte.MaxValue);

        /// <summary>
        /// Check if a value is a valid short.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsShort() => Within(short.MinValue, short.MaxValue);

        /// <summary>
        /// Check if a value is a valid unsigned short.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsUShort() => Within(ushort.MinValue, ushort.MaxValue);

        /// <summary>
        /// Check if a value is a valid int.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsInt() => Within(int.MinValue, int.MaxValue);

        /// <summary>
        /// Check if a value is a valid unsigned int.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsLong() => Within(long.MinValue, long.MaxValue);

        /// <summary>
        /// Check if a value is a valid unsigned long.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsULong() => Within(ulong.MinValue, ulong.MaxValue);

        /// <summary>
        /// Check if a value is an uppercase character.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsUpperChar() => Within((ushort)'A', (ushort)'Z');

        /// <summary>
        /// Check if a value is a lowercase character.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        /// <returns>The instruction of Within.</returns>
        internal Instruction IsLowerChar() => Within((ushort)'a', (ushort)'z');

        /// <summary>
        /// Check if a value is a digit character.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        internal Instruction IsDigitChar() => Within((ushort)'0', (ushort)'9');

        /// <summary>
        /// Check if a value is a letter character.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        internal void IsLetterChar()
        {
            Within((ushort)'A', (ushort)'Z');
            Or();
            Within((ushort)'a', (ushort)'z');
        }

        /// <summary>
        /// Check if a value is a digit or letter character.
        /// stack bottom: value -> stack: result of the comparison
        /// </summary>
        internal void IsDigitOrLetterChar()
        {
            IsDigitChar();
            Or();
            IsLetterChar();
        }


        /// <summary>
        /// Check if a value is within a range, inclusive.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void WithinCheck(BigInteger min, BigInteger max, string? message = null)
        {
            JumpTarget endTarget = new();
            AddInstruction(OpCode.DUP);
            Within(min, max);
            Jump(OpCode.JMPIF, endTarget);
            Throw(message);
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }

        /// <summary>
        /// Check if a value is a valid byte.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsByteCheck() => WithinCheck(byte.MinValue, byte.MaxValue, "Not a valid byte value.");

        /// <summary>
        /// Check if a value is a valid signed byte.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsSByteCheck() => WithinCheck(sbyte.MinValue, sbyte.MaxValue, "Not a valid sbyte value.");

        /// <summary>
        /// Check if a value is a valid short.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsShortCheck() => WithinCheck(short.MinValue, short.MaxValue, "Not a valid short value.");

        /// <summary>
        /// Check if a value is a valid unsigned short.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsUShortCheck() => WithinCheck(ushort.MinValue, ushort.MaxValue, "Not a valid ushort value.");

        /// <summary>
        /// Check if a value is a valid int.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsIntCheck() => WithinCheck(int.MinValue, int.MaxValue, "Not a valid int value.");

        /// <summary>
        /// Check if a value is a valid unsigned int.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsUIntCheck() => WithinCheck(uint.MinValue, uint.MaxValue, "Not a valid uint value.");

        /// <summary>
        /// Check if a value is a valid long.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsLongCheck() => WithinCheck(long.MinValue, long.MaxValue, "Not a valid long value.");

        /// <summary>
        /// Check if a value is a valid unsigned long.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="message">The message to display if the value is not within the range.</param>
        /// <exception cref="Exception">Thrown if the value is not within the range.</exception>
        internal void IsULongCheck() => WithinCheck(ulong.MinValue, ulong.MaxValue, "Not a valid ulong value.");

        /// <summary>
        /// Convert a string to an integer.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="methodConvert">The method convert.</param>
        /// <returns>The instruction of the conversion.</returns>
        internal Instruction Atoi(MethodConvert methodConvert)
        {
            return methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        }

        /// <summary>
        /// Search for a value in memory span.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="methodConvert">The method convert.</param>
        /// <returns>The instruction of the search.</returns>
        internal Instruction MemorySearch(MethodConvert methodConvert)
        {
            return methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "memorySearch", 2, true);
        }

        /// <summary>
        /// Convert an integer to a string.
        /// stack bottom: value -> stack: value
        /// </summary>
        /// <param name="methodConvert">The method convert.</param>
        /// <returns>The instruction of the conversion.</returns>
        internal Instruction Itoa(MethodConvert methodConvert)
        {
            return methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        }

        #endregion

        #region Compound-type

        internal Instruction PackMap() => AddInstruction(OpCode.PACKMAP);

        internal Instruction PackStruct() => AddInstruction(OpCode.PACKSTRUCT);

        /// <summary>
        /// Pack the stack into a byte array.   
        /// stack bottom: item0 item1 ... itemN -> stack: n-sized array
        /// </summary>
        /// <param name="size">The number of items in the stack to be packed.</param>
        /// <returns>The instruction of Pack.</returns>
        internal Instruction Pack(int? size = null)
        {
            if (size.HasValue)
                Push(size.Value);
            return AddInstruction(OpCode.PACK);
        }

        internal Instruction UnPack() => AddInstruction(OpCode.UNPACK);

        internal Instruction NewArray0() => AddInstruction(OpCode.NEWARRAY0);

        /// <summary>
        /// Create a new array.
        /// stack bottom: count -> stack: array     
        /// </summary>
        /// <param name="count">The number of elements in the array.</param>
        /// <returns>The instruction of NewArray.</returns>
        internal Instruction NewArray(int? count)
        {
            if (count.HasValue)
            {
                if (count.Value == 0)
                    return NewArray0();
                Push(count.Value);
            }

            return AddInstruction(OpCode.NEWARRAY);
        }

        internal Instruction NewStruct0() => AddInstruction(OpCode.NEWSTRUCT0);

        internal Instruction NewStruct(int? count)
        {
            if (count.HasValue)
                Push(count.Value);
            return AddInstruction(OpCode.NEWSTRUCT);
        }

        internal Instruction Size() => AddInstruction(OpCode.SIZE);

        internal Instruction HasKey() => AddInstruction(OpCode.HASKEY);

        internal Instruction Keys() => AddInstruction(OpCode.KEYS);

        internal Instruction Values() => AddInstruction(OpCode.VALUES);

        internal Instruction PickItem(int? index = null)
        {
            if (index.HasValue)
                Push(index.Value);
            return AddInstruction(OpCode.PICKITEM);
        }

        internal Instruction Append() => AddInstruction(OpCode.APPEND);

        internal Instruction SetItem() => AddInstruction(OpCode.SETITEM);

        internal Instruction ReverseItems() => AddInstruction(OpCode.REVERSEITEMS);

        internal Instruction Remove() => AddInstruction(OpCode.REMOVE);

        internal Instruction ClearItems() => AddInstruction(OpCode.CLEARITEMS);

        internal Instruction PopItem() => AddInstruction(OpCode.POPITEM);

        #endregion

        #region Types

        internal Instruction IsNull() => AddInstruction(OpCode.ISNULL);

        internal Instruction Istype(StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.ISTYPE,
                Operand = [(byte)type]
            });
        }

        internal Instruction CONVERT(StackItemType type)
        {
            return ChangeType(type);
        }

        #endregion

        #region Extensions

        internal Instruction Abortmsg() => AddInstruction(OpCode.ABORTMSG);

        internal Instruction Assertmsg() => AddInstruction(OpCode.ASSERTMSG);

        #endregion

        #region Constants

        internal Instruction PushInt8(sbyte value)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT8,
                Operand = [unchecked((byte)value)]
            });
        }

        internal Instruction PushInt16(short value)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT16,
                Operand = BitConverter.GetBytes(value)
            });
        }

        internal Instruction PushInt32(int value)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT32,
                Operand = BitConverter.GetBytes(value)
            });
        }

        internal Instruction PushInt64(long value)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT64,
                Operand = BitConverter.GetBytes(value)
            });
        }

        internal Instruction PushInt128(BigInteger value)
        {
            byte[] bytes = value.ToByteArray(isUnsigned: false, isBigEndian: false);
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT128,
                Operand = PadRight(bytes, bytes.Length, 16, value.Sign < 0).ToArray()
            });
        }

        internal Instruction PushInt256(BigInteger value)
        {
            byte[] bytes = value.ToByteArray(isUnsigned: false, isBigEndian: false);
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHINT256,
                Operand = PadRight(bytes, bytes.Length, 32, value.Sign < 0).ToArray()
            });
        }

        internal Instruction PushTrue() => AddInstruction(OpCode.PUSHT);

        internal Instruction PushFalse() => AddInstruction(OpCode.PUSHF);

        internal Instruction PushA(int offset)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.PUSHA,
                Operand = BitConverter.GetBytes(offset)
            });
        }

        internal Instruction PushNull()
        {
            return AddInstruction(OpCode.PUSHNULL);
        }

        internal Instruction PushData(ReadOnlySpan<byte> data)
        {
            if (data.Length < byte.MaxValue)
            {
                byte[] operand = new byte[data.Length + 1];
                operand[0] = (byte)data.Length;
                data.CopyTo(operand.AsSpan(1));
                return AddInstruction(new Instruction
                {
                    OpCode = OpCode.PUSHDATA1,
                    Operand = operand
                });
            }
            else if (data.Length < ushort.MaxValue)
            {
                byte[] operand = new byte[data.Length + 2];
                BinaryPrimitives.WriteUInt16LittleEndian(operand, (ushort)data.Length);
                data.CopyTo(operand.AsSpan(2));
                return AddInstruction(new Instruction
                {
                    OpCode = OpCode.PUSHDATA2,
                    Operand = operand
                });
            }
            else
            {
                byte[] operand = new byte[data.Length + 4];
                BinaryPrimitives.WriteInt32LittleEndian(operand, data.Length);
                data.CopyTo(operand.AsSpan(4));
                return AddInstruction(new Instruction
                {
                    OpCode = OpCode.PUSHDATA4,
                    Operand = operand
                });
            }
        }

        internal Instruction PushM1() => AddInstruction(OpCode.PUSHM1);

        internal Instruction Push0() => AddInstruction(OpCode.PUSH0);

        internal Instruction Push1() => AddInstruction(OpCode.PUSH1);

        internal Instruction Push2() => AddInstruction(OpCode.PUSH2);

        internal Instruction Push3() => AddInstruction(OpCode.PUSH3);

        internal Instruction Push4() => AddInstruction(OpCode.PUSH4);

        internal Instruction Push5() => AddInstruction(OpCode.PUSH5);

        internal Instruction Push6() => AddInstruction(OpCode.PUSH6);

        internal Instruction Push7() => AddInstruction(OpCode.PUSH7);

        internal Instruction Push8() => AddInstruction(OpCode.PUSH8);

        internal Instruction Push9() => AddInstruction(OpCode.PUSH9);

        internal Instruction Push10() => AddInstruction(OpCode.PUSH10);

        internal Instruction Push11() => AddInstruction(OpCode.PUSH11);

        internal Instruction Push12() => AddInstruction(OpCode.PUSH12);

        internal Instruction Push13() => AddInstruction(OpCode.PUSH13);

        internal Instruction Push14() => AddInstruction(OpCode.PUSH14);

        internal Instruction Push15() => AddInstruction(OpCode.PUSH15);

        internal Instruction Push16() => AddInstruction(OpCode.PUSH16);

        #endregion

        #region Slot

        internal Instruction InitSSlot(byte count)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.INITSSLOT,
                Operand = [count]
            });
        }

        internal Instruction InitSlot(byte localCount, byte argumentCount)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.INITSLOT,
                Operand = [localCount, argumentCount]
            });
        }

        internal Instruction LdSFld0() => AddInstruction(OpCode.LDSFLD0);

        internal Instruction LdSFld1() => AddInstruction(OpCode.LDSFLD1);

        internal Instruction LdSFld2() => AddInstruction(OpCode.LDSFLD2);

        internal Instruction LdSFld3() => AddInstruction(OpCode.LDSFLD3);

        internal Instruction LdSFld4() => AddInstruction(OpCode.LDSFLD4);

        internal Instruction LdSFld5() => AddInstruction(OpCode.LDSFLD5);

        internal Instruction LdSFld6() => AddInstruction(OpCode.LDSFLD6);

        internal Instruction LdSFld(byte index)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.LDSFLD,
                Operand = [index]
            });
        }

        internal Instruction StSFld0() => AddInstruction(OpCode.STSFLD0);

        internal Instruction StSFld1() => AddInstruction(OpCode.STSFLD1);

        internal Instruction StSFld2() => AddInstruction(OpCode.STSFLD2);

        internal Instruction StSFld3() => AddInstruction(OpCode.STSFLD3);

        internal Instruction StSFld4() => AddInstruction(OpCode.STSFLD4);

        internal Instruction StSFld5() => AddInstruction(OpCode.STSFLD5);

        internal Instruction StSFld6() => AddInstruction(OpCode.STSFLD6);

        internal Instruction StSFld(byte index)
        {
            return AccessSlot(OpCode.STSFLD, index);
        }

        internal Instruction LdLoc0() => AddInstruction(OpCode.LDLOC0);

        internal Instruction LdLoc1() => AddInstruction(OpCode.LDLOC1);

        internal Instruction LdLoc2() => AddInstruction(OpCode.LDLOC2);

        internal Instruction LdLoc3() => AddInstruction(OpCode.LDLOC3);

        internal Instruction LdLoc4() => AddInstruction(OpCode.LDLOC4);

        internal Instruction LdLoc5() => AddInstruction(OpCode.LDLOC5);

        internal Instruction LdLoc6() => AddInstruction(OpCode.LDLOC6);

        internal Instruction LdLoc(byte index)
        {
            return AccessSlot(OpCode.LDLOC, index);
        }

        internal Instruction StLoc0() => AddInstruction(OpCode.STLOC0);

        internal Instruction StLoc1() => AddInstruction(OpCode.STLOC1);

        internal Instruction StLoc2() => AddInstruction(OpCode.STLOC2);

        internal Instruction StLoc3() => AddInstruction(OpCode.STLOC3);

        internal Instruction StLoc4() => AddInstruction(OpCode.STLOC4);

        internal Instruction StLoc5() => AddInstruction(OpCode.STLOC5);

        internal Instruction StLoc6() => AddInstruction(OpCode.STLOC6);

        internal Instruction StLoc(byte index)
        {
            return AccessSlot(OpCode.STLOC, index);
        }

        internal Instruction LdArg0() => AddInstruction(OpCode.LDARG0);

        internal Instruction LdArg1() => AddInstruction(OpCode.LDARG1);

        internal Instruction LdArg2() => AddInstruction(OpCode.LDARG2);

        internal Instruction LdArg3() => AddInstruction(OpCode.LDARG3);

        internal Instruction LdArg4() => AddInstruction(OpCode.LDARG4);

        internal Instruction LdArg5() => AddInstruction(OpCode.LDARG5);

        internal Instruction LdArg6() => AddInstruction(OpCode.LDARG6);

        internal Instruction LdArg(byte index)
        {
            return AccessSlot(OpCode.LDARG, index);
        }

        internal Instruction StArg0() => AddInstruction(OpCode.STARG0);

        internal Instruction StArg1() => AddInstruction(OpCode.STARG1);

        internal Instruction StArg2() => AddInstruction(OpCode.STARG2);

        internal Instruction StArg3() => AddInstruction(OpCode.STARG3);

        internal Instruction StArg4() => AddInstruction(OpCode.STARG4);

        internal Instruction StArg5() => AddInstruction(OpCode.STARG5);

        internal Instruction StArg6() => AddInstruction(OpCode.STARG6);

        internal Instruction StArg(byte index)
        {
            return AccessSlot(OpCode.STARG, index);
        }

        #endregion

        internal Instruction Ret() => AddInstruction(OpCode.RET);

        /// <summary>
        /// Insert an instruction into the instruction list.
        /// </summary>
        /// <param name="index">The index at which to insert the instruction.</param>
        /// <param name="instruction">The instruction to insert.</param>
        /// <returns>The inserted instruction.</returns>
        internal Instruction Insert(int index, Instruction instruction)
        {
            Instructions.Insert(index, instruction);
            return instruction;
        }

        internal Instruction Jump(OpCode opcode, JumpTarget target)
        {
            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Target = target
            });
        }

        internal void Push(bool value)
        {
            AddInstruction(value ? OpCode.PUSHT : OpCode.PUSHF);
        }

        internal Instruction Push(BigInteger number)
        {
            if (number >= -1 && number <= 16)
                return AddInstruction(number == -1 ? OpCode.PUSHM1 : OpCode.PUSH0 + (byte)(int)number);
            Span<byte> buffer = stackalloc byte[32];
            if (!number.TryWriteBytes(buffer, out var bytesWritten, isUnsigned: false, isBigEndian: false))
                throw new ArgumentOutOfRangeException(nameof(number));
            var instruction = bytesWritten switch
            {
                1 => new Instruction
                {
                    OpCode = OpCode.PUSHINT8,
                    Operand = PadRight(buffer, bytesWritten, 1, number.Sign < 0).ToArray()
                },
                2 => new Instruction
                {
                    OpCode = OpCode.PUSHINT16,
                    Operand = PadRight(buffer, bytesWritten, 2, number.Sign < 0).ToArray()
                },
                <= 4 => new Instruction
                {
                    OpCode = OpCode.PUSHINT32,
                    Operand = PadRight(buffer, bytesWritten, 4, number.Sign < 0).ToArray()
                },
                <= 8 => new Instruction
                {
                    OpCode = OpCode.PUSHINT64,
                    Operand = PadRight(buffer, bytesWritten, 8, number.Sign < 0).ToArray()
                },
                <= 16 => new Instruction
                {
                    OpCode = OpCode.PUSHINT128,
                    Operand = PadRight(buffer, bytesWritten, 16, number.Sign < 0).ToArray()
                },
                <= 32 => new Instruction
                {
                    OpCode = OpCode.PUSHINT256,
                    Operand = PadRight(buffer, bytesWritten, 32, number.Sign < 0).ToArray()
                },
                _ => throw new ArgumentOutOfRangeException($"Number too large: {bytesWritten}")
            };
            AddInstruction(instruction);
            return instruction;
        }

        internal Instruction Push(string s)
        {
            return Push(Utility.StrictUTF8.GetBytes(s));
        }

        internal Instruction Push(byte[] data)
        {
            OpCode opcode;
            byte[] buffer;
            switch (data.Length)
            {
                case <= byte.MaxValue:
                    opcode = OpCode.PUSHDATA1;
                    buffer = new byte[sizeof(byte) + data.Length];
                    buffer[0] = (byte)data.Length;
                    Buffer.BlockCopy(data, 0, buffer, sizeof(byte), data.Length);
                    break;
                case <= ushort.MaxValue:
                    opcode = OpCode.PUSHDATA2;
                    buffer = new byte[sizeof(ushort) + data.Length];
                    BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)data.Length);
                    Buffer.BlockCopy(data, 0, buffer, sizeof(ushort), data.Length);
                    break;
                default:
                    opcode = OpCode.PUSHDATA4;
                    buffer = new byte[sizeof(uint) + data.Length];
                    BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)data.Length);
                    Buffer.BlockCopy(data, 0, buffer, sizeof(uint), data.Length);
                    break;
            }

            return AddInstruction(new Instruction
            {
                OpCode = opcode,
                Operand = buffer
            });
        }

        internal void Push(object? obj)
        {
            switch (obj)
            {
                case bool data:
                    Push(data);
                    break;
                case byte[] data:
                    Push(data);
                    break;
                case string data:
                    Push(data);
                    break;
                case BigInteger data:
                    Push(data);
                    break;
                case char data:
                    Push((ushort)data);
                    break;
                case sbyte data:
                    Push(data);
                    break;
                case byte data:
                    Push(data);
                    break;
                case short data:
                    Push(data);
                    break;
                case ushort data:
                    Push(data);
                    break;
                case int data:
                    Push(data);
                    break;
                case uint data:
                    Push(data);
                    break;
                case long data:
                    Push(data);
                    break;
                case ulong data:
                    Push(data);
                    break;
                case Enum data:
                    Push(BigInteger.Parse(data.ToString("d")));
                    break;
                case null:
                    AddInstruction(OpCode.PUSHNULL);
                    break;
                case float or double or decimal:
                    throw new CompilationException(DiagnosticId.FloatingPointNumber,
                        "Floating-point numbers are not supported.");
                default:
                    throw new NotSupportedException($"Unsupported constant value: {obj}");
            }
        }

        internal Instruction PushDefault(ITypeSymbol type)
        {
            return AddInstruction(type.GetStackItemType() switch
            {
                VM.Types.StackItemType.Boolean or VM.Types.StackItemType.Integer => OpCode.PUSH0,
                _ => OpCode.PUSHNULL,
            });
        }

        // Helper method to reverse stack items
        internal void ReverseStackItems(int count)
        {
            switch (count)
            {
                case 2:
                    AddInstruction(OpCode.SWAP);
                    break;
                case 3:
                    AddInstruction(OpCode.REVERSE3);
                    break;
                case 4:
                    AddInstruction(OpCode.REVERSE4);
                    break;
                default:
                    Push(count);
                    AddInstruction(OpCode.REVERSEN);
                    break;
            }
        }

        internal Instruction IsType(StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.ISTYPE,
                Operand = [(byte)type]
            });
        }

        internal Instruction ChangeType(StackItemType type)
        {
            return AddInstruction(new Instruction
            {
                OpCode = OpCode.CONVERT,
                Operand = [(byte)type]
            });
        }

        /// <summary>
        /// Pad the buffer with the specified length.   
        /// </summary>
        /// <param name="buffer">The buffer to pad.</param>
        /// <param name="dataLength">The length of the data to pad.</param>
        /// <param name="padLength">The length of the padding to add.</param>
        /// <param name="negative">Whether the padding should be negative.</param>
        /// <returns>The padded buffer.</returns>
        internal static ReadOnlySpan<byte> PadRight(Span<byte> buffer, int dataLength, int padLength, bool negative)
        {
            byte pad = negative ? (byte)0xff : (byte)0;
            for (int x = dataLength; x < padLength; x++)
                buffer[x] = pad;
            return buffer[..padLength];
        }

        internal byte[] ToArray()
        {
            return Instructions.SelectMany(i => i.ToArray()).ToArray();
        }

        private Instruction AccessSlot(OpCode opcode, byte index)
        {
            return index >= 7
                ? AddInstruction(new Instruction { OpCode = opcode, Operand = new[] { index } })
                : AddInstruction(opcode - 7 + index);
        }
    }
}
