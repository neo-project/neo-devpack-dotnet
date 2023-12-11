using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_String : SmartContract
    {
        public static int TestStringAdd(string s1, string s2)
        {
            int a = 3;
            string c = s1 + s2;
            if (c == "hello")
            {
                a = 4;
            }
            else if (c == "world")
            {
                a = 5;
            }
            return a;
        }

        public static string TestStringAddInt(string s, int i)
        {
            return s + i;
        }

        public static int memorySearch1(ByteString mem, ByteString value)
        {
            return StdLib.MemorySearch(mem, value);
        }

        public static string[] TestStringSplit(string str, string separator)
        {
            return StdLib.StringSplit(str, separator);
        }

        public static string TestStringReverse(string s)
        {
            return s.Reverse();
        }

        public static string TestStringLast(string s, int count)
        {
            return s.Last(count);
        }

        public static string TestStringTake(string s, int count)
        {
            return s.Take(count);
        }

        public static string TestStringReplace(string s, byte old, byte @new)
        {
            return s.Replace(old, @new);
        }
    }
}
