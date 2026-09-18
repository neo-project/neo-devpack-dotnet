// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NonAsciiCharSearch.cs file belongs to the neo project and is free
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
public class UnitTest_NonAsciiCharSearch
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("contains")]
            public static bool Contains(string value, char c) => value.Contains(c);

            [DisplayName("indexOf")]
            public static int IndexOf(string value, char c) => value.IndexOf(c);
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NonAsciiChar_IsSearchedAsUtf8(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract>(context.CreateExecutable(), context.CreateManifest());

        // "café" ends with é (U+00E9 -> UTF-8 C3 A9).
        Assert.IsTrue(contract.Contains("café", 'é'));
        Assert.AreEqual(new BigInteger(3), contract.IndexOf("café", 'é'));

        // Chinese character 中 (U+4E2D -> UTF-8 E4 B8 AD).
        Assert.IsTrue(contract.Contains("中文测试", '中'));
        Assert.AreEqual(new BigInteger(0), contract.IndexOf("中文测试", '中'));

        // 试 is the 4th char, whose first UTF-8 byte sits at byte offset 9.
        Assert.AreEqual(new BigInteger(9), contract.IndexOf("中文测试", '试'));
    }

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void AsciiAndNulChar_SearchStillWorks(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<Contract>(context.CreateExecutable(), context.CreateManifest());

        Assert.IsTrue(contract.Contains("hello world", 'o'));
        Assert.AreEqual(new BigInteger(4), contract.IndexOf("hello world", 'o'));
        Assert.IsFalse(contract.Contains("hello world", 'x'));
        Assert.AreEqual(new BigInteger(-1), contract.IndexOf("hello world", 'x'));

        // NUL char must still match a single trailing 0x00 byte.
        Assert.IsTrue(contract.Contains("a\0b", '\0'));
        Assert.AreEqual(new BigInteger(1), contract.IndexOf("a\0b", '\0'));
    }

    public abstract class Contract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("contains")]
        public abstract bool Contains(string value, char c);

        [DisplayName("indexOf")]
        public abstract int IndexOf(string value, char c);
    }
}
