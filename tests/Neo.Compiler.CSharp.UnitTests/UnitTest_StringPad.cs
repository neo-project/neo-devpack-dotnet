// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringPad.cs file belongs to the neo project and is free
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
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringPad
{
    [TestMethod]
    public void PadLeftAndPadRight_MatchDotNet()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""padLeft"")]
    public static string PadLeft(string s, int width) => s.PadLeft(width);

    [DisplayName(""padLeftZero"")]
    public static string PadLeftZero(string s, int width) => s.PadLeft(width, '0');

    [DisplayName(""padRight"")]
    public static string PadRight(string s, int width) => s.PadRight(width);

    [DisplayName(""padRightStar"")]
    public static string PadRightStar(string s, int width) => s.PadRight(width, '*');

    [DisplayName(""padLeftNul"")]
    public static string PadLeftNul(string s, int width) => s.PadLeft(width, '\0');

    [DisplayName(""padRightMid"")]
    public static string PadRightMid(string s) => s.PadRight(7, (char)0x0100);

    [DisplayName(""padLeftAccent"")]
    public static string PadLeftAccent(string s, int width) => s.PadLeft(width, 'é');

    [DisplayName(""padRightAccent"")]
    public static string PadRightAccent(string s, int width) => s.PadRight(width, 'é');

    [DisplayName(""padLeftWide"")]
    public static string PadLeftWide(string s, int width) => s.PadLeft(width, '*');
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StringPadContract>(context.CreateExecutable(), context.CreateManifest());

        // Default space padding.
        Assert.AreEqual("abc".PadLeft(6), contract.PadLeft("abc", 6));
        Assert.AreEqual("abc".PadRight(6), contract.PadRight("abc", 6));

        // Exact width and already-longer: returned unchanged (no truncation).
        Assert.AreEqual("abc".PadLeft(3), contract.PadLeft("abc", 3));
        Assert.AreEqual("abc".PadLeft(1), contract.PadLeft("abc", 1));
        Assert.AreEqual("abc".PadRight(1), contract.PadRight("abc", 1));

        // Custom padding character.
        Assert.AreEqual("42".PadLeft(5, '0'), contract.PadLeftZero("42", 5));
        Assert.AreEqual("42".PadRight(5, '*'), contract.PadRightStar("42", 5));

        // Empty input.
        Assert.AreEqual("".PadLeft(3), contract.PadLeft("", 3));

        // A NUL char pads with one 0x00 byte per repetition, and every other fill char uses its
        // UTF-8 encoding, matching .NET for both.
        Assert.AreEqual("ab".PadLeft(5, '\0'), contract.PadLeftNul("ab", 5));
        Assert.AreEqual("ab".PadRight(7, (char)0x0100), contract.PadRightMid("ab"));
        Assert.AreEqual("abc".PadLeft(5, 'é'), contract.PadLeftAccent("abc", 5));
        Assert.AreEqual("abc".PadRight(5, 'é'), contract.PadRightAccent("abc", 5));

        // Wide padding needs only logarithmically many concatenations.
        Assert.AreEqual("abc".PadLeft(20, '*'), contract.PadLeftWide("abc", 20));

        // A call that needs no padding returns the string directly, without CAT/CONVERT.
        using (var watcher = engine.CreateFeeWatcher())
        {
            Assert.AreEqual("abc", contract.PadLeft("abc", 3));
            Assert.AreEqual(1048050, watcher.Value);
        }
    }

    public abstract class StringPadContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("padLeft")]
        public abstract string? PadLeft(string s, BigInteger width);

        [DisplayName("padLeftZero")]
        public abstract string? PadLeftZero(string s, BigInteger width);

        [DisplayName("padRight")]
        public abstract string? PadRight(string s, BigInteger width);

        [DisplayName("padRightStar")]
        public abstract string? PadRightStar(string s, BigInteger width);

        [DisplayName("padLeftNul")]
        public abstract string? PadLeftNul(string s, BigInteger width);

        [DisplayName("padRightMid")]
        public abstract string? PadRightMid(string s);

        [DisplayName("padLeftAccent")]
        public abstract string? PadLeftAccent(string s, BigInteger width);

        [DisplayName("padRightAccent")]
        public abstract string? PadRightAccent(string s, BigInteger width);

        [DisplayName("padLeftWide")]
        public abstract string? PadLeftWide(string s, BigInteger width);
    }
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void RuntimeFillsPreserveUtf8Boundaries(CompilationOptions.OptimizationType level)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            using System.ComponentModel;
            public class Contract : SmartContract
            {
                [DisplayName("left")]
                public static string Left(string s, int width, char fill) => s.PadLeft(width, fill);
                [DisplayName("right")]
                public static string Right(string s, int width, char fill) => s.PadRight(width, fill);
            }
            """;
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = level;
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<PaddingContract>(context.CreateExecutable(), context.CreateManifest());
        foreach (char fill in new[] { '\0', '\x7f', '\x80', '\u07ff', '\u0800', '\ud7ff', '\ud800', '\udfff', '\ue000', '\uffff' })
            foreach (string input in new[] { "", "ab", "\u00e9" })
                foreach (int width in new[] { 0, 1, 4, 9 })
                {
                    var repeat = Math.Max(0, width - Encoding.UTF8.GetByteCount(input));
                    var padding = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(new string(fill, repeat)));
                    var label = $"{level}, U+{(int)fill:X4}, {input}, {width}";
                    Assert.AreEqual(padding + input, contract.Left(input, width, (int)fill), label);
                    Assert.AreEqual(input + padding, contract.Right(input, width, (int)fill), label);
                }
        Assert.ThrowsExactly<TestException>(() => contract.Left(null, 4, 0x80));
        Assert.ThrowsExactly<TestException>(() => contract.Right(null, 4, 0x800));
    }

    public abstract class PaddingContract(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("left")]
        public abstract string? Left(string? input, BigInteger width, BigInteger fill);
        [DisplayName("right")]
        public abstract string? Right(string? input, BigInteger width, BigInteger fill);
    }
}
