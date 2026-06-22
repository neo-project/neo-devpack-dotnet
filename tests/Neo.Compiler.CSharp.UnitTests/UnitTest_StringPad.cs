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
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

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
    }
}
