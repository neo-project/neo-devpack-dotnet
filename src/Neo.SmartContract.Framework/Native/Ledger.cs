#pragma warning disable CS0626

using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xda65b600f7124ce6c79950c1772a36403104f2be")]
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
