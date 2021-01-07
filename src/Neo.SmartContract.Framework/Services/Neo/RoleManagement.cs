#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x35fa4a901392076619a3269626b6580c0b2afdf9")]
    public class RoleManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(DesignationRole role, uint index);
    }
}
