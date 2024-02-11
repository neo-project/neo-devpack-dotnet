using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_shift_bigint : SmartContract.Framework.SmartContract
    {
        public static System.Numerics.BigInteger[] TestMain()
        {
            System.Numerics.BigInteger v = 8;
            var v1 = v << 0;
            var v2 = v << 1;
            var v3 = v >> 1;
            var v4 = v >> 2;
            return new System.Numerics.BigInteger[] { v1, v2, v3, v4 };
        }
    }
}
