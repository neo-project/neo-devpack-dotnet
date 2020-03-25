namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract_ABIOffset : SmartContract.Framework.SmartContract
    {
        public static void Main(string method, object[] args)
        {
            UnitTest_001();
            UnitTest_002();
            UnitTest_003();
            UnitTest_004();
        }
        public static byte[] UnitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }
        public static byte[] UnitTest_002()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }
        public static byte[] UnitTest_003()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }
        public static byte[] UnitTest_004()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

    }
}
