using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Property : SmartContract.Framework.SmartContract
    {
        public static string Symbol => "TokenSymbol";

        private static BigInteger TestStaticProperty { get; set; }

        private BigInteger TestProperty { get; set; }

        public static BigInteger TestStaticPropertyInc()
        {
            TestStaticProperty++;
            TestStaticProperty++;
            TestStaticProperty++;
            return TestStaticProperty;
        }

        public BigInteger TestPropertyInc()
        {
            TestProperty++;
            TestProperty++;
            TestProperty++;
            return TestProperty;
        }
    }
}
