// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_BitwiseComplement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_BitwiseComplement
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using System.Numerics;

        public enum ByteFlags : byte { None = 0 }
        public enum UShortFlags : ushort { None = 0 }
        public enum UIntFlags : uint { None = 0 }
        public enum ULongFlags : ulong { None = 0 }
        public enum SignedFlags : int { None = 0 }

        public class Contract : SmartContract
        {
            public static uint UInt(uint value, bool check) => check ? checked(~value) : unchecked(~value);
            public static ulong ULong(ulong value, bool check) => check ? checked(~value) : unchecked(~value);
            public static int Int(int value, bool check) => check ? checked(~value) : unchecked(~value);
            public static long Long(long value, bool check) => check ? checked(~value) : unchecked(~value);
            public static int Byte(byte value) => ~value;
            public static int SByte(sbyte value) => ~value;
            public static int Short(short value) => ~value;
            public static int UShort(ushort value) => ~value;
            public static int Char(char value) => ~value;
            public static BigInteger Big(BigInteger value, bool check) => check ? checked(~value) : unchecked(~value);
            public static uint? NullableUInt(uint? value) => ~value;
            public static ulong? NullableULong(ulong? value) => ~value;
            public static int EnumByte(byte value, bool check) => (int)(check ? checked(~(ByteFlags)value) : unchecked(~(ByteFlags)value));
            public static int EnumUShort(ushort value, bool check) => (int)(check ? checked(~(UShortFlags)value) : unchecked(~(UShortFlags)value));
            public static long EnumUInt(uint value, bool check) => (long)(check ? checked(~(UIntFlags)value) : unchecked(~(UIntFlags)value));
            public static BigInteger EnumULong(ulong value, bool check) => (BigInteger)(ulong)(check ? checked(~(ULongFlags)value) : unchecked(~(ULongFlags)value));
            public static bool EnumULongPositive(ulong value) => (~(ULongFlags)value) > (ULongFlags)0;
            public static int EnumSigned(int value) => (int)(~(SignedFlags)value);
            public static int? NullableEnumByte(ByteFlags? value) => (int?)(~value);

            private static int _evaluations;
            private static uint Evaluate(uint value) { _evaluations++; return value; }

            public static BigInteger Once(uint value)
            {
                _evaluations = 0;
                uint result = ~Evaluate(value);
                return (BigInteger)result * 10 + _evaluations;
            }
        }
        """;

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, false)]
    [DataRow(CompilationOptions.OptimizationType.None, true)]
    [DataRow(CompilationOptions.OptimizationType.All, false)]
    [DataRow(CompilationOptions.OptimizationType.All, true)]
    public void UIntComplementMatchesClr(CompilationOptions.OptimizationType optimization, bool check)
    {
        var contract = Deploy(optimization);
        foreach (uint value in new uint[] { 0, 1, uint.MaxValue, 0x7fffffff, 0x80000000, 0x80000001, 0xa5a55a5a })
            Assert.AreEqual(new BigInteger(~value), contract.UInt(value, check), $"value={value}, checked={check}");
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None, false)]
    [DataRow(CompilationOptions.OptimizationType.None, true)]
    [DataRow(CompilationOptions.OptimizationType.All, false)]
    [DataRow(CompilationOptions.OptimizationType.All, true)]
    public void ULongComplementMatchesClr(CompilationOptions.OptimizationType optimization, bool check)
    {
        var contract = Deploy(optimization);
        foreach (ulong value in new ulong[] { 0, 1, ulong.MaxValue, 0x7fffffffffffffff, 0x8000000000000000, 0x8000000000000001, 0xa5a55a5aa5a55a5a })
            Assert.AreEqual(new BigInteger(~value), contract.ULong(value, check), $"value={value}, checked={check}");
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SignedComplementMatchesClr(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        foreach (bool check in new[] { false, true })
        {
            foreach (int value in new[] { int.MinValue, -1, 0, 1, int.MaxValue })
                Assert.AreEqual(new BigInteger(~value), contract.Int(value, check));
            foreach (long value in new[] { long.MinValue, -1, 0, 1, long.MaxValue })
                Assert.AreEqual(new BigInteger(~value), contract.Long(value, check));
        }
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void SmallIntegerComplementUsesIntPromotion(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        foreach (byte value in new byte[] { 0, 1, byte.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.Byte(value));
        foreach (sbyte value in new sbyte[] { sbyte.MinValue, -1, 0, 1, sbyte.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.SByte(value));
        foreach (short value in new short[] { short.MinValue, -1, 0, 1, short.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.Short(value));
        foreach (ushort value in new ushort[] { 0, 1, ushort.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.UShort(value));
        foreach (char value in new[] { char.MinValue, (char)1, char.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.Char(value));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void BigIntegerComplementPreservesWideSignedValues(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        BigInteger wide = (BigInteger.One << 200) + 1;
        foreach (bool check in new[] { false, true })
            foreach (BigInteger value in new BigInteger[] { -wide, -1, 0, 1, wide })
                Assert.AreEqual(~value, contract.Big(value, check));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void NonNullLiftedComplementUsesUnsignedWidth(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        foreach (uint value in new uint[] { 0, 1, uint.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.NullableUInt(value));
        foreach (ulong value in new ulong[] { 0, 1, ulong.MaxValue })
            Assert.AreEqual(new BigInteger(~value), contract.NullableULong(value));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void UnsignedEnumComplementMatchesClr(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        foreach (bool check in new[] { false, true })
        {
            foreach (byte value in new byte[] { 0, 1, 200, byte.MaxValue })
                Assert.AreEqual(new BigInteger((byte)~value), contract.EnumByte(value, check));
            foreach (ushort value in new ushort[] { 0, 1, 50000, ushort.MaxValue })
                Assert.AreEqual(new BigInteger((ushort)~value), contract.EnumUShort(value, check));
            foreach (uint value in new uint[] { 0, 1, 5, uint.MaxValue })
                Assert.AreEqual(new BigInteger(~value), contract.EnumUInt(value, check));
            foreach (ulong value in new ulong[] { 0, 1, 5, ulong.MaxValue })
                Assert.AreEqual(new BigInteger(~value), contract.EnumULong(value, check));
        }

        Assert.IsTrue(contract.EnumULongPositive(5));
        Assert.IsFalse(contract.EnumULongPositive(ulong.MaxValue));
        Assert.AreEqual(new BigInteger(~5), contract.EnumSigned(5));
        Assert.AreEqual(new BigInteger(byte.MaxValue), contract.NullableEnumByte(0));
        Assert.IsNull(contract.NullableEnumByte(null));
    }

    [TestMethod]
    [DataRow(CompilationOptions.OptimizationType.None)]
    [DataRow(CompilationOptions.OptimizationType.All)]
    public void ComplementEvaluatesOperandOnce(CompilationOptions.OptimizationType optimization)
    {
        var contract = Deploy(optimization);
        foreach (uint value in new uint[] { 0, 1, uint.MaxValue })
            Assert.AreEqual(new BigInteger(~value) * 10 + 1, contract.Once(value));
    }

    private static ComplementContract Deploy(CompilationOptions.OptimizationType optimization)
    {
        var options = TestHelper.CreateDefaultOptions();
        options.Optimize = optimization;
        var context = TestHelper.CompileSingleContract(Source, options);
        Assert.IsTrue(context.Success, string.Join(Environment.NewLine, context.Diagnostics));
        var (nef, manifest, _) = context.CreateResults();
        var engine = new TestEngine(true);
        return engine.Deploy<ComplementContract>(nef, manifest);
    }

    public abstract class ComplementContract(SmartContractInitialize initialize) : SmartContract.Testing.SmartContract(initialize)
    {
        [DisplayName("uInt")] public abstract BigInteger? UInt(BigInteger value, bool check);
        [DisplayName("uLong")] public abstract BigInteger? ULong(BigInteger value, bool check);
        [DisplayName("int")] public abstract BigInteger? Int(BigInteger value, bool check);
        [DisplayName("long")] public abstract BigInteger? Long(BigInteger value, bool check);
        [DisplayName("byte")] public abstract BigInteger? Byte(BigInteger value);
        [DisplayName("sByte")] public abstract BigInteger? SByte(BigInteger value);
        [DisplayName("short")] public abstract BigInteger? Short(BigInteger value);
        [DisplayName("uShort")] public abstract BigInteger? UShort(BigInteger value);
        [DisplayName("char")] public abstract BigInteger? Char(BigInteger value);
        [DisplayName("big")] public abstract BigInteger? Big(BigInteger value, bool check);
        [DisplayName("nullableUInt")] public abstract BigInteger? NullableUInt(BigInteger? value);
        [DisplayName("nullableULong")] public abstract BigInteger? NullableULong(BigInteger? value);
        [DisplayName("enumByte")] public abstract BigInteger? EnumByte(BigInteger value, bool check);
        [DisplayName("enumUShort")] public abstract BigInteger? EnumUShort(BigInteger value, bool check);
        [DisplayName("enumUInt")] public abstract BigInteger? EnumUInt(BigInteger value, bool check);
        [DisplayName("enumULong")] public abstract BigInteger? EnumULong(BigInteger value, bool check);
        [DisplayName("enumULongPositive")] public abstract bool? EnumULongPositive(BigInteger value);
        [DisplayName("enumSigned")] public abstract BigInteger? EnumSigned(BigInteger value);
        [DisplayName("nullableEnumByte")] public abstract BigInteger? NullableEnumByte(BigInteger? value);
        [DisplayName("once")] public abstract BigInteger? Once(BigInteger value);
    }
}
