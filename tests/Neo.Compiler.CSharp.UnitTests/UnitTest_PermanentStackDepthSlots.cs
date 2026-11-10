// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_PermanentStackDepthSlots.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_PermanentStackDepthSlots
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System;
        using System.ComponentModel;
        using System.Runtime.CompilerServices;

        public class Contract : SmartContract
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int ScaleRate(int divisor) => 10 / divisor;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int ComputeBonus(int divisor) => 20 / divisor;

            [DisplayName("calculatePayout")]
            public static int CalculatePayout(int divisor)
            {
                int payout = ScaleRate(divisor);
                int reserveTag = 111;
                payout += ComputeBonus(divisor);
                return payout * 1000 + reserveTag;
            }

            [DisplayName("settleWithFallback")]
            public static int SettleWithFallback(int divisor)
            {
                (int q, int r) = Math.DivRem(10, divisor);
                string tag = new string('a', 5);
                int total = q + r + tag.Length;
                try
                {
                    total += ThrowIfZero(0);
                }
                catch (Exception)
                {
                    total += 7;
                }
                finally
                {
                    total += 100;
                }
                return total;
            }

            private static int ThrowIfZero(int value)
            {
                if (value == 0) throw new Exception("revert");
                return value;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.Basic)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void StackDepthSlotsRemainReservedAcrossCalls(CompilationOptions.OptimizationType optimization)
    {
        var context = TestHelper.CompileSingleContract(Source, new CompilationOptions { Optimize = optimization });
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));

        var engine = new TestEngine(true);
        var contract = engine.Deploy<StackDepthContract>(context.CreateExecutable(), context.CreateManifest());

        Assert.AreEqual(new BigInteger(6111), contract.CalculatePayout(5));
        Assert.AreEqual(new BigInteger(116), contract.SettleWithFallback(3));
    }

    public abstract class StackDepthContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("calculatePayout")]
        public abstract BigInteger? CalculatePayout(BigInteger divisor);

        [DisplayName("settleWithFallback")]
        public abstract BigInteger? SettleWithFallback(BigInteger divisor);
    }
}
