// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ConditionalIndexer
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;
        public class Contract : SmartContract
        {
            private static int _trace;
            private sealed class Values
            {
                public int this[int index] { get { _trace = _trace * 10 + 4; return 30 + index; } }
                public int this[int first, int second] { get { _trace = _trace * 10 + 4; return first * 10 + second; } }
            }
            private static Values? Receiver(bool present)
            {
                _trace = _trace * 10 + 1;
                return present ? new Values() : null;
            }
            private static int Argument(int value) { _trace = _trace * 10 + value; return value; }
            [DisplayName("read")]
            public static int[] Read(bool present)
            {
                _trace = 0;
                int value = Receiver(present)?[INDEX_ARGUMENTS] ?? -1;
                return new[] { value, _trace };
            }
            [DisplayName("readArray")]
            public static int? ReadArray(bool present)
            {
                int[]? values = present ? new[] { 7, 8 } : null;
                return values?[1];
            }
            [DisplayName("readString")]
            public static char? ReadString(bool present)
            {
                string? value = present ? "ab" : null;
                return value?[1];
            }
            [DisplayName("readBytes")]
            public static byte? ReadBytes(bool present)
            {
                ByteString? value = present ? (ByteString)new byte[] { 7, 8 } : null;
                return value?[1];
            }
            [DisplayName("readList")]
            public static int? ReadList(bool present)
            {
                List<int>? value = present ? new List<int>() { 7, 8 } : null;
                return value?[1];
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, false)]
    [DataRow(CompilationOptions.OptimizationType.All, false)]
    [DataRow(CompilationOptions.OptimizationType.None, true)]
    [DataRow(CompilationOptions.OptimizationType.All, true)]
    public void ConditionalIndexersCallGettersAndPreserveEvaluationOrder(CompilationOptions.OptimizationType optimization, bool multiple)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source.Replace("INDEX_ARGUMENTS", multiple ? "Argument(2), Argument(3)" : "Argument(2)"), options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<IndexerContract>(context.CreateExecutable(), context.CreateManifest());

        CollectionAssert.AreEqual(new[] { multiple ? 23 : 32, multiple ? 1234 : 124 }, contract.Read(true));
        CollectionAssert.AreEqual(new[] { -1, 1 }, contract.Read(false));
        Assert.AreEqual(new BigInteger(8), contract.ReadArray(true));
        Assert.IsNull(contract.ReadArray(false));
        Assert.AreEqual(new BigInteger('b'), contract.ReadString(true));
        Assert.IsNull(contract.ReadString(false));
        Assert.AreEqual(new BigInteger(8), contract.ReadBytes(true));
        Assert.IsNull(contract.ReadBytes(false));
        Assert.AreEqual(new BigInteger(8), contract.ReadList(true));
        Assert.IsNull(contract.ReadList(false));
    }

    public abstract class IndexerContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("read")] public abstract int[] Read(bool present);
        [DisplayName("readArray")] public abstract BigInteger? ReadArray(bool present);
        [DisplayName("readString")] public abstract BigInteger? ReadString(bool present);
        [DisplayName("readBytes")] public abstract BigInteger? ReadBytes(bool present);
        [DisplayName("readList")] public abstract BigInteger? ReadList(bool present);
    }
}
