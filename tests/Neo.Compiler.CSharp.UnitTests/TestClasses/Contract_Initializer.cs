using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Initializer : SmartContract.Framework.SmartContract
    {
        public class data
        {
            public int A = 1;
            public int B = 2;
        }

        public int sum()
        {
            var x = new data();
            return x.A + x.B;
        }

        public int sum1(int a, int b)
        {
            var x = new data
            {
                A = a,
                B = b
            };
            return x.A + x.B;
        }

        public int sum2(int a, int b)
        {
            var x = new data();
            x.A = a;
            x.B = b;
            return x.A + x.B;
        }
    }
}
