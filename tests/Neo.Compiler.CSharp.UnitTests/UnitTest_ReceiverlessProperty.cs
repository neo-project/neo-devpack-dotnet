// Copyright (C) 2015-2026 The Neo Project.
// Distributed under the MIT license; see LICENSE in the project root.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_ReceiverlessProperty
{
    // Deliberately no instance fields or explicit constructor: accessors have no hidden this argument.
    private const string Source = """
        using Neo.SmartContract.Framework;
        using Neo.SmartContract.Framework.Services;
        using System;
        using System.Numerics;
        using System.ComponentModel;
        public class Contract : SmartContract
        {
            private BigInteger Total
            {
                get => (BigInteger)Storage.Get(Storage.CurrentContext, "total");
                set => Storage.Put(Storage.CurrentContext, "total", value);
            }
            public BigInteger this[string key]
            {
                get => (BigInteger)Storage.Get(Storage.CurrentContext, key);
                set => Storage.Put(Storage.CurrentContext, key, value);
            }
            [DisplayName("read")]
            public BigInteger Read(string key) => (BigInteger)Storage.Get(Storage.CurrentContext, key);
            [DisplayName("run")]
            public BigInteger Run()
            {
                Total = 10;
                MUTATION
            }
            [DisplayName("writeInTry")]
            public void WriteInTry(BigInteger callerTag, BigInteger value, int divisor)
            {
                try
                {
                    this.Total = value;
                    int min = int.MinValue;
                    _ = checked(min / divisor);
                }
                catch (Exception) { }
            }
        }
        """;

    [TestMethod]
    [DataRow("return Total = 42;", 42, 42)]
    [DataRow("this.Total = 42; return Total;", 42, 42)]
    [DataRow("return this.Total = 42;", 42, 42)]
    [DataRow("return Total += 5;", 15, 15)]
    [DataRow("return this.Total += 5;", 15, 15)]
    [DataRow("return ++Total;", 11, 11)]
    [DataRow("return --Total;", 9, 9)]
    [DataRow("return ++this.Total;", 11, 11)]
    [DataRow("return --this.Total;", 9, 9)]
    [DataRow("return Total++;", 10, 11)]
    [DataRow("return Total--;", 10, 9)]
    [DataRow("return this.Total++;", 10, 11)]
    [DataRow("return this.Total--;", 10, 9)]
    [DataRow("(this.Total, _) = (42, 0); return Total;", 42, 42)]
    [DataRow("this[\"total\"] = 42; return Total;", 42, 42)]
    [DataRow("return this[\"total\"] = 42;", 42, 42)]
    [DataRow("return this[\"total\"] += 5;", 15, 15)]
    [DataRow("return ++this[\"total\"];", 11, 11)]
    [DataRow("return --this[\"total\"];", 9, 9)]
    [DataRow("return this[\"total\"]++;", 10, 11)]
    [DataRow("return this[\"total\"]--;", 10, 9)]
    public void MutationsPreserveResultAndStorage(string mutation, int result, int stored)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.All })
        {
            var contract = Deploy(mutation, optimization);
            Assert.AreEqual(new BigInteger(result), contract.Run(), $"{optimization}: expression result");
            Assert.AreEqual(new BigInteger(stored), contract.Read("total"), $"{optimization}: stored value");
        }
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SetterMustNotStoreTheCallersFirstArgument(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy("return Total;", optimization);
        contract.WriteInTry(888888, 42, -1);
        Assert.AreEqual(new BigInteger(42), contract.Read("total"));
    }

    private static MutationContract Deploy(string mutation, CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source.Replace("MUTATION", mutation), options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        return engine.Deploy<MutationContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class MutationContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract BigInteger? Run();
        [DisplayName("read")] public abstract BigInteger? Read(string key);
        [DisplayName("writeInTry")] public abstract void WriteInTry(BigInteger callerTag, BigInteger value, int divisor);
    }
}
