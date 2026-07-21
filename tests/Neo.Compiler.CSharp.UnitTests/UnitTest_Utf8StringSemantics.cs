// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Utf8StringSemantics.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_Utf8StringSemantics
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("length")]
            public static int Length(string value) => value.Length;

            [DisplayName("first")]
            public static int First(string value) => value[0];
        }
        """;

    [TestMethod]
    public void NonAsciiStringOffsets_ShouldUseDocumentedUtf8Semantics()
    {
        Assert.AreEqual(1, "é".Length);
        Assert.AreEqual(2, "😀".Length);
        Assert.AreEqual(233, "é"[0]);
        Assert.AreEqual(55357, "😀"[0]);

        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(
            context.Success,
            string.Join(Environment.NewLine, context.Diagnostics.Select(static item => item.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<Utf8StringContract>(
            context.CreateExecutable(),
            context.CreateManifest());

        Assert.AreEqual(new BigInteger(2), contract.Length("é"));
        Assert.AreEqual(new BigInteger(4), contract.Length("😀"));
        Assert.AreEqual(new BigInteger(195), contract.First("é"));
        Assert.AreEqual(new BigInteger(240), contract.First("😀"));
    }

    public abstract class Utf8StringContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("length")]
        public abstract BigInteger? Length(string? value);

        [DisplayName("first")]
        public abstract BigInteger? First(string? value);
    }
}
