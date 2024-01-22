using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Binary : SmartContract
    {
        public static string base58CheckEncode(ByteString input)
        {
            return StdLib.Base58CheckEncode(input);
        }

        public static byte[] base58CheckDecode(string input)
        {
            return (byte[])StdLib.Base58CheckDecode(input);
        }

        public static byte[] base64Decode(string input)
        {
            return (byte[])StdLib.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return StdLib.Base64Encode((ByteString)input);
        }

        public static byte[] base58Decode(string input)
        {
            return (byte[])StdLib.Base58Decode(input);
        }

        public static string base58Encode(byte[] input)
        {
            return StdLib.Base58Encode((ByteString)input);
        }

        public static object atoi(string value, int @base)
        {
            return StdLib.Atoi(value, @base);
        }

        public static string itoa(int value, int @base)
        {
            return StdLib.Itoa(value, @base);
        }

        public static int memoryCompare(ByteString str1, ByteString str2)
        {
            return StdLib.MemoryCompare(str1, str2);
        }

        public static int memorySearch1(ByteString mem, ByteString value)
        {
            return StdLib.MemorySearch(mem, value);
        }

        public static int memorySearch2(ByteString mem, ByteString value, int start)
        {
            return StdLib.MemorySearch(mem, value, start);
        }

        public static int memorySearch3(ByteString mem, ByteString value, int start, bool backward)
        {
            return StdLib.MemorySearch(mem, value, start, backward);
        }

        public static string[] stringSplit1(string str, string separator)
        {
            return StdLib.StringSplit(str, separator);
        }

        public static string[] stringSplit2(string str, string separator, bool removeEmptyEntries)
        {
            return StdLib.StringSplit(str, separator, removeEmptyEntries);
        }
    }
}
