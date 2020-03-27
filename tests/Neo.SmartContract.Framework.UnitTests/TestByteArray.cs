using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class TestByteArray
    {
        //[TestMethod]
        //public void TestByteSet()
        //{
        //    var testengine = new TestEngine();
        //    testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

        //    var result = testengine.ExecuteTestCaseStandard("testByteSet");
        //    var buffer = result.Pop() as Buffer;
        //    Assert.AreEqual(new byte[] { 0x01, 0x04, 0x03 }, buffer.ConvertTo(StackItemType.ByteArray));
        //}

        [TestMethod]
        public void TestByteReverse()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            var result = testengine.ExecuteTestCaseStandard("testByteReverse");
            var buffer = result.Pop() as Buffer;
            Assert.AreEqual(new byte[] { 0x03, 0x02, 0x01 }, buffer.ConvertTo(StackItemType.ByteArray));
        }

        [TestMethod]
        public void TestConcat()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            var result = testengine.ExecuteTestCaseStandard("testByteCocat");

            Assert.AreEqual(1, testengine.Notifications.Count);
            var array = testengine.Notifications[0].State as Array;
            Assert.AreEqual(1, array.Count);
            var buffer = array[0] as Buffer;
            Assert.AreEqual(new byte[] { 0x12, 0x23, 0x32, 0x55, 0x23, 0x01, 0x02 }, buffer.ConvertTo(StackItemType.ByteArray));

            buffer = result.Pop() as Buffer;
            Assert.AreEqual(new byte[] { 0x12, 0x23, 0x32, 0x55, 0x23, 0x01, 0x02 }, buffer.ConvertTo(StackItemType.ByteArray));
        }

        [TestMethod]
        public void TestRange()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            var result = testengine.ExecuteTestCaseStandard("testByteRange");

            Assert.AreEqual(1, testengine.Notifications.Count);
            var array = testengine.Notifications[0].State as Array;
            Assert.AreEqual(1, array.Count);
            var buffer = array[0] as Buffer;
            Assert.AreEqual(new byte[] { 0x12, 0x23 }, buffer.ConvertTo(StackItemType.ByteArray));

            buffer = result.Pop() as Buffer;
            Assert.AreEqual(new byte[] { 0x12, 0x23 }, buffer.ConvertTo(StackItemType.ByteArray));
        }

        [TestMethod]
        public void TestTake()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            var result = testengine.ExecuteTestCaseStandard("testByteTake");

            Assert.AreEqual(1, testengine.Notifications.Count);
            var array = testengine.Notifications[0].State as Array;
            Assert.AreEqual(1, array.Count);
            var buffer = array[0] as Buffer;
            Assert.AreEqual(new byte[] { 0x12 }, buffer.ConvertTo(StackItemType.ByteArray));

            buffer = result.Pop() as Buffer;
            Assert.AreEqual(new byte[] { 0x12 }, buffer.ConvertTo(StackItemType.ByteArray));
        }

        [TestMethod]
        public void TestLast()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            var result = testengine.ExecuteTestCaseStandard("testByteLast");

            Assert.AreEqual(1, testengine.Notifications.Count);
            var array = testengine.Notifications[0].State as Array;
            Assert.AreEqual(1, array.Count);
            var buffer = array[0] as Buffer;
            Assert.AreEqual(new byte[] { 0x32 }, buffer.ConvertTo(StackItemType.ByteArray));

            buffer = result.Pop() as Buffer;
            Assert.AreEqual(new byte[] { 0x32 }, buffer.ConvertTo(StackItemType.ByteArray));
        }
    }
}
