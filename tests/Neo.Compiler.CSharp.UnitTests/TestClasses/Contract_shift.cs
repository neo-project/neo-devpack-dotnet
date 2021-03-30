using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_shift : SmartContract.Framework.SmartContract
    {
        [DisplayName("event")]
        public static event Action<int> notify;

        public static object Main()
        {
            int v = 8;
            var v1 = v << 1;
            var v2 = v >> 1;
            notify(v1);
            notify(v2);
            return false;
        }
    }
}
