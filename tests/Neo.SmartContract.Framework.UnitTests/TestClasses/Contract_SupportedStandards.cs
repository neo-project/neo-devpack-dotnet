using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    [SupportedStandards("NEP10","NEP5")]
    public class Contract_SupportedStandards : SmartContract.Framework.SmartContract
    {
        public static bool AccountIsStandard(byte[] scripthash)
        {
            return Account.IsStandard(scripthash);
        }
    }
}
