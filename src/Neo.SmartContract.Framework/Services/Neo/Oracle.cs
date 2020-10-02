#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x3c05b488bf4cf699d0631bf80190896ebbf38c3b")]
    public class Oracle
    {
        public const uint MinimumResponseFee = 0_60000000; // OracleRequestPrice=0_50000000 gasForResponse=0_10000000
        public static extern string Name { get; }
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
