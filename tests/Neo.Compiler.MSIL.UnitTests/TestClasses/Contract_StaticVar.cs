using System.Numerics;
using System.Globalization;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_staticvar : SmartContract.Framework.SmartContract
    {
        static int a1 = 1;
        static readonly BigInteger a2 = BigInteger.Parse("120");
        static readonly BigInteger a3 = BigInteger.Parse("3", NumberStyles.Any);

        public static object Main()
        {
            testadd();
            testmulti();
            return a1;
        }

        static void testadd()
        {
            a1 += 5;
        }

        static void testmulti()
        {
            a1 *= 7;
        }

        public static BigInteger testBigIntegerParse()
        {
            return a2 + a3;
        }

        public static BigInteger testBigIntegerParse2(string text)
        {
            return BigInteger.Parse(text);
        }
    }
}
