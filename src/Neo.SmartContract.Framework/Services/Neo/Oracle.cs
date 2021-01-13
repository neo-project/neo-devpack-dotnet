#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x8cd3889136056b3304ec59f6d424b8767710ed79")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
