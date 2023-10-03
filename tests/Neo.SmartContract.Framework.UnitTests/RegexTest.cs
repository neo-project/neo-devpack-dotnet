using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class RegexTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript("./TestClasses/Contract_Regex.cs");
        }

        [TestMethod]
        public void TestStartWith()
        {
            var result = _engine.ExecuteTestCaseStandard("testStartWith");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Boolean>();
            Assert.IsTrue(item.GetBoolean());
        }

        [TestMethod]
        public void TestIndexOf()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testIndexOf");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Integer>();
            Assert.AreEqual(4, item.GetInteger());
        }

        [TestMethod]
        public void TestEndWith()
        {
            var result = _engine.ExecuteTestCaseStandard("testEndWith");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Boolean>();
            Assert.IsTrue(item.GetBoolean());
        }
        [TestMethod]
        public void TestContains()
        {
            var result = _engine.ExecuteTestCaseStandard("testContains");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Boolean>();
            Assert.IsTrue(item.GetBoolean());
        }

        [TestMethod]
        public void TestNumberOnly()
        {
            var result = _engine.ExecuteTestCaseStandard("testNumberOnly");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Boolean>();
            Assert.IsTrue(item.GetBoolean());
        }
        [TestMethod]
        public void TestAlphabetOnly()
        {
            var result = _engine.ExecuteTestCaseStandard("testAlphabetOnly");
            Assert.AreEqual(1, result.Count);
            var item = result.Pop<Boolean>();
            Assert.IsTrue(item.GetBoolean());
        }
    }
}
