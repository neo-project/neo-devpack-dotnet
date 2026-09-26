using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Linq;
using System.Numerics;

namespace Example.SmartContract.SampleRoyaltyNEP11Token.UnitTests;

[TestClass]
public class SampleRoyaltyNEP11TokenTest : TestBase<Neo.SmartContract.Testing.SampleRoyaltyNEP11Token>
{
    [TestMethod]
    public void MintUpdatesNep11StateAndEmitsTransfer()
    {
        Assert.AreEqual("SampleRoyalty", Contract.Symbol);
        Assert.AreEqual(new BigInteger(0), Contract.Decimals);

        var owner = Contract.Owner;
        Assert.IsNotNull(owner);
        Engine.SetTransactionSigners(owner!);

        var recipient = TestEngine.BobAccount;
        using var notifications = Engine.CreateNotificationWatcher();

        Contract.Mint(recipient);

        Assert.AreEqual(new BigInteger(1), Contract.CurrentCount);
        Assert.AreEqual(new BigInteger(1), Contract.TotalSupply);
        Assert.AreEqual(new BigInteger(1), Contract.BalanceOf(recipient));
        Assert.AreEqual(recipient, Contract.OwnerOf(new byte[] { 0x01 }));

        var transfer = notifications.Notifications.Single(notification => notification.EventName == "Transfer");
        Assert.IsNotNull(transfer);
        Assert.AreEqual(new BigInteger(1), transfer.State[2].GetInteger());
        CollectionAssert.AreEqual(new byte[] { 0x01 }, transfer.State[3].GetSpan().ToArray());
    }
}
