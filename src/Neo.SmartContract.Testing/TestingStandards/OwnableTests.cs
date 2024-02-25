using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IO;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.TestingStandards;

public class OwnableTests<T> : TestBase<T>
    where T : SmartContract, IOwnable
{
    #region Transfer event checks

    private List<(UInt160? from, UInt160? to)> raisedOnChangeOwner = new();

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest file</param>
    /// <param name="debugInfoFile">Debug info file</param>
    public OwnableTests(string nefFile, string manifestFile, string? debugInfoFile = null)
        : base(File.ReadAllBytes(nefFile).AsSerializable<NefFile>(),
            ContractManifest.Parse(File.ReadAllText(manifestFile)),
            !string.IsNullOrEmpty(debugInfoFile) && NeoDebugInfo.TryLoad(debugInfoFile, out var debugInfo) ? debugInfo : null)
    {
        Contract.OnSetOwner += onSetOwner;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nefFile">Nef file</param>
    /// <param name="manifestFile">Manifest</param>
    /// <param name="debugInfo">Debug info</param>
    public OwnableTests(NefFile nefFile, ContractManifest manifestFile, NeoDebugInfo? debugInfo = null)
        : base(nefFile, manifestFile, debugInfo)
    {
        Contract.OnSetOwner += onSetOwner;
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

    #region Tests

    [TestMethod]
    public virtual void TestVerify()
    {
        if (Contract is IVerificable verificable)
        {
            Engine.SetTransactionSigners(Alice);
            Assert.IsTrue(verificable.Verify);
            Engine.SetTransactionSigners(TestEngine.GetNewSigner());
            Assert.IsFalse(verificable.Verify);
        }
    }

    [TestMethod]
    public virtual void TestSenderAsDefaultOwner()
    {
        var random = TestEngine.GetNewSigner();

        Engine.SetTransactionSigners(random);

        var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
        var check = Engine.FromHash<T>(expectedHash, false);

        check.OnSetOwner += onSetOwner;
        var ownable = Engine.Deploy<T>(NefFile, Manifest, null);
        Assert.AreEqual(check.Hash, ownable.Hash);
        check.OnSetOwner -= onSetOwner;

        AssertOnChangeOwnerEvent(null, random.Account);
        Assert.AreEqual(random.Account, ownable.Owner);
    }

    [TestMethod]
    public virtual void TestSetGetOwner()
    {
        // Alice is the deployer

        Assert.AreEqual(Alice.Account, Contract.Owner);
        Engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner = Bob.Account);

        Engine.SetTransactionSigners(Alice);
        Assert.ThrowsException<Exception>(() => Contract.Owner = UInt160.Zero);
        Assert.ThrowsException<InvalidOperationException>(() => Contract.Owner = InvalidUInt160.Null);
        Assert.ThrowsException<Exception>(() => Contract.Owner = InvalidUInt160.InvalidLength);
        Assert.ThrowsException<Exception>(() => Contract.Owner = InvalidUInt160.InvalidType);

        Contract.Owner = Bob.Account;
        Assert.AreEqual(Bob.Account, Contract.Owner);
        Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner = Bob.Account);

        Engine.SetTransactionSigners(Bob);

        Contract.Owner = Alice.Account;
        Assert.AreEqual(Alice.Account, Contract.Owner);
    }

    #endregion
}
