#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xb82bbf650f963dbf71577d10ea4077e711a13e7b")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
