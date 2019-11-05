namespace Neo.SmartContract.Framework.Services.Neo
{
    public enum TriggerType : byte
    {
        System = 0x01,
        Verification = 0x20,
        Application = 0x40,
        All = System | Verification | Application
    }
}
