#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x136ec44854ad9a714901eb7d714714f1791203f2")]
    public class RoleManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(DesignationRole role, uint index);
    }
}
