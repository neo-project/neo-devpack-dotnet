// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_MathClamp.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_MathClamp : SmartContract.Framework.SmartContract
    {
        public static int ClampConstantBounds(int value)
        {
            return Math.Clamp(value, 0, 100);
        }

        public static int ClampEqualBounds(int value)
        {
            return Math.Clamp(value, 42, 42);
        }

        public static int ClampInvalidBounds(int value)
        {
            return Math.Clamp(value, 100, 0);
        }

        public static int ClampDynamicBounds(int value, int min, int max)
        {
            return Math.Clamp(value, min, max);
        }

        public static int ClampNamedArguments(int value)
        {
            return Math.Clamp(max: 100, value: value, min: 0);
        }

        public static int ClampConstantExpressions(int value)
        {
            const int min = 5;
            const int max = 25;
            return Math.Clamp(value, min - 5, max * 4);
        }

        public static byte ClampByteConstantBounds(byte value)
        {
            return Math.Clamp(value, (byte)10, (byte)200);
        }

        public static long ClampLongConstantBounds(long value)
        {
            return Math.Clamp(value, long.MinValue, 0L);
        }

        public static ulong ClampULongConstantBounds(ulong value)
        {
            return Math.Clamp(value, 5UL, ulong.MaxValue);
        }

        public static BigInteger ClampBigIntegerConstantBounds(BigInteger value)
        {
            return BigInteger.Clamp(value, -10, 10);
        }

        private static int counter;

        public static int ClampIntExtremeBounds(int value)
        {
            return Math.Clamp(value, int.MinValue, int.MaxValue);
        }

        public static sbyte ClampSByteConstantBounds(sbyte value)
        {
            return Math.Clamp(value, (sbyte)-5, (sbyte)5);
        }

        public static short ClampShortConstantBounds(short value)
        {
            return Math.Clamp(value, (short)-300, (short)300);
        }

        public static ushort ClampUShortConstantBounds(ushort value)
        {
            return Math.Clamp(value, (ushort)10, (ushort)60000);
        }

        public static uint ClampUIntConstantBounds(uint value)
        {
            return Math.Clamp(value, 1u, uint.MaxValue);
        }

        public static BigInteger ClampBigIntegerInvalidBounds(BigInteger value)
        {
            return BigInteger.Clamp(value, 10, -10);
        }

        public static int ClampInvalidNamedBounds(int value)
        {
            return Math.Clamp(min: 10, max: 5, value: value);
        }

        public static int ClampSideEffectValueConstantBounds()
        {
            counter = 0;
            var result = Math.Clamp(Next() + 200, 0, 100);
            return result + counter * 1000;
        }

        private static int Next()
        {
            counter++;
            return counter;
        }

        public static int ClampEvaluationOrder()
        {
            counter = 0;
            return Math.Clamp(Next() * 10, Next(), 100) + counter * 1000;
        }

        public static int ClampNested(int value)
        {
            var total = 0;
            for (var i = 0; i < 3; i++)
                total += Math.Clamp(value + i, 0, 10) * (i % 2 == 0 ? 1 : 2);
            return total + Math.Clamp(value, 0, 5);
        }
    }
}
