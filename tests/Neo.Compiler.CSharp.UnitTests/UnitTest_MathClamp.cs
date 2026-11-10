// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_MathClamp.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Testing.Exceptions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_MathClamp : DebugAndTestBase<Contract_MathClamp>
    {
        [TestMethod]
        public void TestClampConstantBoundsSkipsRangeValidation()
        {
            Contract.ClampConstantBounds(50);
            Contract.ClampEqualBounds(50);
            Contract.ClampNamedArguments(50);
            Contract.ClampConstantExpressions(50);
            Contract.ClampByteConstantBounds(50);
            Contract.ClampLongConstantBounds(50);
            Contract.ClampBigIntegerConstantBounds(5);

            // Dynamic bounds keep the runtime min <= max validation.
            Contract.ClampDynamicBounds(50, 0, 100);
        }

        [TestMethod]
        public void TestClampInvalidBoundsVariants()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.ClampBigIntegerInvalidBounds(0));
            Assert.ThrowsExactly<TestException>(() => Contract.ClampInvalidNamedBounds(7));
        }

        [TestMethod]
        public void TestClampMoreOverloadsAndExtremes()
        {
            Assert.AreEqual(int.MinValue, Contract.ClampIntExtremeBounds(int.MinValue));
            Assert.AreEqual(int.MaxValue, Contract.ClampIntExtremeBounds(int.MaxValue));
            Assert.AreEqual(0, Contract.ClampIntExtremeBounds(0));

            Assert.AreEqual((sbyte)-5, Contract.ClampSByteConstantBounds(sbyte.MinValue));
            Assert.AreEqual((sbyte)5, Contract.ClampSByteConstantBounds(sbyte.MaxValue));
            Assert.AreEqual((sbyte)3, Contract.ClampSByteConstantBounds(3));

            Assert.AreEqual((short)-300, Contract.ClampShortConstantBounds(short.MinValue));
            Assert.AreEqual((short)300, Contract.ClampShortConstantBounds(short.MaxValue));

            Assert.AreEqual((ushort)10, Contract.ClampUShortConstantBounds(0));
            Assert.AreEqual((ushort)60000, Contract.ClampUShortConstantBounds(ushort.MaxValue));

            Assert.AreEqual(1u, Contract.ClampUIntConstantBounds(0u));
            Assert.AreEqual(uint.MaxValue, Contract.ClampUIntConstantBounds(uint.MaxValue));
        }

        [TestMethod]
        public void TestClampSideEffectValueWithConstantBounds()
        {
            Assert.AreEqual(1100, Contract.ClampSideEffectValueConstantBounds());
        }

        [TestMethod]
        public void TestClampConstantBounds()
        {
            Assert.AreEqual(0, Contract.ClampConstantBounds(-10));
            Assert.AreEqual(50, Contract.ClampConstantBounds(50));
            Assert.AreEqual(100, Contract.ClampConstantBounds(150));
            Assert.AreEqual(0, Contract.ClampConstantBounds(0));
            Assert.AreEqual(100, Contract.ClampConstantBounds(100));
        }

        [TestMethod]
        public void TestClampEqualBounds()
        {
            Assert.AreEqual(42, Contract.ClampEqualBounds(-1));
            Assert.AreEqual(42, Contract.ClampEqualBounds(42));
            Assert.AreEqual(42, Contract.ClampEqualBounds(1000));
        }

        [TestMethod]
        public void TestClampInvalidConstantBounds()
        {
            Assert.ThrowsExactly<TestException>(() => Contract.ClampInvalidBounds(5));
        }

        [TestMethod]
        public void TestClampDynamicBounds()
        {
            Assert.AreEqual(5, Contract.ClampDynamicBounds(5, 0, 10));
            Assert.AreEqual(10, Contract.ClampDynamicBounds(50, 0, 10));
            Assert.ThrowsExactly<TestException>(() => Contract.ClampDynamicBounds(5, 10, 0));
        }

        [TestMethod]
        public void TestClampNamedArguments()
        {
            Assert.AreEqual(0, Contract.ClampNamedArguments(-5));
            Assert.AreEqual(50, Contract.ClampNamedArguments(50));
            Assert.AreEqual(100, Contract.ClampNamedArguments(500));
        }

        [TestMethod]
        public void TestClampConstantExpressions()
        {
            Assert.AreEqual(0, Contract.ClampConstantExpressions(-5));
            Assert.AreEqual(60, Contract.ClampConstantExpressions(60));
            Assert.AreEqual(100, Contract.ClampConstantExpressions(101));
        }

        [TestMethod]
        public void TestClampConstantBoundsOverloads()
        {
            Assert.AreEqual((byte)10, Contract.ClampByteConstantBounds(0));
            Assert.AreEqual((byte)200, Contract.ClampByteConstantBounds(255));
            Assert.AreEqual((byte)100, Contract.ClampByteConstantBounds(100));

            Assert.AreEqual(long.MinValue, Contract.ClampLongConstantBounds(long.MinValue));
            Assert.AreEqual(0L, Contract.ClampLongConstantBounds(long.MaxValue));
            Assert.AreEqual(-7L, Contract.ClampLongConstantBounds(-7));

            Assert.AreEqual(5UL, Contract.ClampULongConstantBounds(0UL));
            Assert.AreEqual(ulong.MaxValue, Contract.ClampULongConstantBounds(ulong.MaxValue));

            Assert.AreEqual(new BigInteger(-10), Contract.ClampBigIntegerConstantBounds(-100));
            Assert.AreEqual(new BigInteger(10), Contract.ClampBigIntegerConstantBounds(100));
            Assert.AreEqual(new BigInteger(3), Contract.ClampBigIntegerConstantBounds(3));
        }

        [TestMethod]
        public void TestClampEvaluationOrderAndNesting()
        {
            Assert.AreEqual(2010, Contract.ClampEvaluationOrder());
            Assert.AreEqual(45, Contract.ClampNested(50));
            Assert.AreEqual(0, Contract.ClampNested(-5));
            Assert.AreEqual(19, Contract.ClampNested(3));
        }
    }
}
