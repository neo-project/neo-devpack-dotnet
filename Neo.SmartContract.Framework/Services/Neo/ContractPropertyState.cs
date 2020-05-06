namespace Neo.SmartContract.Framework.Services.Neo
{
    public enum ContractPropertyState : byte
    {
        NoProperty = 0,

        HasStorage = 1 << 0,
        HasDynamicInvoke = 1 << 1,
        Payable = 1 << 2
    }
}
