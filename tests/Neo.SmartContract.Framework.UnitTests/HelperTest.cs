using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class HelperTest
    {
        [TestMethod]
        public void TestHexToBytes()
        {
            var engine = new TestEngine();
            engine.AddEntryScript("./TestClasses/Contract_Helper.cs");

            // 0a0b0c0d0E0F

            var result = engine.ExecuteTestCaseStandard("testHexToBytes");
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(ByteArray));
            Assert.AreEqual("0a0b0c0d0e0f", (item as ByteArray).Span.ToHexString());
        }

        [TestMethod]
        public void TestAssertExtension()
        {
            var engine = new TestEngine();
            engine.AddEntryScript("./TestClasses/Contract_Helper.cs");

            // With extension

            var result = engine.ExecuteTestCaseStandard("assertExtension", new Boolean(true));
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetBigInteger(), 5);

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("assertExtension", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, engine.State);
            Assert.AreEqual(0, result.Count);

            // Void With extension

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("voidAssertExtension", new Boolean(true));
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetBigInteger(), 0);

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("voidAssertExtension", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, engine.State);
            Assert.AreEqual(0, result.Count);
        }
    }
}
