// Copyright (C) 2015-2026 The Neo Project.
//
// OwnableContractTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.InvalidTypes;
using Neo.SmartContract.Testing.TestingStandards;
using System.Linq;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractowner
{
    /// <summary>
    /// You need to build the solution to resolve Ownable class.
    /// </summary>
    [TestClass]
    public class OwnableContractTests : OwnableTests<OwnableTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnableContractTests() : base(OwnableTemplate.Nef, OwnableTemplate.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<TestException>(() => Contract.Owner);
        }

        [TestMethod]
        public void TestMyMethod()
        {
            Assert.AreEqual("World", Contract.MyMethod());
        }

        [TestMethod]
        public void TestUpdate()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            Assert.ThrowsException<TestException>(() => Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString()));

            Engine.SetTransactionSigners(Alice);

            // Test Update with the same script

            Contract.Update(NefFile.ToArray(), Manifest.ToJson().ToString());

            // Ensure that it works with the same script

            TestVerify();
        }

        [TestMethod]
        public void TestDeployWithOwner()
        {
            // Alice is the deployer

            Engine.SetTransactionSigners(Bob);

            // Try with invalid owners

            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, UInt160.Zero));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, InvalidUInt160.InvalidLength));
            Assert.ThrowsException<TestException>(() => Engine.Deploy<OwnableTemplate>(NefFile, Manifest, InvalidUInt160.InvalidType));

            // Test SetOwner notification

            UInt160? previousOwnerRaised = null;
            UInt160? newOwnerRaised = null;

            var expectedHash = Engine.GetDeployHash(NefFile, Manifest);
            var check = Engine.FromHash<OwnableTemplate>(expectedHash, false);
            check.OnSetOwner += (previous, newOwner) =>
            {
                previousOwnerRaised = previous;
                newOwnerRaised = newOwner;
            };

            // Deploy with random owner, we can use the same storage
            // because the contract hash contains the Sender, and now it's random

            var rand = TestEngine.GetNewSigner().Account;
            var nep17 = Engine.Deploy<OwnableTemplate>(NefFile, Manifest, rand);
            Assert.AreEqual(check.Hash, nep17.Hash);

            Coverage?.Join(nep17.GetCoverage());

            Assert.AreEqual(rand, nep17.Owner);
            Assert.IsNull(previousOwnerRaised);
            Assert.AreEqual(newOwnerRaised, nep17.Owner);
            Assert.AreEqual(newOwnerRaised, rand);
        }

        [TestMethod]
        public void TestDestroy()
        {
            // Try without being owner

            Engine.SetTransactionSigners(Bob);
            Assert.ThrowsException<TestException>(Contract.Destroy);

            // Try with the owner

            var checkpoint = Engine.Checkpoint();

            Engine.SetTransactionSigners(Alice);
            Contract.Destroy();
            Assert.IsNull(Engine.Native.ContractManagement.GetContract(Contract));

            Engine.Restore(checkpoint);
        }

        [TestMethod]
        public void TestManifestUsesLeastPrivilegePermissions()
        {
            var permissions = new JArray(OwnableTemplate.Manifest.Permissions.Select(p => p.ToJson()).ToArray()).ToString(false);

            Assert.IsFalse(permissions.Contains("\"contract\":\"*\""), "Ownable template should not grant wildcard contract permissions.");
            Assert.IsTrue(permissions.Contains("\"destroy\""), "Ownable template should keep the required destroy permission.");
            Assert.IsTrue(permissions.Contains("\"update\""), "Ownable template should keep the required update permission.");
        }
    }
}
