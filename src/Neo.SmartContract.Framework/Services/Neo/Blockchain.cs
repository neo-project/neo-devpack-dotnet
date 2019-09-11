using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Blockchain
    {
        [Syscall("Neo.Blockchain.GetHeight")]
        public static extern uint GetHeight();

        [Syscall("Neo.Blockchain.GetBlock")]
        public static extern Block GetBlock(uint height);

        [Syscall("Neo.Blockchain.GetBlock")]
        public static extern Block GetBlock(byte[] hash);

        [Syscall("Neo.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(byte[] hash);

        [Syscall("System.Blockchain.GetTransactionHeight")]
        public static extern BigInteger GetTransactionHeight(byte[] hash);

        [Syscall("Neo.Blockchain.GetContract")]
        public static extern Contract GetContract(byte[] script_hash);
    }
}
