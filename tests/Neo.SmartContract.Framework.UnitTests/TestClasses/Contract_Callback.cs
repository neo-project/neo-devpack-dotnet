using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Callback : SmartContract.Framework.SmartContract
    {
        public static object CreateCallback()
        {
            return new Func<int>(MyMethod);
        }

        public static int MyMethod()
        {
            return 123;
        }
    }
}
