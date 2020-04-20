using Neo.SmartContract.Framework.Services.System;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        //define and staticvar and initit with a runtime code.
        static byte[] callscript = ExecutionEngine.EntryScriptHash;

        public static object staticinit()
        {
            return testStaticInit();
        }

        public static object directget()
        {
            return ExecutionEngine.EntryScriptHash;
        }

        static byte[] testStaticInit()
        {
            return callscript;
        }
    }
}
