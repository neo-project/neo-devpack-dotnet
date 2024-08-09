using System.Numerics;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_String : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var firstName = "Mark";
            var lastName = $"";
            var timestamp = Ledger.GetBlock(Ledger.CurrentHash).Timestamp;
            Runtime.Log($"Hello, {firstName} {lastName}! Current timestamp is {timestamp}.");
        }

        public static void TestEqual()
        {
            var str = "hello";
            var str2 = "hello";
            Runtime.Log(str.Equals(str2).ToString());
        }

        public static void TestSubstring()
        {
            var str = "01234567";
            Runtime.Log(str.Substring(1));
            Runtime.Log(str.Substring(1, 4));
        }

        public static string TestEmpty()
        {
            return string.Empty;
        }

        public static bool TestIsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static BigInteger TestStringNull(string str)
        {
            return str.Length;
        }

        public static bool TestEndWith(string str)
        {
            return str.EndsWith("world");
        }

        public static bool TestContains(string str)
        {
            return str.Contains("world");
        }

        public static int TestIndexOf(string str)
        {
            return str.IndexOf("world");
        }
    }
}
