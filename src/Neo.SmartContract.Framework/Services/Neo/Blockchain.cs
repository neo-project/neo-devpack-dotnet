using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    public static class Blockchain
    {
        [Syscall("System.Blockchain.GetHeight")]
        public static extern uint GetHeight();

        [Syscall("System.Blockchain.GetBlock")]
        public static extern Block GetBlock(uint height);

        [Syscall("System.Blockchain.GetBlock")]
        public static extern Block GetBlock(UInt256 hash);

        [Syscall("System.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(UInt256 hash);

        [Syscall("System.Blockchain.GetTransactionFromBlock")]
        public static extern Transaction GetTransactionFromBlock(UInt256 blockHash, int txIndex);

        [Syscall("System.Blockchain.GetTransactionFromBlock")]
        public static extern Transaction GetTransactionFromBlock(uint blockIndex, int txIndex);

        [Syscall("System.Blockchain.GetTransactionHeight")]
        public static extern BigInteger GetTransactionHeight(UInt256 hash);
    }
}
