// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Shift.cs file belongs to the neo project and is free
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
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_Shift : DebugAndTestBase<Contract_shift>
{
    [TestMethod]
    public void Test_Shift()
    {
        var list = Contract.TestShift()?.Cast<BigInteger>().ToArray();
        AssertGasConsumed(1048740);
        CollectionAssert.AreEqual(new BigInteger[] { 16, 4 }, list);
    }

    [TestMethod]
    public void Test_Shift_BigInteger()
    {
        var list = Contract.TestShiftBigInt()?.Cast<BigInteger>().ToArray();
        AssertGasConsumed(1049370);
        CollectionAssert.AreEqual(new BigInteger[] { 8, 16, 4, 2 }, list);
    }

    [TestMethod]
    public void Test_ShiftRightChecked()
    {
        Assert.AreEqual(0, Contract.ShiftRightChecked(8, 4));
        AssertGasConsumed(1047480);
    }

    [TestMethod]
    public void Test_ShiftRightUnchecked()
    {
        Assert.AreEqual(0, Contract.ShiftRightUnchecked(8, 4));
        AssertGasConsumed(1047480);
    }

    [TestMethod]
    public void Test_ShiftLeftCheckedByte()
    {
        Assert.AreEqual(128, Contract.ShiftLeftCheckedByte(8, 4));
        AssertGasConsumed(1047930);

        Assert.ThrowsException<TestException>(() => Contract.ShiftLeftCheckedByte(8, 33));
        AssertGasConsumed(1062780);
    }

    [TestMethod]
    public void Test_ShiftLeftCheckedShort()
    {
        Assert.AreEqual(128, Contract.ShiftLeftCheckedShort(8, 4));
        AssertGasConsumed(1047930);

        Assert.ThrowsException<TestException>(() => Contract.ShiftLeftCheckedShort(8, 33));
        AssertGasConsumed(1062780);
    }
}
