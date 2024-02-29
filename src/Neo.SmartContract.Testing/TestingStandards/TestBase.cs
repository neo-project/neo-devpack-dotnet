using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.TestingStandards;

public class TestBase<T> where T : SmartContract
{
    private readonly List<string> _contractLogs = new();

    public static CoveredContract? Coverage { get; private set; }
    public static Signer Alice { get; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; } = TestEngine.GetNewSigner();

    public NefFile NefFile { get; }
    public ContractManifest Manifest { get; }
    public NeoDebugInfo? DebugInfo { get; }
    public TestEngine Engine { get; }
    public T Contract { get; }
    public UInt160 ContractHash => Contract.Hash;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest file</param>
    /// <param name="debugInfoFile">Debug info file</param>
    public TestBase(string nefFile, string manifestFile, string? debugInfoFile = null) :
        this(File.ReadAllBytes(nefFile).AsSerializable<NefFile>(),
            ContractManifest.Parse(File.ReadAllText(manifestFile)),
            !string.IsNullOrEmpty(debugInfoFile) && NeoDebugInfo.TryLoad(debugInfoFile, out var debugInfo) ? debugInfo : null)
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest</param>
    /// <param name="debugInfo">Debug info</param>
    public TestBase(NefFile nefFile, ContractManifest manifestFile, NeoDebugInfo? debugInfo = null)
    {
        NefFile = nefFile;
        Manifest = manifestFile;
        DebugInfo = debugInfo;

        Engine = new TestEngine(true);
        Engine.SetTransactionSigners(Alice);
        Contract = Engine.Deploy<T>(NefFile, Manifest, null);

        if (Coverage is null)
        {
            Coverage = Contract.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }

        Contract.OnRuntimeLog += Contract_OnRuntimeLog;
    }

    private void Contract_OnRuntimeLog(string message)
    {
        _contractLogs.Add(message);
    }

    #region Asserts

    /// <summary>
    /// Assert that Log event was raised
    /// </summary>
    /// <param name="logs">Logs</param>
    public void AssertLogs(params string[] logs)
    {
        Assert.AreEqual(logs.Length, _contractLogs.Count);
        CollectionAssert.AreEqual(_contractLogs, logs);
        _contractLogs.Clear();
    }

    /// <summary>
    /// Assert that Transfer event was NOT raised
    /// </summary>
    public void AssertNoLogs()
    {
        Assert.AreEqual(0, _contractLogs.Count);
    }

    #endregion

    [TestCleanup]
    public virtual void OnCleanup()
    {
        // Join the current coverage into the static one

        Contract.OnRuntimeLog -= Contract_OnRuntimeLog;
        Coverage?.Join(Contract.GetCoverage());
    }
}
