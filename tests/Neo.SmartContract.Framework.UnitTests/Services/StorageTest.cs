// Copyright (C) 2015-2025 The Neo Project.
//
// StorageTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Org.BouncyCastle.Math;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo.SmartContract.Framework.UnitTests.Services
{
    [TestClass]
    public class StorageTest : DebugAndTestBase<Contract_Storage>
    {
        [TestMethod]
        public void Test_StorageMap()
        {
            Assert.AreEqual(456, Contract.SerializeTest([1, 2, 3], 456));
        }

        private void EnsureChanges(byte[] prefix, byte[] key, byte[] value, int count)
        {
            var concat = prefix.Concat(key).ToArray();

            Assert.AreEqual(count, Engine.Storage.Snapshot.GetChangeSet()
                .Count(a =>
                    a.Key.Key.Span.SequenceEqual(concat) &&
                    a.Value.Item.Value.Span.SequenceEqual(value)));
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
        public void Test_IncreaseDecrease()
        {
            var prefix = new byte[] { 0xA0, 0xAF };
            var key = new byte[] { 0x01, 0x02, 0x03 };

            // Put

            Assert.AreEqual(Contract.TestIncrease(key), 1);
            EnsureChanges(prefix, key, BigInteger.One.ToByteArray(), 1);

            Assert.AreEqual(Contract.TestIncrease(key), 2);
            EnsureChanges(prefix, key, BigInteger.Two.ToByteArray(), 1);

            // Decrease

            Assert.AreEqual(Contract.TestDecrease(key), 1);
            EnsureChanges(prefix, key, BigInteger.One.ToByteArray(), 1);

            Assert.AreEqual(Contract.TestDecrease(key), 0);
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

        //[TestMethod]
        //public void Test_String()
        //{
        //    var prefix = new byte[] { 0x61, 0x61 };
        //    var key = new byte[] { 0x01, 0x02, 0x03 };
        //    var value = new byte[] { 0x04, 0x05, 0x06 };

        //    // Put

        //    Assert.IsTrue(Contract.TestPutString(key, value));
        //    EnsureChanges(prefix, key, value, 1);

        //    // Get

        //    var getVal = Contract.TestGetString(key);
        //    CollectionAssert.AreEqual(value, getVal);
        //    EnsureChanges(prefix, key, value, 1);

        //    // Delete

        //    Contract.TestDeleteString(key);
        //    EnsureChanges(prefix, key, 0);
        //}

        [TestMethod]
        public void Test_ReadOnly()
        {
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put

            var exception = Assert.ThrowsException<TestException>(() => Contract.TestPutReadOnly(key, value));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
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

            Assert.IsTrue(Contract.TestIndexPut(key, value));
            CollectionAssert.AreEqual(value, Contract.TestIndexGet(key));
        }

        [TestMethod]
        public void Test_NewGetMethods()
        {
            Assert.IsTrue(Contract.TestNewGetMethods());
        }

        [TestMethod]
        public void Test_NewGetByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x00, 0x01 }, Contract.TestNewGetByteArray());
        }
    }
}
