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

        public static (byte Quotient, byte Remainder) DivRemByte(byte left, byte right)
        {
            return Math.DivRem(left, right);
        }

        public static (short Quotient, short Remainder) DivRemShort(short left, short right)
        {
            return Math.DivRem(left, right);
        }

        public static (int Quotient, int Remainder) DivRemInt(int left, int right)
        {
            return Math.DivRem(left, right);
        }

        public static (long Quotient, long Remainder) DivRemLong(long left, long right)
        {
            return Math.DivRem(left, right);
        }

        public static (sbyte Quotient, sbyte Remainder) DivRemSbyte(sbyte left, sbyte right)
        {
            return Math.DivRem(left, right);
        }

        public static (ushort Quotient, ushort Remainder) DivRemUshort(ushort left, ushort right)
        {
            return Math.DivRem(left, right);
        }

        public static (uint Quotient, uint Remainder) DivRemUint(uint left, uint right)
        {
            return Math.DivRem(left, right);
        }

        public static (ulong Quotient, ulong Remainder) DivRemUlong(ulong left, ulong right)
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
