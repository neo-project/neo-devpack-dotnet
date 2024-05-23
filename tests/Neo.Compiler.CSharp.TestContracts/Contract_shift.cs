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
    }
}
