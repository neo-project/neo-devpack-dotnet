// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Recursion.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.VM.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Recursion : DebugAndTestBase<Contract_Recursion>
    {
        [TestMethod]
        public void Test_Factorial()
        {
            BigInteger prevResult = 1;
            BigInteger result;
            for (int i = 0; i < 10; i++)
            {
                result = Contract.Factorial(i)!.Value;
                Assert.AreEqual(i > 0 ? prevResult * i : 1, result);
                prevResult = result;
            }
        }

        [TestMethod]
        public void Test_HanoiTower()
        {
            int src = 100, aux = 200, dst = 300;
            var result = Contract.HanoiTower(1, src, aux, dst)!;
            AssertGasConsumed(1356600);
            Assert.AreEqual(result.Count, 1);
            List<(BigInteger rodId, BigInteger src, BigInteger dst)> expectedResult = [(1, src, dst)];
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                StackItem[] step = ((Struct)result[i]).SubItems.ToArray();
                Assert.AreEqual(step[0], expectedResult[i].rodId);
                Assert.AreEqual(step[1], expectedResult[i].src);
                Assert.AreEqual(step[2], expectedResult[i].dst);
            }

            result = Contract.HanoiTower(3, src, aux, dst)!;
            expectedResult = new() {
                (1, src, dst),
                (2, src, aux),
                (1, dst, aux),
                (3, src, dst),
                (1, aux, src),
                (2, aux, dst),
                (1, src, dst),
            };
            for (int i = 0; i < expectedResult.Count; ++i)
            {
                StackItem[] step = ((Struct)result[i]).SubItems.ToArray();
                Assert.AreEqual(step[0], expectedResult[i].rodId);
                Assert.AreEqual(step[1], expectedResult[i].src);
                Assert.AreEqual(step[2], expectedResult[i].dst);
            }
        }

        [TestMethod]
        public void Test_MutualRecursion()
        {
            Assert.IsTrue(Contract.Odd(7));
            AssertGasConsumed(1180830);
            Assert.IsFalse(Contract.Even(9));
            AssertGasConsumed(1218750);
            Assert.IsTrue(Contract.Odd(-11));
            AssertGasConsumed(1257330);
            Assert.IsTrue(Contract.Even(-10));
            AssertGasConsumed(1238310);
            Assert.IsFalse(Contract.Even(-9));
            AssertGasConsumed(1219290);
        }
    }
}
