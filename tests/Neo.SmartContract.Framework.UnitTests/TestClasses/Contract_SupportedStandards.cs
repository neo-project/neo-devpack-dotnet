using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.MSIL.TestClasses
{
    [SupportedStandards("NEP10","NEP5")]
    public class Contract_SupportedStandards : SmartContract.Framework.SmartContract
    {
        public static bool TestStandard()
        {
            return true;
        }
    }
}
