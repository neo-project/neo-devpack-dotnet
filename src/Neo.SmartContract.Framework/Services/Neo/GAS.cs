#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xb399c051778cf37a1e4ef88509b2e054d0420a32")]
    public class GAS
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern int Id { get; }
        public static extern uint ActiveBlockIndex { get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    }
}
