using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
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


        // Test for BigInteger Pow(BigInteger x, BigInteger y)
        public static BigInteger TestPow(BigInteger a, BigInteger b)
        {
            return Math.Pow(a, b);
        }

        // Test for BigInteger Pow(long x, long y)
        public static BigInteger TestPow1(long a, long b)
        {
            return Math.Pow(a, b);
        }

        // Test for BigInteger Pow(int x, int y)
        public static BigInteger TestPow2(int a, int b)
        {
            return Math.Pow(a, b);
        }

        // Test for Abs methods
        public static sbyte TestAbs(sbyte x)
        {
            return Math.Abs(x);
        }

        public static short TestAbs2(short x)
        {
            return Math.Abs(x);
        }

        public static int TestAbs3(int x)
        {
            return Math.Abs(x);
        }

        public static long TestAbs4(long x)
        {
            return Math.Abs(x);
        }

        // Test for Sign methods
        public static sbyte TestSign(sbyte x)
        {
            return Math.Sign(x);
        }

        public static short TestSign1(short x)
        {
            return Math.Sign(x);
        }

        public static int TestSign2(int x)
        {
            return Math.Sign(x);
        }

        public static long TestSign3(long x)
        {
            return Math.Sign(x);
        }

        // Test for Max methods
        public static byte TestMax(byte x, byte y)
        {
            return Math.Max(x, y);
        }

        public static sbyte TestMax1(sbyte x, sbyte y)
        {
            return Math.Max(x, y);
        }

        public static short TestMax2(short x, short y)
        {
            return Math.Max(x, y);
        }

        public static ushort TestMax3(ushort x, ushort y)
        {
            return Math.Max(x, y);
        }

        public static int TestMax4(int x, int y)
        {
            return Math.Max(x, y);
        }

        public static uint TestMax5(uint x, uint y)
        {
            return Math.Max(x, y);
        }

        public static long TestMax6(long x, long y)
        {
            return Math.Max(x, y);
        }

        public static ulong TestMax7(ulong x, ulong y)
        {
            return Math.Max(x, y);
        }

        public static BigInteger TestMax8(BigInteger x, BigInteger y)
        {
            return Math.Max(x, y);
        }

        // Test for Min methods
        public static byte TestMin(byte x, byte y)
        {
            return Math.Min(x, y);
        }

        public static sbyte TestMin1(sbyte x, sbyte y)
        {
            return Math.Min(x, y);
        }

        public static short TestMin2(short x, short y)
        {
            return Math.Min(x, y);
        }

        public static ushort TestMin3(ushort x, ushort y)
        {
            return Math.Min(x, y);
        }

        public static int TestMin4(int x, int y)
        {
            return Math.Min(x, y);
        }

        public static uint TestMin5(uint x, uint y)
        {
            return Math.Min(x, y);
        }

        public static long TestMin6(long x, long y)
        {
            return Math.Min(x, y);
        }

        public static ulong TestMin7(ulong x, ulong y)
        {
            return Math.Min(x, y);
        }

        public static BigInteger TestMin8(BigInteger x, BigInteger y)
        {
            return Math.Min(x, y);
        }
    }
}
