// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Default.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Default : DebugAndTestBase<Contract_Default>
    {

        [TestMethod]
        public void TestBooleanDefault()
        {
            var result = Contract.TestBooleanDefault();
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestByteDefault()
        {
            var result = Contract.TestByteDefault();
            Assert.AreEqual((byte)0, result);
        }

        [TestMethod]
        public void TestSByteDefault()
        {
            var result = Contract.TestSByteDefault();
            Assert.AreEqual((sbyte)0, result);
        }

        [TestMethod]
        public void TestInt16Default()
        {
            var result = Contract.TestInt16Default();
            Assert.AreEqual((short)0, result);
        }

        [TestMethod]
        public void TestUInt16Default()
        {
            var result = Contract.TestUInt16Default();
            Assert.AreEqual((ushort)0, result);
        }

        [TestMethod]
        public void TestInt32Default()
        {
            var result = Contract.TestInt32Default();
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestUInt32Default()
        {
            var result = Contract.TestUInt32Default();
            Assert.AreEqual(0U, result);
        }

        [TestMethod]
        public void TestInt64Default()
        {
            var result = Contract.TestInt64Default();
            Assert.AreEqual(0L, result);
        }

        [TestMethod]
        public void TestUInt64Default()
        {
            var result = Contract.TestUInt64Default();
            Assert.AreEqual(0UL, result);
        }

        [TestMethod]
        public void TestCharDefault()
        {
            var result = Contract.TestCharDefault();
            Assert.AreEqual('\0', result);
        }

        [TestMethod]
        public void TestStringDefault()
        {
            var result = Contract.TestStringDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestObjectDefault()
        {
            var result = Contract.TestObjectDefault();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestBigIntegerDefault()
        {
            var result = Contract.TestBigIntegerDefault();
            Assert.AreEqual(BigInteger.Zero, result);
        }

        [TestMethod]
        public void TestStructDefault()
        {
            var result = Contract.TestStructDefault();
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void TestClassDefault()
        {
            var result = Contract.TestClassDefault();
            Assert.IsNull(result);
        }
    }
}
