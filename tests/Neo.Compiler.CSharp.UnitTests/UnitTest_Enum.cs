// Copyright (C) 2015-2026 The Neo Project.
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
using System;
using System.Collections.Generic;
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
            Assert.AreEqual(new Integer(21), Contract.TestEnumParseWithContinuation());
            Assert.ThrowsException<TestException>(() => Contract.TestEnumParse("InvalidValue"));
            AssertGasConsumed(1067580);
        }

        [TestMethod]
        public void TestEnumParseWithLargeEnum()
        {
            var context = TestHelper.CompileSingleContract("""
                using Neo.SmartContract.Framework;
                using System;

                public class Contract : SmartContract
                {
                    private enum LargeEnum
                    {
                        Value00,
                        Value01,
                        Value02,
                        Value03,
                        Value04,
                        Value05,
                        Value06,
                        Value07,
                        Value08,
                        Value09,
                        Value10,
                        Value11,
                        Value12,
                        Value13,
                        Value14,
                        Value15,
                        Value16,
                        Value17,
                        Value18,
                        Value19,
                        Value20,
                        Value21,
                        Value22,
                        Value23,
                        Value24,
                        Value25,
                        Value26,
                        Value27,
                        Value28,
                        Value29,
                        Value30,
                        Value31
                    }

                    public static int Parse(string value)
                    {
                        var parsed = (LargeEnum)Enum.Parse(typeof(LargeEnum), value);
                        return (int)parsed + 10;
                    }
                }
                """);

            Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics.Select(p => p.ToString())));
            _ = context.CreateExecutable();
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
        public void TestEnumParseIgnoreCaseFromExpression()
        {
            Assert.AreEqual(new Integer(1), Contract.TestEnumParseIgnoreCaseFromExpression("value1", false));
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
        public void TestEnumTryParseIgnoreCaseFromExpression()
        {
            Assert.IsTrue(Contract.TestEnumTryParseIgnoreCaseFromExpression("value1", false));
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
        public void TestEnumGetName_EmitSwitch_EmitsJmpeqPerCase()
        {
            // Locate testEnumGetName in the manifest and determine its byte range.
            var methods = Contract_Enum.Manifest.Abi.Methods
                .OrderBy(m => m.Offset).ToArray();
            var methodDesc = methods.First(m => m.Name == "testEnumGetName");
            int startOffset = methodDesc.Offset;
            int endOffset = methods.First(m => m.Offset > startOffset).Offset;

            // Walk the instructions inside the method boundary.
            var script = new Neo.VM.Script(Contract_Enum.Nef.Script);
            var opcodes = new List<Neo.VM.OpCode>();
            for (int pos = startOffset; pos < endOffset;)
            {
                var inst = script.GetInstruction(pos);
                opcodes.Add(inst.OpCode);
                pos += inst.Size;
            }

            // EmitSwitch generates exactly one JMPEQ per enum member.
            // TestEnum has 3 members: Value1, Value2, Value3.
            int jmpeqCount = opcodes.Count(
                op => op == Neo.VM.OpCode.JMPEQ || op == Neo.VM.OpCode.JMPEQ_L);
            Assert.AreEqual(3, jmpeqCount, $"Expected exactly 3 JMPEQ opcodes (one per enum member), found {jmpeqCount}.");
        }

        [TestMethod]
        public void TestEnumGetName_EmitSwitch_GasDeltaIsConstantPerSkippedCase()
        {
            // Prime the engine so the first call is never cold.
            Contract.TestEnumGetName(1);
            long gasCase1 = Engine.FeeConsumed.Value;

            Contract.TestEnumGetName(2);
            long gasCase2 = Engine.FeeConsumed.Value;

            Contract.TestEnumGetName(3);
            long gasCase3 = Engine.FeeConsumed.Value;

            long delta12 = gasCase2 - gasCase1;
            long delta23 = gasCase3 - gasCase2;

            // Each additional skipped case costs exactly delta12 (DUP + PUSH + JMPEQ).
            // If NUMEQUAL+JMPIF+NOP were used the deltas would not be equal.
            Assert.AreEqual(delta12, delta23, $"Gas delta per skipped case must be constant. Got delta12={delta12}, delta23={delta23}.");
            Assert.IsTrue(delta12 > 0, "Each additional skipped case must cost positive gas.");
        }

        [TestMethod]
        public void TestUlongEnumGetName_DoesNotThrowForValueAboveLongMaxValue()
        {
            // Regression test: HandleEnumGetName must not narrow enum constants through
            // Convert.ToInt64, since a ulong-backed enum member can exceed long.MaxValue.
            Assert.AreEqual("Value1", Contract.TestUlongEnumGetName(1));
            Assert.AreEqual("MaxValue", Contract.TestUlongEnumGetName((System.Numerics.BigInteger)ulong.MaxValue));
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
