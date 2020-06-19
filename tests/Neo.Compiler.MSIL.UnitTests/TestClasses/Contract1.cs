namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        public static byte[] unitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

    }
}
