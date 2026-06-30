// Copyright (C) 2015-2026 The Neo Project.
//
// Ownable2StepTest.cs file belongs to the neo project and is free
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
public class Ownable2StepTest
{
    private static readonly Signer Alice = TestEngine.Alice;
    private static readonly Signer Bob = TestEngine.Bob;
    private static readonly Signer Charlie = TestEngine.Charlie;

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) _compiled;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) => _compiled = CompileContract();

    [TestMethod]
    public void Deploy_InitializesOwnerToSender_NoPending()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        Assert.AreEqual(Alice.Account, contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void Deploy_WithExplicitOwner_InitializesOwnerFromData()
    {
        var engine = CreateEngine();
        var contract = engine.Deploy<OwnableTwoStepProxy>(_compiled.nef, _compiled.manifest, Bob.Account);

        Assert.AreEqual(Bob.Account, contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void HappyPath_TwoStepTransfer()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        // Step 1: owner proposes Bob — owner unchanged, pending == Bob.
        contract.TransferOwnership(Bob.Account);
        Assert.AreEqual(Alice.Account, contract.GetOwner());
        Assert.AreEqual(Bob.Account, contract.GetPendingOwner());

        // Step 2: Bob accepts — Bob is the owner, pending cleared.
        engine.SetTransactionSigners(Bob);
        contract.AcceptOwnership();
        Assert.AreEqual(Bob.Account, contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void TransferOwnership_ByNonOwner_Throws()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(Bob.Account));
        Merge(contract);
    }

    [TestMethod]
    public void TransferOwnership_ToSelf_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(Alice.Account));
        Merge(contract);
    }

    [TestMethod]
    public void TransferOwnership_ToZero_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(UInt160.Zero));
        Merge(contract);
    }

    [TestMethod]
    public void AcceptOwnership_NoPending_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void AcceptOwnership_InvalidStoredPending_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.SetPendingOwnerForTest(UInt160.Zero);

        Assert.AreEqual(UInt160.Zero, contract.GetPendingOwner());
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void AcceptOwnership_ByWrongAccount_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);

        // Current owner cannot accept on the pending owner's behalf.
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());

        // A third party cannot accept either.
        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());

        // Pending is still Bob; the offer survives the failed attempts.
        Assert.AreEqual(Bob.Account, contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void AcceptOwnership_Twice_SecondAborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        engine.SetTransactionSigners(Bob);
        contract.AcceptOwnership();

        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void Renounce_ThenStaleAccept_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        contract.RenounceOwnership();

        Assert.IsNull(contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());

        // Bob can no longer seize the abandoned contract.
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void CancelOwnershipTransfer_ClearsPending_OwnerKept()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        contract.CancelOwnershipTransfer();

        Assert.AreEqual(Alice.Account, contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());

        // The formerly-pending account can no longer accept.
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void CancelOwnershipTransfer_NothingPending_Aborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        Assert.ThrowsException<TestException>(() => contract.CancelOwnershipTransfer());
        Merge(contract);
    }

    [TestMethod]
    public void CancelOwnershipTransfer_ByNonOwner_Throws()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        engine.SetTransactionSigners(Charlie);
        Assert.ThrowsException<TestException>(() => contract.CancelOwnershipTransfer());
        Merge(contract);
    }

    [TestMethod]
    public void RenounceOwnership_RemovesOwner_AndLocksOut()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.RenounceOwnership();
        Assert.IsNull(contract.GetOwner());

        // Every owner-gated method is now permanently uncallable.
        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(Bob.Account));
        Assert.ThrowsException<TestException>(() => contract.CancelOwnershipTransfer());
        Assert.ThrowsException<TestException>(() => contract.RenounceOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void RenounceOwnership_ByNonOwner_Throws()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.RenounceOwnership());
        Merge(contract);
    }

    [TestMethod]
    public void OwnerGatedMethods_WithNoStoredOwner_Throw()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.ClearOwnerForTest();

        Assert.IsNull(contract.GetOwner());
        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(Bob.Account));
        Merge(contract);
    }

    [TestMethod]
    public void InitializeOwner_UpdateIsNoOp_AndInvalidOwnerAborts()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        contract.InitializeForTest(Charlie.Account, true);

        Assert.AreEqual(Alice.Account, contract.GetOwner());
        Assert.AreEqual(Bob.Account, contract.GetPendingOwner());

        Assert.ThrowsException<TestException>(() => contract.InitializeForTest(UInt160.Zero, false));
        Assert.AreEqual(Alice.Account, contract.GetOwner());
        Assert.AreEqual(Bob.Account, contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void InitializeOwner_UpdateInitializesWhenOwnerMissing()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.ClearOwnershipStateForTest();
        Assert.IsNull(contract.GetOwner());

        contract.InitializeForTest(Bob.Account, true);

        Assert.AreEqual(Bob.Account, contract.GetOwner());
        Assert.IsNull(contract.GetPendingOwner());
        Merge(contract);
    }

    [TestMethod]
    public void InitializeOwner_UpdateDoesNotReinitializeAfterRenounce()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.RenounceOwnership();
        contract.InitializeForTest(Bob.Account, true);

        Assert.IsNull(contract.GetOwner());
        Assert.ThrowsException<TestException>(() => contract.TransferOwnership(Charlie.Account));
        Merge(contract);
    }

    [TestMethod]
    public void RePropose_SupersedesPending()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        contract.TransferOwnership(Bob.Account);
        contract.TransferOwnership(Charlie.Account);

        Assert.AreEqual(Charlie.Account, contract.GetPendingOwner());

        // Bob's offer was superseded; he can no longer accept.
        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.AcceptOwnership());

        // Charlie can.
        engine.SetTransactionSigners(Charlie);
        contract.AcceptOwnership();
        Assert.AreEqual(Charlie.Account, contract.GetOwner());
        Merge(contract);
    }

    [TestMethod]
    public void Events_StartedAcceptedCanceledRenounced_AreRaised()
    {
        var engine = CreateEngine();
        var contract = Deploy(engine, out _, out _);

        UInt160? startedPrev = null, startedNew = null;
        UInt160? transferredPrev = null, transferredNew = null;
        UInt160? canceledOwner = null, canceledPending = null;
        bool transferredRaised = false;
        contract.OnOwnershipTransferStarted += (p, n) => { startedPrev = p; startedNew = n; };
        contract.OnOwnershipTransferred += (p, n) => { transferredPrev = p; transferredNew = n; transferredRaised = true; };
        contract.OnOwnershipTransferCanceled += (o, c) => { canceledOwner = o; canceledPending = c; };

        // Propose -> Started(Alice, Bob)
        contract.TransferOwnership(Bob.Account);
        Assert.AreEqual(Alice.Account, startedPrev);
        Assert.AreEqual(Bob.Account, startedNew);

        // Re-propose -> Canceled(Alice, Bob) then Started(Alice, Charlie)
        contract.TransferOwnership(Charlie.Account);
        Assert.AreEqual(Alice.Account, canceledOwner);
        Assert.AreEqual(Bob.Account, canceledPending);
        Assert.AreEqual(Charlie.Account, startedNew);

        // Accept -> Transferred(Alice, Charlie)
        engine.SetTransactionSigners(Charlie);
        contract.AcceptOwnership();
        Assert.IsTrue(transferredRaised);
        Assert.AreEqual(Alice.Account, transferredPrev);
        Assert.AreEqual(Charlie.Account, transferredNew);

        // Renounce -> Transferred(Charlie, null)
        transferredRaised = false;
        contract.RenounceOwnership();
        Assert.IsTrue(transferredRaised);
        Assert.AreEqual(Charlie.Account, transferredPrev);
        Assert.IsNull(transferredNew);
        Merge(contract);
    }

    [TestMethod]
    public void Manifest_SafeFlags_GettersSafe_MutatorsUnsafe()
    {
        var (_, manifest, _) = _compiled;
        var abi = manifest.Abi.Methods;

        foreach (var safe in new[] { "getOwner", "getPendingOwner" })
            Assert.IsTrue(abi.Single(m => m.Name == safe).Safe, $"{safe} must be safe");

        foreach (var mutator in new[] { "transferOwnership", "acceptOwnership", "cancelOwnershipTransfer", "renounceOwnership" })
            Assert.IsFalse(abi.Single(m => m.Name == mutator).Safe, $"{mutator} must not be safe");
    }

    private static OwnableTwoStepProxy Deploy(TestEngine engine, out NefFile nef, out ContractManifest manifest)
    {
        nef = _compiled.nef;
        manifest = _compiled.manifest;
        return engine.Deploy<OwnableTwoStepProxy>(nef, manifest, null);
    }

    private static void Merge(OwnableTwoStepProxy contract)
        => DynamicCoverageMergeHelper.Merge(contract, _compiled.debugInfo);

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

public class Contract : Ownable2Step
{
    public static void _deploy(object data, bool update)
    {
        InitializeOwner(data, update);
    }

    public static void InitializeForTest(object data, bool update)
    {
        InitializeOwner(data, update);
    }

    public static void SetPendingOwnerForTest(UInt160 pendingOwner)
    {
        Storage.Put(new byte[] { 0xFC }, pendingOwner);
    }

    public static void ClearOwnerForTest()
    {
        Storage.Delete(new byte[] { 0xFD });
    }

    public static void ClearOwnershipStateForTest()
    {
        Storage.Delete(new byte[] { 0xFD });
        Storage.Delete(new byte[] { 0xFB });
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

    public abstract class OwnableTwoStepProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public delegate void OwnershipTransferStartedDelegate(UInt160? previousOwner, UInt160? newOwner);
        public delegate void OwnershipTransferredDelegate(UInt160? previousOwner, UInt160? newOwner);
        public delegate void OwnershipTransferCanceledDelegate(UInt160? currentOwner, UInt160? canceledPendingOwner);

        [DisplayName("OwnershipTransferStarted")]
        public event OwnershipTransferStartedDelegate? OnOwnershipTransferStarted;

        [DisplayName("OwnershipTransferred")]
        public event OwnershipTransferredDelegate? OnOwnershipTransferred;

        [DisplayName("OwnershipTransferCanceled")]
        public event OwnershipTransferCanceledDelegate? OnOwnershipTransferCanceled;

        [DisplayName("getOwner")]
        public abstract UInt160? GetOwner();

        [DisplayName("getPendingOwner")]
        public abstract UInt160? GetPendingOwner();

        [DisplayName("initializeForTest")]
        public abstract void InitializeForTest(object data, bool update);

        [DisplayName("setPendingOwnerForTest")]
        public abstract void SetPendingOwnerForTest(UInt160 pendingOwner);

        [DisplayName("clearOwnerForTest")]
        public abstract void ClearOwnerForTest();

        [DisplayName("clearOwnershipStateForTest")]
        public abstract void ClearOwnershipStateForTest();

        [DisplayName("transferOwnership")]
        public abstract void TransferOwnership(UInt160 newOwner);

        [DisplayName("acceptOwnership")]
        public abstract void AcceptOwnership();

        [DisplayName("cancelOwnershipTransfer")]
        public abstract void CancelOwnershipTransfer();

        [DisplayName("renounceOwnership")]
        public abstract void RenounceOwnership();
    }
}
