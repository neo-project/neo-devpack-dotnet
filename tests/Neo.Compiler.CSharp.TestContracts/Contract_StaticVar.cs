using System.Numerics;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_StaticVar : SmartContract.Framework.SmartContract
    {
        static int a1 = 1;
        static readonly BigInteger a2 = BigInteger.Parse("120");
        static readonly BigInteger a3 = BigInteger.Parse("3");

        [InitialValue("hello world", Neo.SmartContract.ContractParameterType.String)]
        public static readonly string a4 = default;

        public static string testinitalvalue() => a4;

        public static object TestMain()
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
