using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.TestingStandards;

public class TestBase<T> where T : SmartContract, IContractInfo
{
    private readonly List<string> _contractLogs = [];

    public static CoveredContract? Coverage { get; private set; }
    public static Signer Alice { get; set; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; set; } = TestEngine.GetNewSigner();

    public NefFile NefFile { get; private set; } = default!;
    public ContractManifest Manifest { get; private set; } = default!;
    public NeoDebugInfo? DebugInfo { get; set; }
    public TestEngine Engine { get; private set; } = default!;
    public T Contract { get; private set; } = default!;
    public UInt160 ContractHash => Contract.Hash;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public TestBase() => TestBaseSetup(T.Nef, T.Manifest);

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
        TestBaseSetup(nefFile, manifestFile, debugInfo);
    }

    /// <summary>
    /// Setup the test contract
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest</param>
    /// <param name="debugInfo">Debug info</param>
    public virtual void TestBaseSetup(NefFile nefFile, ContractManifest manifestFile, NeoDebugInfo? debugInfo = null)
    {
        NefFile = nefFile;
        Manifest = manifestFile;
        DebugInfo = debugInfo;

        Engine = CreateTestEngine();
        Contract = Engine.Deploy<T>(NefFile, Manifest, null);

        if (Coverage is null)
        {
            Coverage = Contract.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }

        Contract.OnRuntimeLog += Contract_OnRuntimeLog;
    }

    /// <summary>
    /// Configure the initial testEngine
    /// </summary>
    /// <returns>TestEngine</returns>
    protected virtual TestEngine CreateTestEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    private void Contract_OnRuntimeLog(UInt160 sender, string message)
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
