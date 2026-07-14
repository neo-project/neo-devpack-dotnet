// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_UnicodeStringConstantEncoding.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_UnicodeStringConstantEncoding
{
    private const string Latin1 = "\u00E9";
    private const string Cjk = "\u4F60\u597D";
    private const string Mixed = "Neo-\u00E9-\u4F60\u597D-\U0001F642";
    private static readonly Lazy<(NefFile Nef, ContractManifest Manifest)> CompiledContract = new(CompileContract);

    [TestMethod]
    public void StringLiterals_MatchDotNetValues()
    {
        var contract = DeployContract();

        Assert.AreEqual(Latin1, contract.Latin1Literal());
        Assert.AreEqual(Cjk, contract.CjkLiteral());
        Assert.AreEqual(Mixed, contract.MixedLiteral());
    }

    [TestMethod]
    public void StringSearchAndEquality_MatchDotNetOrdinalResults()
    {
        var contract = DeployContract();

        AssertContainsMatchesDotNet(Latin1, $"before-{Latin1}-after", contract.ContainsLatin1);
        AssertContainsMatchesDotNet(Cjk, $"before-{Cjk}-after", contract.ContainsCjk);
        AssertContainsMatchesDotNet(Mixed, $"before-{Mixed}-after", contract.ContainsMixed);

        AssertEqualityMatchesDotNet(Latin1, Latin1, contract.EqualsLatin1);
        AssertEqualityMatchesDotNet(Cjk, Cjk, contract.EqualsCjk);
        AssertEqualityMatchesDotNet(Mixed, Mixed, contract.EqualsMixed);

        AssertContainsMatchesDotNet(Latin1, "missing", contract.ContainsLatin1);
        AssertEqualityMatchesDotNet(Mixed, "different", contract.EqualsMixed);
    }

    [TestMethod]
    public void ByteStringConstants_PreserveRawBytesAcrossContexts()
    {
        var contract = DeployContract();
        byte[] expected = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

        CollectionAssert.AreEqual(expected, contract.RawByteStringReturn());
        CollectionAssert.AreEqual(expected, contract.RawByteStringLocal());
        CollectionAssert.AreEqual(expected, contract.RawByteStringArgument());
        CollectionAssert.AreEqual(expected, contract.RawByteStringExplicit());
        CollectionAssert.AreEqual(expected, contract.RawByteStringParenthesizedCast());
        CollectionAssert.AreEqual(new byte[] { 0xE9 }, contract.RawLatin1ByteString());
    }

    private static void AssertContainsMatchesDotNet(string search, string value, Func<string, bool?> neoVmContains)
    {
        bool expected = value.Contains(search, StringComparison.Ordinal);
        Assert.AreEqual(expected, neoVmContains(value));
    }

    private static void AssertEqualityMatchesDotNet(string expectedValue, string value, Func<string, bool?> neoVmEquals)
    {
        bool expected = string.Equals(value, expectedValue, StringComparison.Ordinal);
        Assert.AreEqual(expected, neoVmEquals(value));
    }

    private static UnicodeStringConstantContract DeployContract()
    {
        var (nef, manifest) = CompiledContract.Value;
        var engine = new TestEngine(true);
        return engine.Deploy<UnicodeStringConstantContract>(nef, manifest);
    }

    private static (NefFile Nef, ContractManifest Manifest) CompileContract()
    {
        string path = Path.Combine(
            SyntaxProbeLoader.GetRepositoryRoot(),
            "tests",
            "Neo.Compiler.CSharp.UnitTests",
            "TestSources",
            "Contract_UnicodeStringConstantEncoding.cs.txt");
        var context = TestHelper.CompileSingleContract(File.ReadAllText(path));

        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        return (context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class UnicodeStringConstantContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("latin1Literal")]
        public abstract string? Latin1Literal();

        [DisplayName("cjkLiteral")]
        public abstract string? CjkLiteral();

        [DisplayName("mixedLiteral")]
        public abstract string? MixedLiteral();

        [DisplayName("containsLatin1")]
        public abstract bool? ContainsLatin1(string value);

        [DisplayName("containsCjk")]
        public abstract bool? ContainsCjk(string value);

        [DisplayName("containsMixed")]
        public abstract bool? ContainsMixed(string value);

        [DisplayName("equalsLatin1")]
        public abstract bool? EqualsLatin1(string value);

        [DisplayName("equalsCjk")]
        public abstract bool? EqualsCjk(string value);

        [DisplayName("equalsMixed")]
        public abstract bool? EqualsMixed(string value);

        [DisplayName("rawByteStringReturn")]
        public abstract byte[]? RawByteStringReturn();

        [DisplayName("rawByteStringLocal")]
        public abstract byte[]? RawByteStringLocal();

        [DisplayName("rawByteStringArgument")]
        public abstract byte[]? RawByteStringArgument();

        [DisplayName("rawByteStringExplicit")]
        public abstract byte[]? RawByteStringExplicit();

        [DisplayName("rawByteStringParenthesizedCast")]
        public abstract byte[]? RawByteStringParenthesizedCast();

        [DisplayName("rawLatin1ByteString")]
        public abstract byte[]? RawLatin1ByteString();
    }
}
