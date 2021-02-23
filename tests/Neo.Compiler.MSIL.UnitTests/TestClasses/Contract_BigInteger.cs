using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{

    class Contract_BigInteger : SmartContract.Framework.SmartContract
    {
        public static object TestPow(BigInteger x, int y)
        {
            return BigInteger.Pow(x, y);
        }

        public static object TestPowInt(int x, int y)
        {
            return x.Pow(y);
        }

        public static object TestPowUInt(uint x, int y)
        {
            return x.Pow(y);
        }

        public static object TestPowLong(long x, int y)
        {
            return x.Pow(y);
        }

        public static object TestPowULong(ulong x, int y)
        {
            return x.Pow(y);
        }

        public static object TestPowBigInteger(BigInteger x, int y)
        {
            return x.Pow(y);
        }


        public static object TestSqrt(BigInteger x)
        {
            return x.Sqrt();
        }

        public static object TestSqrtInt(int x)
        {
            return x.Sqrt();
        }

        public static object TestSqrtUInt(uint x)
        {
            return x.Sqrt();
        }

        public static object TestSqrtLong(long x)
        {
            return x.Sqrt();
        }

        public static object TestSqrtULong(ulong x)
        {
            return x.Sqrt();
        }




    }
}
