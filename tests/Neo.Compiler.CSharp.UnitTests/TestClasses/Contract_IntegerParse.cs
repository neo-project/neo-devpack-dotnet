using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_IntegerParse : SmartContract.Framework.SmartContract
    {
        public static sbyte testSbyteparse(string s)
        {
            try
            {
                return sbyte.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static byte testByteparse(string s)
        {
            try
            {
                return byte.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static ushort testUshortparse(string s)
        {
            try
            {
                return ushort.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static short testShortparse(string s)
        {
            try
            {
                return short.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static ulong testUlongparse(string s)
        {
            try
            {
                return ulong.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static long testLongparse(string s)
        {
            try
            {
                return long.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static uint testUintparse(string s)
        {
            try
            {
                return uint.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public static int testIntparse(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                throw new System.Exception();
            }
        }
    }
}
