using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_Property
    {
        [TestMethod]
        public void UnitTest_TestABIOffsetWithoutOptimizer()
        {
            var buildScript = NeonTestTool.BuildScript("./TestClasses/Contract_Property.cs", true, false);
            var abi = buildScript.finalABI;

            var property = abi["methods"].GetArray()[0];
            Assert.AreEqual("symbol", property["name"].GetString());
        }
    }
}
