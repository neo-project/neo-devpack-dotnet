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

        public static object TestSqrt(BigInteger x)
        {
            return x.Sqrt();
        }
    }
}
