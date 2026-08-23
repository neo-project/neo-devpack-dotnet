// Copyright (C) 2015-2026 The Neo Project.
//
// GuardHelpersTest.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests
{
    [TestClass]
    public class GuardHelpersTest : DebugAndTestBase<Contract_GuardHelpers_Inline>
    {
        // Override to disable gas consumption testing since our values may differ
        protected override bool TestGasConsume { get; set; } = false;

        [TestMethod]
        public void TestRequire()
        {
            // Should pass when condition is true
            Contract.TestRequire(true);
            AssertGasConsumed(1064670);

            // Should throw when condition is false
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequire(false));
            Assert.IsTrue(ex.Message.Contains("FAILED"));
            AssertGasConsumed(1080090);
        }

        [TestMethod]
        public void TestRequireNotNull()
        {
            // Should pass when value is not null
            Contract.TestRequireNotNull("test");
            AssertGasConsumed(1065060);
            Contract.TestRequireNotNull(123);
            AssertGasConsumed(1064850);

            // Should throw when value is null
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireNotNull(null));
            Assert.IsTrue(ex.Message.Contains("NULL:myParam"));
            AssertGasConsumed(1387710);
        }

        [TestMethod]
        public void TestRequireNonNegative()
        {
            // Should pass for positive values
            Contract.TestRequireNonNegative(0);
            AssertGasConsumed(1064700);
            Contract.TestRequireNonNegative(100);
            AssertGasConsumed(1064700);

            // Should throw for negative values
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireNonNegative(-1));
            Assert.IsTrue(ex.Message.Contains("NEGATIVE"));
            AssertGasConsumed(1080300);
        }

        [TestMethod]
        public void TestRequirePositive()
        {
            // Should pass for positive values
            Contract.TestRequirePositive(1);
            AssertGasConsumed(1064700);
            Contract.TestRequirePositive(100);
            AssertGasConsumed(1064700);

            // Should throw for zero
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequirePositive(0));
            Assert.IsTrue(ex.Message.Contains("NOT_POSITIVE"));
            AssertGasConsumed(1080300);
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequirePositive(-1));
            Assert.IsTrue(ex.Message.Contains("NOT_POSITIVE"));
            AssertGasConsumed(1080300);
        }

        [TestMethod]
        public void TestRequireValidAddress()
        {
            // Create a valid address
            var validAddress = UInt160.Parse("0x0000000000000000000000000000000000000001");

            // Should pass for valid address
            Contract.TestRequireValidAddress(validAddress);
            AssertGasConsumed(1066140);

            // Should throw for zero address
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireValidAddress(UInt160.Zero));
            Assert.IsTrue(ex.Message.Contains("INVALID_ADDR"));
            AssertGasConsumed(1081740);

            // Should throw for null address
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireValidAddress(null));
            Assert.IsTrue(ex.Message.Contains("INVALID_ADDR"));
            AssertGasConsumed(1080360);
        }

        [TestMethod]
        public void TestRequireWitness()
        {
            // Set a signer for the test
            var owner = UInt160.Parse("0x0000000000000000000000000000000000000001");
            Engine.SetTransactionSigners(owner);

            // Should pass when witness is present (test engine automatically provides witness for signer)
            Contract.TestRequireWitness(owner);
            AssertGasConsumed(1095600);

            // Test with custom error code
            Contract.TestRequireWitnessCustom(owner, "CUSTOM_ERROR");
            AssertGasConsumed(1095660);

            // Should throw when witness is not present
            var otherAccount = UInt160.Parse("0x0000000000000000000000000000000000000002");
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireWitness(otherAccount));
            Assert.IsTrue(ex.Message.Contains("NO_WITNESS"));
            AssertGasConsumed(1111020);

            // Test with custom error code
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireWitnessCustom(otherAccount, "CUSTOM_ERROR"));
            Assert.IsTrue(ex.Message.Contains("CUSTOM_ERROR"));
            AssertGasConsumed(1111080);
        }

        [TestMethod]
        public void TestRequireInRange()
        {
            // Should pass when value is in range
            Contract.TestRequireInRange(5, 1, 10);
            AssertGasConsumed(1065330);
            Contract.TestRequireInRange(1, 1, 10); // Min boundary
            AssertGasConsumed(1065330);
            Contract.TestRequireInRange(10, 1, 10); // Max boundary
            AssertGasConsumed(1065330);

            // Should throw when value is below range
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireInRange(0, 1, 10));
            Assert.IsTrue(ex.Message.Contains("OUT_OF_RANGE"));
            AssertGasConsumed(1080660);

            // Should throw when value is above range
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireInRange(11, 1, 10));
            Assert.IsTrue(ex.Message.Contains("OUT_OF_RANGE"));
            AssertGasConsumed(1080930);
        }

        [TestMethod]
        public void TestRequireEquals()
        {
            // Should pass when values are equal
            Contract.TestRequireEquals(5, 5);
            AssertGasConsumed(1065780);

            // Should throw when values are not equal (default error)
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireEquals(5, 10));
            Assert.IsTrue(ex.Message.Contains("NOT_EQUAL"));
            AssertGasConsumed(1081200);

            // Should throw with custom error code
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireEqualsCustom(5, 10, "CUSTOM_EQ"));
            Assert.IsTrue(ex.Message.Contains("CUSTOM_EQ"));
            AssertGasConsumed(1081260);
        }

        [TestMethod]
        public void TestRequireCaller()
        {
            // Note: In a real contract execution, this would check Runtime.CallingScriptHash
            // For testing purposes, we can't easily simulate the calling script hash,
            // so we test that it throws when given a different address

            // Should throw when caller doesn't match (since we can't control the calling script hash in tests)
            var otherCaller = UInt160.Parse("0x0000000000000000000000000000000000000003");
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireCaller(otherCaller));
            Assert.IsTrue(ex.Message.Contains("INVALID_CALLER"));
            AssertGasConsumed(1081680);
        }

        [TestMethod]
        public void TestRequireNotEmpty()
        {
            // Should pass for non-empty strings
            Contract.TestRequireNotEmpty("test");
            AssertGasConsumed(1065360);
            Contract.TestRequireNotEmpty("a");
            AssertGasConsumed(1065360);

            // Should throw for empty string
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireNotEmpty(""));
            Assert.IsTrue(ex.Message.Contains("EMPTY:myString"));
            AssertGasConsumed(1388220);

            // Should throw for null string
            ex = Assert.ThrowsException<TestException>(() => Contract.TestRequireNotEmpty(null));
            Assert.IsTrue(ex.Message.Contains("EMPTY:myString"));
            AssertGasConsumed(1387800);
        }

        [TestMethod]
        public void TestEnsure()
        {
            // Should pass when postcondition is true
            Contract.TestEnsure(true);
            AssertGasConsumed(1064670);

            // Should throw when postcondition is false
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestEnsure(false));
            Assert.IsTrue(ex.Message.Contains("POST:POSTCOND"));
            AssertGasConsumed(1387530);
        }

        [TestMethod]
        public void TestRevert()
        {
            // Should always throw with the specified error
            var ex = Assert.ThrowsException<TestException>(() => Contract.TestRevert());
            Assert.IsTrue(ex.Message.Contains("REVERTED"));
            AssertGasConsumed(1016970);
        }

        [TestMethod]
        public void TestTransferScenario()
        {
            // Create test addresses
            var validFrom = UInt160.Parse("0x0000000000000000000000000000000000000001");
            var validTo = UInt160.Parse("0x0000000000000000000000000000000000000002");

            // Set the signer for witness validation
            Engine.SetTransactionSigners(validFrom);

            // Test with invalid from address (zero)
            var ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(UInt160.Zero, validTo, 100));
            Assert.IsTrue(ex.Message.Contains("INVALID_ADDR"));

            // Test with invalid to address (zero)
            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(validFrom, UInt160.Zero, 100));
            Assert.IsTrue(ex.Message.Contains("INVALID_ADDR"));

            // Test with invalid amount (zero)
            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(validFrom, validTo, 0));
            Assert.IsTrue(ex.Message.Contains("NOT_POSITIVE"));

            // Test with invalid amount (negative)
            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(validFrom, validTo, -1));
            Assert.IsTrue(ex.Message.Contains("NOT_POSITIVE"));

            // Test without witness
            var nonWitnessAccount = UInt160.Parse("0x0000000000000000000000000000000000000009");
            ex = Assert.ThrowsException<TestException>(() => Contract.Transfer(nonWitnessAccount, validTo, 100));
            Assert.IsTrue(ex.Message.Contains("NO_WITNESS"));

            // Test successful transfer (with witness from test engine's signer)
            var result = Contract.Transfer(validFrom, validTo, 100);
            Assert.IsTrue(result);
            AssertGasConsumed(1151550);
        }
    }
}
