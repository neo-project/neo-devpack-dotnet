// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_BitOperations.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_BitOperations : SmartContract.Framework.SmartContract
{
    public static int Log2UInt(uint value)
    {
        return BitOperations.Log2(value);
    }

    public static int Log2ULong(ulong value)
    {
        return BitOperations.Log2(value);
    }

    public static int PopCountUInt(uint value)
    {
        return BitOperations.PopCount(value);
    }

    public static int PopCountULong(ulong value)
    {
        return BitOperations.PopCount(value);
    }

    public static int LeadingZeroCountUInt(uint value)
    {
        return BitOperations.LeadingZeroCount(value);
    }

    public static int LeadingZeroCountULong(ulong value)
    {
        return BitOperations.LeadingZeroCount(value);
    }

    public static uint RotateLeftUInt(uint value, int offset)
    {
        return BitOperations.RotateLeft(value, offset);
    }

    public static ulong RotateLeftULong(ulong value, int offset)
    {
        return BitOperations.RotateLeft(value, offset);
    }

    public static uint RotateRightUInt(uint value, int offset)
    {
        return BitOperations.RotateRight(value, offset);
    }

    public static ulong RotateRightULong(ulong value, int offset)
    {
        return BitOperations.RotateRight(value, offset);
    }
}
