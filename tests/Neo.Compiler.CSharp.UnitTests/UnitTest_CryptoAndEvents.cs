// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CryptoAndEvents.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Testing;
using Neo.Wallets;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CryptoAndEvents : DebugAndTestBase<Contract_CryptoAndEvents>
    {
        private const string PrivateKey = "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20";

        /// <summary>
        /// The notification arguments are what an off-chain listener reads, so their order and
        /// their types have to match the declared event.
        /// </summary>
        [TestMethod]
        public void Test_EmitMixed_KeepsArgumentOrder()
        {
            using var watcher = new NotificationWatcher(Engine);

            Assert.AreEqual(10, (int)Contract.EmitMixed(7, 3)!.Value);

            var notification = watcher.Notifications.Single(p => p.EventName == "Mixed");
            Assert.AreEqual(5, notification.State.Count);
            Assert.AreEqual(7, (int)notification.State[0].GetInteger());
            Assert.AreEqual("text", notification.State[1].GetString());
            Assert.IsTrue(notification.State[2].GetBoolean());
            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20 }, notification.State[3].GetSpan().ToArray());
            Assert.AreEqual(21, (int)notification.State[4].GetInteger());
        }

        /// <summary>
        /// A valid signature has to verify, and the same signature must not verify under a
        /// different curve or for a different message.
        /// </summary>
        [TestMethod]
        public void Test_VerifyWithECDsa()
        {
            var key = new KeyPair(Convert.FromHexString(PrivateKey));
            byte[] message = "neo compiler test"u8.ToArray();
            byte[] publicKey = key.PublicKey.EncodePoint(true);
            byte[] signature = Crypto.Sign(message, key.PrivateKey, ECCurve.Secp256r1);

            Assert.IsTrue(Contract.VerifyR1(message, publicKey, signature));
            Assert.IsTrue(Contract.VerifyR1Legacy(message, publicKey, signature));
            Assert.IsFalse(Contract.VerifyR1("neo compiler tesT"u8.ToArray(), publicKey, signature));
            Assert.IsFalse(Contract.VerifyK1(message, publicKey, signature));
        }
    }
}
