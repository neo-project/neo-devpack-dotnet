#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x8dc0e742cbdfdeda51ff8a8b78d46829144c80ee")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
