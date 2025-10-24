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
