// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Overflow.cs file belongs to the neo project and is free
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

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Overflow : DebugAndTestBase<Contract_Overflow>
    {
        [TestMethod]
        public void Test_AddInt()
        {
            Assert.AreEqual(unchecked(int.MinValue - 1), Contract.AddInt(int.MinValue, -1));
            Assert.AreEqual(unchecked(int.MaxValue + 1), Contract.AddInt(int.MaxValue, 1));
            Assert.AreEqual(unchecked(int.MinValue - int.MaxValue), Contract.AddInt(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked(int.MaxValue - int.MinValue), Contract.AddInt(int.MaxValue, unchecked(-int.MinValue)));
        }

        [TestMethod]
        public void Test_MulInt()
        {
            Assert.AreEqual(unchecked(int.MinValue * 2), Contract.MulInt(int.MinValue, 2));
            Assert.AreEqual(unchecked(int.MinValue * (-2)), Contract.MulInt(int.MinValue, -2));
            Assert.AreEqual(unchecked(int.MaxValue * 2), Contract.MulInt(int.MaxValue, 2));
            Assert.AreEqual(unchecked(int.MaxValue * (-2)), Contract.MulInt(int.MaxValue, -2));
            Assert.AreEqual(unchecked(int.MinValue * int.MaxValue), Contract.MulInt(int.MinValue, int.MaxValue));
            Assert.AreEqual(unchecked(int.MinValue * (-int.MaxValue)), Contract.MulInt(int.MinValue, -int.MaxValue));
            Assert.AreEqual(unchecked((-int.MinValue) * int.MaxValue), Contract.MulInt(unchecked(-int.MinValue), int.MaxValue));
        }

        [TestMethod]
        public void Test_AddUInt()
        {
            Assert.AreEqual(unchecked(uint.MinValue - 1), Contract.AddUInt(uint.MinValue, -1));
            Assert.AreEqual(unchecked(uint.MaxValue + 1), Contract.AddUInt(uint.MaxValue, 1));
            Assert.AreEqual(unchecked(uint.MinValue - uint.MaxValue), Contract.AddUInt(uint.MinValue, -uint.MaxValue));
            Assert.AreEqual(unchecked(uint.MaxValue - uint.MinValue), Contract.AddUInt(uint.MaxValue, unchecked(-uint.MinValue)));
        }

        [TestMethod]
        public void Test_MulUInt()
        {
            Assert.AreEqual(unchecked(uint.MinValue * 2), Contract.MulUInt(uint.MinValue, 2));
            Assert.AreEqual(unchecked(uint.MinValue * (-2)), Contract.MulUInt(uint.MinValue, -2));
            Assert.AreEqual(unchecked(uint.MaxValue * 2), Contract.MulUInt(uint.MaxValue, 2));
            Assert.AreEqual(unchecked(uint.MaxValue * (uint)(-2)), Contract.MulUInt(uint.MaxValue, -2));
            Assert.AreEqual(unchecked(uint.MinValue * uint.MaxValue), Contract.MulUInt(uint.MinValue, uint.MaxValue));
            Assert.AreEqual(unchecked(uint.MinValue * (-uint.MaxValue)), Contract.MulUInt(uint.MinValue, -uint.MaxValue));
            Assert.AreEqual(unchecked((-uint.MinValue) * uint.MaxValue), Contract.MulUInt(unchecked(-uint.MinValue), uint.MaxValue));
        }

        [TestMethod]
        public void Test_NegateChecked()
        {
            Assert.AreEqual(-2147483647, Contract.NegateIntChecked(int.MaxValue));
            Assert.AreEqual(-2147483647, Contract.NegateInt(int.MaxValue));

            // VMUnhandledException -int.MinValue
            Assert.ThrowsException<TestException>(() => Contract.NegateIntChecked(int.MinValue));

            Assert.AreEqual(-long.MaxValue, Contract.NegateLongChecked(long.MaxValue));
            Assert.AreEqual(-long.MaxValue, Contract.NegateLong(long.MaxValue));

            // VMUnhandledException -long.MinValue
            Assert.ThrowsException<TestException>(() => Contract.NegateLongChecked(long.MinValue));

            // -short -> int
            Assert.AreEqual(-32767, Contract.NegateShortChecked(32767));
            Assert.AreEqual(32768, Contract.NegateShort(short.MinValue));

            // unchecked(-int.MinValue) == int.MinValue
            Assert.AreEqual(int.MinValue, unchecked(-int.MinValue));

            // unchecked(-long.MinValue) == long.MinValue
            Assert.AreEqual(long.MinValue, unchecked(-long.MinValue));

            // it is different for short.MinValue, because `-short` is an int
            Assert.AreEqual(32768, unchecked(-short.MinValue));


            // add and negate
            Assert.AreEqual(-2147483648, Contract.NegateAddInt(int.MaxValue, 1));
            Assert.ThrowsException<TestException>(() => Contract.NegateAddIntChecked(int.MaxValue, 1));

            Assert.AreEqual(-9223372036854775808, Contract.NegateAddLong(long.MaxValue, 1));
            Assert.ThrowsException<TestException>(() => Contract.NegateAddLongChecked(long.MaxValue, 1));

            Assert.AreEqual(-2147483648, Contract.NegateAddInt(-2147483647, -1));
            Assert.ThrowsException<TestException>(() => Contract.NegateAddIntChecked(-2147483647, -1));

            Assert.AreEqual(-9223372036854775808, Contract.NegateAddLong(-9223372036854775807, -1));
            Assert.ThrowsException<TestException>(() => Contract.NegateAddLongChecked(-9223372036854775807, -1));
        }

        [TestMethod]
        public void Test_DivOverflow()
        {
            Assert.AreEqual(int.MaxValue, Contract.DivInt(int.MaxValue, 1));
            Assert.AreEqual(short.MaxValue, Contract.DivShort(short.MaxValue, 1));

            // VMUnhandledException int.MinValue / -1
            Assert.ThrowsException<TestException>(() => Contract.DivInt(int.MinValue, -1));

            // short / -1 -> int, so no overflow
            Assert.AreEqual(32768, Contract.DivShort(short.MinValue, -1));
        }
    }
}
