using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Assert.IsTrue(debugInfo.HaveDictItem("entrypoint"));
            Assert.AreEqual(debugInfo["entrypoint"].type, MyJson.JsonType.Value_String);
            Assert.IsTrue(debugInfo.HaveDictItem("documents"));
            Assert.AreEqual(debugInfo["documents"].type, MyJson.JsonType.Array);
            Assert.IsTrue(debugInfo["documents"].AsList().Count == 1);
            Assert.IsTrue(debugInfo["documents"].AsList().All(n => n.type == MyJson.JsonType.Value_String));
            Assert.IsTrue(debugInfo.HaveDictItem("methods"));
            Assert.AreEqual(debugInfo["methods"].type, MyJson.JsonType.Array);
            Assert.AreEqual(debugInfo["methods"].AsList().Count, 1);
            Assert.AreEqual(debugInfo["methods"].AsList()[0].asDict()["name"].AsString(), "Neo.Compiler.MSIL.TestClasses.Contract_Event,Main");
            Assert.IsTrue(debugInfo.HaveDictItem("events"));
            Assert.AreEqual(debugInfo["events"].type, MyJson.JsonType.Array);
            Assert.AreEqual(debugInfo["events"].AsList().Count, 1);
            Assert.AreEqual(debugInfo["events"].AsList()[0].asDict()["name"].AsString(), "Neo.Compiler.MSIL.TestClasses.Contract_Event,transfer");
        }
    }
}
