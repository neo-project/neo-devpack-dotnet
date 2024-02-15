using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.TestingStandards;

public class OwnableTests<T> where T : SmartContract, IOwnable
{
    /// <summary>
    /// Required coverage to be success
    /// </summary>
    public static float RequiredCoverage { get; set; } = 0.95F;

    public static CoveredContract? Coverage { get; private set; }
    public static Signer Alice { get; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; } = TestEngine.GetNewSigner();

    public byte[] NefFile { get; }
    public string Manifest { get; }
    public TestEngine Engine { get; }
    public T Ownable { get; }

    #region Transfer event checks

    private List<(UInt160? from, UInt160? to)> raisedOnChangeOwner = new();

    #endregion

    /// <summary>
    /// Initialize Test
    /// </summary>
    public OwnableTests(string nefFile, string manifestFile)
    {
        NefFile = File.ReadAllBytes(nefFile);
        Manifest = File.ReadAllText(manifestFile);

        Engine = new TestEngine(true);
        Engine.SetTransactionSigners(Alice);
        Ownable = Engine.Deploy<T>(NefFile, Manifest, null);

        if (Coverage is null)
        {
            Coverage = Ownable.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }

        Ownable.OnSetOwner += onSetOwner;
    }

    void onSetOwner(UInt160? from, UInt160? to)
    {
        raisedOnChangeOwner.Add((from, to));
    }

    #region Asserts

    /// <summary>
    /// Assert that OnChangeOwner event was raised
    /// </summary>
    /// <param name="from">From</param>
    /// <param name="to">To</param>
    public void AssertOnChangeOwnerEvent(UInt160? from, UInt160? to)
    {
        Assert.AreEqual(1, raisedOnChangeOwner.Count);
        Assert.AreEqual(raisedOnChangeOwner[0].from, from);
        Assert.AreEqual(raisedOnChangeOwner[0].to, to);
        raisedOnChangeOwner.Clear();
    }

    /// <summary>
    /// Assert that Transfer event was NOT raised
    /// </summary>
    public void AssertNoOnChangeOwnerEvent()
    {
        Assert.AreEqual(0, raisedOnChangeOwner.Count);
    }

    #endregion

    [TestCleanup]
    public virtual void OnCleanup()
    {
        // Join the current coverage into the static one

        Coverage?.Join(Ownable.GetCoverage());
    }

    #region Tests

    [TestMethod]
    public void TestDefaultOwner()
    {
        var random = TestEngine.GetNewSigner();

        Engine.SetTransactionSigners(random);

        var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
        var check = Engine.FromHash<T>(expectedHash, false);
        check.OnSetOwner += onSetOwner;
        var ownable = Engine.Deploy<T>(NefFile, Manifest, null);
        check.OnSetOwner -= onSetOwner;

        AssertOnChangeOwnerEvent(null, random.Account);
        Assert.AreEqual(random.Account, ownable.Owner);
    }

    [TestMethod]
    public void TestSetGetOwner()
    {
        // Alice is the deployer

        Assert.AreEqual(Alice.Account, Ownable.Owner);
        Engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<VMUnhandledException>(() => Ownable.Owner = Bob.Account);

        Engine.SetTransactionSigners(Alice);
        Assert.ThrowsException<Exception>(() => Ownable.Owner = UInt160.Zero);
        Assert.ThrowsException<InvalidOperationException>(() => Ownable.Owner = InvalidUInt160.Null);
        Assert.ThrowsException<Exception>(() => Ownable.Owner = InvalidUInt160.Invalid);

        Ownable.Owner = Bob.Account;
        Assert.AreEqual(Bob.Account, Ownable.Owner);
        Assert.ThrowsException<VMUnhandledException>(() => Ownable.Owner = Bob.Account);

        Engine.SetTransactionSigners(Bob);

        Ownable.Owner = Alice.Account;
        Assert.AreEqual(Alice.Account, Ownable.Owner);
    }

    #endregion
}
