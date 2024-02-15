using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing.TestingStandards;

public class Nep17Tests<T> : TestBase<T>
    where T : SmartContract, INep17Standard
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

    #region Transfer event checks

    private List<(UInt160? from, UInt160? to, BigInteger? amount)> raisedTransfer = new();

    #endregion

    /// <summary>
    /// Initialize Test
    /// </summary>
    public Nep17Tests(string nefFile, string manifestFile) : base(nefFile, manifestFile)
    {
        Contract.OnTransfer += onTransfer;
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

    #region Tests

    [TestMethod]
    public virtual void TestDecimals()
    {
        Assert.AreEqual(ExpectedDecimals, Contract.Decimals);
    }

    [TestMethod]
    public virtual void TestSymbol()
    {
        Assert.AreEqual(ExpectedSymbol, Contract.Symbol);
    }

    [TestMethod]
    public virtual void TestTotalSupply()
    {
        Assert.AreEqual(ExpectedTotalSupply, Contract.TotalSupply);
    }

    [TestMethod]
    public void TestBalanceOf()
    {
        Assert.AreEqual(0, Contract.BalanceOf(Bob.Account));
        Assert.ThrowsException<VMUnhandledException>(() => Contract.BalanceOf(InvalidUInt160.Null));
        Assert.ThrowsException<VMUnhandledException>(() => Contract.BalanceOf(InvalidUInt160.Invalid));
    }

    [TestMethod]
    public virtual void TestTransfer()
    {
        // Invoke transfer from Alice to Bob

        Engine.SetTransactionSigners(Alice);

        var initialSupply = Contract.TotalSupply;
        var fromBalance = Contract.BalanceOf(Alice.Account);

        Assert.IsTrue(fromBalance > 5, "Alice needs at least 5 tokens");
        Assert.IsTrue(Contract.Transfer(Alice.Account, Bob.Account, 3));

        Assert.AreEqual(fromBalance - 3, Contract.BalanceOf(Alice.Account));
        Assert.AreEqual(3, Contract.BalanceOf(Bob.Account));
        Assert.AreEqual(initialSupply, Contract.TotalSupply);
        AssertTransferEvent(Alice.Account, Bob.Account, 3);

        // Invoke invalid transfers

        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(Alice.Account, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(InvalidUInt160.Null, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(Alice.Account, InvalidUInt160.Null, 0)));

        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(Alice.Account, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(InvalidUInt160.Invalid, Bob.Account, -1)));
        Assert.ThrowsException<VMUnhandledException>(() => Assert.IsTrue(Contract.Transfer(Alice.Account, InvalidUInt160.Invalid, 0)));

        // Invoke transfer without signature

        Engine.SetTransactionSigners(Bob);
        Assert.IsFalse(Contract.Transfer(Alice.Account, Bob.Account, 1));
        AssertNoTransferEvent();

        // Check with more balance

        Assert.IsFalse(Contract.Transfer(Bob.Account, Alice.Account, 4));
        AssertNoTransferEvent();

        // Check with not signed

        Assert.IsFalse(Contract.Transfer(Alice.Account, Bob.Account, 0));
        AssertNoTransferEvent();

        // Return the balance to Allice

        Assert.IsTrue(Contract.Transfer(Bob.Account, Alice.Account, 3));

        Assert.AreEqual(fromBalance, Contract.BalanceOf(Alice.Account));
        Assert.AreEqual(0, Contract.BalanceOf(Bob.Account));
        Assert.AreEqual(initialSupply, Contract.TotalSupply);
        AssertTransferEvent(Bob.Account, Alice.Account, 3);
    }

    #endregion
}
