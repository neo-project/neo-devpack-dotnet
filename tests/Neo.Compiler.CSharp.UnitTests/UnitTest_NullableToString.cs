// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableToString.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NullableToString
{
    [TestMethod]
    public void NullableIntegerToString_ExecutesInVmAndLoadsReceiverOnce()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;
            using System.Numerics;

            public class Contract : SmartContract
            {
                [DisplayName("direct")]
                public static string Direct(BigInteger? value) => value.ToString();

                [DisplayName("withSuffix")]
                public static string WithSuffix(BigInteger? value) => value.ToString() + "|done";
            }
            """;

        var context = TestHelper.CompileSingleContract(source);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<NullableToStringContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("42", contract.Direct(42));
        Assert.AreEqual("", contract.Direct(null));
        Assert.AreEqual("42|done", contract.WithSuffix(42));
        Assert.AreEqual("|done", contract.WithSuffix(null));

        var methodBlock = ExtractMethodBlock(context.CreateAssembly(), "Contract.Direct(System.Numerics.BigInteger?)");
        var receiverLoads = methodBlock
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(line => line.Contains("LDARG 0", StringComparison.Ordinal));

        Assert.AreEqual(1, receiverLoads, $"Nullable<T>.ToString() should leave only the string result on the stack.\n{methodBlock}");
    }

    [TestMethod]
    public void NullableCharToString_UsesCharacterText()
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;

            public class Contract : SmartContract
            {
                [DisplayName("direct")]
                public static string Direct(char? value) => value.ToString();

                [DisplayName("withSuffix")]
                public static string WithSuffix(char? value) => value.ToString() + "|done";
            }
            """;

        var context = TestHelper.CompileSingleContract(source);

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<NullableCharToStringContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual("A", contract.Direct('A'));
        Assert.AreEqual("", contract.Direct(null));
        Assert.AreEqual("A|done", contract.WithSuffix('A'));
        Assert.AreEqual("|done", contract.WithSuffix(null));
    }

    private static string ExtractMethodBlock(string assembly, string methodSignature)
    {
        var normalized = assembly.Replace("\r\n", "\n", StringComparison.Ordinal);
        var marker = $"// {methodSignature}";
        var start = normalized.IndexOf(marker, StringComparison.Ordinal);
        Assert.IsTrue(start >= 0, $"Method section '{methodSignature}' was not found in generated assembly.\n{assembly}");

        var next = normalized.IndexOf("\n// ", start + marker.Length, StringComparison.Ordinal);
        if (next < 0)
            next = normalized.Length;

        return normalized[start..next];
    }

    public abstract class NullableToStringContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("direct")]
        public abstract string? Direct(BigInteger? value);

        [DisplayName("withSuffix")]
        public abstract string? WithSuffix(BigInteger? value);
    }

    public abstract class NullableCharToStringContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("direct")]
        public abstract string? Direct(char? value);

        [DisplayName("withSuffix")]
        public abstract string? WithSuffix(char? value);
    }
}
