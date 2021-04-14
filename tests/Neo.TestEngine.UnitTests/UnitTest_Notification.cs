using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO.Json;
using Neo.TestEngine.UnitTests.Utils;
using Neo.TestingEngine;
using Neo.VM;
using Neo.VM.Types;
using System.IO;
//using Compiler = Neo.Compiler.Program;

namespace TestEngine.UnitTests
{
    [TestClass]
    public class UnitTest_Notification
    {
        [TestInitialize]
        public void Init()
        {
            string path = Directory.GetCurrentDirectory();
            //var option = new Compiler.Options()
            //{
            //    File = path + "/TestClasses/Contract2.cs"
            //};
            //Compiler.Compile(option);

            CSharpCompiler.Compile(path + "/TestClasses/Contract2.cs");

            //Compile changes the path, reseting so that other UT won't break
            Directory.SetCurrentDirectory(path);
            Engine.Instance.Reset();
        }

        [TestMethod]
        public void Test_Notification()
        {
            StackItem arg1 = 16;
            StackItem arg2 = "Teste";

            var args = new string[] {
                "./TestClasses/Contract2.nef",
                "unitTest_002",
                arg1.ToParameter().ToJson().ToString(),
                arg2.ToParameter().ToJson().ToString()
            };
            var result = Program.Run(args);

            // mustn't have errors
            Assert.IsTrue(result.ContainsProperty("error"));
            Assert.IsNull(result["error"]);

            // test state
            Assert.IsTrue(result.ContainsProperty("vm_state"));
            Assert.AreEqual(result["vm_state"].AsString(), VMState.HALT.ToString());

            // test result
            StackItem wantresult = 3;
            Assert.IsTrue(result.ContainsProperty("result_stack"));
            Assert.IsInstanceOfType(result["result_stack"], typeof(JArray));

            var resultStack = result["result_stack"] as JArray;
            Assert.IsTrue(resultStack.Count == 1);
            Assert.IsTrue(resultStack[0].ContainsProperty("value"));
            Assert.AreEqual(resultStack[0]["value"].AsString(), wantresult.ToJson()["value"].AsString());

            // test notifications
            Assert.IsTrue(result.ContainsProperty("notifications"));
            Assert.IsInstanceOfType(result["notifications"], typeof(JArray));

            var notifications = result["notifications"] as JArray;
            Assert.IsTrue(notifications.Count == 2);

            Assert.IsTrue(notifications[0].ContainsProperty("value"));
            Assert.IsTrue(notifications[0].ContainsProperty("eventName"));
            Assert.AreEqual(notifications[0]["eventName"].AsString(), "event");
            Assert.IsTrue(notifications[0].ContainsProperty("value"));
            var firstNotifications = notifications[0]["value"];
            Assert.IsTrue(firstNotifications.ContainsProperty("value"));
            Assert.AreEqual((firstNotifications["value"] as JArray)[0].AsString(), arg1.ToJson().ToString());

            Assert.IsTrue(notifications[1].ContainsProperty("value"));
            Assert.IsTrue(notifications[1].ContainsProperty("eventName"));
            Assert.AreEqual(notifications[1]["eventName"].AsString(), "event");
            Assert.IsTrue(notifications[1].ContainsProperty("value"));
            var secondNotifications = notifications[1]["value"];
            Assert.IsTrue(secondNotifications.ContainsProperty("value"));
            Assert.AreEqual((secondNotifications["value"] as JArray)[0].AsString(), arg2.ToJson().ToString());
        }
    }
}
