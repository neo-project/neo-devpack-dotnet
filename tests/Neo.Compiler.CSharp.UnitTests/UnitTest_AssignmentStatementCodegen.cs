// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_AssignmentStatementCodegen.cs file belongs to the neo project and is free
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
public class UnitTest_AssignmentStatementCodegen
{
    // Exercises every simple-assignment LHS form as a statement (where the result is
    // discarded and the dead DUP+DROP is now elided) plus expression-context assignments
    // (where the result is still produced). A miscompile would surface as a VM fault or
    // a wrong value here.
    [TestMethod]
    public void AssignmentStatements_AllLhsForms_BehaveCorrectly()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

public class Contract : SmartContract
{
    private static int _static;

    public int Field;
    public int Prop { get; set; }

    [DisplayName(""localAssign"")]
    public static int LocalAssign()
    {
        int a;
        a = 5;        // statement: result discarded
        return a;
    }

    [DisplayName(""staticAssign"")]
    public static int StaticAssign()
    {
        _static = 7;  // statement
        return _static;
    }

    [DisplayName(""indexerAssign"")]
    public static int IndexerAssign()
    {
        var arr = new int[3];
        arr[1] = 9;   // statement (element store)
        return arr[1];
    }

    [DisplayName(""tupleAssign"")]
    public static int TupleAssign()
    {
        int a, b;
        (a, b) = (3, 4);  // statement (tuple)
        return a * 10 + b;
    }

    [DisplayName(""discardAssign"")]
    public static int DiscardAssign()
    {
        int n = 0;
        _ = Inc(ref n);   // statement (discard) — side effect must still run
        return n;
    }

    [DisplayName(""exprContextAssign"")]
    public static int ExprContextAssign()
    {
        int a;
        int b = (a = 5);  // expression context: result still produced
        return a + b;     // 10
    }

    [DisplayName(""chainedAssign"")]
    public static int ChainedAssign()
    {
        int a, b;
        a = b = 6;        // chained: inner result produced, outer discarded
        return a + b;     // 12
    }

    [DisplayName(""compoundAssign"")]
    public static int CompoundAssign()
    {
        int a = 10;
        a += 5;           // compound statement (unchanged path)
        return a;         // 15
    }

    private static int Inc(ref int x) { x = x + 1; return x; }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<AssignmentContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(5), contract.LocalAssign());
        Assert.AreEqual(new BigInteger(7), contract.StaticAssign());
        Assert.AreEqual(new BigInteger(9), contract.IndexerAssign());
        Assert.AreEqual(new BigInteger(34), contract.TupleAssign());
        Assert.AreEqual(new BigInteger(1), contract.DiscardAssign());
        Assert.AreEqual(new BigInteger(10), contract.ExprContextAssign());
        Assert.AreEqual(new BigInteger(12), contract.ChainedAssign());
        Assert.AreEqual(new BigInteger(15), contract.CompoundAssign());
    }

    public abstract class AssignmentContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("localAssign")] public abstract BigInteger? LocalAssign();
        [DisplayName("staticAssign")] public abstract BigInteger? StaticAssign();
        [DisplayName("indexerAssign")] public abstract BigInteger? IndexerAssign();
        [DisplayName("tupleAssign")] public abstract BigInteger? TupleAssign();
        [DisplayName("discardAssign")] public abstract BigInteger? DiscardAssign();
        [DisplayName("exprContextAssign")] public abstract BigInteger? ExprContextAssign();
        [DisplayName("chainedAssign")] public abstract BigInteger? ChainedAssign();
        [DisplayName("compoundAssign")] public abstract BigInteger? CompoundAssign();
    }
}
