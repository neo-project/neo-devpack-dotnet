using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Regex : SmartContract
    {
        public static bool TestStartWith()
        {
            return Regex.StartWith("Hello World", "Hello");
        }

        public static int TestIndexOf()
        {
            return Regex.IndexOf("Hello World", "o");
        }

        public static bool TestEndWith()
        {
            return Regex.EndsWith("Hello World", "World");
        }

        public static bool TestContains()
        {
            return Regex.Contains("Hello World", "ll");
        }

        public static bool TestNumberOnly()
        {
            return Regex.NumberOnly("1234567890");
        }

        public static bool TestAlphabetOnly()
        {
            return Regex.AlphabetOnly("HelloWorld");
        }
    }
}
