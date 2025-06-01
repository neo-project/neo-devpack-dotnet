// Copyright (C) 2015-2025 The Neo Project.
//
// StackHelpers.OpCodes.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Emits a NOP (No Operation) instruction that does nothing but can serve as a jump target.
    /// </summary>
    /// <returns>The NOP instruction that was added.</returns>
    private Instruction Nop()
    {
        return AddInstruction(OpCode.NOP);
    }

    /// <summary>
    /// Pushes the value -1 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSHM1 instruction that was added.</returns>
    private Instruction PushM1()
    {
        return AddInstruction(OpCode.PUSHM1);
    }

    /// <summary>
    /// Pushes the value 0 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH0 instruction that was added.</returns>
    private Instruction Push0()
    {
        return AddInstruction(OpCode.PUSH0);
    }

    /// <summary>
    /// Pushes the value 1 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH1 instruction that was added.</returns>
    private Instruction Push1()
    {
        return AddInstruction(OpCode.PUSH1);
    }

    /// <summary>
    /// Pushes the value 2 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH2 instruction that was added.</returns>
    private Instruction Push2()
    {
        return AddInstruction(OpCode.PUSH2);
    }

    /// <summary>
    /// Pushes the value 3 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH3 instruction that was added.</returns>
    private Instruction Push3()
    {
        return AddInstruction(OpCode.PUSH3);
    }

    /// <summary>
    /// Pushes the value 4 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH4 instruction that was added.</returns>
    private Instruction Push4()
    {
        return AddInstruction(OpCode.PUSH4);
    }

    /// <summary>
    /// Pushes the value 5 onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSH5 instruction that was added.</returns>
    private Instruction Push5()
    {
        return AddInstruction(OpCode.PUSH5);
    }

    /// <summary>
    /// Pushes the boolean value true onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSHT instruction that was added.</returns>
    private Instruction PushT()
    {
        return AddInstruction(OpCode.PUSHT);
    }

    /// <summary>
    /// Pushes the boolean value false onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSHF instruction that was added.</returns>
    private Instruction PushF()
    {
        return AddInstruction(OpCode.PUSHF);
    }

    /// <summary>
    /// Pushes a null reference onto the evaluation stack.
    /// </summary>
    /// <returns>The PUSHNULL instruction that was added.</returns>
    private Instruction PushNull()
    {
        return AddInstruction(OpCode.PUSHNULL);
    }

    #region Constants

    /// <summary>
    /// Terminates the execution of the current script with an error.
    /// </summary>
    /// <returns>The ABORT instruction that was added.</returns>
    private Instruction Abort()
    {
        return AddInstruction(OpCode.ABORT);
    }

    /// <summary>
    /// Asserts that the top stack item is true. If false, terminates execution with an error.
    /// </summary>
    /// <returns>The ASSERT instruction that was added.</returns>
    private Instruction Assert()
    {
        return AddInstruction(OpCode.ASSERT);
    }

    /// <summary>
    /// Throws an exception, terminating the current execution context.
    /// </summary>
    /// <returns>The THROW instruction that was added.</returns>
    private Instruction Throw()
    {
        return AddInstruction(OpCode.THROW);
    }

    // private Instruction Ret()
    // {
    //     return AddInstruction(OpCode.RET);
    // }
    #endregion

    #region Stack

    /// <summary>
    /// Pushes the current stack depth (number of items) onto the evaluation stack.
    /// </summary>
    /// <returns>The DEPTH instruction that was added.</returns>
    private Instruction Depth()
    {
        return AddInstruction(OpCode.DEPTH);
    }

    /// <summary>
    /// Removes the specified number of items from the top of the evaluation stack.
    /// </summary>
    /// <param name="count">The number of items to remove from the stack. Default is 1.</param>
    /// <returns>The last DROP instruction that was added.</returns>
    /// <remarks>
    /// Algorithm: For count > 1, emits multiple DROP instructions.
    /// Stack effect: [item1, item2, ..., itemN] → [] (removes N items)
    /// </remarks>
    private Instruction Drop(int count = 1)
    {
        for (var i = 0; i < count - 1; i++)
        {
            AddInstruction(OpCode.DROP);
        }
        return AddInstruction(OpCode.DROP);
    }

    /// <summary>
    /// Removes the second item from the top of the evaluation stack.
    /// </summary>
    /// <returns>The NIP instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [b] (removes the second-to-top item)
    /// </remarks>
    private Instruction Nip()
    {
        return AddInstruction(OpCode.NIP);
    }

    /// <summary>
    /// Remove n items from the stack.
    /// </summary>
    /// <param name="count">Number of stack items to be removed. If null, expects count on stack.</param>
    /// <returns>The XDROP instruction that was added.</returns>
    /// <remarks>
    /// Try to use <see cref="OpCode.DROP"/> as much as possible, it is more efficient.
    /// Algorithm: If count is provided, pushes count then executes XDROP.
    /// Stack effect: [items..., count] → [] (removes 'count' items from stack)
    /// </remarks>
    private Instruction XDrop(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.XDROP);
    }

    /// <summary>
    /// Removes all items from the evaluation stack.
    /// </summary>
    /// <returns>The CLEAR instruction that was added.</returns>
    private Instruction Clear()
    {
        return AddInstruction(OpCode.CLEAR);
    }

    /// <summary>
    /// Duplicates the top item on the evaluation stack.
    /// </summary>
    /// <returns>The DUP instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a] → [a, a] (duplicates the top item)
    /// </remarks>
    private Instruction Dup()
    {
        return AddInstruction(OpCode.DUP);
    }

    /// <summary>
    /// Pushes true if the top stack item is null, false otherwise.
    /// </summary>
    /// <returns>The ISNULL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [item] → [bool] (true if item is null, false otherwise)
    /// </remarks>
    private Instruction IsNull()
    {
        return AddInstruction(OpCode.ISNULL);
    }

    /// <summary>
    /// Copies the second item from the top of the stack and pushes it onto the top.
    /// </summary>
    /// <returns>The OVER instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a, b, a] (copies second item to top)
    /// </remarks>
    private Instruction Over()
    {
        return AddInstruction(OpCode.OVER);
    }

    /// <summary>
    /// Copies the item at the specified index from the top and pushes it onto the stack.
    /// </summary>
    /// <param name="index">The index of the item to pick. If null, expects index on stack.</param>
    /// <returns>The PICK instruction that was added.</returns>
    /// <remarks>
    /// Algorithm: If index is provided, pushes index then executes PICK.
    /// Stack effect: [items..., index] → [items..., item_at_index] (copies item at index to top)
    /// </remarks>
    private Instruction Pick(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.PICK);
    }

    /// <summary>
    /// Copies the top item and inserts it before the second item.
    /// </summary>
    /// <returns>The TUCK instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [b, a, b] (inserts copy of top item below second item)
    /// </remarks>
    private Instruction Tuck()
    {
        return AddInstruction(OpCode.TUCK);
    }

    /// <summary>
    /// Swaps the top two items on the evaluation stack.
    /// </summary>
    /// <returns>The SWAP instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [b, a] (swaps positions of top two items)
    /// </remarks>
    private Instruction Swap()
    {
        return AddInstruction(OpCode.SWAP);
    }

    /// <summary>
    /// Rotates the top three items on the stack. The third item becomes the top.
    /// </summary>
    /// <returns>The ROT instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b, c] → [b, c, a] (rotates top 3 items)
    /// </remarks>
    private Instruction Rot()
    {
        return AddInstruction(OpCode.ROT);
    }

    /// <summary>
    /// Moves the item at the specified index to the top of the stack.
    /// </summary>
    /// <param name="index">The index of the item to roll. If null, expects index on stack.</param>
    /// <returns>The ROLL instruction that was added.</returns>
    /// <remarks>
    /// Algorithm: If index is provided, pushes index then executes ROLL.
    /// Stack effect: [items..., index] → [items..., item_at_index] (moves item at index to top)
    /// </remarks>
    private Instruction Roll(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.ROLL);
    }

    /// <summary>
    /// Moves the item at the top of the stack to the position specified by the top stack item.
    /// </summary>
    /// <returns>The ROLL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [items..., item, index] → [items..., item_at_new_position]
    /// </remarks>
    private Instruction Roll()
    {
        return AddInstruction(OpCode.ROLL);
    }

    /// <summary>
    /// Reverses the order of the top 3 items on the stack.
    /// </summary>
    /// <returns>The REVERSE3 instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b, c] → [c, b, a] (reverses top 3 items)
    /// </remarks>
    private Instruction Reverse3()
    {
        return AddInstruction(OpCode.REVERSE3);
    }

    /// <summary>
    /// Reverses the order of the top 4 items on the stack.
    /// </summary>
    /// <returns>The REVERSE4 instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b, c, d] → [d, c, b, a] (reverses top 4 items)
    /// </remarks>
    private Instruction Reverse4()
    {
        return AddInstruction(OpCode.REVERSE4);
    }

    /// <summary>
    /// Reverses the order of the top N items on the stack.
    /// </summary>
    /// <param name="count">The number of items to reverse. If null, expects count on stack.</param>
    /// <returns>The REVERSEN instruction that was added.</returns>
    /// <remarks>
    /// Algorithm: If count is provided, pushes count then executes REVERSEN.
    /// Stack effect: [items..., count] → [reversed_items...] (reverses top 'count' items)
    /// </remarks>
    private Instruction ReverseN(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.REVERSEN);
    }

    #endregion

    #region Splice

    /// <summary>
    /// Creates a new buffer with the specified size.
    /// </summary>
    /// <param name="size">The buffer size. If null, expects size on stack.</param>
    /// <returns>The NEWBUFFER instruction that was added.</returns>
    private Instruction NewBuffer(int? size)
    {
        if (size.HasValue)
            Push(size.Value);
        return AddInstruction(OpCode.NEWBUFFER);
    }

    /// <summary>
    /// Copies memory from source to destination.
    /// </summary>
    /// <returns>The MEMCPY instruction that was added.</returns>
    private Instruction Memcpy()
    {
        return AddInstruction(OpCode.MEMCPY);
    }

    /// <summary>
    /// Concatenates two byte strings.
    /// </summary>
    /// <returns>The CAT instruction that was added.</returns>
    private Instruction Cat()
    {
        return AddInstruction(OpCode.CAT);
    }

    /// <summary>
    /// Extracts a substring from a byte string.
    /// </summary>
    /// <returns>The SUBSTR instruction that was added.</returns>
    private Instruction SubStr()
    {
        return AddInstruction(OpCode.SUBSTR);
    }

    /// <summary>
    /// Gets the leftmost characters of a string.
    /// </summary>
    /// <param name="length">The number of characters. If null, expects length on stack.</param>
    /// <returns>The LEFT instruction that was added.</returns>
    private Instruction Left(int? length)
    {
        if (length.HasValue)
            Push(length.Value);
        return AddInstruction(OpCode.LEFT);
    }

    /// <summary>
    /// Gets the rightmost characters of a string.
    /// </summary>
    /// <param name="length">The number of characters. If null, expects length on stack.</param>
    /// <returns>The RIGHT instruction that was added.</returns>
    private Instruction Right(int? length)
    {
        if (length.HasValue)
            Push(length.Value);
        return AddInstruction(OpCode.RIGHT);
    }

    #endregion

    #region Bitwise logic

    /// <summary>
    /// Performs bitwise NOT on the top stack item.
    /// </summary>
    /// <returns>The INVERT instruction that was added.</returns>
    private Instruction Invert()
    {
        return AddInstruction(OpCode.INVERT);
    }

    /// <summary>
    /// Performs bitwise AND on the top two stack items.
    /// </summary>
    /// <returns>The AND instruction that was added.</returns>
    private Instruction And()
    {
        return AddInstruction(OpCode.AND);
    }

    /// <summary>
    /// Performs bitwise OR on the top two stack items.
    /// </summary>
    /// <returns>The OR instruction that was added.</returns>
    private Instruction Or()
    {
        return AddInstruction(OpCode.OR);
    }

    /// <summary>
    /// Performs bitwise XOR on the top two stack items.
    /// </summary>
    /// <returns>The XOR instruction that was added.</returns>
    private Instruction Xor()
    {
        return AddInstruction(OpCode.XOR);
    }

    /// <summary>
    /// Checks if the top two stack items are equal.
    /// </summary>
    /// <returns>The EQUAL instruction that was added.</returns>
    private Instruction Equal()
    {
        return AddInstruction(OpCode.EQUAL);
    }

    /// <summary>
    /// Checks if the top two stack items are not equal.
    /// </summary>
    /// <returns>The NOTEQUAL instruction that was added.</returns>
    private Instruction NotEqual()
    {
        return AddInstruction(OpCode.NOTEQUAL);
    }

    #endregion

    #region Arithmetic

    /// <summary>
    /// Determines the sign of the top stack item and pushes the result.
    /// </summary>
    /// <returns>The SIGN instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [sign] where sign is -1, 0, or 1
    /// Returns: -1 if negative, 0 if zero, 1 if positive
    /// </remarks>
    private Instruction Sign()
    {
        return AddInstruction(OpCode.SIGN);
    }

    /// <summary>
    /// Computes the absolute value of the top stack item.
    /// </summary>
    /// <returns>The ABS instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [|number|] (absolute value)
    /// </remarks>
    private Instruction Abs()
    {
        return AddInstruction(OpCode.ABS);
    }

    /// <summary>
    /// Negates the top stack item (multiplies by -1).
    /// </summary>
    /// <returns>The NEGATE instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [-number] (negated value)
    /// </remarks>
    private Instruction Negate()
    {
        return AddInstruction(OpCode.NEGATE);
    }

    /// <summary>
    /// Increments the top stack item by 1.
    /// </summary>
    /// <returns>The INC instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [number + 1]
    /// </remarks>
    private Instruction Inc()
    {
        return AddInstruction(OpCode.INC);
    }

    /// <summary>
    /// Decrements the top stack item by 1.
    /// </summary>
    /// <returns>The DEC instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [number - 1]
    /// </remarks>
    private Instruction Dec()
    {
        return AddInstruction(OpCode.DEC);
    }

    /// <summary>
    /// Adds the top two stack items.
    /// </summary>
    /// <returns>The ADD instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a + b]
    /// </remarks>
    private Instruction Add()
    {
        return AddInstruction(OpCode.ADD);
    }

    /// <summary>
    /// Subtracts the top stack item from the second stack item.
    /// </summary>
    /// <returns>The SUB instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a - b] (second minus top)
    /// </remarks>
    private Instruction Sub()
    {
        return AddInstruction(OpCode.SUB);
    }

    /// <summary>
    /// Multiplies the top two stack items.
    /// </summary>
    /// <returns>The MUL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a * b]
    /// </remarks>
    private Instruction Mul()
    {
        return AddInstruction(OpCode.MUL);
    }

    /// <summary>
    /// Divides the second stack item by the top stack item.
    /// </summary>
    /// <returns>The DIV instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a / b] (second divided by top)
    /// Throws exception if divisor (top item) is zero.
    /// </remarks>
    private Instruction Div()
    {
        return AddInstruction(OpCode.DIV);
    }

    /// <summary>
    /// Computes the remainder of dividing the second stack item by the top stack item.
    /// </summary>
    /// <returns>The MOD instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a % b] (second modulo top)
    /// Throws exception if divisor (top item) is zero.
    /// </remarks>
    private Instruction Mod()
    {
        return AddInstruction(OpCode.MOD);
    }

    /// <summary>
    /// Raises the second stack item to the power of the top stack item.
    /// </summary>
    /// <returns>The POW instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [base, exponent] → [base ^ exponent]
    /// </remarks>
    private Instruction Pow()
    {
        return AddInstruction(OpCode.POW);
    }

    /// <summary>
    /// Computes the square root of the top stack item.
    /// </summary>
    /// <returns>The SQRT instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [√number]
    /// Throws exception if number is negative.
    /// </remarks>
    private Instruction Sqrt()
    {
        return AddInstruction(OpCode.SQRT);
    }

    /// <summary>
    /// Computes (a * b) % modulus for the top three stack items.
    /// </summary>
    /// <returns>The MODMUL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b, modulus] → [(a * b) % modulus]
    /// More efficient than separate MUL and MOD operations for large numbers.
    /// </remarks>
    private Instruction ModMul()
    {
        return AddInstruction(OpCode.MODMUL);
    }

    /// <summary>
    /// Computes (base ^ exponent) % modulus for the top three stack items.
    /// </summary>
    /// <returns>The MODPOW instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [base, exponent, modulus] → [(base ^ exponent) % modulus]
    /// More efficient than separate POW and MOD operations for large numbers.
    /// </remarks>
    private Instruction ModPow()
    {
        return AddInstruction(OpCode.MODPOW);
    }

    /// <summary>
    /// Shifts the second stack item left by the number of bits specified by the top stack item.
    /// </summary>
    /// <returns>The SHL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [value, shift_count] → [value << shift_count]
    /// Equivalent to multiplying by 2^shift_count.
    /// </remarks>
    private Instruction ShL()
    {
        return AddInstruction(OpCode.SHL);
    }

    /// <summary>
    /// Shifts the second stack item right by the number of bits specified by the top stack item.
    /// </summary>
    /// <returns>The SHR instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [value, shift_count] → [value >> shift_count]
    /// Equivalent to dividing by 2^shift_count (integer division).
    /// </remarks>
    private Instruction ShR()
    {
        return AddInstruction(OpCode.SHR);
    }

    /// <summary>
    /// Performs logical NOT on the top stack item.
    /// </summary>
    /// <returns>The NOT instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [bool] → [!bool]
    /// Converts true to false and false to true.
    /// </remarks>
    private Instruction Not()
    {
        return AddInstruction(OpCode.NOT);
    }

    /// <summary>
    /// Performs logical AND on the top two stack items.
    /// </summary>
    /// <returns>The BOOLAND instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a && b]
    /// Returns true only if both operands are true.
    /// </remarks>
    private Instruction BoolAnd()
    {
        return AddInstruction(OpCode.BOOLAND);
    }

    /// <summary>
    /// Performs logical OR on the top two stack items.
    /// </summary>
    /// <returns>The BOOLOR instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a || b]
    /// Returns true if either operand is true.
    /// </remarks>
    private Instruction BoolOr()
    {
        return AddInstruction(OpCode.BOOLOR);
    }

    /// <summary>
    /// Checks if the top stack item is not zero.
    /// </summary>
    /// <returns>The NZ instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [number] → [number != 0]
    /// Returns true if number is non-zero, false if zero.
    /// </remarks>
    private Instruction Nz()
    {
        return AddInstruction(OpCode.NZ);
    }

    /// <summary>
    /// Checks if the top two stack items are numerically equal.
    /// </summary>
    /// <returns>The NUMEQUAL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a == b]
    /// Performs numeric comparison.
    /// </remarks>
    private Instruction NumEqual()
    {
        return AddInstruction(OpCode.NUMEQUAL);
    }

    /// <summary>
    /// Checks if the top two stack items are numerically not equal.
    /// </summary>
    /// <returns>The NUMNOTEQUAL instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a != b]
    /// Performs numeric comparison.
    /// </remarks>
    private Instruction NumNotEqual()
    {
        return AddInstruction(OpCode.NUMNOTEQUAL);
    }

    /// <summary>
    /// Checks if the second stack item is less than the top stack item.
    /// </summary>
    /// <returns>The LT instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a < b]
    /// </remarks>
    private Instruction Lt()
    {
        return AddInstruction(OpCode.LT);
    }

    /// <summary>
    /// Checks if the second stack item is less than or equal to the top stack item.
    /// </summary>
    /// <returns>The LE instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a <= b]
    /// </remarks>
    private Instruction Le()
    {
        return AddInstruction(OpCode.LE);
    }

    /// <summary>
    /// Checks if the second stack item is greater than the top stack item.
    /// </summary>
    /// <returns>The GT instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a > b]
    /// </remarks>
    private Instruction Gt()
    {
        return AddInstruction(OpCode.GT);
    }

    /// <summary>
    /// Checks if the second stack item is greater than or equal to the top stack item.
    /// </summary>
    /// <returns>The GE instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [a >= b]
    /// </remarks>
    private Instruction Ge()
    {
        return AddInstruction(OpCode.GE);
    }

    /// <summary>
    /// Returns the smaller of the top two stack items.
    /// </summary>
    /// <returns>The MIN instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [min(a, b)]
    /// </remarks>
    private Instruction Min()
    {
        return AddInstruction(OpCode.MIN);
    }

    /// <summary>
    /// Returns the larger of the top two stack items.
    /// </summary>
    /// <returns>The MAX instruction that was added.</returns>
    /// <remarks>
    /// Stack effect: [a, b] → [max(a, b)]
    /// </remarks>
    private Instruction Max()
    {
        return AddInstruction(OpCode.MAX);
    }

    /// <summary>
    /// Within is a VM instruction that checks if a value is within a specified range.
    /// It compares the value on the top of the stack with the specified minimum and maximum values.
    /// If the value is not within the range, it throws an exception.
    /// </summary>
    /// <param name="min">The minimum value of the range, exclusive.</param>
    /// <param name="max">The maximum value of the range, inclusive.</param>
    /// <returns>The value on the top of the stack.</returns>
    private Instruction Within(BigInteger min, BigInteger max)
    {
        Push(min);
        Push(max + 1);
        return AddInstruction(OpCode.WITHIN);
    }

    private Instruction Within()
    {
        return AddInstruction(OpCode.WITHIN);
    }

    private void WithinCheck(BigInteger min, BigInteger max)
    {
        JumpTarget endTarget = new();
        Dup();
        Within(min, max);
        Jump(OpCode.JMPIF, endTarget);
        Throw();
        endTarget.Instruction = Nop();
    }

    #endregion

    #region Compound-type

    /// <summary>
    /// Packs the top items into a map structure.
    /// </summary>
    /// <returns>The PACKMAP instruction that was added.</returns>
    private Instruction PackMap()
    {
        return AddInstruction(OpCode.PACKMAP);
    }

    /// <summary>
    /// Packs the top items into a struct.
    /// </summary>
    /// <returns>The PACKSTRUCT instruction that was added.</returns>
    private Instruction PackStruct()
    {
        return AddInstruction(OpCode.PACKSTRUCT);
    }

    /// <summary>
    /// Packs the top items into an array.
    /// </summary>
    /// <param name="size">The number of items to pack. If null, expects size on stack.</param>
    /// <returns>The PACK instruction that was added.</returns>
    private Instruction Pack(int? size)
    {
        if (size.HasValue)
            Push(size.Value);
        return AddInstruction(OpCode.PACK);
    }

    /// <summary>
    /// Unpacks an array or struct into individual items.
    /// </summary>
    /// <returns>The UNPACK instruction that was added.</returns>
    private Instruction UnPack()
    {
        return AddInstruction(OpCode.UNPACK);
    }

    /// <summary>
    /// Creates a new empty array.
    /// </summary>
    /// <returns>The NEWARRAY0 instruction that was added.</returns>
    private Instruction NewArray0()
    {
        return AddInstruction(OpCode.NEWARRAY0);
    }

    /// <summary>
    /// Creates a new array with the specified size.
    /// </summary>
    /// <param name="count">The array size. If null, expects count on stack.</param>
    /// <returns>The NEWARRAY instruction that was added.</returns>
    private Instruction NewArray(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.NEWARRAY);
    }

    /// <summary>
    /// Creates a new empty struct.
    /// </summary>
    /// <returns>The NEWSTRUCT0 instruction that was added.</returns>
    private Instruction NewStruct0()
    {
        return AddInstruction(OpCode.NEWSTRUCT0);
    }

    /// <summary>
    /// Creates a new struct with the specified size.
    /// </summary>
    /// <param name="count">The struct size. If null, expects count on stack.</param>
    /// <returns>The NEWSTRUCT instruction that was added.</returns>
    private Instruction NewStruct(int? count)
    {
        if (count.HasValue)
            Push(count.Value);
        return AddInstruction(OpCode.NEWSTRUCT);
    }

    /// <summary>
    /// Gets the size of an array, map, or buffer.
    /// </summary>
    /// <returns>The SIZE instruction that was added.</returns>
    private Instruction Size()
    {
        return AddInstruction(OpCode.SIZE);
    }

    /// <summary>
    /// Checks if a map contains the specified key.
    /// </summary>
    /// <returns>The HASKEY instruction that was added.</returns>
    private Instruction HasKey()
    {
        return AddInstruction(OpCode.HASKEY);
    }

    /// <summary>
    /// Gets all keys from a map.
    /// </summary>
    /// <returns>The KEYS instruction that was added.</returns>
    private Instruction Keys()
    {
        return AddInstruction(OpCode.KEYS);
    }

    /// <summary>
    /// Gets all values from a map.
    /// </summary>
    /// <returns>The VALUES instruction that was added.</returns>
    private Instruction Values()
    {
        return AddInstruction(OpCode.VALUES);
    }

    /// <summary>
    /// Gets an item from an array or map at the specified index.
    /// </summary>
    /// <param name="index">The index. If null, expects index on stack.</param>
    /// <returns>The PICKITEM instruction that was added.</returns>
    private Instruction PickItem(int? index)
    {
        if (index.HasValue)
            Push(index.Value);
        return AddInstruction(OpCode.PICKITEM);
    }

    /// <summary>
    /// Gets an item from an array or map using the index on the stack.
    /// </summary>
    /// <returns>The PICKITEM instruction that was added.</returns>
    private Instruction PickItem()
    {
        return AddInstruction(OpCode.PICKITEM);
    }

    /// <summary>
    /// Appends an item to an array.
    /// </summary>
    /// <returns>The APPEND instruction that was added.</returns>
    private Instruction Append()
    {
        return AddInstruction(OpCode.APPEND);
    }

    /// <summary>
    /// Sets an item in an array or map.
    /// </summary>
    /// <returns>The SETITEM instruction that was added.</returns>
    private Instruction SetItem()
    {
        return AddInstruction(OpCode.SETITEM);
    }

    /// <summary>
    /// Reverses the order of items in an array.
    /// </summary>
    /// <returns>The REVERSEITEMS instruction that was added.</returns>
    private Instruction ReverseItems()
    {
        return AddInstruction(OpCode.REVERSEITEMS);
    }

    /// <summary>
    /// Removes an item from an array or map.
    /// </summary>
    /// <returns>The REMOVE instruction that was added.</returns>
    private Instruction Remove()
    {
        return AddInstruction(OpCode.REMOVE);
    }

    /// <summary>
    /// Removes all items from an array or map.
    /// </summary>
    /// <returns>The CLEARITEMS instruction that was added.</returns>
    private Instruction ClearItems()
    {
        return AddInstruction(OpCode.CLEARITEMS);
    }

    /// <summary>
    /// Removes and returns the last item from an array.
    /// </summary>
    /// <returns>The POPITEM instruction that was added.</returns>
    private Instruction PopItem()
    {
        return AddInstruction(OpCode.POPITEM);
    }

    #endregion

    #region Arguments and Locals

    /// <summary>
    /// Loads argument 0 onto the stack.
    /// </summary>
    /// <returns>The LDARG0 instruction that was added.</returns>
    private Instruction LdArg0()
    {
        return AddInstruction(OpCode.LDARG0);
    }

    /// <summary>
    /// Loads argument 1 onto the stack.
    /// </summary>
    /// <returns>The LDARG1 instruction that was added.</returns>
    private Instruction LdArg1()
    {
        return AddInstruction(OpCode.LDARG1);
    }

    /// <summary>
    /// Loads argument 2 onto the stack.
    /// </summary>
    /// <returns>The LDARG2 instruction that was added.</returns>
    private Instruction LdArg2()
    {
        return AddInstruction(OpCode.LDARG2);
    }

    /// <summary>
    /// Loads argument 3 onto the stack.
    /// </summary>
    /// <returns>The LDARG3 instruction that was added.</returns>
    private Instruction LdArg3()
    {
        return AddInstruction(OpCode.LDARG3);
    }

    /// <summary>
    /// Loads argument 4 onto the stack.
    /// </summary>
    /// <returns>The LDARG4 instruction that was added.</returns>
    private Instruction LdArg4()
    {
        return AddInstruction(OpCode.LDARG4);
    }

    /// <summary>
    /// Loads argument 5 onto the stack.
    /// </summary>
    /// <returns>The LDARG5 instruction that was added.</returns>
    private Instruction LdArg5()
    {
        return AddInstruction(OpCode.LDARG5);
    }

    /// <summary>
    /// Loads argument 6 onto the stack.
    /// </summary>
    /// <returns>The LDARG6 instruction that was added.</returns>
    private Instruction LdArg6()
    {
        return AddInstruction(OpCode.LDARG6);
    }

    /// <summary>
    /// Loads local variable 0 onto the stack.
    /// </summary>
    /// <returns>The LDLOC0 instruction that was added.</returns>
    private Instruction LdLoc0()
    {
        return AddInstruction(OpCode.LDLOC0);
    }

    /// <summary>
    /// Loads local variable 1 onto the stack.
    /// </summary>
    /// <returns>The LDLOC1 instruction that was added.</returns>
    private Instruction LdLoc1()
    {
        return AddInstruction(OpCode.LDLOC1);
    }

    /// <summary>
    /// Loads local variable 2 onto the stack.
    /// </summary>
    /// <returns>The LDLOC2 instruction that was added.</returns>
    private Instruction LdLoc2()
    {
        return AddInstruction(OpCode.LDLOC2);
    }

    /// <summary>
    /// Loads local variable 3 onto the stack.
    /// </summary>
    /// <returns>The LDLOC3 instruction that was added.</returns>
    private Instruction LdLoc3()
    {
        return AddInstruction(OpCode.LDLOC3);
    }

    /// <summary>
    /// Loads local variable 4 onto the stack.
    /// </summary>
    /// <returns>The LDLOC4 instruction that was added.</returns>
    private Instruction LdLoc4()
    {
        return AddInstruction(OpCode.LDLOC4);
    }

    /// <summary>
    /// Loads local variable 5 onto the stack.
    /// </summary>
    /// <returns>The LDLOC5 instruction that was added.</returns>
    private Instruction LdLoc5()
    {
        return AddInstruction(OpCode.LDLOC5);
    }

    /// <summary>
    /// Loads local variable 6 onto the stack.
    /// </summary>
    /// <returns>The LDLOC6 instruction that was added.</returns>
    private Instruction LdLoc6()
    {
        return AddInstruction(OpCode.LDLOC6);
    }

    /// <summary>
    /// Stores the top stack item to local variable 0.
    /// </summary>
    /// <returns>The STLOC0 instruction that was added.</returns>
    private Instruction StLoc0()
    {
        return AddInstruction(OpCode.STLOC0);
    }

    /// <summary>
    /// Stores the top stack item to local variable 1.
    /// </summary>
    /// <returns>The STLOC1 instruction that was added.</returns>
    private Instruction StLoc1()
    {
        return AddInstruction(OpCode.STLOC1);
    }

    /// <summary>
    /// Stores the top stack item to local variable 2.
    /// </summary>
    /// <returns>The STLOC2 instruction that was added.</returns>
    private Instruction StLoc2()
    {
        return AddInstruction(OpCode.STLOC2);
    }

    /// <summary>
    /// Stores the top stack item to local variable 3.
    /// </summary>
    /// <returns>The STLOC3 instruction that was added.</returns>
    private Instruction StLoc3()
    {
        return AddInstruction(OpCode.STLOC3);
    }

    /// <summary>
    /// Stores the top stack item to local variable 4.
    /// </summary>
    /// <returns>The STLOC4 instruction that was added.</returns>
    private Instruction StLoc4()
    {
        return AddInstruction(OpCode.STLOC4);
    }

    /// <summary>
    /// Stores the top stack item to local variable 5.
    /// </summary>
    /// <returns>The STLOC5 instruction that was added.</returns>
    private Instruction StLoc5()
    {
        return AddInstruction(OpCode.STLOC5);
    }

    /// <summary>
    /// Stores the top stack item to local variable 6.
    /// </summary>
    /// <returns>The STLOC6 instruction that was added.</returns>
    private Instruction StLoc6()
    {
        return AddInstruction(OpCode.STLOC6);
    }

    /// <summary>
    /// Stores the top stack item to the specified local variable.
    /// </summary>
    /// <param name="index">The index of the local variable to store to.</param>
    /// <returns>The STLOC instruction that was added.</returns>
    private Instruction StLoc(byte index)
    {
        return AccessSlot(OpCode.STLOC, index);
    }

    private Instruction LdLoc(byte index)
    {
        return AccessSlot(OpCode.LDLOC, index);
    }

    #endregion

    #region Types

    /// <summary>
    /// Checks if the top stack item is null.
    /// </summary>
    /// <returns>The ISNULL instruction that was added.</returns>
    private Instruction Isnull()
    {
        return AddInstruction(OpCode.ISNULL);
    }

    /// <summary>
    /// Checks if the top stack item is of the specified type.
    /// </summary>
    /// <param name="type">The stack item type to check.</param>
    /// <returns>The ISTYPE instruction that was added.</returns>
    private Instruction Istype(StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.ISTYPE,
            Operand = [(byte)type]
        });
    }

    /// <summary>
    /// Converts the top stack item to the specified type.
    /// </summary>
    /// <param name="type">The target stack item type.</param>
    /// <returns>The CONVERT instruction that was added.</returns>
    private Instruction CONVERT(StackItemType type)
    {
        return ChangeType(type);
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Terminates execution with a custom error message from the stack.
    /// </summary>
    /// <returns>The ABORTMSG instruction that was added.</returns>
    private Instruction AbortMsg()
    {
        return AddInstruction(OpCode.ABORTMSG);
    }

    /// <summary>
    /// Asserts a condition with a custom error message from the stack.
    /// </summary>
    /// <returns>The ASSERTMSG instruction that was added.</returns>
    private Instruction AssertMsg()
    {
        return AddInstruction(OpCode.ASSERTMSG);
    }

    /// <summary>
    /// Calls a method using the address on the stack.
    /// </summary>
    /// <returns>The CALLA instruction that was added.</returns>
    private Instruction CallA()
    {
        return AddInstruction(OpCode.CALLA);
    }

    /// <summary>
    /// Jumps to the target if the top stack item is false.
    /// </summary>
    /// <param name="target">The jump target.</param>
    /// <returns>The JMPIFNOT instruction that was added.</returns>
    private Instruction JumpIfNot(JumpTarget target)
    {
        return Jump(OpCode.JMPIFNOT, target);
    }

    /// <summary>
    /// Unconditionally jumps to the target.
    /// </summary>
    /// <param name="target">The jump target.</param>
    /// <returns>The JMP instruction that was added.</returns>
    private Instruction Jump(JumpTarget target)
    {
        return Jump(OpCode.JMP, target);
    }

    #endregion
}
