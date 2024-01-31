using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NEPStandard.NEP11, NEPStandard.NEP17)]
    public class Contract_SupportedStandardsEnum : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
