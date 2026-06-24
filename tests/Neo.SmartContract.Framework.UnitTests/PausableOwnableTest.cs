// Copyright (C) 2015-2026 The Neo Project.
//
// PausableOwnableTest.cs file belongs to the neo project and is free
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
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class PausableOwnableTest
{
    private static readonly Signer Alice = TestEngine.Alice;
    private static readonly Signer Bob = TestEngine.Bob;

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static PausableOwnableProxy Deploy(TestEngine engine)
        => engine.Deploy<PausableOwnableProxy>(_compiled.nef, _compiled.manifest, null);

    private static void Merge(PausableOwnableProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    [TestMethod]
    public void Deploy_NotPaused()
    {
        var c = Deploy(CreateEngine());
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void Owner_CanPauseAndUnpause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        UInt160? pausedBy = null, unpausedBy = null;
        c.OnPaused += a => pausedBy = a;
        c.OnUnpaused += a => unpausedBy = a;

        c.Pause();
        Assert.IsTrue(c.Paused);
        Assert.AreEqual(Alice.Account, pausedBy);

        c.Unpause();
        Assert.IsFalse(c.Paused);
        Assert.AreEqual(Alice.Account, unpausedBy);
        Merge(c);
    }

    [TestMethod]
    public void NonOwner_CannotPause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.Pause());
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void NonOwner_CannotUnpause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.Pause();

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.Unpause());
        Assert.IsTrue(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void Pause_WhenAlreadyPaused_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.Pause();

        Assert.ThrowsException<TestException>(() => c.Pause());
        Merge(c);
    }

    [TestMethod]
    public void Unpause_WhenNotPaused_Aborts()
    {
        var c = Deploy(CreateEngine());
        Assert.ThrowsException<TestException>(() => c.Unpause());
        Merge(c);
    }

    [TestMethod]
    public void WhenNotPaused_GuardsBusinessMethod()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        // Runs while not paused.
        Assert.IsTrue(c.DoWork()!.Value);

        // Blocked once paused.
        c.Pause();
        Assert.ThrowsException<TestException>(() => c.DoWork());

        // Runs again after unpausing.
        c.Unpause();
        Assert.IsTrue(c.DoWork()!.Value);
        Merge(c);
    }

    [TestMethod]
    public void WhenPaused_GuardsBusinessMethod()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        // Blocked while not paused.
        Assert.ThrowsException<TestException>(() => c.DoWhilePaused());

        // Runs once paused.
        c.Pause();
        Assert.IsTrue(c.DoWhilePaused()!.Value);
        Merge(c);
    }

    [TestMethod]
    public void Manifest_SafeFlags()
    {
        var abi = _compiled.manifest.Abi.Methods;
        Assert.IsTrue(abi.Single(m => m.Name == "paused").Safe, "paused must be safe");
        Assert.IsFalse(abi.Single(m => m.Name == "pause").Safe, "pause must not be safe");
        Assert.IsFalse(abi.Single(m => m.Name == "unpause").Safe, "unpause must not be safe");
        var names = abi.Select(m => m.Name).ToArray();
        Assert.IsFalse(names.Contains("whenNotPaused"));
        Assert.IsFalse(names.Contains("whenPaused"));
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class Contract : PausableOwnable
{
    public static void _deploy(object data, bool update)
    {
        InitializeOwner(data, update);
    }

    [WhenNotPaused]
    public static bool DoWork()
    {
        return true;
    }

    [WhenPaused]
    public static bool DoWhilePaused()
    {
        return true;
    }
}";

        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempFile, source);
        try
        {
            var options = new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                SkipRestoreIfAssetsPresent = true
            };
            var engine = new CompilationEngine(options);
            var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
            var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");

            var contexts = engine.CompileSources(new CompilationSourceReferences { Projects = new[] { frameworkProject } }, tempFile);
            Assert.AreEqual(1, contexts.Count, "Expected exactly one contract compilation context.");
            var context = contexts[0];
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var (nef, manifest, debugInfoJson) = context.CreateResults(repoRoot);
            return (nef, manifest, NeoDebugInfo.FromDebugInfoJson(debugInfoJson));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    public abstract class PausableOwnableProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public delegate void PausedDelegate(UInt160? account);
        public delegate void UnpausedDelegate(UInt160? account);

        [DisplayName("Paused")]
        public event PausedDelegate? OnPaused;

        [DisplayName("Unpaused")]
        public event UnpausedDelegate? OnUnpaused;

        public abstract bool Paused { [DisplayName("paused")] get; }

        [DisplayName("pause")]
        public abstract void Pause();

        [DisplayName("unpause")]
        public abstract void Unpause();

        [DisplayName("doWork")]
        public abstract bool? DoWork();

        [DisplayName("doWhilePaused")]
        public abstract bool? DoWhilePaused();
    }
}
