// Copyright (C) 2015-2026 The Neo Project.
//
// TestRoleManagement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Cryptography.ECC;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.Collections.Immutable;
using System.Reflection;
using CoreRole = Neo.SmartContract.Native.Role;

namespace Neo.SmartContract.Testing.UnitTests
{
    [TestClass]
    public class TestRoleManagement
    {
        private TestEngine _engine = null!;

        [TestInitialize]
        public void Setup()
        {
            var settings = TestEngine.Default with
            {
                Hardforks = TestEngine.Default.Hardforks.ToImmutableDictionary(p => p.Key, p => 0u)
            };
            _engine = new TestEngine(settings, true);
            _engine.SetTransactionSigners(new Signer
            {
                Account = _engine.CommitteeAddress,
                Scopes = WitnessScope.Global
            });
        }

        [TestMethod]
        public void TestDesignateAsRoleRejectsDuplicateKeys()
        {
            var node = ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1);

            var exception = Assert.ThrowsException<TestException>(() =>
                _engine.Native.RoleManagement.DesignateAsRole(CoreRole.Oracle, [node, node]));

            Assert.IsInstanceOfType<TargetInvocationException>(exception.InnerException);
            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException!.InnerException);
            StringAssert.Contains(exception.InnerException.InnerException!.Message, "Duplicate publickeys");
        }
    }
}
