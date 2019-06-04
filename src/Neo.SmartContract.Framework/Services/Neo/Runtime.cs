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

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);

        [Syscall("System.Runtime.Notify")]
        public static extern void Notify(params object[] state);

        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);
    }
}
