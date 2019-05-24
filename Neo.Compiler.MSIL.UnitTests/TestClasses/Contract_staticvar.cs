using Neo.SmartContract.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        static int a1 = 1;

        public static object Main(string method, object[] args)
        {
            testadd();
            testmulti();
            return a1;
        }

        static void testadd()
        {
            a1 += 5;
        }
        static void testmulti()
        {
            a1 *= 7;
        }
    }
}
