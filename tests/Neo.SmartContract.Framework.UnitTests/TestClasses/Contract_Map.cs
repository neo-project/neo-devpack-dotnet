using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Map : SmartContract
    {
        public static int TestCount(int count)
        {
            Map<int, int> some = new Map<int, int>();
            for (int i = 0; i < count; i++)
            {
                some[i] = i;
            }

            return some.Count;
        }

        public static object TestByteArray(byte[] key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key.ToByteString()] = "teststring2";
            return StdLib.JsonSerialize(some);
        }

        public static object TestClear(byte[] key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key.ToByteString()] = "teststring2";
            some.Clear();
            return StdLib.JsonSerialize(some);
        }

        public static string TestByteArray2()
        {
            Map<string, string> some = new Map<string, string>();
            string key = new byte[] { 0x01, 0x01 }.ToByteString();
            some[key] = StdLib.JsonSerialize("");
            return StdLib.JsonSerialize(some);
        }

        public static string TestUnicode(string key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = "129840test10022939";
            return StdLib.JsonSerialize(some);
        }

        public static string TestUnicodeValue(string value)
        {
            Map<string, string> some = new Map<string, string>();
            some["ab"] = value;
            return StdLib.JsonSerialize(some);
        }

        public static string TestUnicodeKeyValue(string key, string value)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = value;
            return StdLib.JsonSerialize(some);
        }

        public static string TestInt(int key)
        {
            Map<int, string> some = new Map<int, string>();
            some[key] = "string";
            return StdLib.JsonSerialize(some);
        }

        public static string TestBool(bool key)
        {
            Map<bool, string> some = new Map<bool, string>();
            some[key] = "testbool";
            return StdLib.JsonSerialize(some);
        }

        public static object TestDeserialize(string key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = "testdeserialize";
            string sea = StdLib.JsonSerialize(some);
            return StdLib.JsonDeserialize(sea);
        }

        public static object testuint160Key()
        {
            Map<UInt160, int> some = new Map<UInt160, int>();
            UInt160 key = UInt160.Zero;
            some[key] = 1;
            string serializestr = StdLib.JsonSerialize(some);
            return StdLib.JsonDeserialize(serializestr);
        }
    }
}
