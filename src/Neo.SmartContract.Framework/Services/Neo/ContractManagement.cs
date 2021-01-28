#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xa501d7d7d10983673b61b7a2d3a813b36f9f0e43")]
    public class ContractManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Contract GetContract(UInt160 hash);
        public static extern Contract Deploy(ByteString nefFile, string manifest);
        public static extern void Update(ByteString nefFile, string manifest);
        public static extern Contract Deploy(ByteString nefFile, string manifest, object data);
        public static extern void Update(ByteString nefFile, string manifest, object data);
        public static extern void Destroy();
    }
}
