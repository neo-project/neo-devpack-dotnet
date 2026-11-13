// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CheckedUnchecked.cs file belongs to the neo project and is free
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
    public class UnitTest_CheckedUnchecked : DebugAndTestBase<Contract_CheckedUnchecked>
    {
        [TestMethod]
        public void TestAddChecked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.AddChecked(int.MaxValue, 1));
        }

        [TestMethod]
        public void TestAddUnchecked()
        {
            Assert.AreEqual(int.MinValue, Contract.AddUnchecked(int.MaxValue, 1));
        }

        [TestMethod]
        public void TestCastChecked()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.CastChecked(-1));

            Assert.ThrowsExactly<TestException>(() => Contract.CastChecked(int.MinValue));

            Assert.AreEqual(2147483647, Contract.CastChecked(int.MaxValue));

            Assert.AreEqual(0, Contract.CastChecked(ulong.MinValue));

            Assert.ThrowsExactly<TestException>(() => Contract.CastChecked(ulong.MaxValue));

            Assert.ThrowsExactly<TestException>(() => Contract.CastChecked(long.MinValue));

            Assert.ThrowsExactly<TestException>(() => Contract.CastChecked(long.MaxValue));
        }

        [TestMethod]
        public void TestCastUnchecked()
        {
            Assert.AreEqual(uint.MaxValue, Contract.CastUnchecked(-1));

            Assert.AreEqual(2147483648, Contract.CastUnchecked(int.MinValue));

            Assert.AreEqual(2147483647, Contract.CastUnchecked(int.MaxValue));

            Assert.AreEqual(0, Contract.CastUnchecked(ulong.MinValue));

            Assert.AreEqual(4294967295, Contract.CastUnchecked(ulong.MaxValue));

            Assert.AreEqual(0, Contract.CastUnchecked(long.MinValue));

            Assert.AreEqual(4294967295, Contract.CastUnchecked(long.MaxValue));
        }
    }
}
