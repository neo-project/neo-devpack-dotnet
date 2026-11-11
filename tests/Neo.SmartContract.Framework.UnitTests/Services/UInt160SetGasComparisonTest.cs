// Copyright (C) 2015-2026 The Neo Project.
//
// UInt160SetGasComparisonTest.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests.Services;

[TestClass]
public class UInt160SetGasComparisonTest
{
    public TestContext? TestContext { get; set; }

    private static UInt160 Item(int i)
    {
        var bytes = new byte[20];
        bytes[0] = 0x42;
        BitConverter.GetBytes(i).CopyTo(bytes, 1);
        return new UInt160(bytes);
    }

    private static (NefFile nef, ContractManifest manifest) Compile(string fileName)
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        var source = Path.Combine(repoRoot, "tests", "Neo.SmartContract.Framework.TestContracts", fileName);
        var options = new CompilationOptions
        {
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = NullableContextOptions.Enable,
            SkipRestoreIfAssetsPresent = true
        };
        var contexts = new CompilationEngine(options).CompileSources(new CompilationSourceReferences { Projects = [frameworkProject] }, source);
        Assert.AreEqual(1, contexts.Count);
        Assert.IsTrue(contexts[0].Success, string.Join(Environment.NewLine, contexts[0].Diagnostics.Select(p => p.ToString())));
        return (contexts[0].CreateExecutable(), contexts[0].CreateManifest());
    }

    private static long Fee(Func<TestEngine> engine, Action a)
    {
        a();
        return engine().FeeConsumed.Value;
    }

    [TestMethod]
    public void Compare_Gas_Against_Serialized_List_Baseline()
    {
        var setC = Compile("Contract_UInt160Set.cs");
        var naiveC = Compile("Contract_NaiveSerializedSet.cs");

        foreach (var size in new[] { 1, 10, 50, 100, 300 })
        {
            var e1 = new TestEngine(true);
            var s = e1.Deploy<SetProxy>(setC.nef, setC.manifest);
            var e2 = new TestEngine(true);
            var n = e2.Deploy<NaiveProxy>(naiveC.nef, naiveC.manifest);
            for (var i = 0; i < size; i++) { s.Add(Item(i)); n.Add(Item(i)); }

            var x = Item(100000);
            var line = $"size={size,4} | ";
            line += $"add set={Fee(() => e1, () => s.Add(x))} naive={Fee(() => e2, () => n.Add(x))} | ";
            line += $"contains(last) set={Fee(() => e1, () => s.Contains(x))} naive={Fee(() => e2, () => n.Contains(x))} | ";
            line += $"at set={Fee(() => e1, () => s.At(size / 2))} naive={Fee(() => e2, () => n.At(size / 2))} | ";
            line += $"removeFirst set={Fee(() => e1, () => s.Remove(Item(0)))} naive={Fee(() => e2, () => n.Remove(Item(0)))} | ";
            line += $"removeLast set={Fee(() => e1, () => s.Remove(x))} naive={Fee(() => e2, () => n.Remove(x))}";
            TestContext?.WriteLine(line);
        }
    }

    public abstract class SetProxy(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("add")] public abstract bool? Add(UInt160 value);
        [DisplayName("remove")] public abstract bool? Remove(UInt160 value);
        [DisplayName("contains")] public abstract bool? Contains(UInt160 value);
        [DisplayName("at")] public abstract UInt160? At(BigInteger index);
    }

    public abstract class NaiveProxy(SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("add")] public abstract bool? Add(UInt160 value);
        [DisplayName("remove")] public abstract bool? Remove(UInt160 value);
        [DisplayName("contains")] public abstract bool? Contains(UInt160 value);
        [DisplayName("at")] public abstract UInt160? At(BigInteger index);
    }
}
