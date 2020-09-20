using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_OnDeploy
    {
        [TestMethod]
        public void Test_OnDeploy()
        {
            var testengine = new TestEngine();
            var build = testengine.Build("./TestClasses/Contract_OnDeploy.cs", false, true);

            var methods = (build.finalABI["methods"] as JArray);

            Assert.AreEqual(1, methods.Count);
            Assert.AreEqual(methods[0]["name"].AsString(), "_ondeploy");
            Assert.AreEqual(methods[0]["offset"].AsString(), "0");
            Assert.AreEqual(methods[0]["returntype"].AsString(), "Void");

            var args = (methods[0]["parameters"] as JArray);

            Assert.AreEqual(1, args.Count);
            Assert.AreEqual(args[0]["name"].AsString(), "update");
            Assert.AreEqual(args[0]["type"].AsString(), "Boolean");
        }
    }
}
