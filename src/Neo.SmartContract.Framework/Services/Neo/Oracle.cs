#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xb1c37d5847c2ae36bdde31d0cc833a7ad9667f8f")]
    public class Oracle
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public const uint MinimumResponseFee = 0_10000000;
        public static extern int Id { get; }
        public static extern uint ActiveBlockIndex { get; }
        public static extern void Request(string url, string filter, string callback, object userData, long gasForResponse);
    }
}
