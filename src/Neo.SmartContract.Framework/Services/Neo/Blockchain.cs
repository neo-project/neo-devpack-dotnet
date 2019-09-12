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
        public static extern Block GetBlock(byte[] hash);

        [Syscall("System.Blockchain.GetTransaction")]
        public static extern Transaction GetTransaction(byte[] hash);

        [Syscall("System.Blockchain.GetTransactionHeight")]
        public static extern BigInteger GetTransactionHeight(byte[] hash);

        [Syscall("System.Blockchain.GetContract")]
        public static extern Contract GetContract(byte[] script_hash);
    }
}
