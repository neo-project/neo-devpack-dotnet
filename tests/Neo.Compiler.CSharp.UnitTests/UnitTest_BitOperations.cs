// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_BitOperations.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_BitOperations : DebugAndTestBase<Contract_BitOperations>
    {
        protected override bool TestGasConsume => false;

        [TestMethod]
        public void TestLog2UInt()
        {
            Assert.AreEqual(BitOperations.Log2(0U), Contract.Log2UInt(0U));
            Assert.AreEqual(BitOperations.Log2(1U), Contract.Log2UInt(1U));
            Assert.AreEqual(BitOperations.Log2(2U), Contract.Log2UInt(2U));
            Assert.AreEqual(BitOperations.Log2(uint.MaxValue), Contract.Log2UInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestLog2ULong()
        {
            Assert.AreEqual(BitOperations.Log2(0UL), Contract.Log2ULong(0UL));
            Assert.AreEqual(BitOperations.Log2(1UL), Contract.Log2ULong(1UL));
            Assert.AreEqual(BitOperations.Log2(2UL), Contract.Log2ULong(2UL));
            Assert.AreEqual(BitOperations.Log2(ulong.MaxValue), Contract.Log2ULong(ulong.MaxValue));
        }

        [TestMethod]
        public void TestPopCountUInt()
        {
            Assert.AreEqual(BitOperations.PopCount(0U), Contract.PopCountUInt(0U));
            Assert.AreEqual(BitOperations.PopCount(1U), Contract.PopCountUInt(1U));
            Assert.AreEqual(BitOperations.PopCount(uint.MaxValue), Contract.PopCountUInt(uint.MaxValue));
            Assert.AreEqual(BitOperations.PopCount(0xAAAAAAAAU), Contract.PopCountUInt(0xAAAAAAAAU));
            Assert.AreEqual(BitOperations.PopCount(0x55555555U), Contract.PopCountUInt(0x55555555U));
        }

        [TestMethod]
        public void TestPopCountULong()
        {
            Assert.AreEqual(BitOperations.PopCount(0UL), Contract.PopCountULong(0UL));
            Assert.AreEqual(BitOperations.PopCount(1UL), Contract.PopCountULong(1UL));
            Assert.AreEqual(BitOperations.PopCount(ulong.MaxValue), Contract.PopCountULong(ulong.MaxValue));
            Assert.AreEqual(BitOperations.PopCount(0xAAAAAAAAAAAAAAAAUL), Contract.PopCountULong(0xAAAAAAAAAAAAAAAAUL));
            Assert.AreEqual(BitOperations.PopCount(0x5555555555555555UL), Contract.PopCountULong(0x5555555555555555UL));
        }
    }
}
