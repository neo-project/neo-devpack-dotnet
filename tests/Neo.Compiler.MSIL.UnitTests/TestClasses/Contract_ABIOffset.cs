using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract_ABIOffset : SmartContract.Framework.SmartContract
    {
        static int s = 1;

        public static int UnitTest_001()
        {
            var i = 2;
            return i + s;
        }

        //public static byte[] UnitTest_001()
        //{
        //    var nb = new byte[] { 1, 2, 3, 4 };
        //    nb[2] = s.AsByte();
        //    return nb;
        //}
        public static int UnitTest_002()
        {
            int a = 0;
            for (int i = 1; i <= s; i++)
            {
                a += i;
            }
            return a;
        }
        public static int UnitTest_003()
        {
            int c = UnitTest_002() + s;
            return c;
        }
    }
}
