using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Property : SmartContract.Framework.SmartContract
    {
        public static string Symbol => "TokenSymbol";

        // Field
        private static BigInteger TestStaticFieldDefault;

        private static BigInteger TestStaticFieldValue = 1;

        private static BigInteger TestFieldDefault;

        private static BigInteger TestFieldValue = 2;

        // Property
        private static BigInteger TestStaticPropertyDefault { get; set; }

        private static BigInteger TestStaticPropertyValue { get; set; } = 10;

        private static BigInteger TestPropertyDefault { get; set; }

        private static BigInteger TestPropertyValue { get; set; } = 11;

        public static BigInteger TestStaticPropertyDefaultInc()
        {
            TestStaticPropertyDefault++;
            TestStaticPropertyValue++;
            return TestStaticPropertyDefault;
        }

        public static BigInteger TestStaticPropertyValueInc()
        {
            TestStaticPropertyValue++;
            return TestStaticPropertyValue;
        }

        public static BigInteger TestPropertyDefaultInc()
        {
            TestPropertyDefault++;
            TestPropertyDefault++;
            return TestPropertyDefault;
        }

        public static BigInteger TestPropertyValueInc()
        {
            TestPropertyValue++;
            TestPropertyValue++;
            return TestPropertyValue;
        }

        // Getenrate methods for fields
        public static BigInteger IncTestStaticFieldDefault()
        {
            TestStaticFieldDefault++;
            return TestStaticFieldDefault;
        }

        public static BigInteger IncTestStaticFieldValue()
        {
            TestStaticFieldValue++;
            return TestStaticFieldValue;
        }

        public static BigInteger IncTestFieldDefault()
        {
            TestFieldDefault++;
            return TestFieldDefault;
        }

        public static BigInteger IncTestFieldValue()
        {
            TestFieldValue = 2;
            return TestFieldValue;
        }

    }
}
