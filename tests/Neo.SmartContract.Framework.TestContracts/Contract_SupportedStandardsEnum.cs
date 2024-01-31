using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NEPStandard.NEP11, NEPStandard.NEP17)]
    public class ContractSupportedStandardsEnum : SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
