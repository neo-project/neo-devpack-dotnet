// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringSliceSearchMatrix.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringSliceSearchMatrix
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class Contract : SmartContract
        {
            [DisplayName("substring")]
            public static string Substring(string value, int start, int length) => value.Substring(start, length);

            [DisplayName("substringToEnd")]
            public static string SubstringToEnd(string value, int start) => value.Substring(start);

            [DisplayName("indexOf")]
            public static int IndexOf(string value, string search) => value.IndexOf(search);

            [DisplayName("lastIndexOf")]
            public static int LastIndexOf(string value, string search) => value.LastIndexOf(search);
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SubstringAndSearchBoundariesMatchCSharp(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join(Environment.NewLine, context.Diagnostics)}");

        var contract = new TestEngine(true).Deploy<StringSliceSearchContract>(
            context.CreateExecutable(), context.CreateManifest());

        foreach (var (value, start, length) in new[]
        {
            ("", 0, 0),
            ("neo", 0, 3),
            ("neo", 1, 0),
            ("neo", 1, 2),
            ("neo compiler", 5, 7),
            ("aaaa", 1, 2)
        })
        {
            Assert.AreEqual(value.Substring(start, length), contract.Substring(value, start, length),
                $"Substring({value}, {start}, {length}), {optimization}");
        }

        foreach (var (value, start) in new[]
        {
            ("", 0),
            ("neo", 0),
            ("neo", 1),
            ("neo compiler", 5),
            ("aaaa", 4)
        })
        {
            Assert.AreEqual(value.Substring(start), contract.SubstringToEnd(value, start),
                $"SubstringToEnd({value}, {start}), {optimization}");
        }

        foreach (var (value, search) in new[]
        {
            ("", ""),
            ("", "a"),
            ("aaaa", "aa"),
            ("aaaa", "aaa"),
            ("neo compiler", "compiler"),
            ("neo compiler", "compiler"),
            ("neo compiler", "x")
        })
        {
            Assert.AreEqual(value.IndexOf(search), contract.IndexOf(value, search),
                $"IndexOf({value}, {search}), {optimization}");
            Assert.AreEqual(value.LastIndexOf(search), contract.LastIndexOf(value, search),
                $"LastIndexOf({value}, {search}), {optimization}");
        }
    }

    public abstract class StringSliceSearchContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("substring")] public abstract string? Substring(string value, int start, int length);
        [DisplayName("substringToEnd")] public abstract string? SubstringToEnd(string value, int start);
        [DisplayName("indexOf")] public abstract BigInteger? IndexOf(string value, string search);
        [DisplayName("lastIndexOf")] public abstract BigInteger? LastIndexOf(string value, string search);
    }
}
