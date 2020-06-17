using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types : SmartContract.Framework.SmartContract
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

        public static object checkNull() { return null; }
        public static bool checkBoolTrue() { return true; }
        public static bool checkBoolFalse() { return false; }
        public static sbyte checkSbyte() { return (sbyte)5; }
        public static byte checkByte() { return (byte)5; }
        public static short checkShort() { return (short)5; }
        public static ushort checkUshort() { return (ushort)5; }
        public static int checkInt() { return (int)5; }
        public static uint checkUint() { return (uint)5; }
        public static long checkLong() { return (long)5; }
        public static ulong checkUlong() { return (ulong)5; }
        public static char checkChar() { return 'n'; }
        public static string checkString() { return "neo"; }
        public static object[] checkArrayObj() { return new object[] { "neo" }; }
        public static BigInteger checkBigInteger() { return (BigInteger)5; }
        public static byte[] checkByteArray() { return new byte[] { 1, 2, 3 }; }
        public static EDummy checkEnum() { return EDummy.test; }
        public static enumDel checkDelegate()
        {
            return new enumDel(checkEnum);
        }
        public static Func<EDummy> checkLambda()
        {
            return checkEnum;
        }
        public static void checkEvent()
        {
            dummyEvent("neo");
        }
        public static DummyClass checkClass()
        {
            var ret = new DummyClass();
            ret.Value = "neo";
            return ret;
        }
        public static DummyStruct checkStruct()
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
    }
}
