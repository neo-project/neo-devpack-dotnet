// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_NullableSwitch.cs file belongs to the neo project and is free
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
public class UnitTest_NullableSwitch
{
    [TestMethod]
    public void NullableIntegerSwitchFollowsCSharpBranches()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""linear"")]
    public static int Linear(int? value)
    {
        switch (value)
        {
            case 1: return 10;
            case 2: return 20;
            default: return 99;
        }
    }

    [DisplayName(""tree"")]
    public static int Tree(int? value)
    {
        switch (value)
        {
            case 0: return 10;
            case 1: return 11;
            case 2: return 12;
            case 3: return 13;
            case 4: return 14;
            case 5: return 15;
            case 6: return 16;
            case 7: return 17;
            default: return 99;
        }
    }

    [DisplayName(""withNull"")]
    public static int WithNull(int? value)
    {
        switch (value)
        {
            case 1: return 10;
            case null: return 7;
            default: return 99;
        }
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SwitchContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(99), contract.Linear(null));
        Assert.AreEqual(new BigInteger(10), contract.Linear(1));
        Assert.AreEqual(new BigInteger(99), contract.Linear(5));
        Assert.AreEqual(new BigInteger(99), contract.Tree(null));
        Assert.AreEqual(new BigInteger(17), contract.Tree(7));
        Assert.AreEqual(new BigInteger(7), contract.WithNull(null));
    }

    [TestMethod]
    public void ObjectSwitchWithIntegerCasesHandlesNull()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.ComponentModel;

public class Contract : SmartContract
{
    [DisplayName(""objectLinear"")]
    public static int ObjectLinear(object value)
    {
        switch (value)
        {
            case 1: return 10;
            case 2: return 20;
            default: return 99;
        }
    }

    [DisplayName(""objectTree"")]
    public static int ObjectTree(object value)
    {
        switch (value)
        {
            case 0: return 10;
            case 1: return 11;
            case 2: return 12;
            case 3: return 13;
            case 4: return 14;
            case 5: return 15;
            case 6: return 16;
            case 7: return 17;
            default: return 99;
        }
    }
}";

        var context = TestHelper.CompileSingleContract(source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
        var engine = new TestEngine(true);
        var contract = engine.Deploy<SwitchContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(99), contract.ObjectLinear(null));
        Assert.AreEqual(new BigInteger(10), contract.ObjectLinear(1));
        Assert.AreEqual(new BigInteger(99), contract.ObjectLinear(5));
        Assert.AreEqual(new BigInteger(99), contract.ObjectTree(null));
        Assert.AreEqual(new BigInteger(17), contract.ObjectTree(7));
        Assert.AreEqual(new BigInteger(99), contract.ObjectTree(9));
    }

    public abstract class SwitchContract(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("linear")]
        public abstract BigInteger? Linear(BigInteger? value);

        [DisplayName("tree")]
        public abstract BigInteger? Tree(BigInteger? value);

        [DisplayName("withNull")]
        public abstract BigInteger? WithNull(BigInteger? value);

        [DisplayName("objectLinear")]
        public abstract BigInteger? ObjectLinear(object? value);

        [DisplayName("objectTree")]
        public abstract BigInteger? ObjectTree(object? value);
    }
}
