using Neo.SmartContract.Framework.Services.System;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        //define and staticvar and initit with a runtime code.
        static byte[] callscript = (byte[])ExecutionEngine.EntryScriptHash;

        public static object StaticInit()
        {
            return TestStaticInit();
        }

        public static object DirectGet()
        {
            return ExecutionEngine.EntryScriptHash;
        }

        static byte[] TestStaticInit()
        {
            return callscript;
        }
    }
}
