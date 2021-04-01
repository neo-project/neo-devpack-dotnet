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
            testEngine.AddEntryScript("./TestClasses/Contract_Property.cs");
            var abi = testEngine.Manifest["abi"];
            var property = abi["methods"].GetArray()[0];
            Assert.AreEqual("symbol", property["name"].GetString());
        }
    }
}
