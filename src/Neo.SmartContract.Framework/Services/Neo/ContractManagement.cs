#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xbee421fdbb3e791265d2104cb34934f53fcc0e45")]
    public class ContractManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Name { get; }
        public static extern Contract GetContract(UInt160 hash);
        public static extern Contract Deploy(ByteString nefFile, string manifest);
        public static extern void Update(ByteString nefFile, string manifest);
        public static extern void Destroy();
    }
}
