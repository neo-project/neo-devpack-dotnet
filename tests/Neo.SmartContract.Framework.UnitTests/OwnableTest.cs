using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Extensions;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class OwnableTest
{
    private static readonly Signer Alice = TestEngine.GetNewSigner();
    private static readonly Signer Bob = TestEngine.GetNewSigner();

    [TestMethod]
    public void Ownable_UsesSenderAsDefaultOwner_And_OnlyOwnerGuardsProtectedMethods()
    {
        var (nef, manifest, debugInfo) = CompileOwnableContract();
        var engine = CreateEngine();

        UInt160? previousOwnerRaised = null;
        UInt160? newOwnerRaised = null;

        var expectedHash = engine.GetDeployHash(nef, manifest);
        var preview = engine.FromHash<OwnableContractProxy>(expectedHash, false);
        preview.OnSetOwner += (previous, current) =>
        {
            previousOwnerRaised = previous;
            newOwnerRaised = current;
        };

        var contract = engine.Deploy<OwnableContractProxy>(nef, manifest);

        Assert.AreEqual(preview.Hash, contract.Hash);
        Assert.IsNull(previousOwnerRaised);
        Assert.AreEqual(Alice.Account, newOwnerRaised);
        Assert.AreEqual(Alice.Account, contract.Owner);

        Assert.IsTrue(contract.ProtectedAction()!.Value);

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.ProtectedAction());

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void Ownable_SetOwner_TransfersOwnership_And_RaisesEvent()
    {
        var (nef, manifest, debugInfo) = CompileOwnableContract();
        var engine = CreateEngine();
        var contract = engine.Deploy<OwnableContractProxy>(nef, manifest);

        UInt160? previousOwnerRaised = null;
        UInt160? newOwnerRaised = null;
        contract.OnSetOwner += (previous, current) =>
        {
            previousOwnerRaised = previous;
            newOwnerRaised = current;
        };

        engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<TestException>(() => contract.Owner = Bob.Account);

        engine.SetTransactionSigners(Alice);
        contract.Owner = Bob.Account;

        Assert.AreEqual(Alice.Account, previousOwnerRaised);
        Assert.AreEqual(Bob.Account, newOwnerRaised);
        Assert.AreEqual(Bob.Account, contract.Owner);

        Assert.ThrowsException<TestException>(() => contract.Owner = UInt160.Zero);

        engine.SetTransactionSigners(Bob);
        Assert.IsTrue(contract.ProtectedAction()!.Value);

        engine.SetTransactionSigners(Alice);
        Assert.ThrowsException<TestException>(() => contract.ProtectedAction());

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompileOwnableContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class Contract : Ownable
{
    public static void _deploy(object data, bool update)
    {
        InitializeOwner(data, update);
    }

    [OnlyOwner]
    public static bool ProtectedAction()
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

            var contexts = engine.CompileSources(new CompilationSourceReferences
            {
                Projects = new[] { frameworkProject }
            }, tempFile);

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

    private static TestEngine CreateEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    public abstract class OwnableContractProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize), IOwnable
    {
        [DisplayName("SetOwner")]
        public event IOwnable.delSetOwner? OnSetOwner;

        public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

        [DisplayName("protectedAction")]
        public abstract bool? ProtectedAction();
    }
}
