// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_Enum.cs file belongs to the neo project and is free
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
using Neo.VM.Types;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Enum : DebugAndTestBase<Contract_Enum>
    {
        protected override bool TestGasConsume => false;

        [TestMethod]
        public void TestEnumParse()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParse("Value1"));
            AssertGasConsumed(1049490);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParse("Value2"));
            AssertGasConsumed(1050810);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParse("Value3"));
            AssertGasConsumed(1052130);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParse("InvalidValue"));
            AssertGasConsumed(1067580);
        }

        [TestMethod]
        public void TestEnumParseIgnoreCase()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParseIgnoreCase("value1", true));
            AssertGasConsumed(1688250);
            Assert.AreEqual(new Integer(2), Contract.TestEnumParseIgnoreCase("VALUE2", true));
            AssertGasConsumed(1686990);
            Assert.AreEqual(new Integer(3), Contract.TestEnumParseIgnoreCase("VaLuE3", true));
            AssertGasConsumed(1689570);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("value1", false));
            AssertGasConsumed(1065270);
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseIgnoreCase("InvalidValue", true));
            AssertGasConsumed(2098560);
        }

        [TestMethod]
        public void TestEnumTryParse()
        {
            Assert.IsTrue(Contract.TestEnumTryParse("Value1"));
            AssertGasConsumed(1049610);
            Assert.IsTrue(Contract.TestEnumTryParse("Value2"));
            AssertGasConsumed(1050930);
            Assert.IsTrue(Contract.TestEnumTryParse("Value3"));
            AssertGasConsumed(1052250);
            Assert.IsFalse(Contract.TestEnumTryParse("InvalidValue"));
            AssertGasConsumed(1052250);
        }

        [TestMethod]
        public void TestEnumTryParseIgnoreCase()
        {
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("value1", true));
            AssertGasConsumed(1688490);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VALUE2", true));
            AssertGasConsumed(1687230);
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCase("VaLuE3", true));
            AssertGasConsumed(1689810);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("value1", false));
            AssertGasConsumed(1049880);
            Assert.IsFalse(Contract.TestEnumTryParseIgnoreCase("InvalidValue", true));
            AssertGasConsumed(2083290);
        }

        [TestMethod]
        public void TestEnumIsDefined()
        {
            Assert.IsTrue(Contract.TestEnumIsDefined(1));
            AssertGasConsumed(1049010);
            Assert.IsTrue(Contract.TestEnumIsDefined(2));
            AssertGasConsumed(1050120);
            Assert.IsTrue(Contract.TestEnumIsDefined(3));
            AssertGasConsumed(1051230);
            Assert.IsFalse(Contract.TestEnumIsDefined(0));
            AssertGasConsumed(1051230);
            Assert.IsFalse(Contract.TestEnumIsDefined(4));
            AssertGasConsumed(1051230);
        }

        [TestMethod]
        public void TestEnumIsDefinedByName()
        {
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value1"));
            AssertGasConsumed(1049430);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value2"));
            AssertGasConsumed(1050750);
            Assert.IsTrue(Contract.TestEnumIsDefinedByName("Value3"));
            AssertGasConsumed(1052070);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("value1"));
            AssertGasConsumed(1052070);
            Assert.IsFalse(Contract.TestEnumIsDefinedByName("InvalidValue"));
            AssertGasConsumed(1052070);
        }

        [TestMethod]
        public void TestEnumGetName()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetName(1));
            AssertGasConsumed(1048920);
            Assert.AreEqual("Value2", Contract.TestEnumGetName(2));
            AssertGasConsumed(1050030);
            Assert.AreEqual("Value3", Contract.TestEnumGetName(3));
            AssertGasConsumed(1051140);
            Assert.IsNull(Contract.TestEnumGetName(0));
            AssertGasConsumed(1050930);
            Assert.IsNull(Contract.TestEnumGetName(4));
            AssertGasConsumed(1050930);
        }

        [TestMethod]
        public void TestEnumGetNameWithType()
        {
            Assert.AreEqual("Value1", Contract.TestEnumGetNameWithType(1));
            AssertGasConsumed(1049220);
            Assert.AreEqual("Value2", Contract.TestEnumGetNameWithType(2));
            AssertGasConsumed(1050330);
            Assert.AreEqual("Value3", Contract.TestEnumGetNameWithType(3));
            AssertGasConsumed(1051440);
            Assert.IsNull(Contract.TestEnumGetNameWithType(0));
            AssertGasConsumed(1051230);
            Assert.IsNull(Contract.TestEnumGetNameWithType(4));
            AssertGasConsumed(1051230);
        }

        [TestMethod]
        public void TestEnumHasFlagAndToString()
        {
            Assert.IsTrue(Contract.TestEnumHasFlag(3, 1));
            Assert.IsTrue(Contract.TestEnumHasFlag(3, 2));
            Assert.IsFalse(Contract.TestEnumHasFlag(2, 1));

            Assert.AreEqual("Value1", Contract.TestEnumToString(1));
            Assert.AreEqual("Value2", Contract.TestEnumToString(2));
            Assert.AreEqual("Value3", Contract.TestEnumToString(3));
            Assert.AreEqual("99", Contract.TestEnumToStringUnknown(99));
        }

        [TestMethod]
        public void TestEnumGenericParse()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParseGeneric("Value1"));
            Assert.AreEqual(new Integer(2), Contract.TestEnumParseGenericIgnoreCase("value2", true));
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParseGenericIgnoreCase("value1", false));
        }

        [TestMethod]
        public void TestEnumGenericTryParse()
        {
            Assert.IsTrue(Contract.TestEnumTryParseGeneric("Value3"));
            Assert.IsTrue(Contract.TestEnumTryParseGenericIgnoreCase("value2", true));
            Assert.IsFalse(Contract.TestEnumTryParseGeneric("Unknown"));
            Assert.IsFalse(Contract.TestEnumTryParseGenericIgnoreCase("unknown", false));
        }

        [TestMethod]
        public void TestEnumGenericGetValuesAndNames()
        {
            var names = Contract.TestEnumGetNamesGeneric()!;
            CollectionAssert.AreEqual(new[] { "Value1", "Value2", "Value3" }, names.Select(n => ((Neo.VM.Types.ByteString)n!).GetString()).ToArray());

            var values = Contract.TestEnumGetValuesGeneric()!;
            CollectionAssert.AreEqual(new[] { new Neo.VM.Types.Integer(1), new Neo.VM.Types.Integer(2), new Neo.VM.Types.Integer(3) }, values.Select(v => new Neo.VM.Types.Integer((System.Numerics.BigInteger)v!)).ToArray());
        }
    }
}
