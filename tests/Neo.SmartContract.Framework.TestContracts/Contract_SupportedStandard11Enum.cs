using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [SupportedStandards(NEPStandard.NEP11)]
    public class Contract_SupportedStandard11Enum : Nep11Token<Nep11TokenState>
    {
        public static bool TestStandard()
        {
            return true;
        }

        public override string Symbol { [Safe] get; }
    }
}
