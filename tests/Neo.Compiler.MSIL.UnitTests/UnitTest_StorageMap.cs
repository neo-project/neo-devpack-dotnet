using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.MSIL.Utils;
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.MSIL
{
    [TestClass]
    public class UnitTest_StorageMap
    {
        private void Put(TestEngine testengine, string method, byte[] prefix, byte[] key, byte[] value)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key), new ByteArray(value));
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(Integer));
            Assert.AreEqual(1, rItem.GetBigInteger());

            Assert.AreEqual(1,
                testengine.Storages
                .Count(a =>
                    a.Key.Key.SequenceEqual(prefix.Concat(key)) &&
                    a.Value.Value.SequenceEqual(value) &&
                    !a.Value.IsConstant
                    ));
        }

        private byte[] Get(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key));
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(ByteArray));

            Assert.AreEqual(1, testengine.Storages.Count(a => a.Key.Key.SequenceEqual(prefix.Concat(key))));

            return rItem.GetByteArray();
        }

        private bool Delete(TestEngine testengine, string method, byte[] prefix, byte[] key)
        {
            var result = testengine.ExecuteTestCaseStandard(method, new ByteArray(key));
            Assert.AreEqual(1, result.Count);
            var rItem = result.Pop();
            Assert.IsInstanceOfType(rItem, typeof(Boolean));

            Assert.AreEqual(0, testengine.Storages.Count(a => a.Key.Key.SequenceEqual(prefix.Concat(key))));

            return rItem.GetBoolean();
        }

        [TestMethod]
        public void Test_Byte()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StorageMap.cs");

            var prefix = new byte[] { 0xAA, 0x00 };  // The byte is consider as a number, so 0x00 is append at the end
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutByte", prefix, key, value);

            // Get

            var getVal = Get(testengine, "TestGetByte", prefix, key);

            CollectionAssert.AreEqual(value, getVal);

            // Delete

            var del = Delete(testengine, "TestDeleteByte", prefix, key);
            Assert.IsTrue(del);
            del = Delete(testengine, "TestDeleteByte", prefix, key);
            Assert.IsFalse(del);
        }

        [TestMethod]
        public void Test_ByteArray()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StorageMap.cs");

            var prefix = new byte[] { 0x00, 0xFF };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutByteArray", prefix, key, value);

            // Get

            var getVal = Get(testengine, "TestGetByteArray", prefix, key);

            CollectionAssert.AreEqual(value, getVal);

            // Delete

            var del = Delete(testengine, "TestDeleteByteArray", prefix, key);
            Assert.IsTrue(del);
            del = Delete(testengine, "TestDeleteByteArray", prefix, key);
            Assert.IsFalse(del);
        }

        [TestMethod]
        public void Test_String()
        {
            var testengine = new TestEngine();
            testengine.AddEntryScript("./TestClasses/Contract_StorageMap.cs");

            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Put(testengine, "TestPutString", prefix, key, value);

            // Get

            var getVal = Get(testengine, "TestGetString", prefix, key);

            CollectionAssert.AreEqual(value, getVal);

            // Delete

            var del = Delete(testengine, "TestDeleteString", prefix, key);
            Assert.IsTrue(del);
            del = Delete(testengine, "TestDeleteString", prefix, key);
            Assert.IsFalse(del);
        }
    }
}