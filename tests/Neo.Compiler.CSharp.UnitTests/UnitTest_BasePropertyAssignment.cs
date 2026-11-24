// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_BasePropertyAssignment
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.ComponentModel;

        public class BaseValue
        {
            public int Value;
            public virtual int P { get => Value; set => Value = value; }
        }

        public class DerivedValue : BaseValue
        {
            public override int P { get => base.P + 100; set => base.P = value + 1; }
        }

        public class GetterOnlyValue : BaseValue
        {
            public override int P { get => base.P + 100; }
            public int Add() { base.P += 1; return P; }
            public int Increment() { base.P++; return P; }
        }

        public class BaseText
        {
            public virtual string P { get; set; }
        }

        public class DerivedText : BaseText
        {
            public override string P { get => "shadow"; set => base.P = value; }
            public string SetIfMissing() { base.P ??= "base"; return base.P; }
        }

        public class Contract : SmartContract
        {
            [DisplayName("simple")]
            public static int Simple() { var value = new DerivedValue(); value.P = 10; return value.P; }
            [DisplayName("compound")]
            public static int Compound() => new GetterOnlyValue().Add();
            [DisplayName("increment")]
            public static int Increment() => new GetterOnlyValue().Increment();
            [DisplayName("coalesce")]
            public static string Coalesce() => new DerivedText().SetIfMissing();
        }
        """;

    [TestMethod]
    public void BaseAccessorsUseDirectCallsInAssignmentPositions()
    {
        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var contract = new TestEngine(true).Deploy<BasePropertyContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(111), contract.Simple());
        Assert.AreEqual(new BigInteger(101), contract.Compound());
        Assert.AreEqual(new BigInteger(101), contract.Increment());
        Assert.AreEqual("base", contract.Coalesce());
    }

    public abstract class BasePropertyContract(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("simple")] public abstract BigInteger? Simple();
        [DisplayName("compound")] public abstract BigInteger? Compound();
        [DisplayName("increment")] public abstract BigInteger? Increment();
        [DisplayName("coalesce")] public abstract string? Coalesce();
    }
}
