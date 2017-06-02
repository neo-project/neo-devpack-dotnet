namespace AntShares.SmartContract.Framework.Services.System
{
    public static class ExecutionEngine
    {
        public static extern IScriptContainer ScriptContainer
        {
            [Syscall("System.ExecutionEngine.GetScriptContainer")]
            get;
        }

        public static extern byte[] ExecutingScriptHash
        {
            [Syscall("System.ExecutionEngine.GetExecutingScriptHash")]
            get;
        }

        public static extern byte[] CallingScriptHash
        {
            [Syscall("System.ExecutionEngine.GetCallingScriptHash")]
            get;
        }

        public static extern byte[] EntryScriptHash
        {
            [Syscall("System.ExecutionEngine.GetEntryScriptHash")]
            get;
        }
    }
}
