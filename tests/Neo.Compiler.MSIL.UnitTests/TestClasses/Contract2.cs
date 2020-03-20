namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract2 : SmartContract.Framework.SmartContract
    {
        //default smartcontract entry point.
        //but the unittest can be init from anywhere
        //no need to add code in Main.
        public static object Main(string method, object[] args)
        {
            Neo.SmartContract.Framework.Services.Neo.Runtime.Notify(args[0]);
            Neo.SmartContract.Framework.Services.Neo.Runtime.Notify(args[2]);
            return UnitTest_002();
        }
        public static byte UnitTest_002()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb[2];
        }
    }
}
