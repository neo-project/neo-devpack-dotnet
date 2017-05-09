namespace AntShares.SmartContract.Framework.Services.System
{
    public class ExecutionEngine
    {
        public extern IScriptContainer ScriptContainer
        {
            [Syscall("System.ExecutionEngine.GetScriptContainer")]
            get;
        }

        public extern byte[] ExecutingScriptHash
        {
            [Syscall("System.ExecutionEngine.GetExecutingScriptHash")]
            get;
        }

        public extern byte[] CallingScriptHash
        {
            [Syscall("System.ExecutionEngine.GetCallingScriptHash")]
            get;
        }

        public extern byte[] EntryScriptHash
        {
            [Syscall("System.ExecutionEngine.GetEntryScriptHash")]
            get;
        }
    }
}
