using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Create : SmartContract
    {
        public static string OldContract()
        {
            return ContractManagement.GetContract(Runtime.ExecutingScriptHash).Manifest.Name;
        }
    }
}
