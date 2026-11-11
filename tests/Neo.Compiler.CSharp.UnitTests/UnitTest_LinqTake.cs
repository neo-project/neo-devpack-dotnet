// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_LinqTake.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using VMArray = Neo.VM.Types.Array;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_LinqTake
{
    // Snapshot of Take before the optimization; the other contract calls the current framework.
    private const string BaselineSource = """
        public static class BaselineExtensions
        {
            private static void AssertSourceNotNull<T>(this IEnumerable<T> source)
            {
                if (source == null) throw new ArgumentNullException("source is null");
            }

            public static IEnumerable<T> Take<T>(this IEnumerable<T> source, BigInteger count)
            {
                AssertSourceNotNull(source);
                var list = new Neo.SmartContract.Framework.List<T>();
                foreach (var i in source)
                {
                    if (count <= 0) break;
                    list.Add(i);
                    count--;
                }
                return list;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void TakePreservesResultsAndReducesGas(CompilationOptions.OptimizationType optimization)
    {
        var baselineContext = Compile(true, optimization);
        var optimizedContext = Compile(false, optimization);
        var baselineEngine = new TestEngine(true);
        var optimizedEngine = new TestEngine(true);
        var baseline = baselineEngine.Deploy<TakeContract>(baselineContext.CreateExecutable(), baselineContext.CreateManifest());
        var optimized = optimizedEngine.Deploy<TakeContract>(optimizedContext.CreateExecutable(), optimizedContext.CreateManifest());

        var baselineOpCodes = GetTakeOpCodes(baselineContext);
        var optimizedOpCodes = GetTakeOpCodes(optimizedContext);
        Assert.IsFalse(baselineOpCodes.SequenceEqual(optimizedOpCodes), "The compiler must not erase the source-level difference.");
        Assert.IsTrue(System.Array.IndexOf(baselineOpCodes, OpCode.LE) > System.Array.IndexOf(baselineOpCodes, OpCode.PICKITEM));
        Assert.IsTrue(System.Array.IndexOf(optimizedOpCodes, OpCode.LE) < System.Array.IndexOf(optimizedOpCodes, OpCode.SIZE));
        CollectionAssert.Contains(optimizedOpCodes, OpCode.JMPIF);
        Console.WriteLine($"Optimization={optimization}");
        Console.WriteLine($"Original Take ({baselineOpCodes.Length} instructions): {string.Join(' ', baselineOpCodes)}");
        Console.WriteLine($"Optimized Take ({optimizedOpCodes.Length} instructions): {string.Join(' ', optimizedOpCodes)}");
        Console.WriteLine($"NEF script bytes: {baselineContext.CreateExecutable().Script.Length} -> {optimizedContext.CreateExecutable().Script.Length}");

        var values = new object[] { 7, -1, 7, 0, -100, 5 };
        BigInteger[] counts = [-(BigInteger.One << 200), -1, 0, 1, 2, 5, 6, 10, BigInteger.One << 200];
        foreach (var source in new[] { System.Array.Empty<object>(), values })
        {
            foreach (var count in counts)
            {
                var before = (VMArray)baseline.Take(source, count)!;
                long originalGas = baselineEngine.FeeConsumed.Value;
                var after = (VMArray)optimized.Take(source, count)!;
                long optimizedGas = optimizedEngine.FeeConsumed.Value;
                int expectedCount = count <= 0 ? 0 : count < source.Length ? (int)count : source.Length;
                Assert.AreEqual(expectedCount, before.Count);
                Assert.AreEqual(expectedCount, after.Count);
                for (int i = 0; i < expectedCount; i++)
                {
                    var expected = new BigInteger((int)source[i]);
                    Assert.AreEqual(expected, before[i].GetInteger());
                    Assert.AreEqual(expected, after[i].GetInteger());
                }

                Console.WriteLine($"length={source.Length}, count={count}: {originalGas} -> {optimizedGas}, saving={originalGas - optimizedGas} datoshi");
                if (source.Length > 2 && count >= 0 && count <= 2)
                {
                    Assert.IsTrue(optimizedGas < originalGas, $"Take({count}) must improve GAS with {optimization} optimization.");
                    long[] originalExpected = optimization == CompilationOptions.OptimizationType.All
                        ? [1148430, 1397580, 1646730] : [1148610, 1397850, 1647090];
                    long[] optimizedExpected = optimization == CompilationOptions.OptimizationType.All
                        ? [1145580, 1394670, 1643430] : [1145700, 1394880, 1643730];
                    Assert.AreEqual(originalExpected[(int)count], originalGas);
                    Assert.AreEqual(optimizedExpected[(int)count], optimizedGas);
                }
                else if (source.Length == 0 && count > 0)
                {
                    // The early count guard adds a bounded cost when the loop would be empty.
                    long expectedOverhead = optimization == CompilationOptions.OptimizationType.All ? 390 : 420;
                    Assert.AreEqual(expectedOverhead, optimizedGas - originalGas);
                }
                else
                {
                    Assert.IsTrue(optimizedGas < originalGas, "Other measured paths must also improve GAS.");
                }
            }
        }

        foreach (var count in counts)
        {
            var before = Assert.ThrowsExactly<TestException>(() => baseline.Take(null, count));
            var after = Assert.ThrowsExactly<TestException>(() => optimized.Take(null, count));
            Assert.AreEqual("An unhandled exception was thrown. source is null", before.InnerException?.Message);
            Assert.AreEqual(before.InnerException?.Message, after.InnerException?.Message);
        }
    }

    private static CompilationContext Compile(bool baseline, CompilationOptions.OptimizationType optimization)
    {
        var context = TestHelper.CompileSingleContract($$"""
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Linq;
            using System;
            using System.Collections.Generic;
            using System.Numerics;

            public class Contract : SmartContract
            {
                public static object Take(int[] source, BigInteger count)
                {
                    return {{(baseline ? "BaselineExtensions.Take(source, count)" : "source.Take(count)")}};
                }
            }

            {{(baseline ? BaselineSource : "")}}
            """, new CompilationOptions
        {
            Optimize = optimization,
            Debug = CompilationOptions.DebugType.Extended,
            CompilerVersion = "TestingEngine"
        });
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        return context;
    }

    private static OpCode[] GetTakeOpCodes(CompilationContext context)
    {
        var methods = (JArray)context.CreateDebugInformation()["methods"]!;
        var method = methods.OfType<JObject>().Single(m =>
            m["id"]!.GetString().Contains("Extensions.Take", StringComparison.Ordinal));
        var range = method["range"]!.GetString().Split('-');
        int start = int.Parse(range[0]);
        int end = int.Parse(range[1]);
        return ((Script)context.CreateExecutable().Script).EnumerateInstructions()
            .Where(i => i.address >= start && i.address <= end)
            .Select(i => i.instruction.OpCode).ToArray();
    }

    public abstract class TakeContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("take")]
        public abstract object? Take(IList<object>? source, BigInteger count);
    }
}
