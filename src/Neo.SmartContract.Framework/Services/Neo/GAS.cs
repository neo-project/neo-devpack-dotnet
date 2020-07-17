using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x668e0c1f9d7b70a99dd9e06eadd4c784d641afbc")]
    public class GAS
    {
        public static extern int Id();
        public static extern string Name();
        public static extern string Symbol();
        public static extern byte Decimals();
        public static extern BigInteger TotalSupply();
        public static extern BigInteger BalanceOf(byte[] account);
    }
}
