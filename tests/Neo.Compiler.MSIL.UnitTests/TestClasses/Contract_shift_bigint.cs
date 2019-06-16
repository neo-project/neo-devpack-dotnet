using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_shift_bigint : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            System.Numerics.BigInteger v = 8;
            var v1 = v << 1;
            var v2 = v << -1;
            var v3 = v >> 1;
            var v4 = v >> -1;
            Neo.SmartContract.Framework.Services.Neo.Runtime.Notify(v1, v2, v3, v4);
            return false;
        }
    }
}
