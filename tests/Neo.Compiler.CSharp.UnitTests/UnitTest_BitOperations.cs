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

        [TestMethod]
        public void TestLeadingZeroCountUInt()
        {
            Assert.AreEqual(BitOperations.LeadingZeroCount(0U), Contract.LeadingZeroCountUInt(0U));
            Assert.AreEqual(BitOperations.LeadingZeroCount(1U), Contract.LeadingZeroCountUInt(1U));
            Assert.AreEqual(BitOperations.LeadingZeroCount(0x00FF00FFU), Contract.LeadingZeroCountUInt(0x00FF00FFU));
            Assert.AreEqual(BitOperations.LeadingZeroCount(uint.MaxValue), Contract.LeadingZeroCountUInt(uint.MaxValue));
        }

        [TestMethod]
        public void TestLeadingZeroCountULong()
        {
            Assert.AreEqual(BitOperations.LeadingZeroCount(0UL), Contract.LeadingZeroCountULong(0UL));
            Assert.AreEqual(BitOperations.LeadingZeroCount(1UL), Contract.LeadingZeroCountULong(1UL));
            Assert.AreEqual(BitOperations.LeadingZeroCount(0x00FF00FF00FF00FFUL), Contract.LeadingZeroCountULong(0x00FF00FF00FF00FFUL));
            Assert.AreEqual(BitOperations.LeadingZeroCount(ulong.MaxValue), Contract.LeadingZeroCountULong(ulong.MaxValue));
        }

        [TestMethod]
        public void TestRotateLeftUInt()
        {
            Assert.AreEqual(BitOperations.RotateLeft(0x12345678U, 8), Contract.RotateLeftUInt(0x12345678U, 8));
            Assert.AreEqual(BitOperations.RotateLeft(1U, 31), Contract.RotateLeftUInt(1U, 31));
            Assert.AreEqual(BitOperations.RotateLeft(uint.MaxValue, 5), Contract.RotateLeftUInt(uint.MaxValue, 5));
            Assert.AreEqual(BitOperations.RotateLeft(0x80000001U, 1), Contract.RotateLeftUInt(0x80000001U, 1));
        }

        [TestMethod]
        public void TestRotateLeftULong()
        {
            Assert.AreEqual(BitOperations.RotateLeft(0x123456789ABCDEF0UL, 16), Contract.RotateLeftULong(0x123456789ABCDEF0UL, 16));
            Assert.AreEqual(BitOperations.RotateLeft(1UL, 63), Contract.RotateLeftULong(1UL, 63));
            Assert.AreEqual(BitOperations.RotateLeft(ulong.MaxValue, 7), Contract.RotateLeftULong(ulong.MaxValue, 7));
        }

        [TestMethod]
        public void TestRotateRightUInt()
        {
            Assert.AreEqual(BitOperations.RotateRight(0x12345678U, 8), Contract.RotateRightUInt(0x12345678U, 8));
            Assert.AreEqual(BitOperations.RotateRight(1U, 1), Contract.RotateRightUInt(1U, 1));
            Assert.AreEqual(BitOperations.RotateRight(uint.MaxValue, 5), Contract.RotateRightUInt(uint.MaxValue, 5));
            Assert.AreEqual(BitOperations.RotateRight(0x80000001U, 1), Contract.RotateRightUInt(0x80000001U, 1));
        }

        [TestMethod]
        public void TestRotateRightULong()
        {
            Assert.AreEqual(BitOperations.RotateRight(0x123456789ABCDEF0UL, 16), Contract.RotateRightULong(0x123456789ABCDEF0UL, 16));
            Assert.AreEqual(BitOperations.RotateRight(1UL, 1), Contract.RotateRightULong(1UL, 1));
            Assert.AreEqual(BitOperations.RotateRight(ulong.MaxValue, 7), Contract.RotateRightULong(ulong.MaxValue, 7));
        }
    }
}
