// Copyright (C) 2015-2026 The Neo Project.
//
// NotificationWatcherTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using System.Linq;
using System.Numerics;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class NotificationWatcherTests
    {
        [TestMethod]
        public void CreateNotificationWatcherTracksNotifications()
        {
            TestEngine engine = new(true);
            engine.SetTransactionSigners(WitnessScope.Global, engine.ValidatorsAddress);

            var watcher = engine.CreateNotificationWatcher();
            var addressTo = UInt160.Parse("0x1230000000000000000000000000000000000000");

            Assert.IsTrue(engine.Native.NEO.Transfer(engine.Transaction.Sender, addressTo, 123, null));

            var transfer = watcher.Notifications.First(notification =>
                notification.Sender == engine.Native.NEO.Hash && notification.EventName == "Transfer");

            Assert.AreEqual(3, transfer.State.Count);
            Assert.AreEqual(new BigInteger(123), transfer.State[2].GetInteger());

            watcher.Reset();

            Assert.AreEqual(0, watcher.Notifications.Count);

            watcher.Dispose();
            Assert.IsTrue(engine.Native.NEO.Transfer(engine.Transaction.Sender, UInt160.Parse("0x4560000000000000000000000000000000000000"), 1, null));

            Assert.AreEqual(0, watcher.Notifications.Count);
        }

        [TestMethod]
        public void RuntimeNotificationDeepCopiesStateItems()
        {
            var nestedState = new Neo.VM.Types.Array([new Neo.VM.Types.Integer(1)]);
            var sourceState = new Neo.VM.Types.Array([nestedState]);

            var notification = new RuntimeNotification(UInt160.Zero, "Event", sourceState);

            nestedState.Add(new Neo.VM.Types.Integer(2));
            sourceState.Add(new Neo.VM.Types.Integer(3));

            Assert.AreEqual(1, notification.State.Count);
            var capturedNestedState = (Neo.VM.Types.Array)notification.State[0];
            Assert.AreEqual(1, capturedNestedState.Count);
            Assert.AreEqual(new BigInteger(1), capturedNestedState[0].GetInteger());
        }
    }
}
