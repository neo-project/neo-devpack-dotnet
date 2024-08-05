using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_NullableType : SmartContract.Framework.SmartContract
{
    // BigInteger tests
    public static BigInteger TestBigIntegerAdd(BigInteger? a, BigInteger? b) => a.HasValue && b.HasValue ? a.Value + b.Value : 0;
    public static BigInteger TestBigIntegerAddNonNullable(BigInteger a, BigInteger b) => a + b;
    public static bool TestBigIntegerCompare(BigInteger? a, BigInteger? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestBigIntegerCompareNonNullable(BigInteger a, BigInteger b) => a > b;
    public static BigInteger TestBigIntegerDefault(BigInteger? a) => a ?? BigInteger.Zero;
    public static BigInteger TestBigIntegerDefaultNonNullable(BigInteger a) => a;

    // Int tests
    public static int TestIntAdd(int? a, int? b) => a.HasValue && b.HasValue ? a.Value + b.Value : 0;
    public static int TestIntAddNonNullable(int a, int b) => a + b;
    public static bool TestIntCompare(int? a, int? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestIntCompareNonNullable(int a, int b) => a > b;
    public static int TestIntDefault(int? a) => a ?? 0;
    public static int TestIntDefaultNonNullable(int a) => a;

    // UInt tests
    public static uint TestUIntAdd(uint? a, uint? b) => a.HasValue && b.HasValue ? a.Value + b.Value : 0;
    public static uint TestUIntAddNonNullable(uint a, uint b) => a + b;
    public static bool TestUIntCompare(uint? a, uint? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestUIntCompareNonNullable(uint a, uint b) => a > b;
    public static uint TestUIntDefault(uint? a) => a ?? 0;
    public static uint TestUIntDefaultNonNullable(uint a) => a;

    // Long tests
    public static long TestLongAdd(long? a, long? b) => a.HasValue && b.HasValue ? a.Value + b.Value : 0;
    public static long TestLongAddNonNullable(long a, long b) => a + b;
    public static bool TestLongCompare(long? a, long? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestLongCompareNonNullable(long a, long b) => a > b;
    public static long TestLongDefault(long? a) => a ?? 0;
    public static long TestLongDefaultNonNullable(long a) => a;

    // ULong tests
    public static ulong TestULongAdd(ulong? a, ulong? b) => a.HasValue && b.HasValue ? a.Value + b.Value : 0;
    public static ulong TestULongAddNonNullable(ulong a, ulong b) => a + b;
    public static bool TestULongCompare(ulong? a, ulong? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestULongCompareNonNullable(ulong a, ulong b) => a > b;
    public static ulong TestULongDefault(ulong? a) => a ?? 0;
    public static ulong TestULongDefaultNonNullable(ulong a) => a;

    // Short tests
    public static short TestShortAdd(short? a, short? b) => a.HasValue && b.HasValue ? (short)(a.Value + b.Value) : (short)0;
    public static short TestShortAddNonNullable(short a, short b) => (short)(a + b);
    public static bool TestShortCompare(short? a, short? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestShortCompareNonNullable(short a, short b) => a > b;
    public static short TestShortDefault(short? a) => a ?? 0;
    public static short TestShortDefaultNonNullable(short a) => a;

    // UShort tests
    public static ushort TestUShortAdd(ushort? a, ushort? b) => a.HasValue && b.HasValue ? (ushort)(a.Value + b.Value) : (ushort)0;
    public static ushort TestUShortAddNonNullable(ushort a, ushort b) => (ushort)(a + b);
    public static bool TestUShortCompare(ushort? a, ushort? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestUShortCompareNonNullable(ushort a, ushort b) => a > b;
    public static ushort TestUShortDefault(ushort? a) => a ?? 0;
    public static ushort TestUShortDefaultNonNullable(ushort a) => a;

    // SByte tests
    public static sbyte TestSByteAdd(sbyte? a, sbyte? b) => a.HasValue && b.HasValue ? (sbyte)(a.Value + b.Value) : (sbyte)0;
    public static sbyte TestSByteAddNonNullable(sbyte a, sbyte b) => (sbyte)(a + b);
    public static bool TestSByteCompare(sbyte? a, sbyte? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestSByteCompareNonNullable(sbyte a, sbyte b) => a > b;
    public static sbyte TestSByteDefault(sbyte? a) => a ?? 0;
    public static sbyte TestSByteDefaultNonNullable(sbyte a) => a;

    // Byte tests
    public static byte TestByteAdd(byte? a, byte? b) => a.HasValue && b.HasValue ? (byte)(a.Value + b.Value) : (byte)0;
    public static byte TestByteAddNonNullable(byte a, byte b) => (byte)(a + b);
    public static bool TestByteCompare(byte? a, byte? b) => a.HasValue && b.HasValue && a.Value > b.Value;
    public static bool TestByteCompareNonNullable(byte a, byte b) => a > b;
    public static byte TestByteDefault(byte? a) => a ?? 0;
    public static byte TestByteDefaultNonNullable(byte a) => a;

    // Bool tests
    public static bool TestBoolAnd(bool? a, bool? b) => a.HasValue && b.HasValue && (a.Value && b.Value);
    public static bool TestBoolAndNonNullable(bool a, bool b) => a && b;
    public static bool TestBoolOr(bool? a, bool? b) => a.HasValue && b.HasValue && (a.Value || b.Value);
    public static bool TestBoolOrNonNullable(bool a, bool b) => a || b;
    public static bool TestBoolDefault(bool? a) => a ?? false;
    public static bool TestBoolDefaultNonNullable(bool a) => a;

    // UInt160 tests
    public static UInt160 TestUInt160Default(UInt160? a) => a ?? UInt160.Zero;
    public static UInt160 TestUInt160DefaultNonNullable(UInt160 a) => a;

    // UInt256 tests
    public static UInt256 TestUInt256Default(UInt256? a) => a ?? UInt256.Zero;
    public static UInt256 TestUInt256DefaultNonNullable(UInt256 a) => a;

    // Array tests
    public static int TestUInt160ArrayLength(UInt160[]? a) => a?.Length ?? 0;
    public static int TestUInt160ArrayLengthNonNullable(UInt160[] a) => a.Length;
    public static int TestUInt256ArrayLength(UInt256[]? a) => a?.Length ?? 0;
    public static int TestUInt256ArrayLengthNonNullable(UInt256[] a) => a.Length;
    public static int TestByteArrayLength(byte[]? a) => a?.Length ?? 0;
    public static int TestByteArrayLengthNonNullable(byte[] a) => a.Length;

    // String tests
    public static int TestStringLength(string? a) => a?.Length ?? 0;
    public static int TestStringLengthNonNullable(string a) => a.Length;
    public static string TestStringDefault(string? a) => a ?? string.Empty;
    public static string TestStringDefaultNonNullable(string a) => a;
    public static string TestStringConcat(string? a, string? b) => (a ?? "") + (b ?? "");
    public static string TestStringConcatNonNullable(string a, string b) => a + b;
}
