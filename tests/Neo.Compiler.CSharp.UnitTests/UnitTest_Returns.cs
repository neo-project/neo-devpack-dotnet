// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Returns.cs file belongs to the neo project and is free
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
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Returns : DebugAndTestBase<Contract_Returns>
    {
        [TestMethod]
        public void Test_OneReturn()
        {
            Assert.AreEqual(new BigInteger(-4), Contract.Subtract(5, 9));
            AssertGasConsumed(1047660);
        }

        [TestMethod]
        public void Test_DoubleReturnA()
        {
            var array = Contract.Div(9, 5)!;
            AssertGasConsumed(1109190);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(BigInteger.One, array[0]);
            Assert.AreEqual(new BigInteger(4), array[1]);
        }

        [TestMethod]
        public void Test_VoidReturn()
        {
            Assert.AreEqual(new BigInteger(14), Contract.Sum(9, 5));
            AssertGasConsumed(1047660);
        }

        [TestMethod]
        public void Test_DoubleReturnB()
        {
            Assert.AreEqual(new BigInteger(-3), Contract.Mix(9, 5));
            AssertGasConsumed(1206390);
        }

        [TestMethod]
        public void Test_ByteStringAdd()
        {
            Assert.AreEqual("helloworld", Encoding.ASCII.GetString(Contract.ByteStringAdd(Encoding.ASCII.GetBytes("hello"), Encoding.ASCII.GetBytes("world"))!));
        }

        [TestMethod]
        public void Test_TryReturn()
        {
            Assert.AreEqual(2, Contract.TryReturn());
        }
    }
}
