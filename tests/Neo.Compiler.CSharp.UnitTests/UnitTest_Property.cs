using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var testEngine = new TestEngine();
            var buildScript = testEngine.Build("./TestClasses/Contract_Property.cs");
            var abi = buildScript.manifest["abi"];
            var property = abi["methods"].GetArray()[0];
            Assert.AreEqual("symbol", property["name"].GetString());
        }
    }
}
