#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xd2a4cff31913016155e38e474a2c06d08be276cf")]
    public class GAS
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data = null);
        public static extern void Refuel(UInt160 account, long amount);
    }
}
