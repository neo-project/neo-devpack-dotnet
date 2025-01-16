// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_Types_BigInteger.cs file belongs to the neo project and is free
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
    public class UnitTest_Types_BigInteger : DebugAndTestBase<Contract_Types_BigInteger>
    {
        [TestMethod]
        public void BigInteger_Test()
        {
            // Init

            Assert.AreEqual(BigInteger.Parse("100000000000000000000000000"), Contract.Attribute());
            AssertGasConsumed(984150);

            // static vars

            Assert.AreEqual(BigInteger.Zero, Contract.Zero());
            AssertGasConsumed(984060);
            Assert.AreEqual(BigInteger.One, Contract.One());
            AssertGasConsumed(984060);
            Assert.AreEqual(BigInteger.MinusOne, Contract.MinusOne());
            AssertGasConsumed(984060);

            // Parse

            Assert.AreEqual(456, Contract.Parse("456"));
            AssertGasConsumed(2032230);
            Assert.AreEqual(65, Contract.ConvertFromChar());
            AssertGasConsumed(984060);
        }
    }
}
