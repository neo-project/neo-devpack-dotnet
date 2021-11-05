using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Utils;
using Neo.VM;
using Neo.VM.Types;
using System.Collections.Generic;

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
            Assert.IsInstanceOfType(item, typeof(ByteString));
            Assert.AreEqual("0a0b0c0d0e0f", (item as ByteString).GetSpan().ToHexString());
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
            Assert.IsTrue(item.IsNull);

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testToBigInteger", new ByteString(System.Array.Empty<byte>()));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(0, item.GetInteger());

            // Value

            _engine.Reset();
            result = _engine.ExecuteTestCaseStandard("testToBigInteger", new ByteString(new byte[] { 123 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(123, item.GetInteger());
        }

        [TestMethod]
        public void TestBigIntegerParseandCast()
        {
            _engine.Reset();
            var result = _engine.ExecuteTestCaseStandard("testBigIntegerCast", new ByteString(new byte[] { 0x00, 0x00, 0x8d, 0x49, 0xfd, 0x1a, 0x07 }));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);

            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(2000000000000000, item.GetInteger());

            _engine.Reset();
            _engine.ExecuteTestCaseStandard("testBigIntegerParseHexString", "00008d49fd1a07");
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.IsNotNull(_engine.FaultException);
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension
            var logList = new List<string>();
            var logsMethod = new System.EventHandler<LogEventArgs>((object sender, LogEventArgs e) => { logList.Add(e.Message); });
            ApplicationEngine.Log += logsMethod;
            var result = _engine.ExecuteTestCaseStandard("assertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(1, result.Count);
            var item = result.Pop();
            Assert.IsInstanceOfType(item, typeof(Integer));
            Assert.AreEqual(item.GetInteger(), 5);
            Assert.AreEqual(logList.Count, 0);

            _engine.Reset(); logList.Clear();
            result = _engine.ExecuteTestCaseStandard("assertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(logList.Count, 1);
            Assert.AreEqual(logList[0], "ERROR");

            // Void With extension

            _engine.Reset(); logList.Clear();
            result = _engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(true));
            Assert.AreEqual(VMState.HALT, _engine.State);
            Assert.AreEqual(logList.Count, 0);
            Assert.AreEqual(0, result.Count);

            _engine.Reset(); logList.Clear();
            result = _engine.ExecuteTestCaseStandard("voidAssertCall", new Boolean(false));
            Assert.AreEqual(VMState.FAULT, _engine.State);
            Assert.AreEqual(logList.Count, 0);
            Assert.AreEqual(0, result.Count);
            ApplicationEngine.Log -= logsMethod;
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            var result = _engine.ExecuteTestCaseStandard("testByteToByteArray").Pop();

            StackItem wantResult = new byte[] { 0x01 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Reverse()
        {
            var result = _engine.ExecuteTestCaseStandard("testReverse").Pop();

            StackItem wantResult = new byte[] { 0x03, 0x02, 0x01 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            var result = _engine.ExecuteTestCaseStandard("testSbyteToByteArray").Pop();

            StackItem wantResult = new byte[] { 255 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            var result = _engine.ExecuteTestCaseStandard("testStringToByteArray").Pop();

            StackItem wantResult = new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Concat()
        {
            var result = _engine.ExecuteTestCaseStandard("testConcat").Pop();

            StackItem wantResult = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Range()
        {
            var result = _engine.ExecuteTestCaseStandard("testRange").Pop();

            StackItem wantResult = new byte[] { 0x02 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Take()
        {
            var result = _engine.ExecuteTestCaseStandard("testTake").Pop();

            StackItem wantResult = new byte[] { 0x01, 0x02 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_Last()
        {
            var result = _engine.ExecuteTestCaseStandard("testLast").Pop();

            StackItem wantResult = new byte[] { 0x02, 0x03 };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            var result = _engine.ExecuteTestCaseStandard("testToScriptHash").Pop();

            StackItem wantResult = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee };
            Assert.AreEqual(wantResult.ConvertTo(VM.Types.StackItemType.ByteString), result.ConvertTo(VM.Types.StackItemType.ByteString));
        }
    }
}
