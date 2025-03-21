// Copyright (C) 2015-2025 The Neo Project.
//
// OwnerContractTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractnep17
{
    /// <summary>
    /// You need to build the solution to resolve Nep17Contract class.
    /// </summary>
    [TestClass]
    public class OwnerContractTests : OwnableTests<Nep17ContractTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public OwnerContractTests() : base(Nep17ContractTemplate.Nef, Nep17ContractTemplate.Manifest) { }

        [TestMethod]
        public override void TestSetGetOwner()
        {
            base.TestSetGetOwner();

            // Test throw if was stored an invalid owner
            // Technically not possible, but raise 100% coverage

            Contract.Storage.Put(new byte[] { 0xff }, 123);
            Assert.ThrowsException<TestException>(() => Contract.Owner);
        }
    }
}
