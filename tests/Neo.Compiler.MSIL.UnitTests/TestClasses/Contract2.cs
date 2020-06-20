namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract2 : SmartContract.Framework.SmartContract
    {
        public static byte UnitTest_002(object arg1, object arg2)
        {
            Neo.SmartContract.Framework.Services.Neo.Runtime.Notify(arg1);
            Neo.SmartContract.Framework.Services.Neo.Runtime.Notify(arg2);
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb[2];
        }
    }
}
