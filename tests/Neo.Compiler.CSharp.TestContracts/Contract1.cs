namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract1 : SmartContract.Framework.SmartContract
    {
        private static string privateMethod()
        {
            return "NEO3";
        }

        public static byte[] unitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

        public static void testVoid()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
        }

        public static byte[] testArgs1(byte a)
        {
            var nb = new byte[] { 1, 2, 3, 3 };
            nb[3] = a;
            return nb;
        }

        public static byte[] testArgs2(byte[] a)
        {
            return a;
        }

        public static int testArgs3(int a, int b)
        {
            a = a + 2;
            return a;
        }

        public static int testArgs4(int a, int b)
        {
            a = a + 2;
            return a + b;
        }
    }
}
