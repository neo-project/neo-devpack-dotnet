using System;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static int[] Main()
        {
            int v = 8;
            var v1 = v << 1;
            var v2 = v >> 1;
            return new int[] { v1, v2 };
        }
    }
}
