using Neo.SmartContract.Framework;

namespace TestContract
{
    public class TestClass2: SmartContract
    {
        //default smartcontract entry point.
        //but the unittest can be init from anywhere
        //no need to add code in Main.
        public static object Main(string method, object[] args)
        {
            return UnitTest_002();
        }
        public static byte UnitTest_002()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb[2];
        }
    }
}
