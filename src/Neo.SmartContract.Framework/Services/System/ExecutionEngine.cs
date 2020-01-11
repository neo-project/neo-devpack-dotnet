namespace Neo.SmartContract.Framework.Services.System
{
    public static class ExecutionEngine
    {
        public static extern IScriptContainer ScriptContainer
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern byte[] ExecutingScriptHash
        {
            [Syscall("System.Runtime.GetExecutingScriptHash")]
            get;
        }

        public static extern byte[] CallingScriptHash
        {
            [Syscall("System.Runtime.GetCallingScriptHash")]
            get;
        }

        public static extern byte[] EntryScriptHash
        {
            [Syscall("System.Runtime.GetEntryScriptHash")]
            get;
        }
    }
}
