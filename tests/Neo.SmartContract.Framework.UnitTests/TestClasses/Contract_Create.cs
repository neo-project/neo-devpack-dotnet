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

        public static Contract GetContractById(int id)
        {
            return ContractManagement.GetContractById(id);
        }

        public static object GetContractHashes()
        {
            var iter = ContractManagement.GetContractHashes();
            iter.Next();
            return iter.Value.Item2;
        }
    }
}
