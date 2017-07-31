namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Runtime
    {
        [Syscall("Neo.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);

        [Syscall("Neo.Runtime.Notify")]
        public static extern void Notify(params object[] state);

        [Syscall("Neo.Runtime.Log")]
        public static extern void Log(string message);
    }
}
