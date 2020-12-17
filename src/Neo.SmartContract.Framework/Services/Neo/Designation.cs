#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0xc0073f4c7069bf38995780c9da065f9b3949ea7a")]
    public class Designation
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(DesignationRole role, uint index);
    }
}
