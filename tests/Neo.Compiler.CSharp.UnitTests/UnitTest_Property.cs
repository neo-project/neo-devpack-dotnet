using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Property
    {
        private TestEngine testEngine;

        [TestInitialize]
        public void Init()
        {
            testEngine = new TestEngine();
            testEngine.AddEntryScript("./TestClasses/Contract_Property.cs");
        }

        [TestMethod]
        public void TestABIOffsetWithoutOptimizer()
        {
            testEngine.Reset();
            var abi = testEngine.Manifest["abi"];
            var property = abi["methods"].GetArray()[0];
            Assert.AreEqual("symbol", property["name"].GetString());
        }

        [TestMethod]
        public void IndexTest()
        {
            testEngine.Reset();
            var result = testEngine.ExecuteTestCaseStandard("testIndex", 5);
            Assert.AreEqual("5", result.Pop().GetString());
        }
    }
}
