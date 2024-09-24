using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_BigInteger : SmartContract.Framework.SmartContract
    {
        public static BigInteger TestPow(BigInteger x, int y)
        {
            return BigInteger.Pow(x, y);
        }

        public static BigInteger TestSqrt(BigInteger x)
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

        public static char testchar(BigInteger input)
        {
            try
            {
                char x = (char)input;
                return x;
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static string testchartostring(BigInteger input)
        {
            char x = (char)input;
            return x.ToString();
        }

        public static bool testIsEven(BigInteger input)
        {
            return input.IsEven;
        }

        public static bool testIsZero(BigInteger input)
        {
            return input.IsZero;
        }

        public static bool testIsOne(BigInteger input)
        {
            return input.IsOne;
        }

        public static int testSign(BigInteger input)
        {
            return input.Sign;
        }

        public static BigInteger TestAdd(BigInteger x, BigInteger y)
        {
            return BigInteger.Add(x, y);
        }

        public static BigInteger TestSubtract(BigInteger x, BigInteger y)
        {
            return BigInteger.Subtract(x, y);
        }

        public static BigInteger TestNegate(BigInteger x)
        {
            return BigInteger.Negate(x);
        }

        public static BigInteger TestMultiply(BigInteger x, BigInteger y)
        {
            return BigInteger.Multiply(x, y);
        }

        public static BigInteger TestDivide(BigInteger x, BigInteger y)
        {
            return BigInteger.Divide(x, y);
        }

        public static BigInteger TestRemainder(BigInteger x, BigInteger y)
        {
            return BigInteger.Remainder(x, y);
        }

        public static int TestCompare(BigInteger x, BigInteger y)
        {
            return BigInteger.Compare(x, y);
        }

        public static BigInteger TestGreatestCommonDivisor(BigInteger x, BigInteger y)
        {
            return BigInteger.GreatestCommonDivisor(x, y);
        }

        public static bool TestEquals(BigInteger x, BigInteger y)
        {
            return x.Equals(y);
        }

        public static BigInteger ParseConstant()
        {
            return BigInteger.Parse("100000000000000000000000000");
        }

        public static BigInteger TestModPow()
        {
            BigInteger number = 10;
            int exponent = 3;
            BigInteger modulus = 30;
            return BigInteger.ModPow(number, exponent, modulus);
        }
    }
}
