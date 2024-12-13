using System;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Types : SmartContract.Framework.SmartContract
    {
        public enum EDummy : byte
        {
            test = 5
        }

        public delegate EDummy enumDel();
        public delegate void Del(string msg);
        public static event Del DummyEvent = null!;

        public class DummyClass
        {
            public string Value = default!;
        }

        public struct DummyStruct
        {
            public string Value;
        }

        public static string checkBoolString(bool value) { return value.ToString(); }
        public static object? checkNull() { return null; }
        public static bool checkBoolTrue() { return true; }
        public static bool checkBoolFalse() { return false; }
        public static sbyte checkSbyte() { return 5; }
        public static byte checkByte() { return 5; }
        public static short checkShort() { return 5; }
        public static ushort checkUshort() { return 5; }
        public static int checkInt() { return 5; }
        public static uint checkUint() { return 5; }
        public static long checkLong() { return 5; }
        public static ulong checkUlong() { return 5; }
        public static char checkChar() { return 'n'; }
        public static string checkString() { return "neo"; }
        public static char checkStringIndex(string input, int index) => input[index];
        public static object[] checkArrayObj() { return new object[] { "neo" }; }
        public static BigInteger checkBigInteger() { return (BigInteger)5; }
        public static byte[] checkByteArray() { return new byte[] { 1, 2, 3 }; }
        public static object checkEnum() { return EDummy.test; }
        private static EDummy icheckEnum() { return EDummy.test; }
        public static void checkEnumArg(OracleResponseCode arg) { }
        public static string checkNameof() { return nameof(checkNull); }
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
            DummyEvent("neo");
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
        public static string concatByteString(ByteString a, ByteString b)
        {
            return a + b + a.Concat(b);
        }
        public static string toAddress(UInt160 address, byte version)
        {
            return address.ToAddress(version);
        }

        public static object Call(UInt160 scriptHash, string method, CallFlags flag, object[] args)
        {
            return Contract.Call(scriptHash, method, flag, args);
        }

#pragma warning disable CS8625
        public static object Create(byte[] nef, string manifest)
        {
            return ContractManagement.Deploy((ByteString)nef, manifest, null);
        }
#pragma warning restore CS8625
    }
}
