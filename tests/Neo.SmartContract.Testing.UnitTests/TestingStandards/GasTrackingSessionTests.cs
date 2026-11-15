// Copyright (C) 2015-2026 The Neo Project.
//
// GasTrackingSessionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.TestingStandards;
using System;
using System.IO;

namespace Neo.SmartContract.Testing.UnitTests.TestingStandards
{
    [TestClass]
    [DoNotParallelize]
    public class GasTrackingSessionTests
    {
        [TestMethod]
        public void ExternalConsumersDoNotTrackNorWriteWithoutOptIn()
        {
            Assert.IsNull(Environment.GetEnvironmentVariable(GasTrackingSession.ModeVariable));
            var assembly = typeof(GasTrackingSessionTests).Assembly;
            Assert.IsNull(GasTrackingSession.Begin(assembly, "X.Y"));
            Assert.IsNull(GasExecutionTracker.Current);
            GasTrackingSession.Finish(assembly);
        }

        [TestMethod]
        public void InventoryOnlyListsTestBaseClasses()
        {
            var inventory = GasTrackingSession.BuildInventory(typeof(GasTrackingSessionTests).Assembly);
            Assert.IsFalse(inventory.ExpectedMethods.ContainsKey(typeof(GasTrackingSessionTests).FullName + "." + nameof(ExternalConsumersDoNotTrackNorWriteWithoutOptIn)));
        }

        [TestMethod]
        public void ModeWithoutDirectoryIsRejected()
        {
            var previous = Environment.GetEnvironmentVariable(GasTrackingSession.ModeVariable);
            Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, "update");
            GasTrackingSession.Reset();
            try
            {
                Assert.ThrowsExactly<InvalidOperationException>(() => GasTrackingSession.Begin(typeof(GasTrackingSessionTests).Assembly, "X.Y"));
            }
            finally
            {
                Environment.SetEnvironmentVariable(GasTrackingSession.ModeVariable, previous);
                GasTrackingSession.Reset();
            }
        }
    }
}
