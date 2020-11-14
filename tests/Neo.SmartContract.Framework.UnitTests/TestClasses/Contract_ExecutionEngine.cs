using Neo.SmartContract.Framework.Services.System;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_ExecutionEngine : SmartContract.Framework.SmartContract
    {
        public static byte[] CallingScriptHash()
        {
            return (byte[])ExecutionEngine.CallingScriptHash;
        }

        public static byte[] EntryScriptHash()
        {
            return (byte[])ExecutionEngine.EntryScriptHash;
        }

        public static byte[] ExecutingScriptHash()
        {
            return (byte[])ExecutionEngine.ExecutingScriptHash;
        }

        public static object ScriptContainer()
        {
            return ExecutionEngine.ScriptContainer;
        }
    }
}
