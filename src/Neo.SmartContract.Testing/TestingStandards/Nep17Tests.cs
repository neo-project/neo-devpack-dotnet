using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Neo.SmartContract.Testing.TestingStandards;

public class Nep17Tests<T> where T : SmartContract, INep17Standard
{
    /// <summary>
    /// Expected total supply
    /// </summary>
    public virtual BigInteger ExpectedTotalSupply => 0;

    /// <summary>
    /// Expected Decimals
    /// </summary>
    public virtual byte ExpectedDecimals => 8;

    /// <summary>
    /// Expected symbol
    /// </summary>
    public virtual string ExpectedSymbol => "EXAMPLE";

    public static CoveredContract? Coverage { get; private set; }
    public static Signer Alice { get; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; } = TestEngine.GetNewSigner();

    public byte[] NefFile { get; }
    public string Manifest { get; }
    public TestEngine Engine { get; }
    public T Nep17 { get; }

    #region Transfer event checks

    private List<(UInt160? from, UInt160? to, BigInteger? amount)> raisedTransfer = new();

    #endregion

    /// <summary>
    /// Initialize Test
    /// </summary>
    public Nep17Tests(string nefFile, string manifestFile)
    {
        NefFile = File.ReadAllBytes(nefFile);
        Manifest = File.ReadAllText(manifestFile);

        Engine = new TestEngine(true);
        Engine.SetTransactionSigners(Alice);
        Nep17 = Engine.Deploy<T>(NefFile, Manifest, null);

        if (Coverage is null)
        {
            Coverage = Nep17.GetCoverage()!;
            Assert.IsNotNull(Coverage);
        }

        Nep17.OnTransfer += onTransfer;
    }

    void onTransfer(UInt160? from, UInt160? to, BigInteger? amount)
    {
        raisedTransfer.Add((from, to, amount));
    }

    #region Asserts

    /// <summary>
    /// Assert that Transfer event was raised
    /// </summary>
    /// <param name="from">From</param>
    /// <param name="to">To</param>
    /// <param name="amount">Amount</param>
    public void AssertTransferEvent(UInt160? from, UInt160? to, BigInteger? amount)
    {
        Assert.AreEqual(1, raisedTransfer.Count);
        Assert.AreEqual(raisedTransfer[0].from, from);
        Assert.AreEqual(raisedTransfer[0].to, to);
        Assert.AreEqual(raisedTransfer[0].amount, amount);
        raisedTransfer.Clear();
    }

    /// <summary>
    /// Assert that Transfer event was NOT raised
    /// </summary>
    public void AssertNoTransferEvent()
    {
        Assert.AreEqual(0, raisedTransfer.Count);
    }

    #endregion

    [TestCleanup]
    public virtual void OnCleanup()
    {
        // Join the current coverage into the static one

        Coverage?.Join(Nep17.GetCoverage());
    }

    #region Tests

    [TestMethod]
    public virtual void TestDecimals()
    {
        Assert.AreEqual(ExpectedDecimals, Nep17.Decimals);
    }

    [TestMethod]
    public virtual void TestSymbol()
    {
        Assert.AreEqual(ExpectedSymbol, Nep17.Symbol);
    }

    [TestMethod]
    public virtual void TestTotalSupply()
    {
        Assert.AreEqual(ExpectedTotalSupply, Nep17.TotalSupply);
    }

    [TestMethod]
    public void TestBalanceOf()
    {
        Assert.AreEqual(0, Nep17.BalanceOf(Bob.Account));
        Assert.ThrowsException<VMUnhandledException>(() => Nep17.BalanceOf(InvalidUInt160.Null));
        Assert.ThrowsException<VMUnhandledException>(() => Nep17.BalanceOf(InvalidUInt160.Invalid));
    }

    [TestMethod]
    public virtual void TestTransfer()
    {
        // Invoke transfer from Alice to Bob

        Engine.SetTransactionSigners(Alice);

        var initialSupply = Nep17.TotalSupply;
        var fromBalance = Nep17.BalanceOf(Alice.Account);

        Assert.IsTrue(fromBalance > 5, "Alice needs at least 5 tokens");
        Assert.IsTrue(Nep17.Transfer(Alice.Account, Bob.Account, 3));

        Assert.AreEqual(fromBalance - 3, Nep17.BalanceOf(Alice.Account));
        Assert.AreEqual(3, Nep17.BalanceOf(Bob.Account));
        Assert.AreEqual(initialSupply, Nep17.TotalSupply);
        AssertTransferEvent(Alice.Account, Bob.Account, 3);

        // Invoke invalid transfers

        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(Alice.Account, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(InvalidUInt160.Null, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(Alice.Account, InvalidUInt160.Null, 0)));

        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(Alice.Account, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(InvalidUInt160.Invalid, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Nep17.Transfer(Alice.Account, InvalidUInt160.Invalid, 0)));

        // Invoke transfer without signature

        Engine.SetTransactionSigners(Bob);
        Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 1));
        AssertNoTransferEvent();

        // Check with more balance

        Assert.IsFalse(Nep17.Transfer(Bob.Account, Alice.Account, 4));
        AssertNoTransferEvent();

        // Check with not signed

        Assert.IsFalse(Nep17.Transfer(Alice.Account, Bob.Account, 0));
        AssertNoTransferEvent();

        // Return the balance to Allice

        Assert.IsTrue(Nep17.Transfer(Bob.Account, Alice.Account, 3));

        Assert.AreEqual(fromBalance, Nep17.BalanceOf(Alice.Account));
        Assert.AreEqual(0, Nep17.BalanceOf(Bob.Account));
        Assert.AreEqual(initialSupply, Nep17.TotalSupply);
        AssertTransferEvent(Bob.Account, Alice.Account, 3);
    }

    #endregion
}
