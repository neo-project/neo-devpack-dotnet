using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Types : SmartContract.Framework.SmartContract
    {
        public enum EDummy : byte
        {
            test = 5
        }

        public delegate EDummy enumDel();
        public delegate void del(string msg);
        public static event del dummyEvent;

        public class DummyClass
        {
            public string Value;
        }

        public struct DummyStruct
        {
            public string Value;
        }

        public static object checknull() { return null; }
        public static bool checkbooltrue() { return true; }
        public static bool checkboolfalse() { return false; }
        public static sbyte checksbyte() { return (sbyte)5; }
        public static byte checkbyte() { return (byte)5; }
        public static short checkshort() { return (short)5; }
        public static ushort checkushort() { return (ushort)5; }
        public static int checkint() { return (int)5; }
        public static uint checkuint() { return (uint)5; }
        public static long checklong() { return (long)5; }
        public static ulong checkulong() { return (ulong)5; }
        public static char checkChar() { return 'n'; }
        public static string checkString() { return "neo"; }
        public static object[] checkArrayObj() { return new object[] { "neo" }; }
        public static BigInteger checkBigInteger() { return (BigInteger)5; }
        public static byte[] checkByteArray() { return new byte[] { 1, 2, 3 }; }
        public static object checkEnum() { return EDummy.test; }
        private static EDummy icheckEnum() { return EDummy.test; }
        public static void checkEnumArg(Neo.SmartContract.Framework.Native.OracleResponseCode arg) { }
        public static object checkDelegate()
        {
            return new enumDel(icheckEnum);
        }
        public static object checkLambda()
        {
            return new Func<EDummy>(icheckEnum);
        }
        public static void checkEvent()
        {
            dummyEvent("neo");
        }
        public static object checkClass()
        {
            var ret = new DummyClass();
            ret.Value = "neo";
            return ret;
        }
        public static object checkStruct()
        {
            var ret = new DummyStruct();
            ret.Value = "neo";
            return ret;
        }
        public static (string value1, string value2) checkTuple()
        {
            return ("neo", "smart economy");
        }
        public static (string value1, string value2) checkTuple2()
        {
            var tuple = ("neo", "smart economy");
            return tuple;
        }
        public static Tuple<string, string> checkTuple3()
        {
            return new Tuple<string, string>("neo", "smart economy");
        }
    }
}
