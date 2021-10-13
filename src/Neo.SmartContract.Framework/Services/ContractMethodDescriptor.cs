namespace Neo.SmartContract.Framework.Services
{
    public struct ContractMethodDescriptor
    {
        public string Name;
        public ContractParameterDefinition[] Parameters;
        public ContractParameterType ReturnType;
        public int Offset;
        public bool Safe;
    }
}
