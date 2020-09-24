#pragma warning disable CS0626

namespace Neo.SmartContract.Framework.Services.Neo
{
    [Contract("0x763afecf3ebba0a67568a2c8be06e8f068c62666")]
    public class Designation
    {
        public static extern string Name { get; }
        public static extern byte[][] GetDesignatedByRole(DesignationRole role);
    }
}
