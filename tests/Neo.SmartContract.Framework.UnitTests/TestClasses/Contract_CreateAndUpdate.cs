using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_CreateAndUpdate : SmartContract.Framework.SmartContract
    {
        public static int OldContract(byte[] script, string manifest)
        {
            Contract.Update(script, manifest);
            return 123;
        }
    }
}
