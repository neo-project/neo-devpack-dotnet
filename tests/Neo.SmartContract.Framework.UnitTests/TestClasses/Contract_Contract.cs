using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Contract : SmartContract.Framework.SmartContract
    {
        public static object Call(UInt160 scriptHash, string method, CallFlags flag, object[] args)
        {
            return Contract.Call(scriptHash, method, flag, args);
        }

        public static object Create(byte[] nef, string manifest)
        {
            return ContractManagement.Deploy((ByteString)nef, manifest);
        }

        public static void Update(byte[] nef, string manifest)
        {
            ContractManagement.Update((ByteString)nef, manifest);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }

        public static int GetCallFlags()
        {
            return Contract.GetCallFlags();
        }

        public static UInt160 CreateStandardAccount(Cryptography.ECC.ECPoint pubKey)
        {
            return Contract.CreateStandardAccount(pubKey);
        }
    }
}
