// Copyright (C) 2015-2026 The Neo Project.
//
// NeoContractSolutionTests.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Extensions;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.SmartContract.Testing.TestingStandards;

namespace Neo.SmartContract.Template.UnitTests.templates.neocontractsolution
{
    /// <summary>
    /// You need to build the solution to resolve Contract class.
    /// </summary>
    [TestClass]
    public class NeoContractSolutionTests : OwnableTests<NeoContractSolutionTemplate>
    {
        /// <summary>
        /// Initialize Test
        /// </summary>
        public NeoContractSolutionTests() : base(NeoContractSolutionTemplate.Nef, NeoContractSolutionTemplate.Manifest) { }

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
    }
}
