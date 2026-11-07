// Copyright (C) 2015-2026 The Neo Project.
//
// PausableAccessControlTest.cs file belongs to the neo project and is free
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
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class PausableAccessControlTest
{
    private static readonly Signer Alice = TestEngine.Alice;
    private static readonly Signer Bob = TestEngine.Bob;
    private static readonly Signer Charlie = TestEngine.Charlie;

    private static readonly BigInteger DefaultAdmin = BigInteger.Zero;
    private static readonly BigInteger PauserRole = BigInteger.One;

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static PausableAccessControlProxy Deploy(TestEngine engine, object? admin = null)
        => engine.Deploy<PausableAccessControlProxy>(_compiled.nef, _compiled.manifest, admin);

    private static void Merge(PausableAccessControlProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    [TestMethod]
    public void Deploy_InitializesAdminAndPauser()
    {
        var engine = CreateEngine();
        var granted = new List<(BigInteger role, UInt160 account, UInt160 sender)>();
        engine.OnRuntimeNotify += (_, name, state) =>
        {
            if (name == "RoleGranted")
                granted.Add((state[0].GetInteger(), new UInt160(state[1].GetSpan()), new UInt160(state[2].GetSpan())));
        };
        var c = Deploy(engine);

        Assert.IsFalse(c.Paused);
        Assert.AreEqual(PauserRole, c.PAUSER_ROLE());
        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.IsTrue(c.HasRole(PauserRole, Alice.Account));
        Assert.IsFalse(c.Storage.Contains(new byte[] { 0xFE }));
        Assert.IsTrue(c.Storage.Contains(new byte[] { 0xFB, 0x00 }));
        CollectionAssert.AreEqual(new[]
        {
            (DefaultAdmin, Alice.Account, Alice.Account),
            (PauserRole, Alice.Account, Alice.Account)
        }, granted);
        Merge(c);
    }

    [TestMethod]
    public void Deploy_ExplicitAdminReceivesBothRoles()
    {
        var engine = CreateEngine();
        var c = Deploy(engine, Bob.Account);
        Assert.IsTrue(c.HasRole(DefaultAdmin, Bob.Account));
        Assert.IsTrue(c.HasRole(PauserRole, Bob.Account));
        Assert.IsFalse(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.IsFalse(c.HasRole(PauserRole, Alice.Account));
        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        c.Unpause(Bob.Account);
        Merge(c);
    }

    [TestMethod]
    public void Deploy_ZeroAdminRejectedWithoutNotifications()
    {
        var engine = CreateEngine();
        int events = 0;
        engine.OnRuntimeNotify += (_, _, _) => events++;
        Assert.ThrowsExactly<TestException>(() => Deploy(engine, UInt160.Zero));
        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Admin_CanPauseAndUnpause_WithEvents()
    {
        var c = Deploy(CreateEngine());
        var paused = new List<UInt160?>();
        var unpaused = new List<UInt160?>();
        c.OnPaused += a => paused.Add(a);
        c.OnUnpaused += a => unpaused.Add(a);

        c.Pause(Alice.Account);
        Assert.IsTrue(c.Paused);
        Assert.IsTrue(c.Storage.Contains(new byte[] { 0xFE }));
        c.Unpause(Alice.Account);
        Assert.IsFalse(c.Paused);
        Assert.IsFalse(c.Storage.Contains(new byte[] { 0xFE }));

        CollectionAssert.AreEqual(new UInt160?[] { Alice.Account }, paused);
        CollectionAssert.AreEqual(new UInt160?[] { Alice.Account }, unpaused);
        Merge(c);
    }

    [TestMethod]
    public void NoRole_CannotPauseOrUnpause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        int events = 0;
        c.OnPaused += _ => events++;
        c.OnUnpaused += _ => events++;

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.Pause(Bob.Account));
        Assert.IsFalse(c.Paused);
        Assert.AreEqual(0, events);

        engine.SetTransactionSigners(Alice);
        c.Pause(Alice.Account);
        events = 0;
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.Unpause(Bob.Account));
        Assert.IsTrue(c.Paused);
        Assert.AreEqual(0, events);
        Merge(c);
    }

    [TestMethod]
    public void RoleWithoutWitness_CannotPauseOrUnpause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);
        Assert.IsTrue(c.HasRole(PauserRole, Bob.Account));

        int events = 0;
        c.OnPaused += _ => events++;
        c.OnUnpaused += _ => events++;

        // Bob holds the role, but only Charlie signs the transaction.
        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsExactly<TestException>(() => c.Pause(Bob.Account));
        Assert.IsFalse(c.Paused);
        Assert.AreEqual(0, events);

        engine.SetTransactionSigners(Alice);
        c.Pause(Alice.Account);
        events = 0;

        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsExactly<TestException>(() => c.Unpause(Bob.Account));
        Assert.IsTrue(c.Paused);
        Assert.AreEqual(0, events);
        Merge(c);
    }

    [TestMethod]
    public void WitnessWithoutRole_CannotPause()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        // Bob signs and supplies himself, but does not hold the role.
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.Pause(Bob.Account));
        // Bob signs but impersonates Alice (who holds the role).
        Assert.ThrowsExactly<TestException>(() => c.Pause(Alice.Account));
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void ZeroAccount_Rejected()
    {
        var c = Deploy(CreateEngine());
        Assert.ThrowsExactly<TestException>(() => c.Pause(UInt160.Zero));
        Assert.ThrowsExactly<TestException>(() => c.Unpause(UInt160.Zero));
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void MultiplePausers_WorkIndependently()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);
        c.GrantRole(PauserRole, Alice.Account, Charlie.Account);

        var paused = new List<UInt160?>();
        var unpaused = new List<UInt160?>();
        c.OnPaused += a => paused.Add(a);
        c.OnUnpaused += a => unpaused.Add(a);

        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        engine.SetTransactionSigners(Charlie);
        c.Unpause(Charlie.Account);
        Assert.IsFalse(c.Paused);

        CollectionAssert.AreEqual(new UInt160?[] { Bob.Account }, paused);
        CollectionAssert.AreEqual(new UInt160?[] { Charlie.Account }, unpaused);
        Merge(c);
    }

    [TestMethod]
    public void RevokedPauser_CannotPause_AdminCanRegrant()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);

        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        engine.SetTransactionSigners(Alice);
        c.RevokeRole(PauserRole, Alice.Account, Bob.Account);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.Unpause(Bob.Account));
        Assert.IsTrue(c.Paused);

        engine.SetTransactionSigners(Alice);
        c.Unpause(Alice.Account);
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.Pause(Bob.Account));
        Assert.IsFalse(c.Paused);

        engine.SetTransactionSigners(Alice);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);
        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        c.Unpause(Bob.Account);
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void AllPausersRevoked_AdminRecovers()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.Pause(Alice.Account);
        c.RevokeRole(PauserRole, Alice.Account, Alice.Account);
        Assert.ThrowsExactly<TestException>(() => c.Unpause(Alice.Account));

        c.GrantRole(PauserRole, Alice.Account, Alice.Account);
        c.Unpause(Alice.Account);
        Assert.IsFalse(c.Paused);
        Merge(c);
    }

    [TestMethod]
    public void UnauthorizedGrant_Rejected()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsExactly<TestException>(() => c.GrantRole(PauserRole, Bob.Account, Bob.Account));
        Assert.ThrowsExactly<TestException>(() => c.GrantRole(PauserRole, Alice.Account, Bob.Account));
        Assert.IsFalse(c.HasRole(PauserRole, Bob.Account));
        Merge(c);
    }

    [TestMethod]
    public void InvalidStateTransitions_Rejected()
    {
        var c = Deploy(CreateEngine());
        int events = 0;
        c.OnPaused += _ => events++;
        c.OnUnpaused += _ => events++;

        Assert.ThrowsExactly<TestException>(() => c.Unpause(Alice.Account));
        Assert.IsFalse(c.Paused);
        Assert.AreEqual(0, events);

        c.Pause(Alice.Account);
        Assert.AreEqual(1, events);
        Assert.ThrowsExactly<TestException>(() => c.Pause(Alice.Account));
        Assert.IsTrue(c.Paused);
        Assert.AreEqual(1, events);
        Merge(c);
    }

    [TestMethod]
    public void Modifiers_Work()
    {
        var c = Deploy(CreateEngine());

        Assert.IsTrue(c.DoWork()!.Value);
        Assert.ThrowsExactly<TestException>(() => c.DoWhilePaused());

        c.Pause(Alice.Account);
        Assert.ThrowsExactly<TestException>(() => c.DoWork());
        Assert.IsTrue(c.DoWhilePaused()!.Value);

        c.Unpause(Alice.Account);
        Assert.IsTrue(c.DoWork()!.Value);
        Assert.ThrowsExactly<TestException>(() => c.DoWhilePaused());
        Merge(c);
    }

    [TestMethod]
    public void Update_PreservesState_AndReinitializeFails()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);
        c.RevokeRole(PauserRole, Alice.Account, Alice.Account);
        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        engine.SetTransactionSigners(Alice);

        int events = 0;
        engine.OnRuntimeNotify += (sender, _, _) =>
        {
            if (sender == c.Hash) events++;
        };

        c.InitUpdate();

        Assert.IsTrue(c.Paused);
        Assert.IsFalse(c.HasRole(PauserRole, Alice.Account));
        Assert.IsTrue(c.HasRole(PauserRole, Bob.Account));
        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.IsFalse(c.HasRole(DefaultAdmin, Bob.Account));
        Assert.AreEqual(0, events);

        Assert.ThrowsExactly<TestException>(() => c.ReInit(Bob.Account));
        Assert.IsFalse(c.HasRole(DefaultAdmin, Bob.Account));
        Assert.IsTrue(c.Paused);
        Assert.IsFalse(c.HasRole(PauserRole, Alice.Account));
        Assert.AreEqual(0, events);
        Merge(c);
    }

    [TestMethod]
    public void ContractUpdate_PreservesRolesAndPauseState()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(PauserRole, Alice.Account, Bob.Account);
        c.RevokeRole(PauserRole, Alice.Account, Alice.Account);
        engine.SetTransactionSigners(Bob);
        c.Pause(Bob.Account);
        int events = 0;
        engine.OnRuntimeNotify += (sender, _, _) =>
        {
            if (sender == c.Hash) events++;
        };

        var updated = CompileContract(2);
        Assert.ThrowsExactly<TestException>(() => c.Update(updated.nef.ToArray(), updated.manifest.ToJson().ToString(), Bob.Account));
        Assert.AreEqual(1, c.Version());
        Assert.AreEqual(0, events);

        engine.SetTransactionSigners(Alice);
        c.Update(updated.nef.ToArray(), updated.manifest.ToJson().ToString(), Bob.Account);
        Assert.AreEqual(2, c.Version());
        Assert.IsTrue(c.Paused);
        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.IsFalse(c.HasRole(DefaultAdmin, Bob.Account));
        Assert.IsFalse(c.HasRole(PauserRole, Alice.Account));
        Assert.IsTrue(c.HasRole(PauserRole, Bob.Account));
        Assert.AreEqual(0, events);

        c.GrantRole(PauserRole, Alice.Account, Charlie.Account);
        engine.SetTransactionSigners(Charlie);
        c.Unpause(Charlie.Account);
        Assert.IsFalse(c.Paused);
        DynamicCoverageMergeHelper.Merge(c, updated.debugInfo);
    }

    [TestMethod]
    public void Manifest_SafeFlags()
    {
        var abi = _compiled.manifest.Abi.Methods;
        Assert.IsTrue(abi.Single(m => m.Name == "paused").Safe);
        Assert.IsTrue(abi.Single(m => m.Name == "pAUSER_ROLE").Safe);
        Assert.IsFalse(abi.Single(m => m.Name == "pause").Safe);
        Assert.IsFalse(abi.Single(m => m.Name == "unpause").Safe);
        Assert.IsFalse(abi.Any(m => m.Name == "initializePausableAccessControl" || m.Name == "grantRoleInternal" || m.Name == "onlyRole"));
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract(int version = 1)
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

[ContractPermission(""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"", ""update"")]
public class Contract : PausableAccessControl
{
    public static void _deploy(object data, bool update)
    {
        InitializePausableAccessControl((UInt160?)data, update);
    }

    public static void InitUpdate()
    {
        InitializePausableAccessControl(null, true);
    }

    public static void ReInit(UInt160 admin)
    {
        InitializePausableAccessControl(admin, false);
    }

    public static int Version() => 1;

    public static void Update(ByteString nef, string manifest, UInt160 admin)
    {
        OnlyRole(DEFAULT_ADMIN_ROLE(), Runtime.Transaction.Sender);
        ContractManagement.Update(nef, manifest, admin);
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
        File.WriteAllText(tempFile, source.Replace("Version() => 1", $"Version() => {version}"));
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

    public abstract class PausableAccessControlProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public delegate void PausedDelegate(UInt160? account);
        public delegate void UnpausedDelegate(UInt160? account);

        [DisplayName("Paused")]
        public event PausedDelegate? OnPaused;

        [DisplayName("Unpaused")]
        public event UnpausedDelegate? OnUnpaused;

        public abstract bool Paused { [DisplayName("paused")] get; }

        [DisplayName("pAUSER_ROLE")]
        public abstract BigInteger PAUSER_ROLE();

        [DisplayName("hasRole")]
        public abstract bool HasRole(BigInteger role, UInt160 account);

        [DisplayName("grantRole")]
        public abstract void GrantRole(BigInteger role, UInt160 admin, UInt160 account);

        [DisplayName("revokeRole")]
        public abstract void RevokeRole(BigInteger role, UInt160 admin, UInt160 account);

        [DisplayName("pause")]
        public abstract void Pause(UInt160 account);

        [DisplayName("unpause")]
        public abstract void Unpause(UInt160 account);

        [DisplayName("initUpdate")]
        public abstract void InitUpdate();

        [DisplayName("reInit")]
        public abstract void ReInit(UInt160 admin);

        [DisplayName("version")]
        public abstract int Version();

        [DisplayName("update")]
        public abstract void Update(byte[] nef, string manifest, UInt160 admin);

        [DisplayName("doWork")]
        public abstract bool? DoWork();

        [DisplayName("doWhilePaused")]
        public abstract bool? DoWhilePaused();
    }
}
