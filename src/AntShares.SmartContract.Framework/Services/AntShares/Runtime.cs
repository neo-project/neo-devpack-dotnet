namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public static class Runtime
    {
        [Syscall("AntShares.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);

        [Syscall("AntShares.Runtime.Notify")]
        public static extern void Notify(params object[] state);

        [Syscall("AntShares.Runtime.Log")]
        public static extern void Log(string message);
    }
}
