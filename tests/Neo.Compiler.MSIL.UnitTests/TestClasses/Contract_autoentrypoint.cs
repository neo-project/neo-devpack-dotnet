using Neo.SmartContract.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_autoentrypoint : SmartContract.Framework.SmartContract
    {
        //there is no main here, it can be auto generation.
        //object Main(string name,object[])
        //{
        //    if(name=="call01")
        //    {
        //        return call01();
        //    }
        //    if(name=="call02")
        //    {
        //        call02(_params[0],params[1]);
        //        return null;
        //    }
        //}

        private static bool privateMethod()
        {
            return true;
        }

        public static byte[] call01()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }
        public static void call02(string a, int b)
        {
            Neo.SmartContract.Framework.Services.Neo.Runtime.Log(a);
        }
    }
}
