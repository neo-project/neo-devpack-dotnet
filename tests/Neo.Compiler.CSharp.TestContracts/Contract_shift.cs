// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_shift.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static int[] TestShift()
        {
            int v = 8;
            var v1 = v << 1;
            var v2 = v >> 1;
            return new int[] { v1, v2 };
        }

        public static System.Numerics.BigInteger[] TestShiftBigInt()
        {
            System.Numerics.BigInteger v = 8;
            var v1 = v << 0;
            var v2 = v << 1;
            var v3 = v >> 1;
            var v4 = v >> 2;
            return new System.Numerics.BigInteger[] { v1, v2, v3, v4 };
        }

        // Checked shift operations - should throw on invalid shift amounts
        public static int ShiftLeftChecked(int value, int shift)
        {
            return checked(value << shift);
        }

        // Unchecked shift operations - should not throw
        public static int ShiftLeftUnchecked(int value, int shift)
        {
            return unchecked(value << shift);
        }

        // Checked shift with long type
        public static long ShiftLeftCheckedLong(long value, int shift)
        {
            return checked(value << shift);
        }

        // Checked shift with byte type
        public static int ShiftLeftCheckedByte(byte value, int shift)
        {
            return checked(value << shift);
        }

        // BigInteger shift (no limit)
        public static System.Numerics.BigInteger ShiftLeftBigInteger(System.Numerics.BigInteger value, int shift)
        {
            return value << shift;
        }
    }
}
