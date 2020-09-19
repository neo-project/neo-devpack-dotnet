using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.Compiler.MSIL.UnitTests
{
    [TestClass]
    public class UnitTest_Concat
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Concat.cs", false, false);
        }

        [TestMethod]
        public void TestStringAdd1()
        {
            _engine.Reset();
            var result = _engine.GetMethod("testStringAdd1").Run("a").ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("ahello", result.GetString());
        }

        [TestMethod]
        public void TestStringAdd2()
        {
            _engine.Reset();
            var result = _engine.GetMethod("testStringAdd2").Run("a", "b").ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abhello", result.GetString());
        }

        [TestMethod]
        public void TestStringAdd3()
        {
            _engine.Reset();
            var result = _engine.GetMethod("testStringAdd3").Run("a", "b", "c").ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abchello", result.GetString());
        }

        [TestMethod]
        public void TestStringAdd4()
        {
            _engine.Reset();
            var result = _engine.GetMethod("testStringAdd4").Run("a", "b", "c", "d").ConvertTo(StackItemType.ByteString);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual("abcdhello", result.GetString());
        }
    }
}
