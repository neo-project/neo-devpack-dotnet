// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_FoldConstantArithmetic.cs file belongs to the neo project and is free
// software: you can redistribute it and/or modify it under the terms of the
// MIT software license, see the accompanying file LICENSE in the main
// directory of the repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications, are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_FoldConstantArithmetic
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        public class Contract : SmartContract
        {
            static readonly int A = 7;
            static readonly int B = 5;
            static readonly string S1 = "ab";
            static readonly string S2 = "cd";

            public static int Sum() => A + B;
            public static int Diff() => A - B;
            public static int Prod() => A * B;
            public static int Quot() => A / 2;
            public static int Mod() => A % 2;
            public static int And() => A & B;
            public static int Or() => A | B;
            public static int Xor() => A ^ B;
            public static int Shl() => A << 2;
            public static int Shr() => A >> 1;
            public static bool Greater() => A > B;
            public static bool Equal() => A == B;
            public static string Cat() => S1 + S2;
            public static int Len() => S1.Length;
        }
        """;

    private static readonly HashSet<OpCode> NumericPush = new()
    {
        OpCode.PUSHM1,
        OpCode.PUSH0, OpCode.PUSH1, OpCode.PUSH2, OpCode.PUSH3, OpCode.PUSH4,
        OpCode.PUSH5, OpCode.PUSH6, OpCode.PUSH7, OpCode.PUSH8, OpCode.PUSH9,
        OpCode.PUSH10, OpCode.PUSH11, OpCode.PUSH12, OpCode.PUSH13, OpCode.PUSH14,
        OpCode.PUSH15, OpCode.PUSH16,
        OpCode.PUSHINT8, OpCode.PUSHINT16, OpCode.PUSHINT32, OpCode.PUSHINT64,
        OpCode.PUSHINT128, OpCode.PUSHINT256,
    };

    private static readonly HashSet<OpCode> FoldableBinary = new()
    {
        OpCode.ADD, OpCode.SUB, OpCode.MUL, OpCode.DIV, OpCode.MOD,
        OpCode.AND, OpCode.OR, OpCode.XOR, OpCode.SHL, OpCode.SHR,
        OpCode.NUMEQUAL, OpCode.NUMNOTEQUAL,
        OpCode.LT, OpCode.LE, OpCode.GT, OpCode.GE,
        OpCode.MIN, OpCode.MAX,
    };

    private static readonly HashSet<OpCode> DataPush = new()
    {
        OpCode.PUSHDATA1, OpCode.PUSHDATA2, OpCode.PUSHDATA4,
    };

    private static List<Neo.VM.Instruction> OptimizedOps(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join("\n", context.Diagnostics));
        var (nef, _, _) = Neo.Optimizer.Optimizer.Optimize(
            context.CreateExecutable(), context.CreateManifest(), null, optimization);
        return new Script(nef.Script).EnumerateInstructions().Select(kv => kv.instruction).ToList();
    }

    [TestMethod]
    public void ArithmeticPairsAreFolded()
    {
        List<Neo.VM.Instruction> ops = OptimizedOps(CompilationOptions.OptimizationType.All);
        for (int i = 0; i + 2 < ops.Count; i++)
        {
            Assert.IsFalse(
                NumericPush.Contains(ops[i].OpCode)
                && NumericPush.Contains(ops[i + 1].OpCode)
                && FoldableBinary.Contains(ops[i + 2].OpCode),
                $"constant pair followed by {ops[i + 2].OpCode} was not folded at index {i}: " +
                $"{ops[i].OpCode} {ops[i + 1].OpCode} {ops[i + 2].OpCode}");
        }
    }

    [TestMethod]
    public void StringPairsAreFolded()
    {
        List<Neo.VM.Instruction> ops = OptimizedOps(CompilationOptions.OptimizationType.All);
        for (int i = 0; i + 2 < ops.Count; i++)
        {
            Assert.IsFalse(
                DataPush.Contains(ops[i].OpCode) && DataPush.Contains(ops[i + 1].OpCode)
                && ops[i + 2].OpCode == OpCode.CAT,
                $"constant byte strings followed by CAT were not folded at index {i}");
            Assert.IsFalse(
                DataPush.Contains(ops[i].OpCode) && ops[i + 1].OpCode == OpCode.SIZE,
                $"constant byte string followed by SIZE was not folded at index {i}");
        }
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void FoldPreservesResults(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, $"{optimization}: {string.Join("\n", context.Diagnostics)}");

        var engine = new TestEngine(true);
        var contract = engine.Deploy<ContractArtifact>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(12, contract.Sum());
        Assert.AreEqual(2, contract.Diff());
        Assert.AreEqual(35, contract.Prod());
        Assert.AreEqual(3, contract.Quot());
        Assert.AreEqual(1, contract.Mod());
        Assert.AreEqual(5, contract.And());
        Assert.AreEqual(7, contract.Or());
        Assert.AreEqual(2, contract.Xor());
        Assert.AreEqual(28, contract.Shl());
        Assert.AreEqual(3, contract.Shr());
        Assert.IsTrue(contract.Greater());
        Assert.IsFalse(contract.Equal());
        Assert.AreEqual("abcd", contract.Cat());
        Assert.AreEqual(2, contract.Len());
    }

    public abstract class ContractArtifact(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("sum")] public abstract int Sum();
        [DisplayName("diff")] public abstract int Diff();
        [DisplayName("prod")] public abstract int Prod();
        [DisplayName("quot")] public abstract int Quot();
        [DisplayName("mod")] public abstract int Mod();
        [DisplayName("and")] public abstract int And();
        [DisplayName("or")] public abstract int Or();
        [DisplayName("xor")] public abstract int Xor();
        [DisplayName("shl")] public abstract int Shl();
        [DisplayName("shr")] public abstract int Shr();
        [DisplayName("greater")] public abstract bool Greater();
        [DisplayName("equal")] public abstract bool Equal();
        [DisplayName("cat")] public abstract string? Cat();
        [DisplayName("len")] public abstract int Len();
    }
}
