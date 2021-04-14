using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.TestEngine.UnitTests.Utils;
using Neo.TestingEngine;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using System.IO;

namespace TestEngine.UnitTests
{
    [TestClass]
    public class UnitTest_CheckWitness
    {
        [TestInitialize]
        public void Init()
        {
            string path = Directory.GetCurrentDirectory();
            CSharpCompiler.Compile(path + "/TestClasses/Contract_CheckWitness.cs");

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);
            Engine.Instance.Reset();
        }

        [TestMethod]
        public void Test_Check_Witness()
        {
            var scripthash = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var param = new ContractParameter(ContractParameterType.Hash160)
            {
                Value = scripthash.ToString().Substring(2)
            };

            var json = new JObject();
            json["path"] = "./TestClasses/Contract_CheckWitness.nef";
            json["method"] = "testWitness";
            json["arguments"] = new JArray() { param.ToJson() };

            var args = new string[] {
                json.AsString()
            };
            var result = Program.Run(args);

            // mustn't have an error
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // vm state must've faulted
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // result stack must be empty
            StackItem wantresult = false;
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());
        }

        [TestMethod]
        public void Test_Check_Witness_With_Sign()
        {
            var scripthash = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash(ProtocolSettings.Default.AddressVersion);
            var param = new ContractParameter(ContractParameterType.Hash160)
            {
                Value = scripthash.ToString().Substring(2)
            };

            var json = new JObject();
            json["path"] = "./TestClasses/Contract_CheckWitness.nef";
            json["method"] = "testWitness";
            json["arguments"] = new JArray() { param.ToJson() };
            json["signerAccounts"] = new JArray() { scripthash.ToString() };

            var args = new string[] {
                json.AsString()
            };
            var result = Program.Run(args);

            // mustn't have an error
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // vm state must've faulted
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // result stack must be empty
            StackItem wantresult = true;
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());
        }
    }
}
