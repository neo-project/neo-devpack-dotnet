using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Testing.TestingStandards;

public class OwnableTests<T> : TestBase<T>
    where T : SmartContract, IOwnable
{
    #region Transfer event checks

    private List<(UInt160? from, UInt160? to)> raisedOnChangeOwner = new();

    #endregion

    /// <summary>
    /// Initialize Test
    /// </summary>
    public OwnableTests(string nefFile, string manifestFile) : base(nefFile, manifestFile)
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
    public void TestSenderAsDefaultOwner()
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
    public void TestSetGetOwner()
    {
        // Alice is the deployer

        Assert.AreEqual(Alice.Account, Contract.Owner);
        Engine.SetTransactionSigners(Bob);
        Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner = Bob.Account);

        Engine.SetTransactionSigners(Alice);
        Assert.ThrowsException<Exception>(() => Contract.Owner = UInt160.Zero);
        Assert.ThrowsException<InvalidOperationException>(() => Contract.Owner = InvalidUInt160.Null);
        Assert.ThrowsException<Exception>(() => Contract.Owner = InvalidUInt160.Invalid);

        Contract.Owner = Bob.Account;
        Assert.AreEqual(Bob.Account, Contract.Owner);
        Assert.ThrowsException<VMUnhandledException>(() => Contract.Owner = Bob.Account);

        Engine.SetTransactionSigners(Bob);

        Contract.Owner = Alice.Account;
        Assert.AreEqual(Alice.Account, Contract.Owner);
    }

    #endregion
}
