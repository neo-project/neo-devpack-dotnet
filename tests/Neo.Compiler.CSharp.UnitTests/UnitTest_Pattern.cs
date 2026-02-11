// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Pattern.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract.Testing;
using Neo.VM;
using Neo.VM.Types;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Pattern : DebugAndTestBase<Contract_Pattern>
    {
        [TestMethod]
        public void Between_Test()
        {
            Assert.AreEqual(true, Contract.Between(50));
            AssertGasConsumed(1047810);
            Assert.AreEqual(false, Contract.Between(1));
            AssertGasConsumed(1047510);
            Assert.AreEqual(false, Contract.Between(100));
            AssertGasConsumed(1047810);
            Assert.AreEqual(false, Contract.Between(200));
            AssertGasConsumed(1047810);
        }

        [TestMethod]
        public void Between2_Test()
        {
            Assert.AreEqual(true, Contract.Between2(50));
            Assert.AreEqual(false, Contract.Between2(1));
            Assert.AreEqual(false, Contract.Between2(100));
            Assert.AreEqual(false, Contract.Between2(200));
        }

        [TestMethod]
        public void Between3_Test()
        {
            Assert.AreEqual(true, Contract.Between3(50));
            Assert.AreEqual(false, Contract.Between3(1));
            Assert.AreEqual(false, Contract.Between3(100));
            Assert.AreEqual(false, Contract.Between3(200));
        }

        [TestMethod]
        public void RecursivePattern_Test()
        {
            Assert.AreEqual(true, Contract.TestRecursivePattern());
        }

        [TestMethod]
        public void RecursivePattern_EmitsBoolAndForMultiplePropertyChecks()
        {
            var method = Manifest.Abi.GetMethod("testRecursivePattern", 0);
            Assert.IsNotNull(method);

            var methodStart = method.Offset;
            var methodEnd = Manifest.Abi.Methods
                .Where(m => m.Offset > methodStart)
                .Select(m => m.Offset)
                .DefaultIfEmpty(int.MaxValue)
                .Min();

            var script = (Script)NefFile.Script;
            bool hasBoolAnd = false;

            foreach (var (address, instruction) in script.EnumerateInstructions())
            {
                if (address < methodStart)
                    continue;
                if (address >= methodEnd)
                    break;

                if (instruction.OpCode == OpCode.BOOLAND)
                {
                    hasBoolAnd = true;
                    break;
                }
            }

            Assert.IsTrue(hasBoolAnd, "Recursive pattern lowering should emit BOOLAND for multi-property checks.");
        }

        [TestMethod]
        public void TestTypePattern_Test()
        {
            Assert.AreEqual(new BigInteger(2), Contract.TestTypePattern2(1));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2("1"));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2(new ByteString(new byte[] { 1 })));
            Assert.AreEqual(BigInteger.Zero, Contract.TestTypePattern2(new byte[] { 1 }));
            Assert.AreEqual(BigInteger.One, Contract.TestTypePattern2(true));

            // no errors
            Contract.TestTypePattern("1");
            Contract.TestTypePattern(1);
            Contract.TestTypePattern(true);
        }
    }
}
