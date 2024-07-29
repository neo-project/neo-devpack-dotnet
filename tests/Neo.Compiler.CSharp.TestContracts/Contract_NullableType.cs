using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_NullableType : SmartContract.Framework.SmartContract
{
    public static void TestBigInteger(BigInteger? a) { }
    public static void TestInt(int? a) { }
    public static void TestUInt(uint? a) { }
    public static void TestLong(long? a) { }
    public static void TestULong(ulong? a) { }
    public static void TestShort(short? a) { }
    public static void TestUShort(ushort? a) { }
    public static void TestSByte(sbyte? a) { }
    public static void TestByte(byte? a) { }
    public static void TestBool(bool? a) { }
    public static void TestUInt160(UInt160? a) { }
    public static void TestUInt256(UInt256? a) { }
    public static void TestUInt160Array(UInt160[] a) { }
    public static void TestUInt256Array(UInt256[] a) { }
    public static void TestByteArray(byte[] a) { }
    public static void TestString(string a) { }
    public static void TestNullableString(string? a) { }
}
