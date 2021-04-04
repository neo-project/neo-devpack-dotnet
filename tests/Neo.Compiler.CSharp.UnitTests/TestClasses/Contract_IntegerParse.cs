using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_IntegerParse : SmartContract.Framework.SmartContract
    {
        public static sbyte testSbyteparse(string s)
        {
            return sbyte.Parse(s);
        }

        public static byte testByteparse(string s)
        {
            return byte.Parse(s);
        }

        public static ushort testUshortparse(string s)
        {
            return ushort.Parse(s);
        }

        public static short testShortparse(string s)
        {
            return short.Parse(s);
        }

        public static ulong testUlongparse(string s)
        {
            return ulong.Parse(s);
        }

        public static long testLongparse(string s)
        {
            return long.Parse(s);
        }

        public static uint testUintparse(string s)
        {
            return uint.Parse(s);
        }

        public static int testIntparse(string s)
        {
            return int.Parse(s);
        }
    }
}
