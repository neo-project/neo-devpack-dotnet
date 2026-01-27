// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_DivisionOverflow.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_DivisionOverflow : SmartContract.Framework.SmartContract
    {
        // Int32 division - checked
        public static int DivideCheckedInt32(int a, int b)
        {
            return checked(a / b);
        }

        // Int32 division - unchecked
        public static int DivideUncheckedInt32(int a, int b)
        {
            return unchecked(a / b);
        }

        // Int64 division - checked
        public static long DivideCheckedInt64(long a, long b)
        {
            return checked(a / b);
        }

        // Int64 division - unchecked
        public static long DivideUncheckedInt64(long a, long b)
        {
            return unchecked(a / b);
        }

        // nint division - checked
        public static nint DivideCheckedIntPtr(nint a, nint b)
        {
            return checked(a / b);
        }

        // nint division - unchecked
        public static nint DivideUncheckedIntPtr(nint a, nint b)
        {
            return unchecked(a / b);
        }

        // BigInteger division - checked (should not overflow)
        public static BigInteger DivideCheckedBigInteger(BigInteger a, BigInteger b)
        {
            return checked(a / b);
        }

        // BigInteger division - unchecked
        public static BigInteger DivideUncheckedBigInteger(BigInteger a, BigInteger b)
        {
            return unchecked(a / b);
        }

        // uint division - checked (unsigned, no overflow possible)
        public static uint DivideCheckedUInt32(uint a, uint b)
        {
            return checked(a / b);
        }

        // ulong division - checked (unsigned, no overflow possible)
        public static ulong DivideCheckedUInt64(ulong a, ulong b)
        {
            return checked(a / b);
        }
    }
}
