#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xfe723d2bf2e9eace4a21ac7a93d9598710cb0e68")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
