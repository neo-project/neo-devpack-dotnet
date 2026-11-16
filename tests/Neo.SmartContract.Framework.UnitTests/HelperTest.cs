// Copyright (C) 2015-2026 The Neo Project.
//
// HelperTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System.Reflection;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class HelperTest : DebugAndTestBase<Contract_Helper>
    {
        [TestMethod]
        public void TestHexToBytes()
        {
            // 0a0b0c0d0E0F
            Assert.AreEqual("0a0b0c0d0e0f", Contract.TestHexToBytes()!.ToHexString());
        }

        [TestMethod]
        public void TestToBigInteger()
        {
            // 0

            Assert.IsNull(Contract.TestToBigInteger(null));
            Assert.AreEqual(0, Contract.TestToBigInteger([]));

            // Value

            Assert.AreEqual(123, Contract.TestToBigInteger([123]));
        }

        [TestMethod]
        public void TestNumEqual()
        {
            Assert.IsTrue(Contract.TestNumEqual(1, 1));
            Assert.IsFalse(Contract.TestNumEqual(1, 2));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumEqual(null, 2));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumEqual(1, null));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumEqual(null, null));
            Assert.IsFalse(Contract.TestNumNotEqual(-1, -1));
            Assert.IsTrue(Contract.TestNumNotEqual(-1, -2));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumNotEqual(null, -2));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumNotEqual(-1, null));
            Assert.ThrowsExactly<TestException>(() => Contract.TestNumNotEqual(null, null));
        }

        [TestMethod]
        public void TestModPow()
        {
            Assert.AreEqual(4, Contract.ModMultiply(4, 7, 6));
            Assert.AreEqual(9, Contract.ModInverse(3, 26));
            Assert.AreEqual(344, Contract.ModPow(23895, 15, 14189));
        }

        [TestMethod]
        public void TestBigIntegerParseandCast()
        {
            Assert.AreEqual(2000000000000000, Contract.TestBigIntegerCast([0x00, 0x00, 0x8d, 0x49, 0xfd, 0x1a, 0x07]));
            var exception = Assert.ThrowsExactly<TestException>(() => Contract.TestBigIntegerParseHexString("00008d49fd1a07"));
            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
        }

        [TestMethod]
        public void TestAssert()
        {
            // With extension
            Assert.AreEqual(5, Contract.AssertCall(true));
            AssertNoLogs();

            var ex = Assert.ThrowsExactly<TestException>(() => Contract.AssertCall(false));
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Test without notification right

            Engine.CallFlags &= ~CallFlags.AllowNotify;
            ex = Assert.ThrowsExactly<TestException>(() => Contract.AssertCall(false));
            Engine.CallFlags = CallFlags.All;
            AssertNoLogs();
            Assert.IsTrue(ex.Message.Contains("UT-ERROR-123"));

            // Void With extension

            Contract.VoidAssertCall(true);
            AssertNoLogs();

            ex = Assert.ThrowsExactly<TestException>(() => Contract.VoidAssertCall(false));
        }

        [TestMethod]
        public void Test_ByteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01 }, Contract.TestByteToByteArray(0x01));

            CollectionAssert.AreEqual(new byte[] { 0x00 }, Contract.TestByteToByteArray(0x00));
        }

        [TestMethod]
        public void Test_Reverse()
        {
            CollectionAssert.AreEqual(new byte[] { 0x03, 0x02, 0x01 }, Contract.TestReverse());
        }

        [TestMethod]
        public void Test_SbyteToByteArray()
        {
            CollectionAssert.AreEqual(new byte[] { 255 }, Contract.TestSbyteToByteArray(-1));

            CollectionAssert.AreEqual(new byte[] { 0x00 }, Contract.TestSbyteToByteArray(0));
        }

        [TestMethod]
        public void Test_StringToByteArray()
        {
            CollectionAssert.AreEqual("hello world"u8.ToArray(), Contract.TestStringToByteArray());
        }

        [TestMethod]
        public void Test_Concat()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, Contract.TestConcat());
        }

        [TestMethod]
        public void Test_Range()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02 }, Contract.TestRange());
        }

        [TestMethod]
        public void Test_Take()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02 }, Contract.TestTake());
        }

        [TestMethod]
        public void Test_Last()
        {
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x03 }, Contract.TestLast());
        }

        [TestMethod]
        public void Test_ToScriptHash()
        {
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xaa, 0xbb, 0xcc, 0xdd, 0xee }, Contract.TestToScriptHash());
        }

        [TestMethod]
        public void TestBigIntegerAsByte()
        {

            Assert.AreEqual(127, Contract.BigIntegerAsByte(127));

            Assert.AreEqual(-128, Contract.BigIntegerAsByte(-128));

            Assert.AreEqual(0, Contract.BigIntegerAsByte(0));

            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsByte(128));

            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsByte(-129));
        }

        [TestMethod]
        public void TestIntAsByte()
        {
            Assert.AreEqual(127, Contract.IntAsByte(127));

            Assert.AreEqual(-128, Contract.IntAsByte(-128));

            Assert.AreEqual(0, Contract.IntAsByte(0));

            Assert.ThrowsExactly<TestException>(() => Contract.IntAsByte(128));

            Assert.ThrowsExactly<TestException>(() => Contract.IntAsByte(-129));
        }

        [TestMethod]
        public void TestBigIntegerAsSbyte()
        {
            Assert.AreEqual(127, Contract.BigIntegerAsSbyte(127));

            Assert.AreEqual(-128, Contract.BigIntegerAsSbyte(-128));

            Assert.AreEqual(0, Contract.BigIntegerAsSbyte(0));


            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsSbyte(128));

            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsSbyte(256));

            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsSbyte(-129));

            Assert.AreEqual(-128, Contract.BigIntegerToSbyte(128));

            Assert.AreEqual(-1, Contract.BigIntegerToSbyte(255));

            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerToSbyte(256));


            Assert.ThrowsExactly<TestException>(() => Contract.BigIntegerAsSbyte(-129));
        }

        [TestMethod]
        public void TestIntAsSbyte()
        {
            Assert.AreEqual(127, Contract.IntAsSbyte(127));

            Assert.AreEqual(-128, Contract.IntAsSbyte(-128));

            Assert.AreEqual(0, Contract.IntAsSbyte(0));

            Assert.ThrowsExactly<TestException>(() => Contract.IntAsSbyte(128));
            Assert.ThrowsExactly<TestException>(() => Contract.IntAsSbyte(256));

            Assert.ThrowsExactly<TestException>(() => Contract.IntAsSbyte(-129));

            Assert.AreEqual(-128, Contract.IntToSbyte(128));

            Assert.AreEqual(-1, Contract.IntToSbyte(255));

            Assert.ThrowsExactly<TestException>(() => Contract.IntToSbyte(256));


            Assert.ThrowsExactly<TestException>(() => Contract.IntAsSbyte(-129));
        }
    }
}
