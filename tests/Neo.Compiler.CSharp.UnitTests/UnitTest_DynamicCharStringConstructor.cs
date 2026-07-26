// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DynamicCharStringConstructor.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DynamicCharStringConstructor
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;
        using System.Text;

        public class Contract : SmartContract
        {
            [DisplayName("repeat")]
            public static string Repeat(int value) => new string((char)value, 2);

            [DisplayName("toString")]
            public static string ToString(int value) => ((char)value).ToString();

            [DisplayName("append")]
            public static string Append(int value) => new StringBuilder("neo-").Append((char)value).ToString();
        }
        """;

    [DataTestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void DynamicCharConstructorEncodesUtf8(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<DynamicCharStringContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("AA", contract.Repeat('A'), optimization.ToString());
        Assert.AreEqual("éé", contract.Repeat('é'), optimization.ToString());
        Assert.AreEqual("ΩΩ", contract.Repeat('Ω'), optimization.ToString());
        Assert.AreEqual("中中", contract.Repeat('中'), optimization.ToString());
        Assert.AreEqual("\0\0", contract.Repeat('\0'), optimization.ToString());
        Assert.AreEqual("é", contract.ToString('é'), optimization.ToString());
        Assert.AreEqual("Ω", contract.ToString('Ω'), optimization.ToString());
        Assert.AreEqual("中", contract.ToString('中'), optimization.ToString());
        Assert.AreEqual("neo-é", contract.Append('é'), optimization.ToString());
        Assert.AreEqual("neo-Ω", contract.Append('Ω'), optimization.ToString());
        Assert.AreEqual("neo-中", contract.Append('中'), optimization.ToString());
        Assert.AreEqual("\u007f", contract.ToString('\u007f'), optimization.ToString());
        Assert.AreEqual("\u0080", contract.ToString('\u0080'), optimization.ToString());
        Assert.AreEqual("\u07ff", contract.ToString('\u07ff'), optimization.ToString());
        Assert.AreEqual("\u0800", contract.ToString('\u0800'), optimization.ToString());
        Assert.AreEqual("\ud7ff", contract.ToString('\ud7ff'), optimization.ToString());
        Assert.AreEqual("\ue000", contract.ToString('\ue000'), optimization.ToString());
        Assert.AreEqual("\uffff", contract.ToString('\uffff'), optimization.ToString());
        Assert.AreEqual("\ufffd", contract.ToString('\ud800'), optimization.ToString());
    }

    public abstract class DynamicCharStringContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("repeat")]
        public abstract string? Repeat(BigInteger value);

        [DisplayName("toString")]
        public abstract string? ToString(BigInteger value);

        [DisplayName("append")]
        public abstract string? Append(BigInteger value);
    }
}
