namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        //default smartcontract entry point.
        //but the unittest can be init from anywhere
        //no need to add code in Main.
        public static object Main(string method, object[] args)
        {
            return UnitTest_001();
        }
        public static byte[] UnitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

    }
}
