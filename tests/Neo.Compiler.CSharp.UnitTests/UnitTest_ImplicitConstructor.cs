// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ImplicitConstructor
{
    private const string Source = """
        using Neo.SmartContract.Framework;

        public class Base
        {
            public int Log;
            public Base() { Log = 7; }
        }
        public class Implicit : Base { }
        public class ImplicitTwice : Implicit { }
        public class Explicit : Base { public Explicit() { } }
        public class WithInitializer : Base { public int Extra; }
        public class Root { public int Log; public Root() { Log = 7; } }
        public class Middle : Root { }
        public class Leaf : Middle { public Leaf() { } }
        public class VirtualBase
        {
            public int Seen;
            public VirtualBase() { Seen = Probe(); }
            public virtual int Probe() => 1;
        }
        public class VirtualDerived : VirtualBase
        {
            public override int Probe() => 33;
        }
        public class Contract : SmartContract
        {
            public static int Implicit() => new Implicit().Log;
            public static int ImplicitTwice() => new ImplicitTwice().Log;
            public static int WithInitializer() => new WithInitializer { Extra = 1 }.Log;
            public static int MixedChain() => new Leaf().Log;
            public static int VirtualCall() => new VirtualDerived().Seen;
            public static int Explicit() => new Explicit().Log;
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ImplicitConstructorsChainToBase(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var contract = new TestEngine(true).Deploy<ConstructorContract>(context.CreateExecutable(), context.CreateManifest());
        Assert.AreEqual(new BigInteger(7), contract.Implicit());
        Assert.AreEqual(new BigInteger(7), contract.ImplicitTwice());
        Assert.AreEqual(new BigInteger(7), contract.WithInitializer());
        Assert.AreEqual(new BigInteger(7), contract.MixedChain());
        Assert.AreEqual(new BigInteger(33), contract.VirtualCall());
        Assert.AreEqual(new BigInteger(7), contract.Explicit());
    }

    public abstract class ConstructorContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("implicit")] public abstract BigInteger? Implicit();
        [DisplayName("implicitTwice")] public abstract BigInteger? ImplicitTwice();
        [DisplayName("withInitializer")] public abstract BigInteger? WithInitializer();
        [DisplayName("mixedChain")] public abstract BigInteger? MixedChain();
        [DisplayName("virtualCall")] public abstract BigInteger? VirtualCall();
        [DisplayName("explicit")] public abstract BigInteger? Explicit();
    }
}
