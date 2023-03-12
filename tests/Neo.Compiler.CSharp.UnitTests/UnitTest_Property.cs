using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.UnitTest.Framework;

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
            var property = abi["methods"][0];
            Assert.AreEqual("symbol", property["name"].GetString());
        }
    }
}
