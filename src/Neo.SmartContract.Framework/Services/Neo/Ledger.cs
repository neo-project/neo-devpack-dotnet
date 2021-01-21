#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x971d69c6dd10ce88e7dfffec1dc603c6125a8764")]
    public class Ledger
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern UInt256 CurrentHash { get; }
        public static extern uint CurrentIndex { get; }
        public static extern Block GetBlock(uint index);
        public static extern Block GetBlock(UInt256 hash);
        public static extern Transaction GetTransaction(UInt256 hash);
        public static extern Transaction GetTransactionFromBlock(UInt256 blockHash, int txIndex);
        public static extern Transaction GetTransactionFromBlock(uint blockHeight, int txIndex);
        public static extern int GetTransactionHeight(UInt256 hash);
    }
}
