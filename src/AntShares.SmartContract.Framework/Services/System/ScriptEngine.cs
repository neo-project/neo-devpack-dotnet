namespace AntShares.SmartContract.Framework.Services.System
{
    public class ScriptEngine
    {
        public extern IScriptContainer ScriptContainer
        {
            [Syscall("System.ScriptEngine.GetScriptContainer")]
            get;
        }

        public extern byte[] ExecutingScriptHash
        {
            [Syscall("System.ScriptEngine.GetExecutingScriptHash")]
            get;
        }

        public extern byte[] CallingScriptHash
        {
            [Syscall("System.ScriptEngine.GetCallingScriptHash")]
            get;
        }

        public extern byte[] EntryScriptHash
        {
            [Syscall("System.ScriptEngine.GetEntryScriptHash")]
            get;
        }
    }
}
