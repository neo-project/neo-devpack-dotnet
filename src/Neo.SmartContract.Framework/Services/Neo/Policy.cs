#pragma warning disable CS0626

using System.Numerics;

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xcc5e4edd9f5f8dba8bb65734541df7a1c081c67b")]
    public class Policy
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern BigInteger GetFeePerByte();
        public static extern BigInteger GetExecFeeFactor();
        public static extern BigInteger GetStoragePrice();
        public static extern string[] IsBlocked(UInt160 account);
    }
}
