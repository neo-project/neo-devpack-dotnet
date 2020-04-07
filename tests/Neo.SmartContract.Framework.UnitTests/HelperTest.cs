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
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("0a0b0c0d0e0f", (item as ByteString).Span.ToHexString());
        }

        [TestMethod]
        public void TestAssert()
        {
            var engine = new TestEngine();
            engine.AddEntryScript("./TestClasses/Contract_Helper.cs");

            // With extension

            var result = engine.ExecuteTestCaseStandard("assertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetBigInteger(), 5);

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("assertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, engine.State);
            Assert.AreEqual(0, result.Count);

            // Void With extension

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, engine.State);
            Assert.AreEqual(1, result.Count);
            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetBigInteger(), 0);

            engine.Reset();
            result = engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, engine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testByteToByteArray").Run();

            StackItem reverseArray = new byte[] { 0x01 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Reverse()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testReverse").Run();

            StackItem reverseArray = new byte[] { 0x03, 0x02, 0x01 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testSbyteToByteArray").Run();

            StackItem reverseArray = new byte[] { 255 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testStringToByteArray").Run();

            StackItem reverseArray = new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Concat()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testConcat").Run();

            StackItem reverseArray = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Range()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testRange").Run();

            StackItem reverseArray = new byte[] { 0x02 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Take()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testTake").Run();

            StackItem reverseArray = new byte[] { 0x01, 0x02 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Last()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testLast").Run();

            StackItem reverseArray = new byte[] { 0x02, 0x03 };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_Helper.cs");
            var result = testengine.GetMethod("testToScriptHash").Run();

            StackItem reverseArray = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee };
            Assert.AreEqual(reverseArray.ConvertTo(StackItemType.ByteString), result.ConvertTo(StackItemType.ByteString));
        }
    }
}
