// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.RotateHelpers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Emits the opcode sequence for <c>RotateLeft</c> on unsigned values.
    /// Masking and shift steps:
    /// 1. Normalize rotation count: <c>count = count & (bitWidth - 1)</c>.
    /// 2. Constrain the input to the requested width (<c>value & mask</c>).
    /// 3. Shift left, mask the result, then compute the spill bits via right shift.
    /// 4. Combine both parts and apply the mask again to keep the result within range.
    /// </summary>
    private static void EmitRotateLeftUnsigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    /// <summary>
    /// Emits the opcode sequence for <c>RotateLeft</c> on signed values.
    /// Steps mirror the unsigned version with a final two's-complement adjustment when the
    /// high bit is set to ensure the result remains within the signed range.
    /// </summary>
    private static void EmitRotateLeftSigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Dup();
        methodConvert.Push(BigInteger.One << (bitWidth - 1));
        JumpTarget endTarget = new();
        methodConvert.JumpIfLess(endTarget);
        methodConvert.Push(BigInteger.One << bitWidth);
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    /// <summary>
    /// Emits the opcode sequence for <c>RotateRight</c> on unsigned values.
    /// The algorithm reuses the same normalized count and mask as the left rotation, but
    /// computes the right-shifted and left-shifted halves in reverse order before combining.
    /// </summary>
    private static void EmitRotateRightUnsigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.LdArg0();
        methodConvert.Push(bitWidth);
        methodConvert.LdArg1();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShL();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    /// <summary>
    /// Emits the opcode sequence for <c>RotateRight</c> on signed values.
    /// After performing the unsigned rotation logic it adjusts results that overflow the
    /// signed magnitude by subtracting 2^bitWidth, matching two's-complement behaviour.
    /// </summary>
    private static void EmitRotateRightSigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Dup();
        methodConvert.Push(BigInteger.One << (bitWidth - 1));
        JumpTarget endTarget = new();
        methodConvert.JumpIfLess(endTarget);
        methodConvert.Push(BigInteger.One << bitWidth);
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }
}
