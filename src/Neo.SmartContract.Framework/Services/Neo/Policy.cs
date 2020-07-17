#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xce06595079cd69583126dbfd1d2e25cca74cffe9")]
    public class Policy
    {
        public static extern string Name();
        public static extern uint GetMaxTransactionsPerBlock();
        public static extern uint GetMaxBlockSize();
        public static extern long GetMaxBlockSystemFee();
        public static extern BigInteger GetFeePerByte();
        public static extern object GetBlockedAccounts();
        public static extern bool SetMaxBlockSize(uint value);
        public static extern bool SetMaxTransactionsPerBlock(uint value);
        public static extern bool SetMaxBlockSystemFee(long value);
        public static extern bool SetFeePerByte(long value);
        public static extern bool BlockAccount(byte[] account);
        public static extern bool UnblockAccount(byte[] account);
    }
}
