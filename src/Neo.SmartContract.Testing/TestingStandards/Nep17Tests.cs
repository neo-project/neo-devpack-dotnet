using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Neo.IO;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.VM;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Testing.TestingStandards;

public class Nep17Tests<T> : TestBase<T>
    where T : SmartContract, INep17Standard
{
    public abstract class onNEP17PaymentContract : SmartContract
    {
        protected onNEP17PaymentContract(SmartContractInitialize initialize) : base(initialize) { }

        public abstract void onNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);
    }

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
    /// Assert that Transfer event was raised
    /// </summary>
    /// <param name="from">From</param>
    /// <param name="to">To</param>
    /// <param name="amount">Amount</param>
    public void AssertTransferEvent(params (UInt160? from, UInt160? to, BigInteger? amount)[] events)
    {
        Assert.AreEqual(events.Length, raisedTransfer.Count);
        CollectionAssert.AreEqual(raisedTransfer.Select(u => u.from).ToArray(), events.Select(u => u.from).ToArray());
        CollectionAssert.AreEqual(raisedTransfer.Select(u => u.to).ToArray(), events.Select(u => u.to).ToArray());
        CollectionAssert.AreEqual(raisedTransfer.Select(u => u.amount).ToArray(), events.Select(u => u.amount).ToArray());
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
    public virtual void TestBalanceOf()
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
        Assert.AreEqual(0, Contract.BalanceOf(Bob.Account), "Bob must have 0 tokens");
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

        // Test onNEP17Payment with a mock
        // We create a mock contract using the current nef and manifest
        // Only we need to create the manifest method, then it will be redirected

        ContractManifest manifest = ContractManifest.Parse(Manifest);
        manifest.Abi.Methods = new ContractMethodDescriptor[]
        {
            new ()
            {
                Name = "onNEP17Payment",
                ReturnType = ContractParameterType.Void,
                Safe = false,
                Parameters = new ContractParameterDefinition[]
                    {
                        new() { Name = "a", Type = ContractParameterType.Hash160 },
                        new() { Name = "b", Type = ContractParameterType.Integer },
                        new() { Name = "c", Type = ContractParameterType.Any }
                    }
            }
        };

        // Deploy dummy contract

        UInt160? calledFrom = null;
        BigInteger? calledAmount = null;
        byte[]? calledData = null;

        var mock = Engine.Deploy<onNEP17PaymentContract>(NefFile, manifest.ToJson().ToString(), null, m =>
         {
             m
             .Setup(s => s.onNEP17Payment(It.IsAny<UInt160>(), It.IsAny<BigInteger>(), It.IsAny<object>()))
             .Callback(new InvocationAction((i) =>
             {
                 calledFrom = i.Arguments[0] as UInt160;
                 calledAmount = (BigInteger)i.Arguments[1];
                 calledData = (i.Arguments[2] as ByteString)!.GetSpan().ToArray();

                 // Ensure the event was called

                 var me = new UInt160(calledData);
                 AssertTransferEvent(Alice.Account, me, calledAmount);

                 // Return the money back

                 Engine.SetTransactionSigners(me);
                 Assert.IsTrue(Contract.Transfer(me, calledFrom, calledAmount));
                 AssertTransferEvent(me, Alice.Account, calledAmount);
             }));
         });

        // Ensure that was called

        Engine.SetTransactionSigners(Alice);
        Assert.IsTrue(Contract.Transfer(Alice.Account, mock.Hash, 3, mock.Hash.ToArray()));

        Assert.AreEqual(Alice.Account, calledFrom);
        Assert.AreEqual(mock.Hash, new UInt160(calledData));
        Assert.AreEqual(3, calledAmount);
        Assert.AreEqual(0, Contract.BalanceOf(mock.Hash));
    }

    #endregion
}
