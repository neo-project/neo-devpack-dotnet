// Copyright (C) 2015-2026 The Neo Project.
// Licensed under the MIT license.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_NarrowSignedDivRemOverflow
{
    private const string Source = """
        using System;
        using Neo.SmartContract.Framework;

        public class Cases : SmartContract
        {
            public static (sbyte Q, sbyte R) MathSByte(sbyte a, sbyte b) => Math.DivRem(a, b);
            public static (sbyte Q, sbyte R) NumericSByte(sbyte a, sbyte b) => sbyte.DivRem(a, b);
            public static (short Q, short R) MathShort(short a, short b) => Math.DivRem(a, b);
            public static (short Q, short R) NumericShort(short a, short b) => short.DivRem(a, b);

            public static int CaughtMathSByte(sbyte a, sbyte b)
            {
                try
                {
                    var (q, _) = Math.DivRem(a, b);
                    return q;
                }
                catch
                {
                    return -999;
                }
            }
        }
        """;

    public abstract class CasesContract(SmartContractInitialize initialize)
        : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("mathSByte")]
        public abstract IList<object>? MathSByte(BigInteger a, BigInteger b);

        [DisplayName("numericSByte")]
        public abstract IList<object>? NumericSByte(BigInteger a, BigInteger b);

        [DisplayName("mathShort")]
        public abstract IList<object>? MathShort(BigInteger a, BigInteger b);

        [DisplayName("numericShort")]
        public abstract IList<object>? NumericShort(BigInteger a, BigInteger b);

        [DisplayName("caughtMathSByte")]
        public abstract BigInteger? CaughtMathSByte(BigInteger a, BigInteger b);
    }

    [TestMethod]
    public void NarrowSignedDivRem_MinValueByMinusOne_MatchesDotNet()
    {
        var context = TestHelper.CompileSingleContract(Source);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var contract = new TestEngine(true).Deploy<CasesContract>(nef, manifest);

        var sbyteExpected = Math.DivRem(sbyte.MinValue, (sbyte)-1);
        var shortExpected = Math.DivRem(short.MinValue, (short)-1);
        Assert.AreEqual((sbyte.MinValue, (sbyte)0), sbyteExpected);
        Assert.AreEqual((short.MinValue, (short)0), shortExpected);
        Assert.AreEqual(sbyteExpected, sbyte.DivRem(sbyte.MinValue, -1));
        Assert.AreEqual(shortExpected, short.DivRem(short.MinValue, -1));

        Assert.AreEqual(new BigInteger(sbyteExpected.Quotient), contract.CaughtMathSByte(sbyte.MinValue, -1));
        AssertDivRem(contract.MathSByte(sbyte.MinValue, -1), sbyteExpected.Quotient, sbyteExpected.Remainder);
        AssertDivRem(contract.NumericSByte(sbyte.MinValue, -1), sbyteExpected.Quotient, sbyteExpected.Remainder);
        AssertDivRem(contract.MathShort(short.MinValue, -1), shortExpected.Quotient, shortExpected.Remainder);
        AssertDivRem(contract.NumericShort(short.MinValue, -1), shortExpected.Quotient, shortExpected.Remainder);

        AssertDivRem(contract.MathSByte(sbyte.MinValue, 2), -64, 0);
        AssertDivRem(contract.MathShort(-5, -1), 5, 0);
    }

    private static void AssertDivRem(IList<object>? actual, BigInteger expectedQuotient, BigInteger expectedRemainder)
    {
        Assert.IsNotNull(actual);
        Assert.AreEqual(expectedQuotient, (BigInteger)actual[0]);
        Assert.AreEqual(expectedRemainder, (BigInteger)actual[1]);
    }
}
