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
            AssertGasConsumed(1232070);
        }

        [TestMethod]
        public void Test_testArgs1()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.TestArgs1(4));
            AssertGasConsumed(1539180);
        }

        [TestMethod]
        public void Test_testArgs2()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, Contract.TestArgs2([1, 2, 3]));
            AssertGasConsumed(1047240);
        }

        [TestMethod]
        public void Test_testArgs3()
        {
            // No errors
            Assert.AreEqual(3, Contract.TestArgs3(1, 2));
            AssertGasConsumed(1047720);

            Assert.AreEqual(3, Contract.TestArgs3(BigInteger.One, BigInteger.Zero));
            AssertGasConsumed(1047720);

            Assert.AreEqual(1, Contract.TestArgs3(BigInteger.MinusOne, BigInteger.MinusOne));
            AssertGasConsumed(1047720);

            Assert.AreEqual(-2147483647, Contract.TestArgs3(int.MaxValue, int.MaxValue));
            AssertGasConsumed(1048410);

            Assert.AreEqual(-2147483646, Contract.TestArgs3(int.MinValue, int.MaxValue));
            AssertGasConsumed(1047720);
        }

        [TestMethod]
        public void Test_testArgs4()
        {
            Assert.AreEqual(5, Contract.TestArgs4(1, 2));
            AssertGasConsumed(1048320);

            Assert.AreEqual(3, Contract.TestArgs4(BigInteger.One, BigInteger.Zero));
            AssertGasConsumed(1048320);


            Assert.AreEqual(0, Contract.TestArgs4(BigInteger.MinusOne, BigInteger.MinusOne));
            AssertGasConsumed(1048320);

            Assert.AreEqual(0, Contract.TestArgs4(int.MaxValue, int.MaxValue));
            AssertGasConsumed(1049010);

            Assert.AreEqual(1, Contract.TestArgs4(int.MinValue, int.MaxValue));
            AssertGasConsumed(1048320);
        }

        [TestMethod]
        public void Test_testVoid()
        {
            // No errors
            Contract.TestVoid();
            AssertGasConsumed(1232010);
        }
    }
}
