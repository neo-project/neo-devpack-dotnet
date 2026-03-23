extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using CompilationOptions = Neo.Compiler.CompilationOptions;
using ModifierAttribute = scfx.Neo.SmartContract.Framework.Attributes.ModifierAttribute;
using WhenNotPausedAttribute = scfx.Neo.SmartContract.Framework.Attributes.WhenNotPausedAttribute;
using WhenPausedAttribute = scfx.Neo.SmartContract.Framework.Attributes.WhenPausedAttribute;

namespace Neo.SmartContract.Framework.UnitTests;

[TestClass]
public class PausableTest
{
    private static readonly Signer Sender = TestEngine.GetNewSigner();

    [TestMethod]
    public void Pausable_WhenNotPaused_AllowsCalls_And_PauseBlocks()
    {
        var (nef, manifest, debugInfo) = CompilePausableContract();
        var engine = CreateEngine();
        var contract = engine.Deploy<PausableContractProxy>(nef, manifest);

        Assert.IsFalse(contract.Paused!.Value);
        Assert.IsTrue(contract.ProtectedAction()!.Value);
        Assert.ThrowsException<TestException>(() => contract.PausedAction());

        contract.Pause();
        Assert.IsTrue(contract.Paused!.Value);
        Assert.ThrowsException<TestException>(() => contract.ProtectedAction());
        Assert.IsTrue(contract.PausedAction()!.Value);

        contract.Unpause();
        Assert.IsFalse(contract.Paused!.Value);
        Assert.IsTrue(contract.ProtectedAction()!.Value);
        Assert.ThrowsException<TestException>(() => contract.PausedAction());

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void Pausable_Rejects_DoublePause_And_DoubleUnpause()
    {
        var (nef, manifest, debugInfo) = CompilePausableContract();
        var engine = CreateEngine();
        var contract = engine.Deploy<PausableContractProxy>(nef, manifest);

        Assert.ThrowsException<TestException>(() => contract.Unpause());

        contract.Pause();
        Assert.ThrowsException<TestException>(() => contract.Pause());

        DynamicCoverageMergeHelper.Merge(contract, debugInfo);
    }

    [TestMethod]
    public void Pausable_ModifierAttributes_ExposeExpectedMetadata_And_ExitIsNoOp()
    {
        var whenPaused = new WhenPausedAttribute();
        var whenNotPaused = new WhenNotPausedAttribute();
        var whenPausedCustom = new WhenPausedAttribute(0x7A);
        var whenNotPausedCustom = new WhenNotPausedAttribute(0x7A);

        whenPaused.Exit();
        whenNotPaused.Exit();
        whenPausedCustom.Exit();
        whenNotPausedCustom.Exit();

        var whenPausedUsage = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(typeof(WhenPausedAttribute), typeof(AttributeUsageAttribute));
        Assert.IsNotNull(whenPausedUsage);
        Assert.AreEqual(AttributeTargets.Constructor | AttributeTargets.Method, whenPausedUsage.ValidOn);
        Assert.IsFalse(whenPausedUsage.AllowMultiple);

        var whenNotPausedUsage = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(typeof(WhenNotPausedAttribute), typeof(AttributeUsageAttribute));
        Assert.IsNotNull(whenNotPausedUsage);
        Assert.AreEqual(AttributeTargets.Constructor | AttributeTargets.Method, whenNotPausedUsage.ValidOn);
        Assert.IsFalse(whenNotPausedUsage.AllowMultiple);

        Assert.IsTrue(typeof(ModifierAttribute).IsAssignableFrom(typeof(WhenPausedAttribute)));
        Assert.IsTrue(typeof(ModifierAttribute).IsAssignableFrom(typeof(WhenNotPausedAttribute)));
    }

    private static (NefFile nef, ContractManifest manifest, NeoDebugInfo debugInfo) CompilePausableContract()
    {
        const string source = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

public class Contract : Pausable
{
    public static void Pause()
    {
        Pausable.Pause();
    }

    public static void Unpause()
    {
        Pausable.Unpause();
    }

    [WhenNotPaused]
    public static bool ProtectedAction()
    {
        return true;
    }

    [WhenPaused]
    public static bool PausedAction()
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
        engine.SetTransactionSigners(Sender);
        return engine;
    }

    public abstract class PausableContractProxy(SmartContractInitialize initialize)
        : Neo.SmartContract.Testing.SmartContract(initialize)
    {
        public abstract bool? Paused { [DisplayName("paused")] get; }

        [DisplayName("pause")]
        public abstract void Pause();

        [DisplayName("unpause")]
        public abstract void Unpause();

        [DisplayName("protectedAction")]
        public abstract bool? ProtectedAction();

        [DisplayName("pausedAction")]
        public abstract bool? PausedAction();
    }
}
