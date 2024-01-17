using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_ExecutionEngine : SmartContract
    {
        public static byte[] CallingScriptHash()
        {
            return (byte[])Runtime.CallingScriptHash;
        }

        public static byte[] EntryScriptHash()
        {
            return (byte[])Runtime.EntryScriptHash;
        }

        public static byte[] ExecutingScriptHash()
        {
            return (byte[])Runtime.ExecutingScriptHash;
        }

        public static object ScriptContainer()
        {
            return Runtime.ScriptContainer;
        }
    }
}
