using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;
using System.Linq;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Event.cs");
            var debugInfo = testengine.ScriptEntry.debugInfo;
            Assert.IsTrue(debugInfo.ContainsProperty("hash"));
            Assert.IsInstanceOfType(debugInfo["hash"], typeof(JString));
            Neo.UInt160.Parse(debugInfo["hash"].AsString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.AreEqual((debugInfo["documents"] as JArray).Count, 1);
            Assert.IsTrue((debugInfo["documents"] as JArray).All(n => n is JString));
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual((debugInfo["methods"] as JArray).Count, 1);
            Assert.AreEqual((debugInfo["methods"] as JArray)[0]["name"].AsString(), "Neo.Compiler.MSIL.UnitTests.TestClasses.Contract_Event,main");
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual((debugInfo["events"] as JArray).Count, 1);
            Assert.AreEqual((debugInfo["events"] as JArray)[0]["name"].AsString(), "Neo.Compiler.MSIL.UnitTests.TestClasses.Contract_Event,transfer");
        }
    }
}
