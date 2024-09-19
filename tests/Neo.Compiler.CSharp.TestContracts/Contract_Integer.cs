using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Integer : SmartContract.Framework.SmartContract
{
    public static (byte quotient, byte remainder) DivRemByte(byte left, byte right)
    {
        return byte.DivRem(left, right);
    }

    public static (short quotient, short remainder) DivRemShort(short left, short right)
    {
        return short.DivRem(left, right);
    }

    public static (int quotient, int remainder) DivRemInt(int left, int right)
    {
        return int.DivRem(left, right);
    }

    public static (long quotient, long remainder) DivRemLong(long left, long right)
    {
        return long.DivRem(left, right);
    }

    public static (sbyte quotient, sbyte remainder) DivRemSbyte(sbyte left, sbyte right)
    {
        return sbyte.DivRem(left, right);
    }

    public static (ushort quotient, ushort remainder) DivRemUshort(ushort left, ushort right)
    {
        return ushort.DivRem(left, right);
    }

    public static (uint quotient, uint remainder) DivRemUint(uint left, uint right)
    {
        return uint.DivRem(left, right);
    }

    public static (ulong quotient, ulong remainder) DivRemUlong(ulong left, ulong right)
    {
        return ulong.DivRem(left, right);
    }

    public static byte ClampByte(byte value, byte min, byte max)
    {
        return byte.Clamp(value, min, max);
    }

    public static sbyte ClampSByte(sbyte value, sbyte min, sbyte max)
    {
        return sbyte.Clamp(value, min, max);
    }

    public static short ClampShort(short value, short min, short max)
    {
        return short.Clamp(value, min, max);
    }

    public static ushort ClampUShort(ushort value, ushort min, ushort max)
    {
        return ushort.Clamp(value, min, max);
    }

    public static int ClampInt(int value, int min, int max)
    {
        return int.Clamp(value, min, max);
    }

    public static uint ClampUInt(uint value, uint min, uint max)
    {
        return uint.Clamp(value, min, max);
    }

    public static long ClampLong(long value, long min, long max)
    {
        return long.Clamp(value, min, max);
    }

    public static ulong ClampULong(ulong value, ulong min, ulong max)
    {
        return ulong.Clamp(value, min, max);
    }

    public static BigInteger ClampBigInteger(BigInteger value, BigInteger min, BigInteger max)
    {
        return BigInteger.Clamp(value, min, max);
    }

    public static int CopySignInt(int value, int sign)
    {
        return int.CopySign(value, sign);
    }

    public static sbyte CopySignSbyte(sbyte value, sbyte sign)
    {
        return sbyte.CopySign(value, sign);
    }

    public static short CopySignShort(short value, short sign)
    {
        return short.CopySign(value, sign);
    }

    public static long CopySignLong(long value, long sign)
    {
        return long.CopySign(value, sign);
    }

    public static int CreateCheckedInt(int value)
    {
        return int.CreateChecked(value);
    }

    public static byte CreateCheckedByte(byte value)
    {
        return byte.CreateChecked(value);
    }

    public static long CreateCheckedLong(long value)
    {
        return long.CreateChecked(value);
    }

    public static ulong CreateCheckedUlong(ulong value)
    {
        return ulong.CreateChecked(value);
    }

    public static ushort CreateCheckedChar(char value)
    {
        return ushort.CreateChecked(value);
    }

    public static int CreateCheckedShort(short value)
    {
        return short.CreateChecked(value);
    }

    public static sbyte CreateCheckedSbyte(sbyte value)
    {
        return sbyte.CreateChecked(value);
    }

    public static int CreateSaturatingInt(int value)
    {
        return int.CreateSaturating(value);
    }

    public static byte CreateSaturatingByte(byte value)
    {
        return byte.CreateSaturating(value);
    }

    public static long CreateSaturatingLong(long value)
    {
        return long.CreateSaturating(value);
    }

    public static ulong CreateSaturatingUlong(ulong value)
    {
        return ulong.CreateSaturating(value);
    }

    public static ushort CreateSaturatingChar(char value)
    {
        return ushort.CreateSaturating(value);
    }

    public static sbyte CreateSaturatingSbyte(sbyte value)
    {
        return sbyte.CreateSaturating(value);
    }

    public static bool IsEvenIntegerInt(int value)
    {
        return int.IsEvenInteger(value);
    }

    public static bool IsEventUInt(uint value)
    {
        return uint.IsEvenInteger(value);
    }

    public static bool IsEvenLong(long value)
    {
        return long.IsEvenInteger(value);
    }

    public static bool IsEvenUlong(ulong value)
    {
        return ulong.IsEvenInteger(value);
    }

    public static bool IsEvenShort(short value)
    {
        return short.IsEvenInteger(value);
    }

    public static bool IsEvenUshort(ushort value)
    {
        return ushort.IsEvenInteger(value);
    }

    public static bool IsEvenByte(byte value)
    {
        return byte.IsEvenInteger(value);
    }

    public static bool IsEvenSbyte(sbyte value)
    {
        return sbyte.IsEvenInteger(value);
    }

    public static bool IsOddIntegerInt(int value)
    {
        return int.IsOddInteger(value);
    }

    public static bool IsOddUInt(uint value)
    {
        return uint.IsOddInteger(value);
    }

    public static bool IsOddLong(long value)
    {
        return long.IsOddInteger(value);
    }

    public static bool IsOddUlong(ulong value)
    {
        return ulong.IsOddInteger(value);
    }

    public static bool IsOddShort(short value)
    {
        return short.IsOddInteger(value);
    }

    public static bool IsOddUshort(ushort value)
    {
        return ushort.IsOddInteger(value);
    }

    public static bool IsOddByte(byte value)
    {
        return byte.IsOddInteger(value);
    }

    public static bool IsOddSbyte(sbyte value)
    {
        return sbyte.IsOddInteger(value);
    }

    public static bool IsNegativeInt(int value)
    {
        return int.IsNegative(value);
    }

    public static bool IsNegativeLong(long value)
    {
        return long.IsNegative(value);
    }

    public static bool IsNegativeShort(short value)
    {
        return short.IsNegative(value);
    }

    public static bool IsNegativeSbyte(sbyte value)
    {
        return sbyte.IsNegative(value);
    }

    public static bool IsPositiveInt(int value)
    {
        return int.IsPositive(value);
    }

    public static bool IsPositiveLong(long value)
    {
        return long.IsPositive(value);
    }

    public static bool IsPositiveShort(short value)
    {
        return short.IsPositive(value);
    }

    public static bool IsPositiveSbyte(sbyte value)
    {
        return sbyte.IsPositive(value);
    }

    public static bool IsPow2Int(int value)
    {
        return int.IsPow2(value);
    }

    public static bool IsPow2UInt(uint value)
    {
        return uint.IsPow2(value);
    }

    public static bool IsPow2Long(long value)
    {
        return long.IsPow2(value);
    }

    public static bool IsPow2Ulong(ulong value)
    {
        return ulong.IsPow2(value);
    }

    public static bool IsPow2Short(short value)
    {
        return short.IsPow2(value);
    }

    public static bool IsPow2Ushort(ushort value)
    {
        return ushort.IsPow2(value);
    }

    public static bool IsPow2Byte(byte value)
    {
        return byte.IsPow2(value);
    }

    public static bool IsPow2Sbyte(sbyte value)
    {
        return sbyte.IsPow2(value);
    }

    public static int LeadingZeroCountInt(int value)
    {
        return int.LeadingZeroCount(value);
    }

    public static uint LeadingZeroCountUInt(uint value)
    {
        return uint.LeadingZeroCount(value);
    }

    public static long LeadingZeroCountLong(long value)
    {
        return long.LeadingZeroCount(value);
    }

    public static int LeadingZeroCountShort(short value)
    {
        return short.LeadingZeroCount(value);
    }

    public static int LeadingZeroCountUshort(ushort value)
    {
        return ushort.LeadingZeroCount(value);
    }

    public static int LeadingZeroCountByte(byte value)
    {
        return byte.LeadingZeroCount(value);
    }

    public static int LeadingZeroCountSbyte(sbyte value)
    {
        return sbyte.LeadingZeroCount(value);
    }

    public static int Log2Int(int value)
    {
        return int.Log2(value);
    }

    public static uint Log2UInt(uint value)
    {
        return uint.Log2(value);
    }

    public static long Log2Long(long value)
    {
        return long.Log2(value);
    }

    public static int Log2Short(short value)
    {
        return short.Log2(value);
    }

    public static int Log2Ushort(ushort value)
    {
        return ushort.Log2(value);
    }

    public static int Log2Byte(byte value)
    {
        return byte.Log2(value);
    }

    public static int Log2Sbyte(sbyte value)
    {
        return sbyte.Log2(value);
    }

    public static int RotateLeftInt(int value, int offset)
    {
        return int.RotateLeft(value, offset);
    }

    public static uint RotateLeftUInt(uint value, int offset)
    {
        return uint.RotateLeft(value, offset);
    }

    public static long RotateLeftLong(long value, int offset)
    {
        return long.RotateLeft(value, offset);
    }

    public static ulong RotateLeftULong(ulong value, int offset)
    {
        return ulong.RotateLeft(value, offset);
    }

    public static short RotateLeftShort(short value, int offset)
    {
        return short.RotateLeft(value, offset);
    }

    public static ushort RotateLeftUShort(ushort value, int offset)
    {
        return ushort.RotateLeft(value, offset);
    }

    public static byte RotateLeftByte(byte value, int offset)
    {
        return byte.RotateLeft(value, offset);
    }

    public static sbyte RotateLeftSByte(sbyte value, int offset)
    {
        return sbyte.RotateLeft(value, offset);
    }

    public static int RotateRightInt(int value, int offset)
    {
        return int.RotateRight(value, offset);
    }

    public static uint RotateRightUInt(uint value, int offset)
    {
        return uint.RotateRight(value, offset);
    }

    public static long RotateRightLong(long value, int offset)
    {
        return long.RotateRight(value, offset);
    }

    public static ulong RotateRightULong(ulong value, int offset)
    {
        return ulong.RotateRight(value, offset);
    }

    public static short RotateRightShort(short value, int offset)
    {
        return short.RotateRight(value, offset);
    }

    public static ushort RotateRightUShort(ushort value, int offset)
    {
        return ushort.RotateRight(value, offset);
    }

    public static byte RotateRightByte(byte value, int offset)
    {
        return byte.RotateRight(value, offset);
    }

    public static sbyte RotateRightSByte(sbyte value, int offset)
    {
        return sbyte.RotateRight(value, offset);
    }

    public static byte PopCountByte(byte value)
    {
        return byte.PopCount(value);
    }

    public static sbyte PopCountSByte(sbyte value)
    {
        return sbyte.PopCount(value);
    }

    public static short PopCountShort(short value)
    {
        return short.PopCount(value);
    }

    public static ushort PopCountUShort(ushort value)
    {
        return ushort.PopCount(value);
    }

    public static int PopCountInt(int value)
    {
        return int.PopCount(value);
    }

    public static uint PopCountUInt(uint value)
    {
        return uint.PopCount(value);
    }

    public static long PopCountLong(long value)
    {
        return long.PopCount(value);
    }

    public static ulong PopCountULong(ulong value)
    {
        return ulong.PopCount(value);
    }

    public static BigInteger PopCountBigInteger(BigInteger value)
    {
        return BigInteger.PopCount(value);
    }

    public static bool IsPow2BigInteger(BigInteger value)
    {
        return value.IsPowerOfTwo;
    }
}
