#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x7ab39c37afd995f2f947a7ecbf40e91307058595")]
    public class Designation
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(DesignationRole role, uint index);
    }
}
