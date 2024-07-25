using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    public static bool GetNullableValue()
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

        byte? aa = null;
        sbyte? bb = null;
        short? cc = null;
        ushort? dd = null;
        int? ee = null;
        uint? ff = null;
        long? gg = null;
        ulong? hh = null;
        bool? ii = null;
        char? xx = null;
        BigInteger? yy = null;


        // test HasValue
        return a.HasValue
             && b.HasValue
             && c.HasValue
             && d.HasValue
             && e.HasValue
             && f.HasValue
             && g.HasValue
             && h.HasValue
             && i.HasValue
             && x.HasValue
             && y.HasValue
             && !aa.HasValue
             && !bb.HasValue
             && !cc.HasValue
             && !dd.HasValue
             && !ee.HasValue
             && !ff.HasValue
             && !gg.HasValue
             && !hh.HasValue
             && !ii.HasValue
             && !xx.HasValue
             && !yy.HasValue
             && 1 == a.Value
             && 1 == b.Value
             && 1 == c.Value
             && 1 == d.Value
             && 1 == e.Value
             && 1 == f.Value
             && 1 == g.Value
             && 1 == h.Value
             && true == i.Value
             && 'a' == x.Value
             && 1 == y.Value;
        // Assert.ThrowsException<Exception>()
    }

    public static bool NullableEqual()
    {
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

        byte aa = 1;
        sbyte bb = 1;
        short cc = 1;
        ushort dd = 1;
        int ee = 1;
        uint ff = 1;
        long gg = 1;
        ulong hh = 1;
        bool ii = true;
        char xx = 'a';
        BigInteger yy = 1;

        byte? aaa = 1;
        sbyte? bbb = 1;
        short? ccc = 1;
        ushort? ddd = 1;
        int? eee = 1;
        uint? fff = 1;
        long? ggg = 1;
        ulong? hhh = 1;
        bool? iii = true;
        char? xxx = 'a';
        BigInteger? yyy = 1;


        return a.Equals(aa)
               && b.Equals(bb)
               && c.Equals(cc)
               && d.Equals(dd)
               && e.Equals(ee)
               && f.Equals(ff)
               && g.Equals(gg)
               && h.Equals(hh)
               && i.Equals(ii)
               && x.Equals(xx)
               && aa.Equals(a)
               && bb.Equals(b)
               && cc.Equals(c)
               && dd.Equals(d)
               && ee.Equals(e)
               && ff.Equals(f)
               && gg.Equals(g)
               && hh.Equals(h)
               && ii.Equals(i)
               && xx.Equals(x)
               && a.Equals(aaa)
               && b.Equals(bbb)
               && c.Equals(ccc)
               && d.Equals(ddd)
               && e.Equals(eee)
               && f.Equals(fff)
               && g.Equals(ggg)
               && h.Equals(hhh)
               && i.Equals(iii)
               && x.Equals(xxx);
    }

    public static bool NullableToString()
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
        bool i = true;
        char? x = 'a';
        BigInteger? y = 1;

        byte? aa = null;
        sbyte? bb = null;
        short? cc = null;
        ushort? dd = null;
        int? ee = null;
        uint? ff = null;
        long? gg = null;
        ulong? hh = null;
        bool? ii = null;
        char? xx = null;
        BigInteger? yy = null;

        return
            "1" == a.ToString()
        && "1" == b.ToString()
        && "1" == c.ToString()
        && "1" == d.ToString()
        && "1" == e.ToString()
        && "1" == f.ToString()
        && "1" == g.ToString()
        && "1" == h.ToString()
        && "True" == i.ToString()
        && "a" == x.ToString()
        && "1" == y.ToString()
        && "" == aa.ToString()
        && "" == bb.ToString()
        && "" == cc.ToString()
        && "" == dd.ToString()
        && "" == ee.ToString()
        && "" == ff.ToString()
        && "" == gg.ToString()
        && "" == hh.ToString()
        && "" == ii.ToString()
        && "" == xx.ToString()
        && "" == yy.ToString();
    }
}
