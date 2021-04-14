namespace Neo.SmartContract.Framework.Services
{
    public static class Runtime
    {
        public static extern TriggerType Trigger
        {
            [Syscall("System.Runtime.GetTrigger")]
            get;
        }

        public static extern string Platform
        {
            [Syscall("System.Runtime.Platform")]
            get;
        }

        public static extern object ScriptContainer
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

        public static extern ulong Time
        {
            [Syscall("System.Runtime.GetTime")]
            get;
        }

        public static extern uint InvocationCounter
        {
            [Syscall("System.Runtime.GetInvocationCounter")]
            get;
        }

        public static extern long GasLeft
        {
            [Syscall("System.Runtime.GasLeft")]
            get;
        }

        /// <summary>
        /// This method gets current invocation notifications from specific 'scriptHash'
        /// 'scriptHash' must have 20 bytes, but if it's all zero 0000...0000 it refers to all existing notifications (like a * wildcard)
        /// It will return an array of all matched notifications
        /// Each notification has two elements: a ScriptHash and the stackitem content of notification itself (called a 'State')
        /// The stackitem 'State' can be of any kind (a number, a string, an array, ...), so it's up to the developer perform the expected cast here
        /// </summary>
        [Syscall("System.Runtime.GetNotifications")]
        public static extern Notification[] GetNotifications(UInt160 hash = null);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(UInt160 hash);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(Cryptography.ECC.ECPoint pubkey);

        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);

        [Syscall("System.Runtime.BurnGas")]
        public static extern void BurnGas(long gas);
    }
}
