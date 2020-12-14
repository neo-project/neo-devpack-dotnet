namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xcd97b70d82d69adfcd9165374109419fade8d6ab")]
    public class ManagementContract
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern string Name { get; }
        public static extern Contract GetContract(UInt160 hash);
        public static extern Contract Deploy(byte[] nefFile, string manifest);
        public static extern void Update(byte[] nefFile, string manifest);
        public static extern void Destroy();
    }
}
