using System.Numerics;
using Neo.SmartContract.Framework;
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

        public static bool TestIsNullOrEmpty(string? str)
        {
            return string.IsNullOrEmpty(str);
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

        public static string TestInterpolatedStringHandler()
        {
            const sbyte sbyteValue = -42;
            const byte byteValue = 42;
            const short shortValue = -1000;
            const ushort ushortValue = 1000;
            const int intValue = -1000000;
            const uint uintValue = 1000000;
            const long longValue = -1000000000000;
            const ulong ulongValue = 1000000000000;
            var bigIntValue = BigInteger.Parse("1000000000000000000000");
            const char charValue = 'A';
            const string stringValue = "Hello";
            ECPoint ecPointValue = "NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq";
            var byteStringValue = new byte[] { 1, 2, 3 };
            var uint160Value = UInt160.Zero;
            var uint256Value = UInt256.Zero;
            const bool boolValue = true;

            var str = $"SByte: {sbyteValue}, Byte: {byteValue}, Short: {shortValue}, UShort: {ushortValue}, " +
                      $"Int: {intValue}, UInt: {uintValue}, Long: {longValue}, ULong: {ulongValue}, " +
                      $"BigInteger: {bigIntValue}, Char: {charValue}, String: {stringValue}, " +
                      $"ECPoint: {ecPointValue}, ByteString: {byteStringValue}, " +
                      $"UInt160: {uint160Value}, UInt256: {uint256Value}, Bool: {boolValue}";

            return str;
        }
    }
}
