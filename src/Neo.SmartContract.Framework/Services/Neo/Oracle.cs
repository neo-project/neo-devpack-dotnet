#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x35e4fc2e69a4d04d1db4d755c4150c50aff2e9a9")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern int Id { get; }
        public static extern uint ActiveBlockIndex { get; }
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
