using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_DebugInfo
    {
        [TestMethod]
        public void Test_DebugInfo()
        {
            var builder = new BuildScript();
            builder.Build("./TestClasses/Contract_Event.cs");

            var debugInfo = builder.context.CreateDebugInformation();
            Assert.IsTrue(debugInfo.ContainsProperty("hash"));
            Assert.IsInstanceOfType(debugInfo["hash"], typeof(JString));
            Neo.UInt160.Parse(debugInfo["hash"].AsString());
            Assert.IsTrue(debugInfo.ContainsProperty("documents"));
            Assert.IsInstanceOfType(debugInfo["documents"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["documents"] as JArray).Count);
            Assert.IsTrue((debugInfo["documents"] as JArray).All(n => n is JString), "All documents items should be string!");
            Assert.IsTrue(debugInfo.ContainsProperty("methods"));
            Assert.IsInstanceOfType(debugInfo["methods"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["methods"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.MSIL.UnitTests.TestClasses.Contract_Event,main", (debugInfo["methods"] as JArray)[0]["name"].AsString());
            Assert.IsTrue(debugInfo.ContainsProperty("events"));
            Assert.IsInstanceOfType(debugInfo["events"], typeof(JArray));
            Assert.AreEqual(1, (debugInfo["events"] as JArray).Count);
            Assert.AreEqual("Neo.Compiler.MSIL.UnitTests.TestClasses.Contract_Event,transfer", (debugInfo["events"] as JArray)[0]["name"].AsString());
        }
    }
}
