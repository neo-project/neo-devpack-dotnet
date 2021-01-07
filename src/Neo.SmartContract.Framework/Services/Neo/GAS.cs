#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x149a7f61eb3b4763b9655836ec7e75ddafdd1717")]
    public class GAS
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Symbol { get; }
        public static extern byte Decimals { get; }
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(UInt160 account);
        public static extern bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data);
    }
}
