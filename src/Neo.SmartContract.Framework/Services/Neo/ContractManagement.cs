#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xc530c494119164a1374a755aa54b1016749dc339")]
    public class ContractManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Name { get; }
        public static extern Contract GetContract(UInt160 hash);
        public static extern Contract Deploy(byte[] nefFile, string manifest);
        public static extern void Update(byte[] nefFile, string manifest);
        public static extern void Destroy();
    }
}
