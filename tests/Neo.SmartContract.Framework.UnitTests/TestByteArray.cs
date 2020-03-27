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
            //var testengine = new TestEngine();
            //testengine.AddEntryScript("./TestClasses/Contract_ByteArray.cs");

            //var result = testengine.ExecuteTestCaseStandard("testByteLast");

            //Assert.AreEqual(1, testengine.Notifications.Count);
            //var array = testengine.Notifications[0].State as Array;
            //Assert.AreEqual(1, array.Count);
            //var buffer = array[0] as Buffer;
            //Assert.AreEqual(new byte[] { 0x32 }, buffer.ConvertTo(StackItemType.ByteArray));

            //buffer = result.Pop() as Buffer;
            //Assert.AreEqual(new byte[] { 0x32 }, buffer.ConvertTo(StackItemType.ByteArray));


            Dictionary<byte[], int> map = new Dictionary<byte[], int>();

            var k1 = new byte[] { 0x00, 0x01 };
            var k2 = new byte[] { 0x01, 0x01 };
            map[k1] = 1;
            map[k2] = 2;

            System.Console.WriteLine("before map[k1]: " + map[k1] + "\t k1 hashcode: " + k1.GetHashCode());
            System.Console.WriteLine("before map[k2]: " + map[k2] + "\t k2 hashcode: " + k2.GetHashCode());

            k1[0] = 0x01;
            k2[0] = 0x00;

            System.Console.WriteLine("after map[k1]: " + map[k1] + "\t k1 hashcode: " + k1.GetHashCode());
            System.Console.WriteLine("after map[k2]: " + map[k2] + "\t k2 hashcode: " + k2.GetHashCode());

            int a = 1;
            System.Console.WriteLine(a + " hashcode " + a.GetHashCode());
            a = 2;
            System.Console.WriteLine(a + " hashcode " + a.GetHashCode());
        }
    }
}
