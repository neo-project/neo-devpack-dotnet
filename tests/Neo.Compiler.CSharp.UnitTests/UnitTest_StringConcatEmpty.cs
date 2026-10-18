// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StringConcatEmpty.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StringConcatEmpty
{
    private static readonly string?[] Inputs = [null, "", "Neo", "\u4F60\u597DNeo"];

    private static readonly CompilationOptions.OptimizationType[] Levels =
    [
        CompilationOptions.OptimizationType.None,
        CompilationOptions.OptimizationType.Basic,
        CompilationOptions.OptimizationType.All
    ];

    private static string CreateSource(string body) => $@"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{{
    private static string Next(ref int counter)
    {{
        counter++;
        return ""Neo"";
    }}

    [DisplayName(""run"")]
    public static string? Run(string? v)
    {{
        {body}
    }}
}}";

    private static (StringConcatContract Contract, TestEngine Engine, OpCode[] OpCodes, int ScriptLength) Deploy(string body, CompilationOptions.OptimizationType level)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = level;
        var context = TestHelper.CompileSingleContract(CreateSource(body), options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var nef = context.CreateExecutable();
        var script = new Script(nef.Script);
        var opCodes = new System.Collections.Generic.List<OpCode>();
        for (int ip = 0; ip < script.Length;)
        {
            var instruction = script.GetInstruction(ip);
            opCodes.Add(instruction.OpCode);
            ip += instruction.Size;
        }

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StringConcatContract>(nef, context.CreateManifest());
        return (contract, engine, opCodes.ToArray(), nef.Script.Length);
    }

    private static void AssertScenario(string body, Func<string?, string> expected, bool optimized)
    {
        foreach (var level in Levels)
        {
            var (contract, _, opCodes, _) = Deploy(body, level);
            foreach (var input in Inputs)
                Assert.AreEqual(expected(input), contract.Run(input), $"{body} / {level} / {input}");

            if (level == CompilationOptions.OptimizationType.All)
            {
                if (optimized)
                {
                    Assert.IsFalse(opCodes.Contains(OpCode.CAT), $"Unexpected CAT: {body}");
                    Assert.IsFalse(opCodes.Contains(OpCode.CONVERT), $"Unexpected CONVERT: {body}");
                }
                else
                {
                    Assert.IsTrue(opCodes.Contains(OpCode.CAT), $"Expected CAT: {body}");
                }
            }
        }
    }

    [TestMethod]
    public void EmptyRight() => AssertScenario("return string.Concat(v, \"\");", v => v ?? "", true);

    [TestMethod]
    public void EmptyLeft() => AssertScenario("return string.Concat(\"\", v);", v => v ?? "", true);

    [TestMethod]
    public void EmptyConstRight() => AssertScenario("const string e = \"\"; return string.Concat(v, e);", v => v ?? "", true);

    [TestMethod]
    public void EmptyConstLeft() => AssertScenario("const string e = \"\"; return string.Concat(e, v);", v => v ?? "", true);

    [TestMethod]
    public void EmptyNamedArguments() => AssertScenario("return string.Concat(str1: \"\", str0: v);", v => v ?? "", true);

    [TestMethod]
    public void EmptyNamedArgumentsRight() => AssertScenario("return string.Concat(str1: v, str0: \"\");", v => v ?? "", true);

    [TestMethod]
    public void BothEmpty() => AssertScenario("return string.Concat(\"\", \"\");", _ => "", true);

    [TestMethod]
    public void NullConstantWithEmpty() => AssertScenario("return string.Concat((string?)null, \"\");", _ => "", true);

    [TestMethod]
    public void NonEmptyStillConcatenates()
    {
        AssertScenario("return string.Concat(v, \"N3\");", v => v + "N3", false);
        AssertScenario("return string.Concat(\"Neo\", v);", v => "Neo" + v, false);
        AssertScenario("return string.Concat(v, v);", v => v + v, false);
    }

    [TestMethod]
    public void EmptyRuntimeExpressionIsNotAssumedEmpty()
    {
        AssertScenario("var e = v == null ? \"\" : \"x\"; return string.Concat(v, e);", v => v is null ? "" : v + "x", false);
    }

    [TestMethod]
    public void SideEffectsEvaluatedOnceInOrder()
    {
        AssertScenario("int c = 0; var r = string.Concat(Next(ref c), \"\"); return c + \":\" + r;", _ => "1:Neo", false);
        AssertScenario("int c = 0; var r = string.Concat(\"\", Next(ref c)); return c + \":\" + r;", _ => "1:Neo", false);
        AssertScenario("int c = 0; var r = string.Concat(c++ == 0 ? v : null, \"\"); return c + \":\" + r;", v => "1:" + (v ?? ""), false);
    }

    [TestMethod]
    public void ThrowingExpressionStillThrows()
    {
        foreach (var level in Levels)
        {
            var (contract, _, _, _) = Deploy("return string.Concat(v!.Substring(5), \"\");", level);
            Assert.ThrowsExactly<TestException>(() => contract.Run("Neo"));
            Assert.ThrowsExactly<TestException>(() => contract.Run(null));
            Assert.AreEqual("", contract.Run("Neo12"));
            Assert.AreEqual("xyz", contract.Run("Neo12xyz"));
        }
    }

    [TestMethod]
    public void GasConsumed()
    {
        var scenarios = new (string Name, string Body, string? Input, long Gas)[]
        {
            ("Concat(\"Neo\", \"\")", "return string.Concat(\"Neo\", \"\");", null, 1047930),
            ("Concat(\"\", \"Neo\")", "return string.Concat(\"\", \"Neo\");", null, 1047930),
            ("Concat(\"\", \"\")", "return string.Concat(\"\", \"\");", null, 1047750),
            ("Concat((string?)null, \"\")", "return string.Concat((string?)null, \"\");", null, 1048020),
            ("Concat(\"\", (string?)null)", "return string.Concat(\"\", (string?)null);", null, 1048020),
            ("Concat(v=\"Neo\", \"\")", "return string.Concat(v, \"\");", "Neo", 1047960),
            ("Concat(\"\", v=\"Neo\")", "return string.Concat(\"\", v);", "Neo", 1047960),
            ("Concat(\"Neo\", \"N3\")", "return string.Concat(\"Neo\", \"N3\");", null, 1355610),
        };

        foreach (var (name, body, input, gas) in scenarios)
        {
            var (contract, engine, _, _) = Deploy(body, CompilationOptions.OptimizationType.All);
            using var watcher = engine.CreateFeeWatcher();
            contract.Run(input);
            Assert.AreEqual(gas, watcher.Value, name);
        }
    }

    public abstract class StringConcatContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")]
        public abstract string? Run(string? v);
    }
}
