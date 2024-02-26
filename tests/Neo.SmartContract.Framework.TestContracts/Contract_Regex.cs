namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Regex : SmartContract
    {
        public static bool TestStartWith()
        {
            return ((ByteString)"Hello World").StartWith("Hello");
        }

        public static int TestIndexOf()
        {
            return ((ByteString)"Hello World").IndexOf("o");
        }

        public static bool TestEndWith()
        {
            return ((ByteString)"Hello World").EndsWith("World");
        }

        public static bool TestContains()
        {
            return ((ByteString)"Hello World").Contains("ll");
        }

        public static bool TestNumberOnly()
        {
            return ((ByteString)"0123456789").IsNumber();
        }

        public static bool TestAlphabetOnly()
        {
            return ((ByteString)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz").IsAlphabet();
        }

        public static bool TestLowerAlphabetOnly()
        {
            return ((ByteString)"abcdefghijklmnopqrstuvwxyz").IsAlphabet();
        }

        public static bool TestUpperAlphabetOnly()
        {
            return ((ByteString)"ABCDEFGHIJKLMNOPQRSTUVWXYZ").IsAlphabet();
        }
    }
}
