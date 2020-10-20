using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class HelperTest
    {
        private TestEngine _engine;

        [TestInitialize]
        public void Init()
        {
            _engine = new TestEngine();
            _engine.AddEntryScript("./TestClasses/Contract_Helper.cs");
        }

        [TestMethod]
        public void TestHexToBytes()
        {
            // 0a0b0c0d0E0F

            var result = _engine.ExecuteTestCaseStandard("testHexToBytes");
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(VM.Types.ByteString));
            Assert.AreEqual("0a0b0c0d0e0f", (item as VM.Types.ByteString).GetSpan().ToHexString());
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            // 0

            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testToBigInteger", StackItem.Null);
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetInteger());

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testToBigInteger", new VM.Types.ByteString(new byte[0]));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetInteger());

            // Value

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testToBigInteger", new VM.Types.ByteString(new byte[] { 123 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetInteger());
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension

            var result = _engine.ExecuteTestCaseStandard("assertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetInteger(), 5);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("assertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);

            // Void With extension

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(0, result.Count);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            var result = _engine.GetMethod("testByteToByteArray").Run();

            StackItem wantResult = new byte[] { 0x01 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Reverse()
        {
            var result = _engine.GetMethod("testReverse").Run();

            StackItem wantResult = new byte[] { 0x03, 0x02, 0x01 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            var result = _engine.GetMethod("testSbyteToByteArray").Run();

            StackItem wantResult = new byte[] { 255 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            var result = _engine.GetMethod("testStringToByteArray").Run();

            StackItem wantResult = new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Concat()
        {
            var result = _engine.GetMethod("testConcat").Run();

            StackItem wantResult = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Range()
        {
            var result = _engine.GetMethod("testRange").Run();

            StackItem wantResult = new byte[] { 0x02 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Take()
        {
            var result = _engine.GetMethod("testTake").Run();

            StackItem wantResult = new byte[] { 0x01, 0x02 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Last()
        {
            var result = _engine.GetMethod("testLast").Run();

            StackItem wantResult = new byte[] { 0x02, 0x03 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            var result = _engine.GetMethod("testToScriptHash").Run();

            StackItem wantResult = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }
    }
}
