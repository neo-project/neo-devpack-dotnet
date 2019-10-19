using Neo.SmartContract.Framework.Services.System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_ExecutionEngine : SmartContract.Framework.SmartContract
    {
        public static byte[] CallingScriptHash()
        {
            return ExecutionEngine.CallingScriptHash;
        }

        public static byte[] EntryScriptHash()
        {
            return ExecutionEngine.EntryScriptHash;
        }

        public static byte[] ExecutingScriptHash()
        {
            return ExecutionEngine.ExecutingScriptHash;
        }

        public static object ScriptContainer()
        {
            return ExecutionEngine.ScriptContainer;
        }
    }
}
