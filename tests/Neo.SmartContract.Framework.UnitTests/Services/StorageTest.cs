// Copyright (C) 2015-2026 The Neo Project.
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
            AssertGasConsumed(7347500);
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
            AssertGasConsumed(3660970);
            EnsureChanges(prefix, key, value, 1);

            // Get
            var getVal = Contract.TestGetByte(key);
            AssertGasConsumed(2960880);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete
            Contract.TestDeleteByte(key);
            AssertGasConsumed(2714880);
            EnsureChanges(prefix, key, 0);
        }

        [TestMethod]
        public void Test_IncreaseDecrease()
        {
            var prefix = new byte[] { 0xA0, 0xAF };
            var key = new byte[] { 0x01, 0x02, 0x03 };

            // Put
            Assert.AreEqual(Contract.TestIncrease(key), 1);
            AssertGasConsumed(4449780);
            EnsureChanges(prefix, key, BigInteger.One.ToByteArray(), 1);

            Assert.AreEqual(Contract.TestIncrease(key), 2);
            AssertGasConsumed(4195390);
            EnsureChanges(prefix, key, BigInteger.Two.ToByteArray(), 1);

            // Decrease
            Assert.AreEqual(Contract.TestDecrease(key), 1);
            AssertGasConsumed(4196140);
            EnsureChanges(prefix, key, BigInteger.One.ToByteArray(), 1);

            Assert.AreEqual(Contract.TestDecrease(key), 0);
            AssertGasConsumed(4096140);
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
            AssertGasConsumed(3753440);
            EnsureChanges(prefix, key, value, 1);

            // Get
            var getVal = Contract.TestGetByteArray(key);
            AssertGasConsumed(2953770);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete
            Contract.TestDeleteByteArray(key);
            AssertGasConsumed(2707350);
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
            AssertGasConsumed(7298700);
        }

        [TestMethod]
        public void Test_String()
        {
            var prefix = new byte[] { 0x61, 0x61 };
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put
            Assert.IsTrue(Contract.TestPutString(key, value));
            AssertGasConsumed(3507680);
            EnsureChanges(prefix, key, value, 1);

            // Get
            var getVal = Contract.TestGetString(key);
            AssertGasConsumed(2707530);
            CollectionAssert.AreEqual(value, getVal);
            EnsureChanges(prefix, key, value, 1);

            // Delete
            Contract.TestDeleteString(key);
            AssertGasConsumed(2461590);
            EnsureChanges(prefix, key, 0);
        }

        [TestMethod]
        public void Test_ReadOnly()
        {
            var key = new byte[] { 0x01, 0x02, 0x03 };
            var value = new byte[] { 0x04, 0x05, 0x06 };

            // Put
            var exception = Assert.ThrowsExactly<TestException>(() => Contract.TestPutReadOnly(key, value));
            AssertGasConsumed(2953950);
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void Test_Find()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestFind());
            AssertGasConsumed(5920780);
        }

        [TestMethod]
        public void Test_Index()
        {
            var key = Encoding.UTF8.GetBytes("key");
            var value = Encoding.UTF8.GetBytes("123");

            Assert.IsTrue(Contract.TestIndexPut(key, value));
            AssertGasConsumed(3507740);
            CollectionAssert.AreEqual(value, Contract.TestIndexGet(key));
            AssertGasConsumed(2707530);
        }

        [TestMethod]
        public void Test_GetNullInteger()
        {
            var key = Encoding.UTF8.GetBytes("key");
            Assert.IsNull(Contract.TestGetInteger(key));
            AssertGasConsumed(2647200);
        }

        [TestMethod]
        public void Test_NewGetMethods()
        {
            Assert.IsTrue(Contract.TestNewGetMethods());
            AssertGasConsumed(30701790);
        }

        [TestMethod]
        public void Test_NewGetByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x00, 0x01 }, Contract.TestNewGetByteArray());
            AssertGasConsumed(5299000);
        }

        [TestMethod]
        public void Test_LocalString()
        {
            var key = new byte[] { 0x11 };
            var value = new byte[] { 0x22 };

            Assert.IsTrue(Contract.PutLocalString(key, value));
            AssertGasConsumed(3047530);
            var got = Contract.GetLocalString(key);
            AssertGasConsumed(2647320);
            CollectionAssert.AreEqual(value, got);

            Contract.DeleteLocalString(key);
            AssertGasConsumed(2401440);
            got = Contract.GetLocalString(key);
            AssertGasConsumed(2647320);
            Assert.IsNull(got);

            value = new byte[] { 0x33, 0x44, 0x55 };
            Assert.IsTrue(Contract.PutLocalString(key, value));
            AssertGasConsumed(3247530);
            got = Contract.GetLocalString(key);
            AssertGasConsumed(2647320);
            CollectionAssert.AreEqual(value, got);

            Contract.DeleteLocalString(key);
            AssertGasConsumed(2401440);
            got = Contract.GetLocalString(key);
            AssertGasConsumed(2647320);
            Assert.IsNull(got);
        }

        [TestMethod]
        public void Test_LocalByteArray()
        {
            var key = new byte[] { 0x11 };
            var value = new byte[] { 0x22 };
            Assert.IsTrue(Contract.PutLocalByteArray(key, value));
            AssertGasConsumed(3293290);
            var got = Contract.GetLocalByteArray(key);
            AssertGasConsumed(2893080);
            CollectionAssert.AreEqual(value, got);

            Contract.DeleteLocalByteArray(key);
            AssertGasConsumed(2647200);
            got = Contract.GetLocalByteArray(key);
            AssertGasConsumed(2893080);
            Assert.IsNull(got);

            value = new byte[] { 0x33, 0x44, 0x55 };
            Assert.IsTrue(Contract.PutLocalByteArray(key, value));
            AssertGasConsumed(3493290);
            got = Contract.GetLocalByteArray(key);
            AssertGasConsumed(2893080);
            CollectionAssert.AreEqual(value, got);

            Contract.DeleteLocalByteArray(key);
            AssertGasConsumed(2647200);
            got = Contract.GetLocalByteArray(key);
            AssertGasConsumed(2893080);
            Assert.IsNull(got);
        }

        [TestMethod]
        public void Test_LocalFind()
        {
            var got = Contract.LocalFind();
            AssertGasConsumed(6910740);
            CollectionAssert.AreEqual(new byte[] { 0x01 }, got);
        }

        [TestMethod]
        public void Test_LocalIndex()
        {
            var key = new byte[] { 0x01 };
            var value = new byte[] { 0x02 };
            Contract.LocalIndexPut(key, value);
            AssertGasConsumed(3347560);

            var got = Contract.LocalIndexGet(key);
            AssertGasConsumed(2647320);
            CollectionAssert.AreEqual(value, got);
        }

        [TestMethod]
        public void Test_LocalGetPut()
        {
            Assert.IsTrue(Contract.LocalGetPut());
            AssertGasConsumed(29985870);

            var key = new byte[] { 0x22 };
            Assert.AreEqual(123, Contract.LocalGetPutObject(key, 123));
            AssertGasConsumed(7419200);
        }

        [TestMethod]
        public void Test_LocalIncDec()
        {
            var key = new byte[] { 0x33 };
            var value = Contract.LocalIntegerOrZero(key);
            AssertGasConsumed(2647470);
            Assert.AreEqual(0, value);

            value = Contract.LocalIncrease(key);
            AssertGasConsumed(4130020);
            Assert.AreEqual(1, value);

            value = Contract.LocalDecrease(key);
            AssertGasConsumed(3976380);
            Assert.AreEqual(0, value);

            value = Contract.LocalIncrease(key);
            AssertGasConsumed(4130020);
            Assert.AreEqual(1, value);

            value = Contract.LocalIncrease(key);
            AssertGasConsumed(4075630);
            Assert.AreEqual(2, value);

            value = Contract.LocalDecrease(key);
            AssertGasConsumed(4076380);
            Assert.AreEqual(1, value);

            value = Contract.LocalDecrease(key);
            AssertGasConsumed(3976380);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void Test_LocalPutGetDeleteBytes()
        {
            var key = new byte[] { 0x44 };
            var value = new byte[] { 0x55 };
            Assert.IsTrue(Contract.TestLocalPutBytes(key, value));
            AssertGasConsumed(3200940);
            var got = Contract.TestLocalGetBytes(key);
            AssertGasConsumed(2900730);
            CollectionAssert.AreEqual(value, got);

            Contract.TestLocalDeleteBytes(key);
            AssertGasConsumed(2654850);
            got = Contract.TestLocalGetBytes(key);
            AssertGasConsumed(2900730);
            Assert.IsNull(got);
        }
    }
}
