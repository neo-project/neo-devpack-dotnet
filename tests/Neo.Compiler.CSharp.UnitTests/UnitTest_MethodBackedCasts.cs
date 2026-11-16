// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_MethodBackedCasts.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_MethodBackedCasts
{
    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void CastsApplyOperatorAndStandardConversions(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract("""
            using System.Numerics;
            using Neo.SmartContract.Framework;

            public struct ByteBox
            {
                public byte Value;
                public static explicit operator ByteBox(byte value) => new ByteBox { Value = value };
            }

            public struct IntBox
            {
                public int Value;
                public IntBox(int value) { Value = value; }
                public static explicit operator int(IntBox value) => value.Value;
            }

            public struct NullableIntBox
            {
                public int? Value;
                public NullableIntBox(int? value) { Value = value; }
                public static explicit operator int?(NullableIntBox value) => value.Value;
            }

            public struct NullableByteBox
            {
                public byte? Value;
                public static explicit operator NullableByteBox(byte? value) => new NullableByteBox { Value = value };
            }

            public class Contract : SmartContract
            {
                public static byte? BigIntegerToByte(BigInteger value) => (byte?)value;
                public static uint? BigIntegerToUInt(BigInteger value) => (uint?)value;
                public static byte? NullableBigIntegerToByte(BigInteger? value) => (byte?)value;
                public static ulong? NullableBigIntegerToULong(BigInteger? value) => (ulong?)value;
                public static BigInteger UnwrapInteger(int? value) => (BigInteger)value;
                public static byte UncheckedByteBox(int value) => unchecked((ByteBox)value).Value;
                public static byte CheckedByteBox(int value) => checked((ByteBox)value).Value;
                public static byte? NullableParamByteBox(int? value) => unchecked((NullableByteBox)value).Value;
                public static byte? CheckedNullableParamByteBox(int? value) => checked((NullableByteBox)value).Value;
                public static byte UncheckedIntBox(int value) => unchecked((byte)new IntBox(value));
                private static IntBox? CreateIntBox(int? value) => value is null ? null : new IntBox(value.Value);
                public static int? LiftedIntBox(int? value) => (int?)CreateIntBox(value);
                public static int UnwrappedIntBox(int? value) => (int)CreateIntBox(value);
                public static byte? LiftedIntBoxToByte(int? value) => unchecked((byte?)CreateIntBox(value));
                public static byte? NullableReturnToNullableByte(int? value) => unchecked((byte?)new NullableIntBox(value));
                public static int NullableReturnToInt(int? value) => (int)new NullableIntBox(value);
                public static byte NullableReturnToByte(int? value) => unchecked((byte)new NullableIntBox(value));
                private static int _calls;
                private static IntBox? Evaluate(int? value) { _calls++; return CreateIntBox(value); }
                public static int CastOperatorOnce(int? value)
                {
                    _calls = 0;
                    byte? result = unchecked((byte?)Evaluate(value));
                    return _calls * 1000 + (result ?? 0);
                }
            }
            """, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var contract = new TestEngine(true).Deploy<MethodBackedCastContract>(nef, manifest);

        Assert.AreEqual(new BigInteger(44), contract.UncheckedByteBox(300));
        Assert.ThrowsExactly<TestException>(() => contract.CheckedByteBox(300));
        Assert.IsNull(contract.NullableParamByteBox(null));
        Assert.AreEqual(new BigInteger(44), contract.NullableParamByteBox(300));
        Assert.ThrowsExactly<TestException>(() => contract.CheckedNullableParamByteBox(300));
        Assert.AreEqual(new BigInteger(44), contract.UncheckedIntBox(300));
        Assert.AreEqual(new BigInteger(200), contract.BigIntegerToByte(200));
        Assert.ThrowsExactly<TestException>(() => contract.BigIntegerToByte(300));
        Assert.ThrowsExactly<TestException>(() => contract.BigIntegerToUInt(-1));
        Assert.IsNull(contract.NullableBigIntegerToByte(null));
        Assert.AreEqual(new BigInteger(200), contract.NullableBigIntegerToByte(200));
        Assert.ThrowsExactly<TestException>(() => contract.NullableBigIntegerToByte(300));
        Assert.ThrowsExactly<TestException>(() => contract.NullableBigIntegerToULong(-1));
        Assert.AreEqual(new BigInteger(42), contract.UnwrapInteger(42));
        Assert.ThrowsExactly<TestException>(() => contract.UnwrapInteger(null));
        Assert.IsNull(contract.LiftedIntBox(null));
        Assert.AreEqual(new BigInteger(7), contract.LiftedIntBox(7));
        Assert.ThrowsExactly<TestException>(() => contract.UnwrappedIntBox(null));
        Assert.IsNull(contract.LiftedIntBoxToByte(null));
        Assert.AreEqual(new BigInteger(44), contract.LiftedIntBoxToByte(300));
        Assert.IsNull(contract.NullableReturnToNullableByte(null));
        Assert.AreEqual(new BigInteger(44), contract.NullableReturnToNullableByte(300));
        Assert.ThrowsExactly<TestException>(() => contract.NullableReturnToInt(null));
        Assert.AreEqual(new BigInteger(300), contract.NullableReturnToInt(300));
        Assert.ThrowsExactly<TestException>(() => contract.NullableReturnToByte(null));
        Assert.AreEqual(new BigInteger(44), contract.NullableReturnToByte(300));
        Assert.AreEqual(new BigInteger(1000), contract.CastOperatorOnce(null));
        Assert.AreEqual(new BigInteger(1044), contract.CastOperatorOnce(300));
    }

    public abstract class MethodBackedCastContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("bigIntegerToByte")] public abstract BigInteger? BigIntegerToByte(BigInteger value);
        [DisplayName("bigIntegerToUInt")] public abstract BigInteger? BigIntegerToUInt(BigInteger value);
        [DisplayName("nullableBigIntegerToByte")] public abstract BigInteger? NullableBigIntegerToByte(BigInteger? value);
        [DisplayName("nullableBigIntegerToULong")] public abstract BigInteger? NullableBigIntegerToULong(BigInteger? value);
        [DisplayName("unwrapInteger")] public abstract BigInteger? UnwrapInteger(BigInteger? value);
        [DisplayName("uncheckedByteBox")] public abstract BigInteger? UncheckedByteBox(BigInteger value);
        [DisplayName("checkedByteBox")] public abstract BigInteger? CheckedByteBox(BigInteger value);
        [DisplayName("nullableParamByteBox")] public abstract BigInteger? NullableParamByteBox(BigInteger? value);
        [DisplayName("checkedNullableParamByteBox")] public abstract BigInteger? CheckedNullableParamByteBox(BigInteger? value);
        [DisplayName("uncheckedIntBox")] public abstract BigInteger? UncheckedIntBox(BigInteger value);
        [DisplayName("liftedIntBox")] public abstract BigInteger? LiftedIntBox(BigInteger? value);
        [DisplayName("unwrappedIntBox")] public abstract BigInteger? UnwrappedIntBox(BigInteger? value);
        [DisplayName("liftedIntBoxToByte")] public abstract BigInteger? LiftedIntBoxToByte(BigInteger? value);
        [DisplayName("nullableReturnToNullableByte")] public abstract BigInteger? NullableReturnToNullableByte(BigInteger? value);
        [DisplayName("nullableReturnToInt")] public abstract BigInteger? NullableReturnToInt(BigInteger? value);
        [DisplayName("nullableReturnToByte")] public abstract BigInteger? NullableReturnToByte(BigInteger? value);
        [DisplayName("castOperatorOnce")] public abstract BigInteger? CastOperatorOnce(BigInteger? value);
    }
}
