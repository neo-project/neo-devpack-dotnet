using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Update : SmartContract
    {
        public static string NewContract()
        {
            return ContractManagement.GetContract(Runtime.ExecutingScriptHash).Manifest.Name;
        }
    }
}
