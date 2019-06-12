using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            int v = 8;
            var v1 = v << 1;
            var v2 = v << -1;
            var v3 = v >> 1;
            var v4 = v >> -1;
            Neo.SmartContract.Framework.Services.Neo.Runtime.Log((string)(object)v1);
            Neo.SmartContract.Framework.Services.Neo.Runtime.Log((string)(object)v2);
            Neo.SmartContract.Framework.Services.Neo.Runtime.Log((string)(object)v3);
            Neo.SmartContract.Framework.Services.Neo.Runtime.Log((string)(object)v4);
            return false;
        }
    }
}
