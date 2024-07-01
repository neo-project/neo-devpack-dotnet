using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.SmartContract;
using System.Linq;
using Neo.SmartContract.TestEngine;

namespace Neo.Compiler.CSharp.UnitTests.OldEngine
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var testEngine = new TestEngine();
            testEngine.AddEntryScript(Utils.Extensions.TestContractRoot + "Contract_Event.cs");

            var debugInfo = testEngine.DebugInfo;
            Assert.AreEqual(testEngine.Nef.Script.Span.ToScriptHash().ToString(), debugInfo["hash"].GetString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.IsTrue((debugInfo["documents"] as JArray).Count > 0);
            Assert.IsTrue((debugInfo["documents"] as JArray).All(n => n is JString), "All documents items should be string!");
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["methods"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,Main2", (debugInfo["methods"] as JArray)[0]["name"].AsString());
            Assert.AreEqual("3[0]21:28-21:29;4[0]21:13-21:29;5[0]21:13-21:29;6[0]21:13-21:29;7[0]22:28-22:32;8[0]22:13-22:32;9[0]22:13-22:32;10[0]22:13-22:32;11[0]23:9-23:10",
                string.Join(';', ((debugInfo["methods"] as JArray)[0]["sequence-points"] as JArray).Select(u => u.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["events"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.CSharp.TestContracts.Contract_Event,Transferred", (debugInfo["events"] as JArray)[0]["name"].AsString());
            Assert.AreEqual("arg1,ByteArray,0;arg2,ByteArray,1;arg3,Integer,2", string.Join(';', ((debugInfo["events"] as JArray)[0]["params"] as JArray).Select(u => u.AsString())));
            Assert.IsTrue(debugInfo.ContainsProperty("static-variables"));
            Assert.AreEqual("MyStaticVar1,Integer,0;MyStaticVar2,Boolean,1", string.Join(';', (debugInfo["static-variables"] as JArray).Select(u => u.AsString())));
        }
    }
}
