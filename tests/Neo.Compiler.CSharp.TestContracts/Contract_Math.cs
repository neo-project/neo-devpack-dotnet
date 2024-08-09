using System;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Math : SmartContract.Framework.SmartContract
    {
        public static int max(int a, int b)
        {
            return Math.Max(a, b);
        }

        public static int min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static int sign(int a)
        {
            return Math.Sign(a);
        }

        public static int abs(int a)
        {
            return Math.Abs(a);
        }

        public static long bigMul(int a, int b)
        {
            return Math.BigMul(a, b);
        }

        public static (byte quotient, byte remainder) DivRemByte(byte left, byte right)
        {
            return Math.DivRem(left, right);
        }

        public static (short quotient, short remainder) DivRemShort(short left, short right)
        {
            return Math.DivRem(left, right);
        }

        public static (int quotient, int remainder) DivRemInt(int left, int right)
        {
            return Math.DivRem(left, right);
        }

        public static (long quotient, long remainder) DivRemLong(long left, long right)
        {
            return Math.DivRem(left, right);
        }

        public static (sbyte quotient, sbyte remainder) DivRemSbyte(sbyte left, sbyte right)
        {
            return Math.DivRem(left, right);
        }

        public static (ushort quotient, ushort remainder) DivRemUshort(ushort left, ushort right)
        {
            return Math.DivRem(left, right);
        }

        public static (uint quotient, uint remainder) DivRemUint(uint left, uint right)
        {
            return Math.DivRem(left, right);
        }

        public static (ulong quotient, ulong remainder) DivRemUlong(ulong left, ulong right)
        {
            return Math.DivRem(left, right);
        }

        public static byte ClampByte(byte value, byte min, byte max)
        {
            return Math.Clamp(value, min, max);
        }

        public static sbyte ClampSByte(sbyte value, sbyte min, sbyte max)
        {
            return Math.Clamp(value, min, max);
        }

        public static short ClampShort(short value, short min, short max)
        {
            return Math.Clamp(value, min, max);
        }

        public static ushort ClampUShort(ushort value, ushort min, ushort max)
        {
            return Math.Clamp(value, min, max);
        }

        public static int ClampInt(int value, int min, int max)
        {
            return Math.Clamp(value, min, max);
        }

        public static uint ClampUInt(uint value, uint min, uint max)
        {
            return Math.Clamp(value, min, max);
        }

        public static long ClampLong(long value, long min, long max)
        {
            return Math.Clamp(value, min, max);
        }

        public static ulong ClampULong(ulong value, ulong min, ulong max)
        {
            return Math.Clamp(value, min, max);
        }
    }
}
