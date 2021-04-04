using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_BigInteger : SmartContract.Framework.SmartContract
    {
        public static object TestPow(BigInteger x, int y)
        {
            return BigInteger.Pow(x, y);
        }

        public static object TestSqrt(BigInteger x)
        {
            return x.Sqrt();
        }

        public static sbyte testsbyte(BigInteger input)
        {
            try
            {
                sbyte x = (sbyte)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static byte testbyte(BigInteger input)
        {
            try
            {
                byte x = (byte)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static short testshort(BigInteger input)
        {
            try
            {
                short x = (short)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static ushort testushort(BigInteger input)
        {
            try
            {
                ushort x = (ushort)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static int testint(BigInteger input)
        {
            try
            {
                int x = (int)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static uint testuint(BigInteger input)
        {
            try
            {
                uint x = (uint)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static long testlong(BigInteger input)
        {
            try
            {
                long x = (long)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static ulong testulong(BigInteger input)
        {
            try
            {
                ulong x = (ulong)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }
    }
}
