// Copyright (C) 2015-2026 The Neo Project.
//
// AccessControlTest.cs file belongs to the neo project and is free
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
using System.Numerics;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class AccessControlTest
{
    private static readonly Signer Alice = TestEngine.Alice;
    private static readonly Signer Bob = TestEngine.Bob;
    private static readonly Signer Charlie = TestEngine.Charlie;

    private static readonly BigInteger DefaultAdmin = BigInteger.Zero;
    private static readonly BigInteger Minter = BigInteger.One;
    private static readonly BigInteger MinterAdmin = new(2);

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    private static AccessControlProxy Deploy(TestEngine engine, object? admin = null)
        => engine.Deploy<AccessControlProxy>(_compiled.nef, _compiled.manifest, admin);

    private static void Merge(AccessControlProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    // ---- Bootstrap / init ----

    [TestMethod]
    public void Init_GrantsDefaultAdminToSender()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.AreEqual(BigInteger.One, c.GetRoleMemberCount(DefaultAdmin));
        Merge(c);
    }

    [TestMethod]
    public void Init_DefaultAdminRole_IsZero()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.AreEqual(BigInteger.Zero, c.DEFAULT_ADMIN_ROLE());
        Merge(c);
    }

    [TestMethod]
    public void Init_Replay_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        // A second initialization must be rejected so an attacker cannot add a co-admin.
        Assert.ThrowsException<TestException>(() => c.ReInit(Bob.Account));
        Assert.IsFalse(c.HasRole(DefaultAdmin, Bob.Account));
        Assert.AreEqual(BigInteger.One, c.GetRoleMemberCount(DefaultAdmin));
        Merge(c);
    }

    [TestMethod]
    public void Init_ZeroAdmin_Aborts()
    {
        var engine = CreateEngine();
        Assert.ThrowsException<TestException>(() => Deploy(engine, UInt160.Zero));
    }

    // ---- Reads ----

    [TestMethod]
    public void HasRole_FalseBeforeGrant_TrueAfter()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.IsFalse(c.HasRole(Minter, Bob.Account));
        c.GrantRole(Minter, Alice.Account, Bob.Account);
        Assert.IsTrue(c.HasRole(Minter, Bob.Account));
        Merge(c);
    }

    [TestMethod]
    public void HasRole_NegativeRole_Faults()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.ThrowsException<TestException>(() => c.HasRole(new BigInteger(-1), Alice.Account));
        Merge(c);
    }

    [TestMethod]
    public void GetRoleAdmin_DefaultsToDefaultAdminRole()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.AreEqual(DefaultAdmin, c.GetRoleAdmin(Minter));
        Merge(c);
    }

    // ---- Grant ----

    [TestMethod]
    public void GrantRole_ByAdmin_Succeeds()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        BigInteger? grantedRole = null;
        UInt160? grantedAccount = null, grantedSender = null;
        c.OnRoleGranted += (r, a, s) => { grantedRole = r; grantedAccount = a; grantedSender = s; };

        c.GrantRole(Minter, Alice.Account, Bob.Account);

        Assert.IsTrue(c.HasRole(Minter, Bob.Account));
        Assert.AreEqual(BigInteger.One, c.GetRoleMemberCount(Minter));
        Assert.AreEqual(Minter, grantedRole);
        Assert.AreEqual(Bob.Account, grantedAccount);
        Assert.AreEqual(Alice.Account, grantedSender);
        Merge(c);
    }

    [TestMethod]
    public void GrantRole_ByNonAdmin_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.GrantRole(Minter, Bob.Account, Charlie.Account));
        Assert.IsFalse(c.HasRole(Minter, Charlie.Account));
        Merge(c);
    }

    [TestMethod]
    public void GrantRole_AdminWithoutWitness_Aborts()
    {
        // Confused-deputy: Alice holds the admin role, but the call is signed by Bob, so
        // CheckWitness(Alice) is false. Mere admin membership without a witness must not pass.
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.GrantRole(Minter, Alice.Account, Charlie.Account));
        Assert.IsFalse(c.HasRole(Minter, Charlie.Account));
        Merge(c);
    }

    [TestMethod]
    public void GrantRole_Idempotent_NoSecondEvent()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        int events = 0;
        c.OnRoleGranted += (_, _, _) => events++;
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        Assert.AreEqual(0, events);
        Assert.AreEqual(BigInteger.One, c.GetRoleMemberCount(Minter));
        Merge(c);
    }

    [TestMethod]
    public void GrantRole_ZeroAccount_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.ThrowsException<TestException>(() => c.GrantRole(Minter, Alice.Account, UInt160.Zero));
        Merge(c);
    }

    // ---- Revoke / renounce ----

    [TestMethod]
    public void RevokeRole_ByAdmin_Succeeds()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        c.RevokeRole(Minter, Alice.Account, Bob.Account);
        Assert.IsFalse(c.HasRole(Minter, Bob.Account));
        Assert.AreEqual(BigInteger.Zero, c.GetRoleMemberCount(Minter));
        Merge(c);
    }

    [TestMethod]
    public void RevokeRole_Idempotent_NoEvent()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        int events = 0;
        c.OnRoleRevoked += (_, _, _) => events++;
        c.RevokeRole(Minter, Alice.Account, Bob.Account); // never granted
        Assert.AreEqual(0, events);
        Merge(c);
    }

    [TestMethod]
    public void RenounceRole_Self_Succeeds()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        engine.SetTransactionSigners(Bob);
        c.RenounceRole(Minter, Bob.Account);
        Assert.IsFalse(c.HasRole(Minter, Bob.Account));
        Merge(c);
    }

    [TestMethod]
    public void RenounceRole_Other_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        // Charlie tries to renounce Bob's role.
        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsException<TestException>(() => c.RenounceRole(Minter, Bob.Account));
        Assert.IsTrue(c.HasRole(Minter, Bob.Account));
        Merge(c);
    }

    [TestMethod]
    public void RenounceRole_NotHeld_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.RenounceRole(Minter, Bob.Account));
        Assert.IsFalse(c.HasRole(Minter, Bob.Account));
        Merge(c);
    }

    // ---- Last-admin guard ----

    [TestMethod]
    public void RevokeRole_LastDefaultAdmin_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.ThrowsException<TestException>(() => c.RevokeRole(DefaultAdmin, Alice.Account, Alice.Account));
        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Merge(c);
    }

    [TestMethod]
    public void RenounceRole_LastDefaultAdmin_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        Assert.ThrowsException<TestException>(() => c.RenounceRole(DefaultAdmin, Alice.Account));
        Assert.IsTrue(c.HasRole(DefaultAdmin, Alice.Account));
        Merge(c);
    }

    [TestMethod]
    public void DefaultAdmin_SafeHandoff()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        // Grant to a successor first (count -> 2), then the original can renounce (count -> 1).
        c.GrantRole(DefaultAdmin, Alice.Account, Bob.Account);
        Assert.AreEqual(new BigInteger(2), c.GetRoleMemberCount(DefaultAdmin));

        c.RenounceRole(DefaultAdmin, Alice.Account);
        Assert.IsFalse(c.HasRole(DefaultAdmin, Alice.Account));
        Assert.AreEqual(BigInteger.One, c.GetRoleMemberCount(DefaultAdmin));

        // Administration still works through Bob.
        engine.SetTransactionSigners(Bob);
        c.GrantRole(Minter, Bob.Account, Charlie.Account);
        Assert.IsTrue(c.HasRole(Minter, Charlie.Account));
        Merge(c);
    }

    [TestMethod]
    public void LastAdminGuard_DoesNotRestrictOtherRoles()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        // The sole minter can be revoked freely.
        c.RevokeRole(Minter, Alice.Account, Bob.Account);
        Assert.IsFalse(c.HasRole(Minter, Bob.Account));
        Merge(c);
    }

    // ---- Admin hierarchy ----

    [TestMethod]
    public void SetRoleAdmin_ChangesAuthority()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        BigInteger? evRole = null, evPrev = null, evNew = null;
        c.OnRoleAdminChanged += (r, p, n) => { evRole = r; evPrev = p; evNew = n; };

        // Make MinterAdmin the admin of Minter, and give it to Bob.
        c.SetRoleAdmin(Minter, MinterAdmin, Alice.Account);
        Assert.AreEqual(Minter, evRole);
        Assert.AreEqual(DefaultAdmin, evPrev);
        Assert.AreEqual(MinterAdmin, evNew);
        Assert.AreEqual(MinterAdmin, c.GetRoleAdmin(Minter));

        c.GrantRole(MinterAdmin, Alice.Account, Bob.Account);

        // Default admin (Alice) can no longer grant Minter; only MinterAdmin holders can.
        Assert.ThrowsException<TestException>(() => c.GrantRole(Minter, Alice.Account, Charlie.Account));

        engine.SetTransactionSigners(Bob);
        c.GrantRole(Minter, Bob.Account, Charlie.Account);
        Assert.IsTrue(c.HasRole(Minter, Charlie.Account));
        Merge(c);
    }

    [TestMethod]
    public void SetRoleAdmin_ByNonAdmin_Aborts()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => c.SetRoleAdmin(Minter, MinterAdmin, Bob.Account));
        Merge(c);
    }

    // ---- OnlyRole guard ----

    [TestMethod]
    public void OnlyRole_GuardedAction()
    {
        var engine = CreateEngine();
        var c = Deploy(engine);
        c.GrantRole(Minter, Alice.Account, Bob.Account);

        // Holder with witness passes.
        engine.SetTransactionSigners(Bob);
        Assert.IsTrue(c.GuardedAction(Minter, Bob.Account)!.Value);

        // Non-holder fails.
        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsException<TestException>(() => c.GuardedAction(Minter, Charlie.Account));
        Merge(c);
    }

    // ---- Manifest / ABI surface ----

    [TestMethod]
    public void Manifest_SafeFlags_And_ProtectedNotExported()
    {
        var abi = _compiled.manifest.Abi.Methods;
        var names = abi.Select(m => m.Name).ToArray();

        foreach (var safe in new[] { "dEFAULT_ADMIN_ROLE", "hasRole", "getRoleAdmin", "getRoleMemberCount" })
            Assert.IsTrue(abi.Single(m => m.Name == safe).Safe, $"{safe} must be safe");

        foreach (var mutator in new[] { "grantRole", "revokeRole", "renounceRole" })
            Assert.IsFalse(abi.Single(m => m.Name == mutator).Safe, $"{mutator} must not be safe");

        // Protected cores and private helpers must not be exported.
        foreach (var hidden in new[] { "roleBytes", "checkRole", "onlyRole", "GrantRoleInternal", "RevokeRoleInternal", "SetRoleAdminInternal",
            "initializeAccessControl", "validateRole", "guardLastAdmin", "memberKey", "adminKey", "countKey" })
            Assert.IsFalse(names.Contains(hidden), $"{hidden} must not be exported to the ABI");
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using System.Numerics;

public class Contract : AccessControl
{
    private enum TestRole
    {
        Minter = 1,
        MinterAdmin = 2
    }

    public static void _deploy(object data, bool update)
    {
        InitializeAccessControl((UInt160?)data, update);
    }

    public static bool GuardedAction(BigInteger role, UInt160 actor)
    {
        OnlyRole(role, actor);
        return true;
    }

    public static void SetRoleAdmin(BigInteger role, BigInteger adminRole, UInt160 admin)
    {
        OnlyRole(GetRoleAdmin(role), admin);
        AccessControl.SetRoleAdminInternal(role, adminRole);
    }

    public static void ReInit(UInt160 admin)
    {
        InitializeAccessControl(admin, false);
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

    public abstract class AccessControlProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public delegate void RoleGrantedDelegate(BigInteger? role, UInt160? account, UInt160? sender);
        public delegate void RoleRevokedDelegate(BigInteger? role, UInt160? account, UInt160? sender);
        public delegate void RoleAdminChangedDelegate(BigInteger? role, BigInteger? previousAdminRole, BigInteger? newAdminRole);

        [DisplayName("RoleGranted")]
        public event RoleGrantedDelegate? OnRoleGranted;

        [DisplayName("RoleRevoked")]
        public event RoleRevokedDelegate? OnRoleRevoked;

        [DisplayName("RoleAdminChanged")]
        public event RoleAdminChangedDelegate? OnRoleAdminChanged;

        [DisplayName("dEFAULT_ADMIN_ROLE")]
        public abstract BigInteger DEFAULT_ADMIN_ROLE();

        [DisplayName("hasRole")]
        public abstract bool HasRole(BigInteger role, UInt160 account);

        [DisplayName("getRoleAdmin")]
        public abstract BigInteger GetRoleAdmin(BigInteger role);

        [DisplayName("getRoleMemberCount")]
        public abstract BigInteger GetRoleMemberCount(BigInteger role);

        [DisplayName("grantRole")]
        public abstract void GrantRole(BigInteger role, UInt160 admin, UInt160 account);

        [DisplayName("revokeRole")]
        public abstract void RevokeRole(BigInteger role, UInt160 admin, UInt160 account);

        [DisplayName("renounceRole")]
        public abstract void RenounceRole(BigInteger role, UInt160 account);

        [DisplayName("guardedAction")]
        public abstract bool? GuardedAction(BigInteger role, UInt160 actor);

        [DisplayName("setRoleAdmin")]
        public abstract void SetRoleAdmin(BigInteger role, BigInteger adminRole, UInt160 admin);

        [DisplayName("reInit")]
        public abstract void ReInit(UInt160 admin);
    }
}
