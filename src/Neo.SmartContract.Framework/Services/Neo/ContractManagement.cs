#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x081514120c7894779309255b7fb18b376cec731a")]
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
