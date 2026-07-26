// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringCasingUtf8.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringCasingUtf8
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("lower")]
            public static string Lower(string value) => value.ToLower();

            [DisplayName("upper")]
            public static string Upper(string value) => value.ToUpper();
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CasingPreservesUtf8BytesOutsideAsciiLetters(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StringCasingContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("abc \u00c9\u03a9\u4e2d\ud83d\ude00 xyz", contract.Lower("ABC \u00c9\u03a9\u4e2d\ud83d\ude00 XYZ"));
        Assert.AreEqual("ABC \u00e9\u03c9\u4e2d\ud83d\ude00 XYZ", contract.Upper("abc \u00e9\u03c9\u4e2d\ud83d\ude00 xyz"));
        Assert.AreEqual("a\0z", contract.Lower("A\0Z"));
        Assert.AreEqual("A\0Z", contract.Upper("a\0z"));
    }

    public abstract class StringCasingContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("lower")]
        public abstract string? Lower(string value);

        [DisplayName("upper")]
        public abstract string? Upper(string value);
    }
}
