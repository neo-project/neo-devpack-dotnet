using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.SmartContract.Framework.TestContracts
{
    public class Contract_Nullable : SmartContract
    {
        public static bool BigIntegerNullableEqual(BigInteger a, BigInteger? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool BigIntegerNullableNotEqual(BigInteger a, BigInteger? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool BigIntegerNullableEqualNull(BigInteger? a)
        {
            return a == null && !a.HasValue;
        }

#pragma warning disable CS8602
        public static bool H160NullableNotEqual(UInt160 a, UInt160? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool H160NullableEqualNull(UInt160? a)
        {
            return a == null;
        }

        public static bool H256NullableNotEqual(UInt256 a, UInt256? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }
#pragma warning restore CS8602

        public static bool H256NullableEqual(UInt256 a, UInt256? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool ByteNullableEqual(byte a, byte? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool ByteNullableNotEqual(byte a, byte? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool ByteNullableEqualNull(byte? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool SByteNullableEqual(sbyte a, sbyte? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool SByteNullableNotEqual(sbyte a, sbyte? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool SByteNullableEqualNull(sbyte? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool ShortNullableEqual(short a, short? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool ShortNullableNotEqual(short a, short? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool ShortNullableEqualNull(short? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool UShortNullableEqual(ushort a, ushort? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool UShortNullableNotEqual(ushort a, ushort? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool UShortNullableEqualNull(ushort? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool IntNullableEqual(int a, int? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool IntNullableNotEqual(int a, int? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool IntNullableEqualNull(int? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool UIntNullableEqual(uint a, uint? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool UIntNullableNotEqual(uint a, uint? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool UIntNullableEqualNull(uint? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool LongNullableEqual(long a, long? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool LongNullableNotEqual(long a, long? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool LongNullableEqualNull(long? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool ULongNullableEqual(ulong a, ulong? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool ULongNullableNotEqual(ulong a, ulong? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool ULongNullableEqualNull(ulong? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool BoolNullableEqual(bool a, bool? b)
        {
            return a == b && a.Equals(b) && b.Equals(a) && b == a;
        }

        public static bool BoolNullableNotEqual(bool a, bool? b)
        {
            return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
        }

        public static bool BoolNullableEqualNull(bool? a)
        {
            return a == null && !a.HasValue;
        }

        public static bool ByteNullableToString(byte? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool SByteNullableToString(sbyte? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool ShortNullableToString(short? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool UShortNullableToString(ushort? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool IntNullableToString(int? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool UIntNullableToString(uint? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool LongNullableToString(long? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool ULongNullableToString(ulong? a)
        {
            return (a?.ToString() ?? "") == "1";
        }

        public static bool BoolNullableToString(bool? a)
        {
            return (a?.ToString() ?? "") == "True";
        }

        public static bool BigIntegerNullableToString(BigInteger? a)
        {
            return (a?.ToString() ?? "") == "1";
        }
    }
}
