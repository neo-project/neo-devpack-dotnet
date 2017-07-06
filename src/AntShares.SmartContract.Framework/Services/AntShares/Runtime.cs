namespace AntShares.SmartContract.Framework.Services.AntShares
{
    public static class Runtime
    {
        [Syscall("AntShares.Runtime.CheckWitness")]
        public static extern bool CheckWitness(byte[] hashOrPubkey);
    }
}
