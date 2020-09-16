using System;
using System.Collections.Generic;
using System.Text;
using Neo.SmartContract.Framework.Services.Neo;

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
            return Json.Serialize(some);
        }

        public static object TestClear(byte[] key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key.ToByteString()] = "teststring2";
            some.Clear();
            return Json.Serialize(some);
        }

        public static string TestByteArray2()
        {
            Map<string, string> some = new Map<string, string>();
            string key = new byte[] { 0x01, 0x01 }.ToByteString();
            some[key] = Json.Serialize("");
            return Json.Serialize(some);
        }

        public static string TestUnicode(string key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = "129840test10022939";
            return Json.Serialize(some);
        }

        public static string TestUnicodeValue(string value)
        {
            Map<string, string> some = new Map<string, string>();
            some["ab"] = value;
            return Json.Serialize(some);
        }

        public static string TestUnicodeKeyValue(string key, string value)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = value;
            return Json.Serialize(some);
        }

        public static string TestInt(int key)
        {
            Map<int, string> some = new Map<int, string>();
            some[key] = "string";
            return Json.Serialize(some);
        }

        public static string TestBool(bool key)
        {
            Map<bool, string> some = new Map<bool, string>();
            some[key] = "testbool";
            return Json.Serialize(some);
        }

        public static object TestDeserialize(string key)
        {
            Map<string, string> some = new Map<string, string>();
            some[key] = "testdeserialize";
            string sea = Json.Serialize(some);
            return Json.Deserialize(sea);
        }
    }
}
