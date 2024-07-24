using System.Numerics;

namespace Neo.SmartContract.Framework.TestContracts;

public class Contract_Nullable : SmartContract
{

    public static bool BigIntegerNullableEqual()
    {
        BigInteger a = 1;
        BigInteger? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool BigIntegerNullableNotEqual()
    {
        BigInteger a = 1;
        BigInteger? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool BigIntegerNullableEqualNull()
    {
        BigInteger? a = null;
        return a == null && !a.HasValue;
    }

    public static bool H160NullableNotEqual()
    {
        UInt160 a = UInt160.Zero;
        UInt160? b = "NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq";

        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool H160NullableEqualNull()
    {
        UInt160? a = null;
        return a == null;
    }


    public static bool H256NullableNotEqual()
    {
        UInt256 a = UInt256.Zero;
        UInt256? b = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool H256NullableEqual()
    {
        UInt256 a = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
        UInt256? b = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool ByteNullableEqual()
    {
        byte a = 1;
        byte? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool ByteNullableNotEqual()
    {
        byte a = 1;
        byte? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool ByteNullableEqualNull()
    {
        byte? a = null;
        return a == null && !a.HasValue;
    }

    public static bool SByteNullableEqual()
    {
        sbyte a = 1;
        sbyte? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool SByteNullableNotEqual()
    {
        sbyte a = 1;
        sbyte? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool SByteNullableEqualNull()
    {
        sbyte? a = null;
        return a == null && !a.HasValue;
    }

    public static bool ShortNullableEqual()
    {
        short a = 1;
        short? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool ShortNullableNotEqual()
    {
        short a = 1;
        short? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool ShortNullableEqualNull()
    {
        short? a = null;
        return a == null && !a.HasValue;
    }

    public static bool UShortNullableEqual()
    {
        ushort a = 1;
        ushort? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool UShortNullableNotEqual()
    {
        ushort a = 1;
        ushort? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool UShortNullableEqualNull()
    {
        ushort? a = null;
        return a == null && !a.HasValue;
    }

    public static bool IntNullableEqual()
    {
        int a = 1;
        int? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool IntNullableNotEqual()
    {
        int a = 1;
        int? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool IntNullableEqualNull()
    {
        int? a = null;
        return a == null && !a.HasValue;
    }

    public static bool UIntNullableEqual()
    {
        uint a = 1;
        uint? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool UIntNullableNotEqual()
    {
        uint a = 1;
        uint? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool UIntNullableEqualNull()
    {
        uint? a = null;
        return a == null && !a.HasValue;
    }

    public static bool LongNullableEqual()
    {
        long a = 1;
        long? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool LongNullableNotEqual()
    {
        long a = 1;
        long? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool LongNullableEqualNull()
    {
        long? a = null;
        return a == null && !a.HasValue;
    }

    public static bool ULongNullableEqual()
    {
        ulong a = 1;
        ulong? b = 1;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool ULongNullableNotEqual()
    {
        ulong a = 1;
        ulong? b = 2;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool ULongNullableEqualNull()
    {
        ulong? a = null;
        return a == null && !a.HasValue;
    }

    public static bool BoolNullableEqual()
    {
        bool a = true;
        bool? b = true;
        return a == b && a.Equals(b) && b.Equals(a) && b == a;
    }

    public static bool BoolNullableNotEqual()
    {
        bool a = true;
        bool? b = false;
        return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    }

    public static bool BoolNullableEqualNull()
    {
        bool? a = null;
        return a == null && !a.HasValue;
    }

    public static void GetNullableValue()
    {
        // byte, sbyte, short, ushort, int, uint, long, ulong, bool, char
        byte? a = 1;
        sbyte? b = 1;
        short? c = 1;
        ushort? d = 1;
        int? e = 1;
        uint? f = 1;
        long? g = 1;
        ulong? h = 1;
        bool? i = true;
        char? x = 'a';
        BigInteger? y = 1;
        byte j = a.Value;
        sbyte k = b.Value;
        short l = c.Value;
        ushort m = d.Value;
        int n = e.Value;
        uint o = f.Value;
        long p = g.Value;
        ulong q = h.Value;
        bool r = i.Value;
        char z = x.Value;
        BigInteger s = y.Value;
    }

    // struct Struct
    // {
    //     public int a;
    //
    //     public override bool Equals(object obj)
    //     {
    //         return obj is Struct s && s.a == a;
    //     }
    //
    //     public override int GetHashCode()
    //     {
    //         return a;
    //     }
    //
    //     public static bool operator ==(Struct left, Struct right)
    //     {
    //         return left.Equals(right);
    //     }
    //
    //     public static bool operator !=(Struct left, Struct right)
    //     {
    //         return !left.Equals(right);
    //     }
    // }
    //
    // public static bool StructNullableEqual()
    // {
    //
    //     Struct a = new();
    //     Struct? b = new();
    //     return a == b && a.Equals(b) && b.Equals(a) && b == a;
    // }
    //
    // public static bool StructNullableNotEqual()
    // {
    //     Struct a = new();
    //     Struct? b = new();
    //     return a != b && !a.Equals(b) && !b.Equals(a) && b != a;
    // }

    // public static bool StructNullableEqualNull()
    // {
    //     Struct? a = null;
    //     return a == null && !a.HasValue;
    // }
}
