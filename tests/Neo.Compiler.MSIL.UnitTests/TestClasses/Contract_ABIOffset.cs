namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract_ABIOffset : SmartContract.Framework.SmartContract
    {
        public static byte[] UnitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }
        public static int UnitTest_002()
        {
            int a = 0;
            for (int i = 1; i <= 5; i++)
            {
                a += i;
            }
            return a;
        }
        public static int UnitTest_003()
        {
            int c = UnitTest_002() + 3;
            return c;
        }
    }
}
