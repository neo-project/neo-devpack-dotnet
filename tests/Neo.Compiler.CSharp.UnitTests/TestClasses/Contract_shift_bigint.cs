using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_shift_bigint : SmartContract.Framework.SmartContract
    {
        [DisplayName("event")]
        public static event Action<System.Numerics.BigInteger> notify;

        public static object Main()
        {
            System.Numerics.BigInteger v = 8;
            var v1 = v << 0;
            var v2 = v << 1;
            var v3 = v >> 1;
            var v4 = v >> 2;
            notify(v1);
            notify(v2);
            notify(v3);
            notify(v4);
            return false;
        }
    }
}
