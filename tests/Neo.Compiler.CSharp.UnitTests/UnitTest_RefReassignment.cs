// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_RefReassignment
{
    [TestMethod]
    [DataRow("if (flag) alias = ref second;")]
    [DataRow("if (flag) { alias = ref second; } else { alias = ref first; }")]
    [DataRow("for (int i = 0; i < (flag ? 1 : 0); i++) alias = ref second;")]
    [DataRow("while (flag) { alias = ref second; break; }")]
    [DataRow("switch (flag) { case true: alias = ref second; break; }")]
    [DataRow("try { if (!flag) throw new Exception(); alias = ref second; } catch { }")]
    [DataRow("if (!flag) goto done; alias = ref second; done: ;")]
    [DataRow("do { alias = ref second; } while (flag);")]
    [DataRow("foreach (int value in new int[] { 1 }) { alias = ref second; }")]
    [DataRow("try { } finally { alias = ref second; }")]
    [DataRow("try { throw new Exception(); } catch { alias = ref second; }")]
    [DataRow("int value = flag ? (alias = ref second) : alias;")]
    public void RejectRebindingAcrossControlFlow(string statement)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.Basic, CompilationOptions.OptimizationType.All })
        {
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(Source(statement), options);
            Assert.IsFalse(context.Success, statement + " / " + optimization);
            Assert.IsTrue(context.Diagnostics.Any(d => d.Id == DiagnosticId.SyntaxNotSupported &&
                d.Severity == DiagnosticSeverity.Error && d.GetMessage().Contains("Ref reassignment is only supported")),
                string.Join(Environment.NewLine, context.Diagnostics));
        }
    }

    [TestMethod]
    [DataRow("alias = ref second;", 1099)]
    [DataRow("int value = (alias = ref second);", 1099)]
    [DataRow("checked { alias = ref second; }", 1099)]
    [DataRow("{ alias = ref second; }", 1099)]
    [DataRow("alias = ref second; alias = ref first;", 99007)]
    [DataRow("alias = ref alias;", 99007)]
    [DataRow("if (flag) second = 7; alias = ref second;", 1099)]
    [DataRow("int[] items = new int[] { 1 }; alias = ref items[0]; alias = ref alias; alias = 8; first = items[0];", 8007)]
    public void PreserveStraightLineRebinding(string statement, int expected)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.Basic, CompilationOptions.OptimizationType.All })
        {
            var options = TestHelper.CreateDefaultOptions();
            options.Optimize = optimization;
            var context = TestHelper.CompileSingleContract(Source(statement), options);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
            var engine = new TestEngine(true);
            var contract = engine.Deploy<BoundaryContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(expected), contract.Run(false), statement + " / " + optimization);
            Assert.AreEqual(new BigInteger(expected), contract.Run(true), statement + " / " + optimization);
        }
    }

    private static string Source(string statement) => $$"""
        using Neo.SmartContract.Framework;
        using System;
        using System.ComponentModel;
        public class Contract : SmartContract
        {
            [DisplayName("run")]
            public static int Run(bool flag)
            {
                int first = 1, second = 7;
                ref int alias = ref first;
                {{statement}}
                alias = 99;
                return first * 1000 + second;
            }
        }
        """;

    public abstract class BoundaryContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract BigInteger? Run(bool flag);
    }
}
