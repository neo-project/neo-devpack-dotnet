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
            private static int _receiverCalls;
            private static Contract ObserveReceiver()
            {
                _receiverCalls++;
                return new Contract();
            }
            private static string _text;
            private static string ObserveText() { _receiverCalls++; return "first"; }
            private static int ObserveIndex() { _receiverCalls++; return 0; }
            private string Text { get => _text; set => _text = value; }
            public string this[int key] { get => _text; set => _text = value; }
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
            [DisplayName("conditionalWithArgument")]
            public void ConditionalWithArgument(string unused)
            {
                this?.Total = 42;
                this?["total"] += 5;
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
    [DataRow("((Contract)this).Total = 42; return Total;", 42, 42)]
    [DataRow("return ((Contract)this).Total = 42;", 42, 42)]
    [DataRow("return ((Contract)((Contract)this)!).Total += 5;", 15, 15)]
    [DataRow("return ++((Contract)this).Total;", 11, 11)]
    [DataRow("return ((Contract)this).Total++;", 10, 11)]
    [DataRow("(((Contract)this).Total, _) = (42, 0); return Total;", 42, 42)]
    [DataRow("return ((Contract)this)[\"total\"] = 42;", 42, 42)]
    [DataRow("return ((Contract)this)[\"total\"] += 5;", 15, 15)]
    [DataRow("return ++((Contract)this)[\"total\"];", 11, 11)]
    [DataRow("return ((Contract)this)[\"total\"]++;", 10, 11)]
    [DataRow("((Contract)ObserveReceiver()).Total = 42; return Total + _receiverCalls * 100;", 142, 42)]
    [DataRow("this?.Total = 42; return Total;", 42, 42)]
    [DataRow("this?.Total += 5; return Total;", 15, 15)]
    [DataRow("this?[\"total\"] = 42; return Total;", 42, 42)]
    [DataRow("this?[\"total\"] += 5; return Total;", 15, 15)]
    [DataRow("((Contract)this)?.Total = 42; return Total;", 42, 42)]
    [DataRow("((Contract)((Contract)this)!)?[\"total\"] += 5; return Total;", 15, 15)]
    [DataRow("this?.Text ??= \"first\"; this?.Text ??= \"second\"; return Text == \"first\" ? 1 : 0;", 1, 10)]
    [DataRow("this?[0] ??= \"first\"; this?[0] ??= \"second\"; return this[0] == \"first\" ? 1 : 0;", 1, 10)]
    [DataRow("ObserveReceiver()?.Total += 5; return Total + _receiverCalls * 100;", 115, 15)]
    [DataRow("Contract receiver = null; receiver?.Total = 42; receiver?[\"total\"] += 5; receiver?.Text ??= \"unused\"; receiver?[0] ??= \"unused\"; return Total;", 10, 10)]
    [DataRow("this?.Text ??= ObserveText(); this?.Text ??= ObserveText(); return _receiverCalls;", 1, 10)]
    [DataRow("this?[ObserveIndex()] ??= ObserveText(); this?[ObserveIndex()] ??= ObserveText(); return _receiverCalls;", 3, 10)]
    [DataRow("Contract receiver = null; receiver?[ObserveIndex()] ??= ObserveText(); return _receiverCalls;", 0, 10)]
    [DataRow("Text ??= \"first\"; Text ??= \"second\"; return Text == \"first\" ? 1 : 0;", 1, 10)]
    [DataRow("this.Text ??= \"first\"; this.Text ??= \"second\"; return Text == \"first\" ? 1 : 0;", 1, 10)]
    [DataRow("this[0] ??= \"first\"; this[0] ??= \"second\"; return this[0] == \"first\" ? 1 : 0;", 1, 10)]
    [DataRow("ObserveReceiver().Text ??= ObserveText(); ObserveReceiver().Text ??= ObserveText(); return _receiverCalls;", 3, 10)]
    public void MutationsPreserveResultAndStorage(string mutation, int result, int stored)
    {
        foreach (var optimization in new[] { CompilationOptions.OptimizationType.None, CompilationOptions.OptimizationType.All })
        {
            // Exercise both calling conventions so receiverless fixes cannot break real receivers.
            foreach (bool explicitConstructor in new[] { false, true })
            {
                var contract = Deploy(mutation, optimization, explicitConstructor);
                string scenario = $"{optimization}, explicit constructor: {explicitConstructor}";
                Assert.AreEqual(new BigInteger(result), contract.Run(), $"{scenario}: expression result");
                Assert.AreEqual(new BigInteger(stored), contract.Read("total"), $"{scenario}: stored value");
            }
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
        contract.ConditionalWithArgument(null);
        Assert.AreEqual(new BigInteger(47), contract.Read("total"));
    }

    private static MutationContract Deploy(string mutation, CompilationOptions.OptimizationType optimization, bool explicitConstructor = false)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        string source = Source.Replace("MUTATION", mutation);
        if (explicitConstructor)
            source = source.Replace("private static int _receiverCalls;", "public Contract() { } private static int _receiverCalls;");
        var context = TestHelper.CompileSingleContract(source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var engine = new TestEngine(true);
        return engine.Deploy<MutationContract>(context.CreateExecutable(), context.CreateManifest());
    }

    public abstract class MutationContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("run")] public abstract BigInteger? Run();
        [DisplayName("read")] public abstract BigInteger? Read(string key);
        [DisplayName("writeInTry")] public abstract void WriteInTry(BigInteger callerTag, BigInteger value, int divisor);
        [DisplayName("conditionalWithArgument")] public abstract void ConditionalWithArgument(string? unused);
    }
}
