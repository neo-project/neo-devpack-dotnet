using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Contract : SmartContract.Framework.SmartContract
    {
        public static object Call(UInt160 scriptHash, string method, object[] arguments)
        {
            return Contract.Call(scriptHash, method, arguments);
        }

        public static object Create(byte[] nef, string manifest)
        {
            return ManagementContract.Deploy(nef, manifest);
        }

        public static void Update(byte[] nef, string manifest)
        {
            ManagementContract.Update(nef, manifest);
        }

        public static void Destroy()
        {
            ManagementContract.Destroy();
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
