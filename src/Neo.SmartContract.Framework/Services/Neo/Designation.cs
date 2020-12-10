#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x7062149f9377e3a110a343f811b9e406f8ef7824")]
    public class Designation
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(DesignationRole role, uint index);
    }
}
