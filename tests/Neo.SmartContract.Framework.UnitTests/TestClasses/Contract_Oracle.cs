using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Account : SmartContract.Framework.SmartContract
    {
        public static byte[] getHash()
        {
            return Oracle.Hash;
        }

        public static byte[] get1(string url, byte[] filter, string filterMethod)
        {
            return Oracle.Get(url, filter, filterMethod);
        }

        public static byte[] get2(string url)
        {
            return Oracle.Get(url);
        }
    }
}
