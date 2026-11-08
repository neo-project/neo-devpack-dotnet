// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Foreach.cs file belongs to the neo project and is free
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
using System.Numerics;
using System.Text;
using Neo.Extensions;
using System;
using System.ComponentModel;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Foreach : DebugAndTestBase<Contract_Foreach>
    {
        [TestMethod]
        public void IntForeachTest()
        {
            Assert.AreEqual(10, Contract.IntForeach());
            AssertGasConsumed(1061280);
            Assert.AreEqual(6, Contract.IntForeachBreak(3));
            AssertGasConsumed(1125150);
        }

        [TestMethod]
        public void IntForLoopTest()
        {
            Assert.AreEqual(10, Contract.IntForLoopLt());
            AssertGasConsumed(1062600);
            Assert.AreEqual(10, Contract.IntForLoopLtNullable());
            AssertGasConsumed(1065330);
            Assert.AreEqual(10, Contract.IntForLoopNonCompare());
            AssertGasConsumed(1065510);
            Assert.AreEqual(10, Contract.IntForLoopLifted());
            AssertGasConsumed(1062600);
            Assert.AreEqual(10, Contract.IntForLoopLe());
            AssertGasConsumed(1064550);
            Assert.AreEqual(10, Contract.IntForLoopEq());
            AssertGasConsumed(1063200);
            Assert.AreEqual(10, Contract.IntForLoopNe());
            AssertGasConsumed(1062600);
            Assert.AreEqual(10, Contract.IntForLoopGe());
            AssertGasConsumed(1062390);
            Assert.AreEqual(9, Contract.IntForLoopGt());
            AssertGasConsumed(1058940);
            Assert.AreEqual(1, Contract.IntForLoopNoCondition());
            AssertGasConsumed(1050390);
            Assert.AreEqual("abcd", Contract.IntForLoopString());
            AssertGasConsumed(2305650);
            Assert.AreEqual(6, Contract.IntForeachBreak(3));
            AssertGasConsumed(1125150);
        }

        [TestMethod]
        public void StringForeachTest()
        {
            Assert.AreEqual("abcdefhij", Contract.StringForeach());
            AssertGasConsumed(1978950);
        }

        [TestMethod]
        [DataRow("KeepGoing(i, limit)", 403, 100, 100)]
        [DataRow("KeepGoing(i, limit) && i >= 0", 403, 100, 100)]
        [DataRow("KeepGoing(i, limit) & (i >= 0)", 403, 100, 100)]
        [DataRow("(int?)i < bound", 3, 0, 0)]
        [DataRow("(i < limit ? \"keep\" : \"stop\") != \"stop\"", 3, 0, 0)]
        public void TestForConditionFallbacks(string condition, int positive, int zero, int negative)
        {
            var context = TestHelper.CompileSingleContract($$"""
                using Neo.SmartContract.Framework;

                public class Contract : SmartContract
                {
                    private static int calls;

                    private static bool KeepGoing(int value, int limit)
                    {
                        calls++;
                        return value < limit;
                    }

                    public static int Run(int limit)
                    {
                        calls = 0;
                        int? bound = limit < 0 ? null : limit;
                        int sum = 0;
                        for (int i = 0; {{condition}}; i++)
                        {
                            if (i >= 10) return -1;
                            sum += i;
                        }
                        return calls * 100 + sum;
                    }
                }
                """);
            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));

            var engine = new TestEngine(true);
            var contract = engine.Deploy<ForConditionContract>(context.CreateExecutable(), context.CreateManifest());
            Assert.AreEqual(new BigInteger(positive), contract.Run(3));
            Assert.AreEqual(new BigInteger(zero), contract.Run(0));
            Assert.AreEqual(new BigInteger(negative), contract.Run(-1));
        }

        public abstract class ForConditionContract(SmartContractInitialize initialize)
            : Neo.SmartContract.Testing.SmartContract(initialize)
        {
            [DisplayName("run")] public abstract BigInteger? Run(BigInteger limit);
        }

        [TestMethod]
        public void ByteStringEmptyTest()
        {
            Assert.AreEqual(0, Contract.ByteStringEmpty());
            AssertGasConsumed(986430);
        }

        [TestMethod]
        public void BytestringForeachTest()
        {
            Assert.AreEqual("abcdefhij", Encoding.ASCII.GetString(Contract.ByteStringForeach()!));
            AssertGasConsumed(2599740);
        }

        [TestMethod]
        public void StructForeachTest()
        {
            var map = Contract.StructForeach()!;
            AssertGasConsumed(2660790);

            Assert.AreEqual(map[(ByteString)"test1"], new BigInteger(1));
            Assert.AreEqual(map[(ByteString)"test2"], new BigInteger(2));
        }

        [TestMethod]
        public void ByteArrayForeachTest()
        {
            var array = Contract.ByteArrayForeach()!;
            AssertGasConsumed(1978500);

            Assert.AreEqual(array[0], new BigInteger(1));
            Assert.AreEqual(array[1], new BigInteger(10));
            Assert.AreEqual(array[2], new BigInteger(17));
        }

        [TestMethod]
        public void Uint160ForeachTest()
        {
            var array = Contract.UInt160Foreach()!;
            AssertGasConsumed(1546110);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("0000000000000000000000000000000000000000", (array[0] as ByteString)!.GetSpan().ToHexString());
            Assert.AreEqual("0000000000000000000000000000000000000000", (array[1] as ByteString)!.GetSpan().ToHexString());
        }

        [TestMethod]
        public void Uint256ForeachTest()
        {
            var array = Contract.UInt256Foreach()!;
            AssertGasConsumed(1546110);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000000", (array[0] as ByteString)!.GetSpan().ToHexString());
            Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000000", (array[1] as ByteString)!.GetSpan().ToHexString());
        }

        [TestMethod]
        public void EcpointForeachTest()
        {
            var array = Contract.ECPointForeach()!;
            AssertGasConsumed(2038530);

            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (array[0] as ByteString)!.GetSpan().ToHexString());
            Assert.AreEqual("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9", (array[1] as ByteString)!.GetSpan().ToHexString());
        }

        [TestMethod]
        public void BigintegerForeachTest()
        {
            var array = Contract.BigIntegerForeach()!;
            AssertGasConsumed(2042490);
            BigInteger[] expected = [10_000, 1000_000, 1000_000_000, 1000_000_000_000_000_000];

            Assert.AreEqual(4, array.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(array[i], expected[i]);
            }
        }

        [TestMethod]
        public void ObjectarrayForeachTest()
        {
            var array = Contract.ObjectArrayForeach()!;
            AssertGasConsumed(2040240);

            Assert.AreEqual(3, array.Count);
            CollectionAssert.AreEqual(array[0] as byte[], new byte[] { 0x01, 0x02 });
            Assert.AreEqual("test", (array[1] as ByteString)!.GetString());
            Assert.AreEqual(array[2], new BigInteger(123));
        }
    }
}
