#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern long GetFeePerByte();
        public static extern uint GetExecFeeFactor();
        public static extern uint GetStoragePrice();
        public static extern bool IsBlocked(UInt160 account);
    }
}
