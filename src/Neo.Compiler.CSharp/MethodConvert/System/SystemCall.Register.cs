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

        // Math Clamp
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

        // CompareTo
        RegisterHandler((byte x, byte y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((sbyte x, sbyte y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((short x, short y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((ushort x, ushort y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((int x, int y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((uint x, uint y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((long x, long y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((ulong x, ulong y) => x.CompareTo(y), HandleBigIntegerCompare);
        RegisterHandler((char x, char y) => x.CompareTo(y), HandleBigIntegerCompare);

        // Parse
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

        //IsEvenInteger
        RegisterHandler((byte x) => byte.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((sbyte x) => sbyte.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((short x) => short.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((ushort x) => ushort.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((int x) => int.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((uint x) => uint.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((long x) => long.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((ulong x) => ulong.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((BigInteger x) => BigInteger.IsEvenInteger(x), HandleBigIntegerIsEven);

        //IsOddInteger
        RegisterHandler((byte x) => byte.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((sbyte x) => sbyte.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((short x) => short.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((ushort x) => ushort.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((int x) => int.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((uint x) => uint.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((long x) => long.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((ulong x) => ulong.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((BigInteger x) => BigInteger.IsOddInteger(x), HandleBigIntegerIsOdd);

        //IsNegative
        RegisterHandler((sbyte x) => sbyte.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((short x) => short.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((int x) => int.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((long x) => long.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((BigInteger x) => BigInteger.IsNegative(x), HandleBigIntegerIsNegative);

        //IsPositive
        RegisterHandler((sbyte x) => sbyte.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((short x) => short.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((int x) => int.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((long x) => long.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((BigInteger x) => BigInteger.IsPositive(x), HandleBigIntegerIsPositive);

        // IsPow2
        RegisterHandler((byte x) => byte.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((sbyte x) => sbyte.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((short x) => short.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((ushort x) => ushort.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((int x) => int.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((uint x) => uint.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((long x) => long.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((ulong x) => ulong.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((BigInteger x) => BigInteger.IsPow2(x), HandleBigIntegerIsPow2);

        //LeadingZeroCount
        RegisterHandler((byte x) => byte.LeadingZeroCount(x), HandleByteLeadingZeroCount);
        RegisterHandler((sbyte x) => sbyte.LeadingZeroCount(x), HandleSByteLeadingZeroCount);
        RegisterHandler((short x) => short.LeadingZeroCount(x), HandleShortLeadingZeroCount);
        RegisterHandler((ushort x) => ushort.LeadingZeroCount(x), HandleUShortLeadingZeroCount);
        RegisterHandler((int x) => int.LeadingZeroCount(x), HandleIntLeadingZeroCount);
        RegisterHandler((uint x) => uint.LeadingZeroCount(x), HandleUIntLeadingZeroCount);
        RegisterHandler((long x) => long.LeadingZeroCount(x), HandleLongLeadingZeroCount);
        RegisterHandler((ulong x) => ulong.LeadingZeroCount(x), HandleULongLeadingZeroCount);
        // RegisterHandler((BigInteger x) => BigInteger.LeadingZeroCount(x), HandleBigIntegerLeadingZeroCount);

        // Log2

        RegisterHandler((byte x) => byte.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((sbyte x) => sbyte.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((short x) => short.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((ushort x) => ushort.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((int x) => int.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((uint x) => uint.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((long x) => long.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((ulong x) => ulong.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((BigInteger x) => BigInteger.IsPow2(x), HandleBigIntegerLog2);

        // Sign
        RegisterHandler((byte x) => byte.Sign(x), HandleBigIntegerSign);
        RegisterHandler((sbyte x) => sbyte.Sign(x), HandleBigIntegerSign);
        RegisterHandler((short x) => short.Sign(x), HandleBigIntegerSign);
        RegisterHandler((ushort x) => ushort.Sign(x), HandleBigIntegerSign);
        RegisterHandler((int x) => int.Sign(x), HandleBigIntegerSign);
        RegisterHandler((uint x) => uint.Sign(x), HandleBigIntegerSign);
        RegisterHandler((long x) => long.Sign(x), HandleBigIntegerSign);
        RegisterHandler((ulong x) => ulong.Sign(x), HandleBigIntegerSign);

        // DivRem
        RegisterHandler((byte x, byte y) => byte.DivRem(x, y), HandleMathByteDivRem);
        RegisterHandler((sbyte x, sbyte y) => sbyte.DivRem(x, y), HandleMathSByteDivRem);
        RegisterHandler((short x, short y) => short.DivRem(x, y), HandleMathShortDivRem);
        RegisterHandler((ushort x, ushort y) => ushort.DivRem(x, y), HandleMathUShortDivRem);
        RegisterHandler((int x, int y) => int.DivRem(x, y), HandleMathIntDivRem);
        RegisterHandler((uint x, uint y) => uint.DivRem(x, y), HandleMathUIntDivRem);
        RegisterHandler((long x, long y) => long.DivRem(x, y), HandleMathLongDivRem);
        RegisterHandler((ulong x, ulong y) => ulong.DivRem(x, y), HandleMathULongDivRem);

        // Clamp
        RegisterHandler((byte value, byte min, byte max) => byte.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((sbyte value, sbyte min, sbyte max) => sbyte.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((short value, short min, short max) => short.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ushort value, ushort min, ushort max) => ushort.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((int value, int min, int max) => int.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((uint value, uint min, uint max) => uint.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((long value, long min, long max) => long.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ulong value, ulong min, ulong max) => ulong.Clamp(value, min, max), HandleMathClamp);

        //CopySign
        RegisterHandler((sbyte x, sbyte y) => sbyte.CopySign(x, y), HandleSByteCopySign);
        RegisterHandler((short x, short y) => short.CopySign(x, y), HandleShortCopySign);
        RegisterHandler((int x, int y) => int.CopySign(x, y), HandleIntCopySign);
        RegisterHandler((long x, long y) => long.CopySign(x, y), HandleLongCopySign);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.CopySign(x, y), HandleBigIntegerCopySign);

        //CreateChecked
        RegisterHandler((byte x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((sbyte x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((short x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((ushort x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((int x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((uint x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((long x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((ulong x) => byte.CreateChecked(x), HandleByteCreateChecked);
        RegisterHandler((char x) => byte.CreateChecked(x), HandleByteCreateChecked);

        RegisterHandler((byte x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((sbyte x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((short x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((ushort x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((int x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((uint x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((long x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((ulong x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);
        RegisterHandler((char x) => sbyte.CreateChecked(x), HandleSByteCreateChecked);

        RegisterHandler((byte x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((sbyte x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((short x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((ushort x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((int x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((uint x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((long x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((ulong x) => short.CreateChecked(x), HandleShortCreateChecked);
        RegisterHandler((char x) => short.CreateChecked(x), HandleShortCreateChecked);

        RegisterHandler((byte x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((sbyte x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((short x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((ushort x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((int x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((uint x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((long x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((ulong x) => ushort.CreateChecked(x), HandleUShortCreateChecked);
        RegisterHandler((char x) => ushort.CreateChecked(x), HandleUShortCreateChecked);

        RegisterHandler((byte x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((sbyte x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((short x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((ushort x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((int x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((uint x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((long x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((ulong x) => int.CreateChecked(x), HandleIntCreateChecked);
        RegisterHandler((char x) => int.CreateChecked(x), HandleIntCreateChecked);

        RegisterHandler((byte x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((sbyte x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((short x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((ushort x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((int x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((uint x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((long x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((ulong x) => uint.CreateChecked(x), HandleUIntCreateChecked);
        RegisterHandler((char x) => uint.CreateChecked(x), HandleUIntCreateChecked);

        RegisterHandler((byte x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((sbyte x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((short x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((ushort x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((int x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((uint x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((long x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((ulong x) => long.CreateChecked(x), HandleLongCreateChecked);
        RegisterHandler((char x) => long.CreateChecked(x), HandleLongCreateChecked);

        RegisterHandler((byte x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((sbyte x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((short x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((ushort x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((int x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((uint x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((long x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((ulong x) => ulong.CreateChecked(x), HandleULongCreateChecked);
        RegisterHandler((char x) => ulong.CreateChecked(x), HandleULongCreateChecked);

        // CreateSaturating
        RegisterHandler((byte x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((sbyte x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((short x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((ushort x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((int x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((uint x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((long x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((ulong x) => byte.CreateSaturating(x), HandleByteCreateSaturating);
        RegisterHandler((char x) => byte.CreateSaturating(x), HandleByteCreateSaturating);

        RegisterHandler((byte x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((sbyte x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((short x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((ushort x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((int x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((uint x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((long x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((ulong x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);
        RegisterHandler((char x) => sbyte.CreateSaturating(x), HandleSByteCreateSaturating);

        RegisterHandler((byte x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((sbyte x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((short x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((ushort x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((int x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((uint x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((long x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((ulong x) => short.CreateSaturating(x), HandleShortCreateSaturating);
        RegisterHandler((char x) => short.CreateSaturating(x), HandleShortCreateSaturating);

        RegisterHandler((byte x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((sbyte x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((short x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((ushort x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((int x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((uint x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((long x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((ulong x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);
        RegisterHandler((char x) => ushort.CreateSaturating(x), HandleUShortCreateSaturating);

        RegisterHandler((byte x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((sbyte x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((short x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((ushort x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((int x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((uint x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((long x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((ulong x) => int.CreateSaturating(x), HandleIntCreateSaturating);
        RegisterHandler((char x) => int.CreateSaturating(x), HandleIntCreateSaturating);

        RegisterHandler((byte x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((sbyte x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((short x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((ushort x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((int x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((uint x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((long x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((ulong x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);
        RegisterHandler((char x) => uint.CreateSaturating(x), HandleUIntCreateSaturating);

        RegisterHandler((byte x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((sbyte x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((short x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((ushort x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((int x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((uint x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((long x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((ulong x) => long.CreateSaturating(x), HandleLongCreateSaturating);
        RegisterHandler((char x) => long.CreateSaturating(x), HandleLongCreateSaturating);

        RegisterHandler((byte x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((sbyte x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((short x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((ushort x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((int x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((uint x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((long x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((ulong x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);
        RegisterHandler((char x) => ulong.CreateSaturating(x), HandleULongCreateSaturating);

        // char methods
        RegisterHandler((char c) => char.IsDigit(c), HandleCharIsDigit);
        RegisterHandler((char c) => char.IsLetter(c), HandleCharIsLetter);
        RegisterHandler((char c) => char.IsWhiteSpace(c), HandleCharIsWhiteSpace);
        RegisterHandler((char c) => char.IsLower(c), HandleCharIsLower);
        RegisterHandler((char c) => char.IsUpper(c), HandleCharIsUpper);
        RegisterHandler((char c) => char.ToLower(c), HandleCharToLower);
        RegisterHandler((char c) => char.ToUpper(c), HandleCharToUpper);
        RegisterHandler((char c) => char.IsPunctuation(c), HandleCharIsPunctuation);
        RegisterHandler((char c) => char.IsSymbol(c), HandleCharIsSymbol);
        RegisterHandler((char c) => char.IsControl(c), HandleCharIsControl);
        RegisterHandler((char c) => char.IsSurrogate(c), HandleCharIsSurrogate);
        RegisterHandler((char c) => char.IsHighSurrogate(c), HandleCharIsHighSurrogate);
        RegisterHandler((char c) => char.IsLowSurrogate(c), HandleCharIsLowSurrogate);
        RegisterHandler((char c) => char.GetNumericValue(c), HandleCharGetNumericValue);
        RegisterHandler((char c) => char.GetNumericValue(c), HandleCharGetNumericValue);
        RegisterHandler((char c) => char.IsLetterOrDigit(c), HandleCharIsLetterOrDigit);
        RegisterHandler((char x, char min, char max) => char.IsBetween(x, min, max), HandleCharIsBetween);
    }
}
