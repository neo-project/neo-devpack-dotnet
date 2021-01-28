using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;

namespace Neo.Compiler.MSIL.TestClasses
{
    [DisplayName("Contract_CreateAndUpdate")]
    public class Contract_CreateAndUpdate : SmartContract.Framework.SmartContract
    {
        public static int OldContract(byte[] nefFile, string manifest)
        {
            ContractManagement.Update((ByteString)nefFile, manifest, null);
            return 123;
        }
    }
}
