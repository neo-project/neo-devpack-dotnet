#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x79bcd398505eb779df6e67e4be6c14cded08e2f2")]
    public class Policy
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern uint GetMaxTransactionsPerBlock();
        public static extern uint GetMaxBlockSize();
        public static extern long GetMaxBlockSystemFee();
        public static extern BigInteger GetFeePerByte();
        public static extern string[] IsBlocked(UInt160 account);
        public static extern bool SetMaxBlockSize(uint value);
        public static extern bool SetMaxTransactionsPerBlock(uint value);
        public static extern bool SetMaxBlockSystemFee(long value);
        public static extern bool SetFeePerByte(long value);
        public static extern bool BlockAccount(UInt160 account);
        public static extern bool UnblockAccount(UInt160 account);
    }
}
