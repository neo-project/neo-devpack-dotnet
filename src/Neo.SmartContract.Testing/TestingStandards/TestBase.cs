using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Coverage;
using System.IO;

namespace Neo.SmartContract.Testing.TestingStandards;

public class TestBase<T> where T : SmartContract
{
    public static CoveredContract? Coverage { get; private set; }
    public static Signer Alice { get; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; } = TestEngine.GetNewSigner();

    public byte[] NefFile { get; }
    public string Manifest { get; }
    public TestEngine Engine { get; }
    public T Contract { get; }
    public UInt160 ContractHash => Contract.Hash;

    /// <summary>
    /// Initialize Test
    /// </summary>
    public TestBase(string nefFile, string manifestFile)
    {
        NefFile = File.ReadAllBytes(nefFile);
        Manifest = File.ReadAllText(manifestFile);

        Engine = new TestEngine(true);
        Engine.SetTransactionSigners(Alice);
        Contract = Engine.Deploy<T>(NefFile, Manifest, null);

        if (Coverage is null)
        {
            Coverage = Contract.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }
    }

    [TestCleanup]
    public virtual void OnCleanup()
    {
        // Join the current coverage into the static one

        Coverage?.Join(Contract.GetCoverage());
    }
}
