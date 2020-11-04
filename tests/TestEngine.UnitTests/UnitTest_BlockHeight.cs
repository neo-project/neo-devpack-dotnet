using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using Neo.TestingEngine;
using Neo.VM;
using Neo.VM.Types;
using System.IO;
using Compiler = Neo.Compiler.Program;

namespace TestEngine.UnitTests
{
    [TestClass]
    public class UnitTest_BlockHeight
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

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);
            Engine.Instance.Reset();
        }

        [TestMethod]
        public void Test_Json()
        {
            var height = 16;

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

            Assert.AreEqual(height, Engine.Instance.BlockCount);
        }
    }
}
