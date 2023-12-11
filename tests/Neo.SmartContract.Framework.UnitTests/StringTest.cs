using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class StringTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine(snapshot: new TestDataCache());
            _engine.AddEntryScript("./TestClasses/Contract_String.cs");
        }

        [TestMethod]
        public void TestStringAdd()
        {
            // ab => 3
            var result = _engine.ExecuteTestCaseStandard("testStringAdd", "a", "b");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(3, item);

            // hello => 4
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testStringAdd", "he", "llo");
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(4, item);

            // world => 5
            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testStringAdd", "wo", "rld");
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(5, item);
        }

        [TestMethod]
        public void TestStringAddInt()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringAddInt", "Neo", 3);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("Neo3", item.GetString());
        }

        [TestMethod]
        public void TestStringReverse()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringReverse", "Neo");
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("oeN", item.GetString());
        }

        [TestMethod]
        public void TestStringLast()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringLast", "Neo", 2);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("eo", item.GetString());
        }

        [TestMethod]
        public void TestStringTake()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringTake", "Neo", 2);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("Ne", item.GetString());
        }

        [TestMethod]
        public void TestStringReplace()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringReplace", "Neb", 'b', 'o');
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("Neo", item.GetString());
        }
    }
}
