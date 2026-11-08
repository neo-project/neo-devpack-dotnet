using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.Numerics;

namespace Example.SmartContract.Event.UnitTests;

[TestClass]
public class EventTests : TestBase<SampleEvent>
{
    [TestMethod]
    public void MainEmitsBothDeclaredEventsWithExpectedState()
    {
        using var notifications = Engine.CreateNotificationWatcher();

        Assert.IsFalse(Contract.Main());

        Assert.AreEqual(2, notifications.Notifications.Count);

        var renamedEvent = notifications.Notifications[0];
        Assert.AreEqual("new_event_name", renamedEvent.EventName);
        Assert.AreEqual(3, renamedEvent.State.Count);
        CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03 }, renamedEvent.State[0].GetSpan().ToArray());
        Assert.AreEqual("oi", renamedEvent.State[1].GetString());
        Assert.AreEqual(new BigInteger(10), renamedEvent.State[2].GetInteger());

        var secondEvent = notifications.Notifications[1];
        Assert.AreEqual("event2", secondEvent.EventName);
        Assert.AreEqual(2, secondEvent.State.Count);
        CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x03 }, secondEvent.State[0].GetSpan().ToArray());
        Assert.AreEqual(new BigInteger(50), secondEvent.State[1].GetInteger());
    }
}
