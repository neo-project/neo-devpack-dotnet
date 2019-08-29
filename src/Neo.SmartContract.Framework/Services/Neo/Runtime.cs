namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Runtime
    {
        public static extern TriggerType Trigger
        {
            [Syscall("System.Runtime.GetTrigger")]
            get;
        }

        public static extern uint Time
        {
            [Syscall("System.Runtime.GetTime")]
            get;
        }

        public static extern uint InvocationCounter
        {
            [Syscall("System.Runtime.GetInvocationCounter")]
            get;
        }

        /// <summary>
        /// This method gets current invocation notifications from specific 'scriptHash'
        /// 'scriptHash' must have 20 bytes, but if it's all zero 0000...0000 it refers to all existing notifications (like a * wildcard)
        /// It will return an array (object[]) with all matched notifications
        /// Each notification has two elements: a scriptHash and the stackitem content of notification itself (called a 'state')
        /// The stackitem 'state' can be of any kind (a number, a string, an array, ...), so it's up to the developer perform the expected cast here
        /// </summary>
        [Syscall("System.Runtime.GetNotifications")]
        public static extern object[] GetNotifications(byte[] scriptHash);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);

        [Syscall("System.Runtime.Notify")]
        public static extern void Notify(params object[] state);

        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);
    }
}
