#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x49cf4e5378ffcd4dec034fd98a174c5491e395e2")]
    public class RoleManagement
    {
        public static extern UInt160 Hash { [ContractHash] get; }
        public static extern Cryptography.ECC.ECPoint[] GetDesignatedByRole(Role role, uint index);
    }
}
