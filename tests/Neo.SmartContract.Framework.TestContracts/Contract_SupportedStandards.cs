using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    // Both NEP-10 and NEP-5 are obsolete, but this is just a test contract
    [SupportedStandards("NEP-10", "NEP-5")]
    public class Contract_SupportedStandards : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
