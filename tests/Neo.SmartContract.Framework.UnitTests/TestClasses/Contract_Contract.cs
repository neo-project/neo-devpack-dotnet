using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Contract : SmartContract.Framework.SmartContract
    {
        public static object Call(UInt160 scriptHash, string method, byte flag, bool hasReturnValue, ushort pcount)
        {
            return Contract.Call(scriptHash, method, flag, hasReturnValue, pcount);
        }

        public static object Create(byte[] nef, string manifest)
        {
            return ContractManagement.Deploy(nef, manifest);
        }

        public static void Update(byte[] nef, string manifest)
        {
            ContractManagement.Update(nef, manifest);
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
