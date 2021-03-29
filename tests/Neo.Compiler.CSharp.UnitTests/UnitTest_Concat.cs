using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Concat
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Concat.cs");
        }

        [TestMethod]
        public void TestStringAdd1()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringAdd1", "a");
            //.ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("ahello", result.Pop().GetString());
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringAdd2", "a", "b");
            //.ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abhello", result.Pop().GetString());
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringAdd3", "a", "b", "c");//.ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abchello", result.Pop().GetString());
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testStringAdd4", "a", "b", "c", "d");//ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abcdhello", result.Pop().GetString());
        }
    }
}
