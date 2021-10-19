using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_CreateAndUpdate : SmartContract
    {
        public static string OldContract(byte[] nefFile, string manifest)
        {
            ContractManagement.Update((ByteString)nefFile, manifest, null);
            return ContractManagement.GetContract(Runtime.ExecutingScriptHash).Manifest.Name;
        }
    }
}
