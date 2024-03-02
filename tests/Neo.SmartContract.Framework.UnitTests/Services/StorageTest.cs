using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class StorageTest : TestBase<Contract_Storage>
    {
        public StorageTest() : base(Contract_Storage.Nef, Contract_Storage.Manifest) { }

        [TestMethod]
        public void Test_StorageMap()
        {
            Assert.AreEqual(456, Contract.SerializeTest(new byte[] { 1, 2, 3 }, 456));
        }

        private void EnsureChanges(byte[] prefix, byte[] key, byte[] value, int count)
        {
            var concat = prefix.Concat(key).ToArray();

            Assert.AreEqual(count, Engine.Storage.Snapshot.GetChangeSet()
                .Count(a =>
                    a.Key.Key.Span.SequenceEqual(concat) &&
                    a.Item.Value.Span.SequenceEqual(value)));
        }

        private void EnsureChanges(byte[] prefix, byte[] key, int count)
        {
            var concat = prefix.Concat(key).ToArray();

            Assert.AreEqual(count, Engine.Storage.Snapshot.GetChangeSet()
                .Count(a => a.Key.Key.Span.SequenceEqual(concat)));
        }

        [TestMethod]
        public void Test_Byte()
        {
            var prefix = new byte[] { 0x11 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Assert.IsTrue(Contract.TestPutByte(key, value));
            EnsureChanges(prefix, key, value, 1);

            // Get

            var getVal = Contract.TestGetByte(key);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete

            Contract.TestDeleteByte(key);
            EnsureChanges(prefix, key, 0);
        }

        [TestMethod]
        public void Test_ByteArray()
        {
            var prefix = new byte[] { 0x00, 0xFF };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Assert.IsTrue(Contract.TestPutByteArray(key, value));
            EnsureChanges(prefix, key, value, 1);

            // Get

            var getVal = Contract.TestGetByteArray(key);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete

            Contract.TestDeleteByteArray(key);
            EnsureChanges(prefix, key, 0);
        }

        [TestMethod]
        public void Test_LongBytes()
        {
            CollectionAssert.AreEqual(new byte[] {
                0x3b, 0x00, 0x32, 0x03, 0x23, 0x23, 0x23, 0x23, 0x02, 0x23,
                0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23, 0x02, 0x23, 0x23,
                0x02, 0x23, 0x23, 0x02
                }, Contract.TestOver16Bytes());
        }

        [TestMethod]
        public void Test_String()
        {
            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Assert.IsTrue(Contract.TestPutString(key, value));
            EnsureChanges(prefix, key, value, 1);

            // Get

            var getVal = Contract.TestGetString(key);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete

            Contract.TestDeleteString(key);
            EnsureChanges(prefix, key, 0);
        }

        [TestMethod]
        public void Test_ReadOnly()
        {
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            Assert.ThrowsException<TargetInvocationException>(() => Contract.TestPutReadOnly(key, value));
        }

        [TestMethod]
        public void Test_Find()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestFind());
        }

        [TestMethod]
        public void Test_Index()
        {
            var key = Encoding.UTF8.GetBytes("key");
            var value = Encoding.UTF8.GetBytes("123");

            Assert.AreEqual(true, Contract.TestIndexPut(key, value));
            CollectionAssert.AreEqual(value, Contract.TestIndexGet(key));
        }

        [TestMethod]
        public void Test_NewGetMethods()
        {
            Assert.AreEqual(true, Contract.TestNewGetMethods());
        }

        [TestMethod]
        public void Test_NewGetByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x00, 0x01 }, Contract.TestNewGetByteArray());
        }
    }
}
