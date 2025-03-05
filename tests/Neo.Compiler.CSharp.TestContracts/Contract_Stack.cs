// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Stack.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Stack : SmartContract.Framework.SmartContract
{
    public static BigInteger Test_Push_Integer(BigInteger value)
    {
        return value;
    }

    public static (byte, byte, sbyte, sbyte, short, short, ushort, uint, int, int, ulong, long, long) Test_Push_Integer_Internal()
    {
        return (byte.MinValue, byte.MaxValue, sbyte.MinValue, sbyte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, uint.MaxValue, int.MinValue, int.MaxValue, ulong.MaxValue, long.MinValue, long.MaxValue);
    }

    public static (int, int, long) Test_External()
    {
        const int minusOne = -1;
        const int intValue = -1000000;
        const long longValue = -1000000000000;

        return (minusOne, intValue, longValue);
    }
}
