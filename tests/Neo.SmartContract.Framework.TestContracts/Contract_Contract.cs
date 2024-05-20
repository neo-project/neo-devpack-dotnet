using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Contract : SmartContract
    {
        public static object Call(UInt160 scriptHash, string method, CallFlags flag, object[] args)
        {
            return Contract.Call(scriptHash, method, flag, args);
        }

        public static object Create(byte[] nef, string manifest)
        {
            return ContractManagement.Deploy((ByteString)nef, manifest, null);
        }

        public static int GetCallFlags()
        {
            return (int)Contract.GetCallFlags();
        }

        public static UInt160 CreateStandardAccount(ECPoint pubKey)
        {
            return Contract.CreateStandardAccount(pubKey);
        }
    }
}
