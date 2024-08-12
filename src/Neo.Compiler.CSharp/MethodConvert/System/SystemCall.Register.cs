using System;
using System.Numerics;

namespace Neo.Compiler;

partial class MethodConvert
{
    private static void RegisterSystemCallHandlers()
    {
        // Register BigInteger property handlers
        RegisterHandler(() => BigInteger.One, HandleBigIntegerOne);
        RegisterHandler(() => BigInteger.MinusOne, HandleBigIntegerMinusOne);
        RegisterHandler(() => BigInteger.Zero, HandleBigIntegerZero);
        RegisterHandler((BigInteger b) => b.IsZero, HandleBigIntegerIsZero);
        RegisterHandler((BigInteger b) => b.IsOne, HandleBigIntegerIsOne);
        RegisterHandler((BigInteger b) => b.IsEven, HandleBigIntegerIsEven);
        RegisterHandler((BigInteger b) => b.Sign, HandleBigIntegerSign);

        // Register BigInteger method handlers
        RegisterHandler((BigInteger x, int y) => BigInteger.Pow(x, y), HandleBigIntegerPow);
        RegisterHandler((BigInteger x, BigInteger y, BigInteger z) => BigInteger.ModPow(x, y, z), HandleBigIntegerModPow);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Add(x, y), HandleBigIntegerAdd);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Subtract(x, y), HandleBigIntegerSubtract);
        RegisterHandler((BigInteger x) => BigInteger.Negate(x), HandleBigIntegerNegate);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Multiply(x, y), HandleBigIntegerMultiply);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Divide(x, y), HandleBigIntegerDivide);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Remainder(x, y), HandleBigIntegerRemainder);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Compare(x, y), HandleBigIntegerCompare);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.GreatestCommonDivisor(x, y), HandleBigIntegerGreatestCommonDivisor);
        RegisterHandler((BigInteger x) => x.ToByteArray(), HandleBigIntegerToByteArray);
        // RegisterHandler((BigInteger x) => x.ToString("X"), HandleBigIntegerToHexString);
        // RegisterHandler((BigInteger x) => x.IsPowerOfTwo, HandleBigIntegerIsPowerOfTwo);
        // RegisterHandler((BigInteger x) => x.GetBitLength(), HandleBigIntegerGetBitLength);
        // RegisterHandler((BigInteger x) => x.GetByteCount(), HandleBigIntegerGetByteCount);


        // Register explicit conversion handlers
        RegisterHandler((BigInteger b) => (sbyte)b, HandleBigIntegerToSByte);
        RegisterHandler((BigInteger b) => (byte)b, HandleBigIntegerToByte);
        RegisterHandler((BigInteger b) => (short)b, HandleBigIntegerToShort);
        RegisterHandler((BigInteger b) => (ushort)b, HandleBigIntegerToUShort);
        RegisterHandler((BigInteger b) => (char)b, HandleBigIntegerToUShort);
        RegisterHandler((BigInteger b) => (int)b, HandleBigIntegerToInt);
        RegisterHandler((BigInteger b) => (uint)b, HandleBigIntegerToUInt);
        RegisterHandler((BigInteger b) => (long)b, HandleBigIntegerToLong);
        RegisterHandler((BigInteger b) => (ulong)b, HandleBigIntegerToULong);

        // Register implicit conversion handlers
        RegisterHandler((char c) => (BigInteger)c, HandleToBigInteger);
        RegisterHandler((sbyte b) => (BigInteger)b, HandleToBigInteger);
        RegisterHandler((byte b) => (BigInteger)b, HandleToBigInteger);
        RegisterHandler((short s) => (BigInteger)s, HandleToBigInteger);
        RegisterHandler((ushort s) => (BigInteger)s, HandleToBigInteger);
        RegisterHandler((int i) => (BigInteger)i, HandleToBigInteger);
        RegisterHandler((uint i) => (BigInteger)i, HandleToBigInteger);
        RegisterHandler((long l) => (BigInteger)l, HandleToBigInteger);
        RegisterHandler((ulong l) => (BigInteger)l, HandleToBigInteger);

        // Register Math method handlers
        RegisterHandler((sbyte x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((short x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((int x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((long x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((BigInteger x) => BigInteger.Abs(x), HandleMathAbs);

        RegisterHandler((sbyte x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((short x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((int x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((long x) => Math.Sign(x), HandleMathSign);

        RegisterHandler((byte x, byte y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((sbyte x, sbyte y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((short x, short y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((ushort x, ushort y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((int x, int y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((uint x, uint y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((long x, long y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((ulong x, ulong y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Max(x, y), HandleBigIntegerMax);

        RegisterHandler((byte x, byte y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((sbyte x, sbyte y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((short x, short y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((ushort x, ushort y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((int x, int y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((uint x, uint y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((long x, long y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((ulong x, ulong y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Min(x, y), HandleBigIntegerMin);

        RegisterHandler((byte x, byte y) => Math.DivRem(x, y), HandleMathByteDivRem);
        RegisterHandler((sbyte x, sbyte y) => Math.DivRem(x, y), HandleMathSByteDivRem);
        RegisterHandler((short x, short y) => Math.DivRem(x, y), HandleMathShortDivRem);
        RegisterHandler((ushort x, ushort y) => Math.DivRem(x, y), HandleMathUShortDivRem);
        RegisterHandler((int x, int y) => Math.DivRem(x, y), HandleMathIntDivRem);
        RegisterHandler((uint x, uint y) => Math.DivRem(x, y), HandleMathUIntDivRem);
        RegisterHandler((long x, long y) => Math.DivRem(x, y), HandleMathLongDivRem);
        RegisterHandler((ulong x, ulong y) => Math.DivRem(x, y), HandleMathULongDivRem);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.DivRem(x, y), HandleBigIntegerDivRem);

        RegisterHandler((byte value, byte min, byte max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((sbyte value, sbyte min, sbyte max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((short value, short min, short max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ushort value, ushort min, ushort max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((int value, int min, int max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((uint value, uint min, uint max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((long value, long min, long max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ulong value, ulong min, ulong max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((BigInteger value, BigInteger min, BigInteger max) => BigInteger.Clamp(value, min, max), HandleMathClamp);

        // Register ToString handlers
        RegisterHandler((sbyte x) => x.ToString(), HandleToString);
        RegisterHandler((byte x) => x.ToString(), HandleToString);
        RegisterHandler((short x) => x.ToString(), HandleToString);
        RegisterHandler((ushort x) => x.ToString(), HandleToString);
        RegisterHandler((int x) => x.ToString(), HandleToString);
        RegisterHandler((uint x) => x.ToString(), HandleToString);
        RegisterHandler((long x) => x.ToString(), HandleToString);
        RegisterHandler((ulong x) => x.ToString(), HandleToString);
        RegisterHandler((BigInteger x) => x.ToString(), HandleToString);
        RegisterHandler((bool x) => x.ToString(), HandleBoolToString);
        RegisterHandler((char x) => x.ToString(), HandleCharToString);
        RegisterHandler((object x) => x.ToString(), HandleObjectToString);
        RegisterHandler((string s) => s.ToString(), HandleStringToString);

        // Register equality method handlers
        RegisterHandler((byte x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((sbyte x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((short x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((ushort x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((int x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((uint x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((long x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((ulong x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((bool x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((char x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((BigInteger x, object? y) => x.Equals(y), HandleEquals);
        RegisterHandler((BigInteger x, long y) => x.Equals(y), HandleEquals);
        RegisterHandler((BigInteger x, ulong y) => x.Equals(y), HandleEquals);
        RegisterHandler((BigInteger x, BigInteger y) => x.Equals(y), HandleEquals);
        RegisterHandler((string x, string? y) => x.Equals(y), HandleObjectEquals);
        RegisterHandler((object? x, object? y) => x.Equals(y), HandleObjectEquals);

        // Register array and string method handlers
        RegisterHandler((Array a) => a.Length, HandleLength);
        RegisterHandler((string s) => s.Length, HandleLength);


        // RegisterHandler((Array a, int index, object? value) => a.SetValue(value, index), HandleArraySetValue);
        // RegisterHandler((Array a, int index) => a.GetValue(index), HandleArrayGetValue);
        // RegisterHandler((Array a) => a.Clone(), HandleArrayClone);

        RegisterHandler((string? s) => string.IsNullOrEmpty(s), HandleStringIsNullOrEmpty);
        RegisterHandler((string s, int startIndex, int length) => s.Substring(startIndex, length), HandleStringSubstring);
        RegisterHandler((string s, string value) => s.Contains(value), HandleStringContains);
        RegisterHandler((string s, string value) => s.EndsWith(value), HandleStringEndsWith);
        RegisterHandler((string s, string value) => s.IndexOf(value), HandleStringIndexOf);
        // RegisterHandler((string s, string value) => s.StartsWith(value), HandleStringStartsWith);
        RegisterHandler((string s, int index) => s[index], HandleStringPickItem);
        RegisterHandler((string s, int startIndex, int length) => s.Substring(startIndex, length), HandleStringSubstring);
        RegisterHandler((string s, int startIndex) => s.Substring(startIndex), HandleStringSubStringToEnd);
        // RegisterHandler((string s, int startIndex, int length) => s.Remove(startIndex, length), HandleStringRemove);
        // RegisterHandler((string s, int startIndex) => s.Remove(startIndex), HandleStringRemoveToEnd);
        // RegisterHandler((string s) => s.Trim(), HandleStringTrim);
        // RegisterHandler((string s) => s.TrimStart(), HandleStringTrimStart);
        // RegisterHandler((string s) => s.TrimEnd(), HandleStringTrimEnd);
        RegisterHandler((string? s1, string? s2) => string.Concat(s1, s2), HandleStringConcat);
        // RegisterHandler((string s, string oldValue, string newValue) => s.Replace(oldValue, newValue), HandleStringReplace);

        // Register additional Math method handlers
        RegisterHandler((int x, int min, int max) => Math.Clamp(x, min, max), HandleMathClamp);
        RegisterHandler((long x, long min, long max) => Math.Clamp(x, min, max), HandleMathClamp);
        RegisterHandler((int x, int y) => Math.BigMul(x, y), HandleMathBigMul);

        // Register additional char method handlers
        RegisterHandler((char c) => char.IsDigit(c), HandleCharIsDigit);
        RegisterHandler((char c) => char.IsLetter(c), HandleCharIsLetter);
        RegisterHandler((char c) => char.IsWhiteSpace(c), HandleCharIsWhiteSpace);
        RegisterHandler((char c) => char.IsLower(c), HandleCharIsLower);
        RegisterHandler((char c) => char.IsUpper(c), HandleCharIsUpper);
        // RegisterHandler((char c) => char.GetNumericValue(c), HandleCharGetNumericValue);
        // RegisterHandler((char c) => char.IsPunctuation(c), HandleCharIsPunctuation);
        // RegisterHandler((char c) => char.IsSymbol(c), HandleCharIsSymbol);
        // RegisterHandler((char c) => char.IsControl(c), HandleCharIsControl);
        // RegisterHandler((char c) => char.IsSurrogate(c), HandleCharIsSurrogate);
        // RegisterHandler((char c) => char.IsHighSurrogate(c), HandleCharIsHighSurrogate);
        // RegisterHandler((char c) => char.IsLowSurrogate(c), HandleCharIsLowSurrogate);

        // Register additional methods for basic types
        RegisterHandler((byte x, byte y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((sbyte x, sbyte y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((short x, short y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((ushort x, ushort y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((int x, int y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((uint x, uint y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((long x, long y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((ulong x, ulong y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((char x, char y) => x.CompareTo(y), HandleBigIntegerCompare);

        RegisterHandler((string s) => byte.Parse(s), HandleByteParse);
        RegisterHandler((string s) => sbyte.Parse(s), HandleSByteParse);
        RegisterHandler((string s) => short.Parse(s), HandleShortParse);
        RegisterHandler((string s) => ushort.Parse(s), HandleUShortParse);
        RegisterHandler((string s) => int.Parse(s), HandleIntParse);
        RegisterHandler((string s) => uint.Parse(s), HandleUIntParse);
        RegisterHandler((string s) => long.Parse(s), HandleLongParse);
        RegisterHandler((string s) => ulong.Parse(s), HandleULongParse);
        // RegisterHandler((string s) => bool.Parse(s), HandleBoolParse);
        RegisterHandler((string s) => BigInteger.Parse(s), HandleBigIntegerParse);
    }
}
