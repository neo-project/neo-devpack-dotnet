using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Contract : SmartContract.Framework.SmartContract
    {
        public static object Call(byte[] scriptHash, string method, object[] arguments)
        {
            return Contract.Call(scriptHash, method, arguments);
        }

        public static object Create(byte[] script, string manifest)
        {
            return Contract.Create(script, manifest);
        }

        public static void Update(byte[] script, string manifest)
        {
            Contract.Update(script, manifest);
        }

        public static void Destroy()
        {
            Contract.Destroy();
        }

        public static int GetCallFlags()
        {
            return Contract.GetCallFlags();
        }

        public static byte[] CreateStandardAccount(byte[] pubKey)
        {
            return Contract.CreateStandardAccount(pubKey);
        }
    }
}
