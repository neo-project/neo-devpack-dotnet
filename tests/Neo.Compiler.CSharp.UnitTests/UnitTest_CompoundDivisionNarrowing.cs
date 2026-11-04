// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CompoundDivisionNarrowing.cs file belongs to the neo project and is free
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
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_CompoundDivisionNarrowing
{
    [TestMethod]
    public void NarrowSignedDivisionRespectsCheckedContext()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""checkedSByte"")]
    public static int CheckedSByte()
    {
        checked { sbyte value = sbyte.MinValue; value /= -1; return value; }
    }

    [DisplayName(""uncheckedSByte"")]
    public static int UncheckedSByte()
    {
        unchecked { sbyte value = sbyte.MinValue; value /= -1; return value; }
    }

    [DisplayName(""checkedShort"")]
    public static int CheckedShort()
    {
        checked { short value = short.MinValue; value /= -1; return value; }
    }

    [DisplayName(""uncheckedShort"")]
    public static int UncheckedShort()
    {
        unchecked { short value = short.MinValue; value /= -1; return value; }
    }

    [DisplayName(""checkedNullableSByte"")]
    public static int? CheckedNullableSByte()
    {
        checked { sbyte? value = sbyte.MinValue; value /= -1; return value; }
    }

    [DisplayName(""uncheckedNullableShort"")]
    public static int? UncheckedNullableShort()
    {
        unchecked { short? value = short.MinValue; value /= -1; return value; }
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<DivisionContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.ThrowsExactly<TestException>(() => contract.CheckedSByte());
        Assert.ThrowsExactly<TestException>(() => contract.CheckedShort());
        Assert.AreEqual(new BigInteger(sbyte.MinValue), contract.UncheckedSByte());
        Assert.AreEqual(new BigInteger(short.MinValue), contract.UncheckedShort());
        Assert.ThrowsExactly<TestException>(() => contract.CheckedNullableSByte());
        Assert.AreEqual(new BigInteger(short.MinValue), contract.UncheckedNullableShort());
    }

    public abstract class DivisionContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("checkedSByte")]
        public abstract BigInteger? CheckedSByte();

        [DisplayName("uncheckedSByte")]
        public abstract BigInteger? UncheckedSByte();

        [DisplayName("checkedShort")]
        public abstract BigInteger? CheckedShort();

        [DisplayName("uncheckedShort")]
        public abstract BigInteger? UncheckedShort();

        [DisplayName("checkedNullableSByte")]
        public abstract BigInteger? CheckedNullableSByte();

        [DisplayName("uncheckedNullableShort")]
        public abstract BigInteger? UncheckedNullableShort();
    }
}
