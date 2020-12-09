using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_CreateAndUpdate : SmartContract.Framework.SmartContract
    {
        public static int OldContract(byte[] nefFile, string manifest)
        {
            Contract.Update(script, manifest);
            Contract.Call(ManagementContract.Hash, "update", new object[] { nefFile, manifest });
            return 123;
        }
    }
}
