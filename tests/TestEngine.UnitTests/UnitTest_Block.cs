using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.TestingEngine;
using Neo.VM;
using Neo.VM.Types;
using System.IO;
using Compiler = Neo.Compiler.Program;

namespace TestEngine.UnitTests
{
    [TestClass]
    public class UnitTest_Block
    {
        [TestInitialize]
        public void Init()
        {
            string path = Directory.GetCurrentDirectory();
            var option = new Compiler.Options()
            {
                File = path + "/TestClasses/Contract1.cs"
            };
            Compiler.Compile(option);

            option.File = path + "/TestClasses/Contract_Time.cs";
            Compiler.Compile(option);

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);
            Engine.Instance.Reset();
        }

        [TestMethod]
        public void Test_Block_Height()
        {
            uint height = 16;

            var json = new JObject();
            json["path"] = "./TestClasses/Contract1.nef";
            json["method"] = "testVoid";
            json["height"] = height;

            var args = new string[] {
                json.AsString()
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            Assert.AreEqual(height, Engine.Instance.Height);
        }

        [TestMethod]
        public void Test_Include_Block()
        {
            uint height = 10;
            ulong timestamp = Blockchain.GenesisBlock.Timestamp + 20 * Blockchain.MillisecondsPerBlock;

            var blockJson = new JObject();
            blockJson["index"] = height;
            blockJson["timestamp"] = timestamp;
            blockJson["transactions"] = new JArray();

            var json = new JObject();
            json["path"] = "./TestClasses/Contract_Time.nef";
            json["method"] = "getTime";
            json["blocks"] = new JArray() { blockJson };

            var args = new string[] {
                json.AsString()
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            Assert.AreEqual(height, Engine.Instance.Height);

            // test result
            StackItem wantresult = timestamp;
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());
        }
    }
}
