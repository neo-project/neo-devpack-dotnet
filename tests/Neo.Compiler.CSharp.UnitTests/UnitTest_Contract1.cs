// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Contract1.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Contract1 : DebugAndTestBase<Contract1>
    {
        [TestMethod]
        public void Test_ByteArray_New()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.UnitTest_001());
        }

        [TestMethod]
        public void Test_testArgs1()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.TestArgs1(4));
        }

        [TestMethod]
        public void Test_testArgs2()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, Contract.TestArgs2([1, 2, 3]));
        }

        [TestMethod]
        public void Test_testArgs3()
        {
            // No errors
            Assert.AreEqual(3, Contract.TestArgs3(1, 2));

            Assert.AreEqual(3, Contract.TestArgs3(BigInteger.One, BigInteger.Zero));

            Assert.AreEqual(1, Contract.TestArgs3(BigInteger.MinusOne, BigInteger.MinusOne));

            Assert.AreEqual(-2147483647, Contract.TestArgs3(int.MaxValue, int.MaxValue));

            Assert.AreEqual(-2147483646, Contract.TestArgs3(int.MinValue, int.MaxValue));
        }

        [TestMethod]
        public void Test_testArgs4()
        {
            Assert.AreEqual(5, Contract.TestArgs4(1, 2));

            Assert.AreEqual(3, Contract.TestArgs4(BigInteger.One, BigInteger.Zero));


            Assert.AreEqual(0, Contract.TestArgs4(BigInteger.MinusOne, BigInteger.MinusOne));

            Assert.AreEqual(0, Contract.TestArgs4(int.MaxValue, int.MaxValue));

            Assert.AreEqual(1, Contract.TestArgs4(int.MinValue, int.MaxValue));
        }

        [TestMethod]
        public void Test_testVoid()
        {
            // No errors
            Contract.TestVoid();
        }
    }
}
