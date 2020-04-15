namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Oracle
    {
        [Syscall("Neo.Oracle.Get")]
        public static extern byte[] Get(string url, byte[] filterAddress, string filterMethod);

        [OpCode(OpCode.PUSHNULL)]   // filterMethod
        [OpCode(OpCode.PUSHNULL)]   // filterAddress
        [OpCode(OpCode.ROT)]        // Move URL (this could be optimized by NefOptimizer)
        [Syscall("Neo.Oracle.Get")]
        public static extern byte[] Get(string url);

        public static extern byte[] Hash
        {
            [Syscall("Neo.Oracle.Hash")]
            get;
        }
    }
}
