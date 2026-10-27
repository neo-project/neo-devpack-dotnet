// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Out : DebugAndTestBase<Contract_Out>
    {

        // The by-ref dispatch contract grew past the default 20 GAS deployment budget;
        // raise the transaction fee for this test class only.
        protected override TestEngine CreateTestEngine()
        {
            var engine = base.CreateTestEngine();
            engine.Fee = 200_00000000;
            return engine;
        }
        [TestMethod]
        public void Test_BasicOut()
        {
            Assert.AreEqual(42, Contract.TestOutVar());
            Assert.AreEqual(42, Contract.TestExistingVar());
        }

        [TestMethod]
        public void Test_OutInLoop()
        {
            Assert.AreEqual(210, Contract.TestOutInLoop()); // 42 * 5
        }

        [TestMethod]
        public void Test_OutConditional()
        {
            Assert.AreEqual("42", Contract.TestOutConditional(true));
            Assert.AreEqual("Hello", Contract.TestOutConditional(false));
        }

        [TestMethod]
        public void Test_OutSwitch()
        {
            Assert.AreEqual(42, Contract.TestOutSwitch(1));
            Assert.AreEqual(10, Contract.TestOutSwitch(2));
            Assert.AreEqual(-1, Contract.TestOutSwitch(3));
        }

        [TestMethod]
        public void Test_OutDiscard()
        {
            // This test is mainly to ensure that the method compiles and runs without error
            // Since we're discarding values, we can't really assert on them
            Contract.TestOutDiscard();
        }

        [TestMethod]
        public void Test_NestedOut()
        {
            var result = Contract.TestNestedOut()!;
            Assert.AreEqual((BigInteger)84, result[0], "TestNestedOut should return 84 as the first element");
            Assert.AreEqual((BigInteger)42, result[1], "TestNestedOut should return 42 as the second element");
        }

        [TestMethod]
        public void Test_OutStaticField()
        {
            Assert.AreEqual(42, Contract.TestOutStaticField(), "Static field should receive the value assigned through out parameter.");
        }

        [TestMethod]
        public void Test_OutNamedArguments()
        {
            var result = Contract.TestOutNamedArguments();
            Assert.IsNotNull(result, "Contract should return a tuple for named out arguments.");
            var tuple = result!;

            BigInteger intValue = tuple[0] switch
            {
                BigInteger value => value,
                _ => throw new AssertFailedException($"Integer out parameter should materialize as BigInteger, but got {tuple[0]?.GetType().Name ?? "null"}.")
            };
            Assert.AreEqual((BigInteger)10, intValue, "Named arguments should correctly bind the integer out parameter.");
            var messageCandidate = tuple[1] ?? throw new AssertFailedException("String out parameter should not be null.");
            string? message = messageCandidate switch
            {
                StackItem stackItem => stackItem.GetString(),
                string str => str,
                _ => throw new AssertFailedException($"Unexpected result type for string out parameter: {messageCandidate.GetType().Name}")
            };
            if (message is null)
                throw new AssertFailedException("String out parameter resolved to null.");
            var flagCandidate = tuple[2] ?? throw new AssertFailedException("Boolean out parameter should not be null.");
            bool flag = flagCandidate switch
            {
                StackItem stackItem => stackItem.GetBoolean(),
                bool value => value,
                _ => throw new AssertFailedException($"Unexpected result type for boolean out parameter: {flagCandidate.GetType().Name}")
            };

            Assert.AreEqual("Hello", message, "Named arguments should correctly bind the string out parameter.");
            Assert.IsTrue(flag, "Named arguments should correctly bind the boolean out parameter.");
        }

        [TestMethod]
        public void Test_OutInstanceField()
        {
            var result = Contract.TestOutInstanceField();
            Assert.IsNotNull(result, "Contract should return a collection for instance field out arguments.");
            var tuple = result!;

            BigInteger intValue = tuple[0] switch
            {
                BigInteger value => value,
                _ => throw new AssertFailedException($"Integer out parameter should materialize as BigInteger, but got {tuple[0]?.GetType().Name ?? "null"}.")
            };

            var messageCandidate = tuple[1] ?? throw new AssertFailedException("String out parameter should not be null.");
            string? message = messageCandidate switch
            {
                StackItem stackItem => stackItem.GetString(),
                string str => str,
                _ => throw new AssertFailedException($"Unexpected result type for string out parameter: {messageCandidate.GetType().Name}")
            };
            if (message is null)
                throw new AssertFailedException("String out parameter resolved to null.");

            var flagCandidate = tuple[2] ?? throw new AssertFailedException("Boolean out parameter should not be null.");
            bool flag = flagCandidate switch
            {
                StackItem stackItem => stackItem.GetBoolean(),
                bool value => value,
                _ => throw new AssertFailedException($"Unexpected result type for boolean out parameter: {flagCandidate.GetType().Name}")
            };

            Assert.AreEqual((BigInteger)10, intValue, "Instance field should receive the integer value assigned through out parameter.");
            Assert.AreEqual("Hello", message, "Instance field should receive the string value assigned through out parameter.");
            Assert.IsTrue(flag, "Instance field should receive the boolean value assigned through out parameter.");
        }

        [TestMethod]
        public void Test_RefStorageSurvivesSkippedBranches()
        {
            Assert.AreEqual(500, Contract.TestRefLocalAfterCoalesce(false));
            AssertGasConsumed(1608630);
            Assert.AreEqual(101, Contract.TestRefLocalAfterCoalesce(true));
            AssertGasConsumed(1626990);
            Assert.AreEqual(507, Contract.TestRefParameterAfterCoalesce());
            AssertGasConsumed(1562850);
            Assert.AreEqual(3, Contract.TestRefAfterConditionalBranch(false));
            AssertGasConsumed(1606800);
            Assert.AreEqual(4, Contract.TestRefAfterConditionalBranch(true));
            AssertGasConsumed(1625130);
        }

        [TestMethod]
        public void Test_ByRefAliases()
        {
            Assert.AreEqual(2, Contract.TestRefAliases());
            AssertGasConsumed(1564680);
            Assert.AreEqual(2, Contract.TestOutAliases());
            AssertGasConsumed(1563960);
            Assert.AreEqual(111, Contract.TestDistinctRefArguments());
            AssertGasConsumed(1566000);
            Assert.AreEqual(20111, Contract.TestMixedRefAliases());
            AssertGasConsumed(1586220);
        }

        [TestMethod]
        public void Test_FieldRefAliases()
        {
            Assert.AreEqual(2, Contract.TestStaticFieldRefAliases());
            AssertGasConsumed(1563000);
            Assert.AreEqual(2, Contract.TestInstanceFieldRefAliases());
            AssertGasConsumed(1882740);
            Assert.AreEqual(111, Contract.TestDistinctInstanceFieldRefs());
            AssertGasConsumed(2193390);
            Assert.AreEqual(12311, Contract.TestRecursiveRefAliases());
        }

        [TestMethod]
        public void Test_VirtualByRefArguments()
        {
            Assert.AreEqual(14313, Contract.TestVirtualRefAliases());
            Assert.AreEqual(77617, Contract.TestVirtualDistinctRefArguments());
            Assert.AreEqual(143207617, Contract.TestVirtualRefAliasesThenDistinctCalls());
            Assert.AreEqual(13006017, Contract.TestVirtualRefAliasWritebackIsolation());
            Assert.AreEqual(1463340849, Contract.TestVirtualRefAliasSignatures());
            Assert.AreEqual(679818, Contract.TestVirtualRefAliasesThroughBaseCall());
            Assert.AreEqual(201718, Contract.TestVirtualRecursiveRefAliasesThroughBaseCall());
            Assert.AreEqual(238204823, Contract.TestVirtualOutArguments());
        }
    }
}
