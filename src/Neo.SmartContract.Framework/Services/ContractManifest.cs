namespace Neo.SmartContract.Framework.Services
{
    public struct ContractManifest
    {
        public string Name;
        public ContractGroup[] Groups;
        public readonly object Reserved;
        public string[] SupportedStandards;
        public ContractAbi Abi;
        public ContractPermission[] Permissions;
        public ByteString[] Trusts;
        public string Extra;
    }
}
