#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x597b1471bbce497b7809e2c8f10db67050008b02")]
    public class RoleManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(Role role, uint index);
    }
}
