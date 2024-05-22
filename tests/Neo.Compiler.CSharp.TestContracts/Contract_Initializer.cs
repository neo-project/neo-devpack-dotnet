using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Initializer : SmartContract.Framework.SmartContract
    {
        public class Data
        {
            public int A = 1;
            public int B = 2;
        }

        public int sum()
        {
            var x = new Data();
            return x.A + x.B;
        }

        public int sum1(int a, int b)
        {
            var x = new Data
            {
                A = a,
                B = b
            };
            return x.A + x.B;
        }

        public int sum2(int a, int b)
        {
            var x = new Data();
            x.A = a;
            x.B = b;
            return x.A + x.B;
        }

        public void anonymousObjectCreation()
        {
            var v = new { Amount = 108, Message = "Hello" };
            Runtime.Log(v.Message);
            var anonArray = new[] { new { name = "apple", diam = 4 }, new { name = "grape", diam = 1 } };
            Runtime.Log(anonArray[0].name);
        }
    }
}
