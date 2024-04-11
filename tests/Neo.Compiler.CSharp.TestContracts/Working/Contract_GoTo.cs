using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_GoTo : SmartContract.Framework.SmartContract
    {
        public static int test()
        {
            int a = 1;
        sum:
            a++;
            if (a == 3) return a;

            goto sum;
        }

        public static int testTry()
        {
            int a = 1;
        sum:
            try
            {
                a++;
                if (a == 3) return a;
            }
            catch { }
            goto sum;
        }
    }
}
