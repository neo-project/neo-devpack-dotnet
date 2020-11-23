namespace Neo.SmartContract.Framework.Services.System
{
    public static class ExecutionEngine
    {
        public static extern IScriptContainer ScriptContainer
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern UInt160 ExecutingScriptHash
        {
            [Syscall("System.Runtime.GetExecutingScriptHash")]
            get;
        }

        public static extern UInt160 CallingScriptHash
        {
            [Syscall("System.Runtime.GetCallingScriptHash")]
            get;
        }

        public static extern UInt160 EntryScriptHash
        {
            [Syscall("System.Runtime.GetEntryScriptHash")]
            get;
        }
    }
}
