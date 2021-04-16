using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;
using Neo.SmartContract;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var testEngine = new TestEngine();
            testEngine.AddEntryScript("./TestClasses/Contract_Event.cs");

            var debugInfo = testEngine.DebugInfo;
            Assert.AreEqual(testEngine.Nef.Script.ToScriptHash().ToString(), debugInfo["hash"].GetString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["documents"] as JArray).Count);
            Assert.IsTrue((debugInfo["documents"] as JArray).All(n => n is JString), "All documents items should be string!");
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["methods"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.CSharp.UnitTests.TestClasses.Contract_Event,Main", (debugInfo["methods"] as JArray)[0]["name"].AsString());
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["events"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.CSharp.UnitTests.TestClasses.Contract_Event,Transferred", (debugInfo["events"] as JArray)[0]["name"].AsString());
        }
    }
}
