using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        //default smartcontract entry point.
        //but the unittest can be init from anywhere
        //no need to add code in Main.
        public static object Main(string method, object[] args)
        {
            var byteArray = UnitTest_001();
            var result = Storage.Get(byteArray);
            Storage.Put(byteArray, byteArray);
            return result;
        }
        public static byte[] UnitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

    }
}
