using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards("NEP10","NEP5")]
    public class Contract_SupportedStandards : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
