using Neo.SmartContract.Framework.Services.System;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_staticconstruct : SmartContract.Framework.SmartContract
    {
        static int a;
        //define and staticvar and initit with a runtime code.
        static Contract_staticconstruct()
        {
            int b = 3;
            a = b + 1;
        }

        public static object TestStatic()
        {
            return a;
        }
    }
}
