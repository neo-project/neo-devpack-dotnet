using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.VM;
using System.Linq;
using Neo.Cryptography;

namespace Neo.Compiler.MSIL.Utils
{
    class JsonTestTool
    {
        static Dictionary<string, TestJson> jsons = new Dictionary<string, TestJson>();
        static Dictionary<string, BuildScript> builds = new Dictionary<string, BuildScript>();

        public static void TestAllCaseInJson(string jsonfile)
        {
            var json = GetJson(jsonfile);
            if (json != null)
            {
                foreach (var item in json.testCases)
                {
                    TestJsonCase(item.Value);
                }
            }
            else
            {
                Assert.Fail("Error on get json:" + jsonfile);
            }
        }

        public static void TestOneCaseInJson(string jsonfile, string test)
        {
            var json = GetJson(jsonfile);
            if (json != null)
            {
                if (json.testCases.TryGetValue(test, out JObject value))
                {
                    TestJsonCase(value);
                }
                else
                {
                    Assert.Fail("Error can't find case:" + test + " in " + jsonfile);
                }
            }
            else
            {
                Assert.Fail("Error on get json:" + jsonfile);
            }

        }

        static TestJson GetJson(string filename)
        {
            if (jsons.ContainsKey(filename))
            {
                return jsons[filename];
            }
            var json = TestJson.LoadFrom(filename);
            if (json != null)
            {
                jsons[filename] = json;
                return json;
            }
            return null;
        }

        static void TestJsonCase(JObject json)
        {
            List<BuildScript> thiscasebuilds = new List<BuildScript>();

            //step 1.build
            var contracs = (JArray)json["build"];
            List<string> needtobuild = new List<string>();
            foreach (string file in contracs)
            {
                needtobuild.Add(file);
            }
            if (json.ContainsKey("entryscript"))
            {
                string entryscript = (string)json["entryscript"];
                if (!needtobuild.Contains(entryscript))
                    needtobuild.Add(entryscript);
            }


            foreach (string file in needtobuild)
            {
                if (builds.ContainsKey(file) == false)
                {
                    var script = NeonTestTool.BuildScript(file);
                    Console.WriteLine("build:" + file);
                    builds[file] = script;
                }
                var _script = builds[file];
                if (_script.IsBuild == false)
                {
                    Assert.Fail("a contract is not build succ:" + file + " err=" + _script.Error.Message);
                }
                thiscasebuilds.Add(_script);
            }
            //step 2.do test
            var testmethod = (string)json["testmethod"];
            if (testmethod == "execute")
            {
                StackItem[] item = StackItemListFromJson((JArray)json["testparams"]);
                StackItem wantresult = StackItemFromJson(json["testresult"]);

                string entryscript = (string)json["entryscript"];
                var testengine = new TestEngine(thiscasebuilds);
                var executeresult = testengine.ExecuteTestCase(builds[entryscript], item);

                if (wantresult == null)//void result
                {
                    Assert.Equals(executeresult.Count, 0);
                    return;
                }

                var result = executeresult.Peek();
                var testequal = StackItemEqual(wantresult, result);
                Assert.IsTrue(testequal);
            }
            else
            {
                Assert.Fail("bad testmethod:" + testmethod);
            }


        }
        public static string Bytes2HexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }
        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }
        static System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();

        public static byte[] GetPublicKeyHashFromAddress(string address)
        {
            var alldata = Base58.Decode(address);
            if (alldata.Length != 25)
                throw new Exception("error length.");
            var data = alldata.Take(alldata.Length - 4).ToArray();
            if (data[0] != 0x17)
                throw new Exception("not a address");
            var hash = sha256.ComputeHash(data);
            hash = sha256.ComputeHash(hash);
            var hashbts = hash.Take(4).ToArray();
            var datahashbts = alldata.Skip(alldata.Length - 4).ToArray();
            if (hashbts.SequenceEqual(datahashbts) == false)
                throw new Exception("not match hash");
            var pkhash = data.Skip(1).ToArray();
            return pkhash;
        }
        static StackItem StackItemFromJson(JToken json)
        {
            if (json is JValue)//bool 或小数字
            {
                JValue j = (JValue)json;

                if (j.Value is bool)
                {
                    return (bool)j.Value;
                }
                else if (j.Value is Int64)
                {
                    return (Int64)j.Value;
                }
                else if (j.Value is double)
                {
                    throw new Exception("unsupport float type");
                }
                else if (j.Value is string)//复杂格式
                {
                    return StackItemFromString((string)j.Value);
                }
                else
                {
                    throw new Exception("unkonwn type in json");
                }
            }
            else if (json is JArray)
            {
                var list = (JArray)json;
                StackItem[] result = new StackItem[list.Count];
                for (var i = 0; i < list.Count; i++)
                {
                    result[i] = StackItemFromJson(list[i]);
                }
                return result;
            }
            else
            {
                throw new Exception("should not pass a {}");
            }
        }
        static StackItem StackItemFromString(string str)
        {
            if (str[0] != '(')
                throw new Exception("must start with:(string) or (bytes) or (address) or (hexint) or (int) or (int256) or (int160)");

            if (str.IndexOf("(str)") == 0)
            {
                return str.Substring(5);
            }
            else if (str.IndexOf("(string)") == 0)
            {
                return str.Substring(8);
            }
            //(bytes) or([])开头，表示就是一个bytearray
            else if (str.IndexOf("(bytes)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(7));
                return hex;
            }
            else if (str.IndexOf("([])") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(4));
                return hex;
            }
            //(address) or(addr)开头，表示是一个地址，转换为脚本hash
            else if (str.IndexOf("(address)") == 0)
            {
                var addr = (str.Substring(9));
                byte[] hex = GetPublicKeyHashFromAddress(addr);
                return hex;
            }
            else if (str.IndexOf("(addr)") == 0)
            {
                var addr = (str.Substring(6));
                byte[] hex = GetPublicKeyHashFromAddress(addr);
                return hex;
            }
            //(integer) or(int) 开头，表示是一个大整数
            else if (str.IndexOf("(integer)") == 0)
            {
                var num = System.Numerics.BigInteger.Parse(str.Substring(9));
                return num;
            }
            else if (str.IndexOf("(int)") == 0)
            {
                var num = System.Numerics.BigInteger.Parse(str.Substring(5));
                return num;
            }
            //(hexinteger) or (hexint) or (hex) 开头，表示是一个16进制表示的大整数，转换为bytes就是反序
            else if (str.IndexOf("(hexinteger)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(12));
                return (hex.Reverse().ToArray());
            }
            else if (str.IndexOf("(hexint)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(8));
                return (hex.Reverse().ToArray());
            }
            else if (str.IndexOf("(hex)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(5));
                return (hex.Reverse().ToArray());
            }
            //(int256) or (hex256) 开头,表示是一个定长的256位 16进制大整数
            else if (str.IndexOf("(hex256)") == 0 || str.IndexOf("(int256)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(8));
                if (hex.Length != 32)
                    throw new Exception("error lenght");
                return (hex.Reverse().ToArray());
            }
            else if (str.IndexOf("(uint256)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(9));
                if (hex.Length != 32)
                    throw new Exception("error lenght");
                return (hex.Reverse().ToArray());
            }
            //(int160) or (hex160) 开头,表示是一个定长的160位 16进制大整数
            else if (str.IndexOf("(hex160)") == 0 || str.IndexOf("(int160)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(8));
                if (hex.Length != 20)
                    throw new Exception("error lenght");
                return (hex.Reverse().ToArray());
            }
            else if (str.IndexOf("(uint160)") == 0)
            {
                byte[] hex = HexString2Bytes(str.Substring(9));
                if (hex.Length != 20)
                    throw new Exception("error lenght");
                return (hex.Reverse().ToArray());
            }
            else
                throw new Exception("must start with:(str) or (hex) or (hexbig) or (int)");

        }
        static StackItem[] StackItemListFromJson(JArray json)
        {
            StackItem[] result = new StackItem[json.Count];
            for (var i = 0; i < json.Count; i++)
            {
                result[i] = StackItemFromJson(json[i]);
            }
            return result;
        }
        static bool StackItemEqual(StackItem a, StackItem b)
        {
            return a.Equals(b);
        }
        class TestJson
        {
            public Dictionary<string, JObject> testCases = new Dictionary<string, JObject>();
            public static TestJson LoadFrom(string filename)
            {
                if (!System.IO.File.Exists(filename))
                {
                    return null;
                }
                try
                {
                    TestJson testjson = new TestJson();
                    var txt = System.IO.File.ReadAllText(filename);
                    var json = JObject.Parse(txt);
                    var cases = json["tests"] as JObject;
                    foreach (var item in cases.Properties())
                    {
                        testjson.testCases[item.Name] = (JObject)item.Value;
                    }

                    return testjson;

                }
                catch (Exception err)
                {
                    return null;
                }
            }
        }
    }

}
