using Neo.SmartContract.Framework.Services.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.TestClasses
{

    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        //define and staticvar and initit with a runtime code.
        static byte[] callscript = ExecutionEngine.EntryScriptHash;

        public static object Main(string method, object[] args)
        {
            if (method == "staticinit")
                return testStaticInit();
            if (method == "directget")
                return ExecutionEngine.EntryScriptHash;
            return null;
        }

        static byte[] testStaticInit()
        {
            return callscript;
        }

    }

}
