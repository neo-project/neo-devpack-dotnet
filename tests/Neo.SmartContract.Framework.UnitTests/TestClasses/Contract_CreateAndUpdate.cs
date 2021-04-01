using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_CreateAndUpdate : SmartContract
    {
        public static int OldContract(byte[] nefFile, string manifest)
        {
            ContractManagement.Update((ByteString)nefFile, manifest, null);
            return 123;
        }
    }
}
