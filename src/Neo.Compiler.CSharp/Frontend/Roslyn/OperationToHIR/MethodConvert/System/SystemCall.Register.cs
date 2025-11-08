// Copyright (C) 2015-2025 The Neo Project.
//
// SystemCall.Register.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private static readonly Type[] s_numericCreateSourceTypes =
    {
        typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(BigInteger)
    };

    private static readonly MethodInfo s_registerUnaryHandlerMethod = typeof(MethodConvert)
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
        .First(m => m.Name == nameof(RegisterHandler)
                    && m.IsGenericMethodDefinition
                    && m.GetGenericArguments().Length == 2
                    && IsFunc2Expression(m.GetParameters()[0].ParameterType));

    private static bool IsFunc2Expression(Type parameterType)
    {
        if (!parameterType.IsGenericType || parameterType.GetGenericTypeDefinition() != typeof(Expression<>))
            return false;

        var funcType = parameterType.GetGenericArguments()[0];
        return funcType.IsGenericType && funcType.GetGenericTypeDefinition() == typeof(Func<,>);
    }

    private static void RegisterSystemCallHandlers()
    {
        // BigInteger handlers
        RegisterBigIntegerHandlers();

        // Register BigInteger method handlers
        // Numeric type handlers
        RegisterNumericTypeHandlers();

        // Math method handlers
        RegisterMathHandlers();

        // String handlers
        RegisterStringHandlers();

        // Array handlers
        RegisterArrayHandlers();

        // Object handlers
        RegisterObjectHandlers();

        // Char handlers
        RegisterCharHandlers();

        // Nullable type handlers
        RegisterNullableTypeHandlers();

        // BitOperations handlers
        RegisterBitOperationsHandlers();

        RegisterEnumHandlers();
    }

    private static void RegisterBigIntegerHandlers()
    {
        // Existing BigInteger handlers
        RegisterHandler(() => BigInteger.One, HandleBigIntegerOne);
        RegisterHandler(() => BigInteger.MinusOne, HandleBigIntegerMinusOne);
        RegisterHandler(() => BigInteger.Zero, HandleBigIntegerZero);
        RegisterHandler((BigInteger b) => b.IsZero, HandleBigIntegerIsZero);
        RegisterHandler((BigInteger b) => b.IsOne, HandleBigIntegerIsOne);
        RegisterHandler((BigInteger b) => b.IsEven, HandleBigIntegerIsEven);
        RegisterHandler((BigInteger b) => b.Sign, HandleBigIntegerSign);

        // BigInteger method handlers
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

        // Add missing BigInteger methods
        // RegisterHandler((BigInteger x, double y) => BigInteger.Log(x, y), HandleBigIntegerLogBase);
        RegisterHandler((BigInteger x) => x.IsPowerOfTwo, HandleBigIntegerIsPow2);
        // RegisterHandler((BigInteger x) => BigInteger.PopCount(x), HandleBigIntegerPopCount);
        RegisterHandler((BigInteger x) => BigInteger.CreateSaturating(x), HandleBigIntegerCreateSaturating);
    }

    private static void RegisterNumericTypeHandlers()
    {
        foreach (var descriptor in s_numericTypeDescriptors)
            RegisterNumericHandlers(descriptor);

        RegisterCreateCheckedHandlers(typeof(BigInteger), HandleBigIntegerCreatedChecked);

        // Numeric explicit cast from BigInteger methods
        RegisterHandler((BigInteger b) => (sbyte)b, HandleBigIntegerToSByte);
        RegisterHandler((BigInteger b) => (byte)b, HandleBigIntegerToByte);
        RegisterHandler((BigInteger b) => (short)b, HandleBigIntegerToShort);
        RegisterHandler((BigInteger b) => (ushort)b, HandleBigIntegerToUShort);
        RegisterHandler((BigInteger b) => (char)b, HandleBigIntegerToUShort);
        RegisterHandler((BigInteger b) => (int)b, HandleBigIntegerToInt);
        RegisterHandler((BigInteger b) => (uint)b, HandleBigIntegerToUInt);
        RegisterHandler((BigInteger b) => (long)b, HandleBigIntegerToLong);
        RegisterHandler((BigInteger b) => (ulong)b, HandleBigIntegerToULong);

        // Numeric explicit cast to BigInteger methods
        RegisterHandler((char c) => (BigInteger)c, HandleToBigInteger);
        RegisterHandler((sbyte b) => (BigInteger)b, HandleToBigInteger);
        RegisterHandler((byte b) => (BigInteger)b, HandleToBigInteger);
        RegisterHandler((short s) => (BigInteger)s, HandleToBigInteger);
        RegisterHandler((ushort s) => (BigInteger)s, HandleToBigInteger);
        RegisterHandler((int i) => (BigInteger)i, HandleToBigInteger);
        RegisterHandler((uint i) => (BigInteger)i, HandleToBigInteger);
        RegisterHandler((long l) => (BigInteger)l, HandleToBigInteger);
        RegisterHandler((ulong l) => (BigInteger)l, HandleToBigInteger);

        // Numeric IsEvenInteger methods
        RegisterHandler((byte x) => byte.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((sbyte x) => sbyte.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((short x) => short.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((ushort x) => ushort.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((int x) => int.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((uint x) => uint.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((long x) => long.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((ulong x) => ulong.IsEvenInteger(x), HandleBigIntegerIsEven);
        RegisterHandler((BigInteger x) => BigInteger.IsEvenInteger(x), HandleBigIntegerIsEven);

        // Numeric IsOddInteger methods
        RegisterHandler((byte x) => byte.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((sbyte x) => sbyte.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((short x) => short.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((ushort x) => ushort.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((int x) => int.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((uint x) => uint.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((long x) => long.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((ulong x) => ulong.IsOddInteger(x), HandleBigIntegerIsOdd);
        RegisterHandler((BigInteger x) => BigInteger.IsOddInteger(x), HandleBigIntegerIsOdd);

        // Numeric IsNegative methods
        RegisterHandler((sbyte x) => sbyte.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((short x) => short.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((int x) => int.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((long x) => long.IsNegative(x), HandleBigIntegerIsNegative);
        RegisterHandler((BigInteger x) => BigInteger.IsNegative(x), HandleBigIntegerIsNegative);

        // Numeric IsPositive methods
        RegisterHandler((sbyte x) => sbyte.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((short x) => short.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((int x) => int.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((long x) => long.IsPositive(x), HandleBigIntegerIsPositive);
        RegisterHandler((BigInteger x) => BigInteger.IsPositive(x), HandleBigIntegerIsPositive);

        // Numeric IsPow2 methods
        RegisterHandler((byte x) => byte.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((sbyte x) => sbyte.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((short x) => short.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((ushort x) => ushort.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((int x) => int.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((uint x) => uint.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((long x) => long.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((ulong x) => ulong.IsPow2(x), HandleBigIntegerIsPow2);
        RegisterHandler((BigInteger x) => BigInteger.IsPow2(x), HandleBigIntegerIsPow2);

        // Numeric LeadingZeroCount method for BigInteger
        RegisterHandler((BigInteger x) => BigInteger.LeadingZeroCount(x), HandleBigIntegerLeadingZeroCount);

        // Numeric Log2 methods
        RegisterHandler((byte x) => byte.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((sbyte x) => sbyte.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((short x) => short.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((ushort x) => ushort.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((int x) => int.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((uint x) => uint.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((long x) => long.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((ulong x) => ulong.Log2(x), HandleBigIntegerLog2);
        RegisterHandler((BigInteger x) => BigInteger.Log2(x), HandleBigIntegerLog2);

        // Numeric Sign methods
        RegisterHandler((byte x) => byte.Sign(x), HandleBigIntegerSign);
        RegisterHandler((sbyte x) => sbyte.Sign(x), HandleBigIntegerSign);
        RegisterHandler((short x) => short.Sign(x), HandleBigIntegerSign);
        RegisterHandler((ushort x) => ushort.Sign(x), HandleBigIntegerSign);
        RegisterHandler((int x) => int.Sign(x), HandleBigIntegerSign);
        RegisterHandler((uint x) => uint.Sign(x), HandleBigIntegerSign);
        RegisterHandler((long x) => long.Sign(x), HandleBigIntegerSign);
        RegisterHandler((ulong x) => ulong.Sign(x), HandleBigIntegerSign);

        // Numeric DivRem methods
        RegisterHandler((byte x, byte y) => byte.DivRem(x, y), HandleMathByteDivRem);
        RegisterHandler((sbyte x, sbyte y) => sbyte.DivRem(x, y), HandleMathSByteDivRem);
        RegisterHandler((short x, short y) => short.DivRem(x, y), HandleMathShortDivRem);
        RegisterHandler((ushort x, ushort y) => ushort.DivRem(x, y), HandleMathUShortDivRem);
        RegisterHandler((int x, int y) => int.DivRem(x, y), HandleMathIntDivRem);
        RegisterHandler((uint x, uint y) => uint.DivRem(x, y), HandleMathUIntDivRem);
        RegisterHandler((long x, long y) => long.DivRem(x, y), HandleMathLongDivRem);
        RegisterHandler((ulong x, ulong y) => ulong.DivRem(x, y), HandleMathULongDivRem);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.DivRem(x, y), HandleMathBigIntegerDivRem);

        // Numeric Clamp methods
        RegisterHandler((byte value, byte min, byte max) => byte.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((sbyte value, sbyte min, sbyte max) => sbyte.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((short value, short min, short max) => short.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ushort value, ushort min, ushort max) => ushort.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((int value, int min, int max) => int.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((uint value, uint min, uint max) => uint.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((long value, long min, long max) => long.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ulong value, ulong min, ulong max) => ulong.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((BigInteger value, BigInteger min, BigInteger max) => BigInteger.Clamp(value, min, max), HandleMathClamp);

        // Numeric CopySign method for BigInteger
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.CopySign(x, y), HandleBigIntegerCopySign);

        #region CreateChecked
        #endregion CreateChecked

        // Numeric ToString methods
        #region ToString()
        RegisterHandler((sbyte x) => x.ToString(), HandleToString);
        RegisterHandler((byte x) => x.ToString(), HandleToString);
        RegisterHandler((short x) => x.ToString(), HandleToString);
        RegisterHandler((ushort x) => x.ToString(), HandleToString);
        RegisterHandler((int x) => x.ToString(), HandleToString);
        RegisterHandler((uint x) => x.ToString(), HandleToString);
        RegisterHandler((long x) => x.ToString(), HandleToString);
        RegisterHandler((ulong x) => x.ToString(), HandleToString);
        RegisterHandler((bool x) => x.ToString(), HandleBoolToString);
        RegisterHandler((char x) => x.ToString(), HandleCharToString);
        RegisterHandler((BigInteger x) => x.ToString(), HandleToString);
        #endregion ToString()

        // Numeric Equals() methods
        #region Equals
        RegisterHandler((byte x, byte y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((sbyte x, sbyte y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((short x, short y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((ushort x, ushort y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((int x, int y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((uint x, uint y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((long x, long y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((ulong x, ulong y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((BigInteger x, BigInteger y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((BigInteger x, long y) => x.Equals(y), HandleBigIntegerEquals);
        RegisterHandler((BigInteger x, ulong y) => x.Equals(y), HandleBigIntegerEquals);
        #endregion Equals

        // Numeric Parse() methods
        #region Parse()
        RegisterHandler((string s) => BigInteger.Parse(s), HandleBigIntegerParse);
        #endregion

        // Numeric PopCount
        #region PopCount
        RegisterHandler((BigInteger value) => BigInteger.PopCount(value), HandleBigIntegerPopCount);
        #endregion PopCount
    }

    private static void RegisterCreateCheckedHandlers(Type targetType, SystemCallHandler handler)
    {
        RegisterCreateNumericHandlers("CreateChecked", targetType, handler);
    }

    private static void RegisterCreateSaturatingHandlers(Type targetType, SystemCallHandler handler)
    {
        RegisterCreateNumericHandlers("CreateSaturating", targetType, handler);
    }

    private static void RegisterCreateNumericHandlers(string methodName, Type targetType, SystemCallHandler handler)
    {
        foreach (var sourceType in s_numericCreateSourceTypes)
            RegisterCreateNumericHandler(methodName, targetType, sourceType, handler);
    }

    private static void RegisterCreateNumericHandler(string methodName, Type targetType, Type sourceType, SystemCallHandler handler)
    {
        var methodDefinition = targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 1);
        if (methodDefinition is null)
        {
            throw new InvalidOperationException($"Expected generic factory method '{targetType.FullName}.{methodName}<TValue>(TValue)' to exist for numeric system-call registration.");
        }

        MethodInfo method;
        try
        {
            method = methodDefinition.MakeGenericMethod(sourceType);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Failed to create generic specialization '{targetType.FullName}.{methodName}<{sourceType.FullName}>' for numeric system-call registration.", ex);
        }

        var parameter = Expression.Parameter(sourceType, "value");
        var call = Expression.Call(method, parameter);
        var funcType = typeof(Func<,>).MakeGenericType(sourceType, targetType);
        var lambda = Expression.Lambda(funcType, call, parameter);
        var register = s_registerUnaryHandlerMethod.MakeGenericMethod(sourceType, targetType);
        register.Invoke(null, new object?[] { lambda, handler, null });
    }

    private static void RegisterMathHandlers()
    {
        #region Abs
        RegisterHandler((sbyte x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((short x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((int x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((long x) => Math.Abs(x), HandleMathAbs);
        RegisterHandler((BigInteger x) => BigInteger.Abs(x), HandleMathAbs);
        #endregion Abs

        #region Sign
        RegisterHandler((sbyte x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((short x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((int x) => Math.Sign(x), HandleMathSign);
        RegisterHandler((long x) => Math.Sign(x), HandleMathSign);
        #endregion Sign

        #region Max
        RegisterHandler((byte x, byte y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((sbyte x, sbyte y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((short x, short y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((ushort x, ushort y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((int x, int y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((uint x, uint y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((long x, long y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((ulong x, ulong y) => Math.Max(x, y), HandleMathMax);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Max(x, y), HandleBigIntegerMax);
        #endregion Max

        #region Min
        RegisterHandler((byte x, byte y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((sbyte x, sbyte y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((short x, short y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((ushort x, ushort y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((int x, int y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((uint x, uint y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((long x, long y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((ulong x, ulong y) => Math.Min(x, y), HandleMathMin);
        RegisterHandler((BigInteger x, BigInteger y) => BigInteger.Min(x, y), HandleBigIntegerMin);
        #endregion Min

        #region DivRem
        RegisterHandler((byte x, byte y) => Math.DivRem(x, y), HandleMathByteDivRem);
        RegisterHandler((sbyte x, sbyte y) => Math.DivRem(x, y), HandleMathSByteDivRem);
        RegisterHandler((short x, short y) => Math.DivRem(x, y), HandleMathShortDivRem);
        RegisterHandler((ushort x, ushort y) => Math.DivRem(x, y), HandleMathUShortDivRem);
        RegisterHandler((int x, int y) => Math.DivRem(x, y), HandleMathIntDivRem);
        RegisterHandler((uint x, uint y) => Math.DivRem(x, y), HandleMathUIntDivRem);
        RegisterHandler((long x, long y) => Math.DivRem(x, y), HandleMathLongDivRem);
        RegisterHandler((ulong x, ulong y) => Math.DivRem(x, y), HandleMathULongDivRem);
        #endregion DivRem

        #region Clamp
        RegisterHandler((byte value, byte min, byte max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((sbyte value, sbyte min, sbyte max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((short value, short min, short max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ushort value, ushort min, ushort max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((int value, int min, int max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((uint value, uint min, uint max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((long value, long min, long max) => Math.Clamp(value, min, max), HandleMathClamp);
        RegisterHandler((ulong value, ulong min, ulong max) => Math.Clamp(value, min, max), HandleMathClamp);
        #endregion Clamp

        #region BigMul
        RegisterHandler((int x, int y) => Math.BigMul(x, y), HandleMathBigMul);
        #endregion BigMul
    }

    private static void RegisterStringHandlers()
    {
        RegisterHandler((object x) => x.ToString(), HandleObjectToString);
        RegisterHandler((string s) => s.ToString(), HandleStringToString);

        RegisterHandler((string? s) => string.IsNullOrEmpty(s), HandleStringIsNullOrEmpty);
        RegisterHandler((string? s) => string.IsNullOrWhiteSpace(s), HandleStringIsNullOrWhiteSpace);
        RegisterHandler((string s, int startIndex, int length) => s.Substring(startIndex, length), HandleStringSubstring);
        RegisterHandler((string s, string value) => s.Contains(value), HandleStringContains);
        RegisterHandler((string s, string value) => s.StartsWith(value), HandleStringStartsWith);
        RegisterHandler((string s, string value) => s.EndsWith(value), HandleStringEndsWith);
        RegisterHandler((string s, string value) => s.IndexOf(value), HandleStringIndexOf);
        RegisterHandler((string s, string value) => s.LastIndexOf(value), HandleStringLastIndexOf);
        RegisterHandler((string strA, string strB) => string.Compare(strA, strB), HandleStringCompare);
        RegisterHandler((string s, char separator) => s.Split(separator, StringSplitOptions.None), HandleStringSplit);
        RegisterHandler<string, char, StringSplitOptions, string[]>((s, separator, options) => s.Split(separator, options), HandleStringSplit);
        SystemCallHandlers["string.Split(char, int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[], System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[], int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[])"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[], int)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[]?, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[]?, int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[]?)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(char[]?, int)"] = HandleStringSplit;
        RegisterHandler<string, string?, string[]>((s, separator) => s.Split(separator, StringSplitOptions.None), HandleStringSplit);
        RegisterHandler<string, string?, StringSplitOptions, string[]>((s, separator, options) => s.Split(separator, options), HandleStringSplit);
        SystemCallHandlers["string.Split(string?, int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[], System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[], int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[])"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[], int)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[], System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[], int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[])"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[], int)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[]?, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[]?, int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[]?)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string[]?, int)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[]?, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[]?, int, System.StringSplitOptions)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[]?)"] = HandleStringSplit;
        SystemCallHandlers["string.Split(string?[]?, int)"] = HandleStringSplit;
        RegisterHandler((string s, int index) => s[index], HandleStringPickItem);
        RegisterHandler((string s, int startIndex) => s.Substring(startIndex), HandleStringSubStringToEnd);
        RegisterHandler((string? s1, string? s2) => string.Concat(s1, s2), HandleStringConcat);
        RegisterHandler((string s, char c) => s.IndexOf(c), HandleStringIndexOfChar);
        RegisterHandler((string s) => s.ToLower(), HandleStringToLower);
        RegisterHandler((string s) => s.ToUpper(), HandleStringToUpper);
        RegisterHandler((string s) => s.Trim(), HandleStringTrim);
        RegisterHandler((string s, char trimChar) => s.Trim(trimChar), HandleStringTrimChar);
        RegisterHandler<string, char[], string>((s, trimChars) => s.Trim(trimChars), HandleStringTrimCharArray);
        SystemCallHandlers["string.Trim(char[])"] = HandleStringTrimCharArray;
        SystemCallHandlers["string.Trim(char[]?)"] = HandleStringTrimCharArray;
        SystemCallHandlers["string.Trim(params char[])"] = HandleStringTrimCharArray;
        SystemCallHandlers["string.Trim(params char[]?)"] = HandleStringTrimCharArray;
        RegisterHandler((string s) => s.TrimStart(), HandleStringTrimStart);
        RegisterHandler((string s, char trimChar) => s.TrimStart(trimChar), HandleStringTrimStartChar);
        RegisterHandler<string, char[], string>((s, trimChars) => s.TrimStart(trimChars), HandleStringTrimStartCharArray);
        SystemCallHandlers["string.TrimStart(char[])"] = HandleStringTrimStartCharArray;
        SystemCallHandlers["string.TrimStart(char[]?)"] = HandleStringTrimStartCharArray;
        SystemCallHandlers["string.TrimStart(params char[])"] = HandleStringTrimStartCharArray;
        SystemCallHandlers["string.TrimStart(params char[]?)"] = HandleStringTrimStartCharArray;
        RegisterHandler((string s) => s.TrimEnd(), HandleStringTrimEnd);
        RegisterHandler((string s, char trimChar) => s.TrimEnd(trimChar), HandleStringTrimEndChar);
        RegisterHandler<string, char[], string>((s, trimChars) => s.TrimEnd(trimChars), HandleStringTrimEndCharArray);
        SystemCallHandlers["string.TrimEnd(char[])"] = HandleStringTrimEndCharArray;
        SystemCallHandlers["string.TrimEnd(char[]?)"] = HandleStringTrimEndCharArray;
        SystemCallHandlers["string.TrimEnd(params char[])"] = HandleStringTrimEndCharArray;
        SystemCallHandlers["string.TrimEnd(params char[]?)"] = HandleStringTrimEndCharArray;
        RegisterHandler((string s, string oldValue, string newValue) => s.Replace(oldValue, newValue), HandleStringReplace);
        RegisterHandler((string s, int startIndex) => s.Remove(startIndex), HandleStringRemove);
        RegisterHandler((string s, int startIndex, int count) => s.Remove(startIndex, count), HandleStringRemove);
        RegisterHandler((string s, int startIndex, string value) => s.Insert(startIndex, value), HandleStringInsert);
        RegisterHandler((string s) => s.Length, HandleStringLength);
    }

    private static void RegisterArrayHandlers()
    {
        RegisterHandler((Array a) => a.Length, HandleLength);
        RegisterHandler((Array array) => Array.Reverse(array), HandleArrayReverse);
    }

    private static void RegisterObjectHandlers()
    {
        // Existing object handlers
#pragma warning disable CS8602
        RegisterHandler((object? x, object? y) => x.Equals(y), HandleObjectEquals);
#pragma warning restore CS8602
    }

    private static void RegisterCharHandlers()
    {
        // Existing char handlers
        RegisterHandler((char c) => char.IsDigit(c), HandleCharIsDigit);
        RegisterHandler((char c) => char.IsLetter(c), HandleCharIsLetter);
        RegisterHandler((char c) => char.IsWhiteSpace(c), HandleCharIsWhiteSpace);
        RegisterHandler((char c) => char.IsLower(c), HandleCharIsLower);
        RegisterHandler((char c) => char.ToLower(c), HandleCharToLower);
        RegisterHandler((char c) => char.IsUpper(c), HandleCharIsUpper);
        RegisterHandler((char c) => char.ToUpper(c), HandleCharToUpper);
        RegisterHandler((char c) => char.IsPunctuation(c), HandleCharIsPunctuation);
        RegisterHandler((char c) => char.IsSymbol(c), HandleCharIsSymbol);
        RegisterHandler((char c) => char.IsControl(c), HandleCharIsControl);
        RegisterHandler((char c) => char.IsSurrogate(c), HandleCharIsSurrogate);
        RegisterHandler((char c) => char.IsHighSurrogate(c), HandleCharIsHighSurrogate);
        RegisterHandler((char c) => char.IsLowSurrogate(c), HandleCharIsLowSurrogate);
        RegisterHandler((char c) => char.GetNumericValue(c), HandleCharGetNumericValue);
        RegisterHandler((char c) => char.IsLetterOrDigit(c), HandleCharIsLetterOrDigit);
        RegisterHandler((char x, char min, char max) => char.IsBetween(x, min, max), HandleCharIsBetween);

        RegisterHandler((char c) => char.ToLowerInvariant(c), HandleCharToLowerInvariant);
        RegisterHandler((char c) => char.ToUpperInvariant(c), HandleCharToUpperInvariant);
        RegisterHandler((char c) => char.IsAscii(c), HandleCharIsAscii);
        RegisterHandler((char c) => char.IsAsciiDigit(c), HandleCharIsAsciiDigit);
        RegisterHandler((char c) => char.IsAsciiLetter(c), HandleCharIsAsciiLetter);
    }

    private static void RegisterNullableTypeHandlers()
    {
        // Nullable HasValue methods
        #region HasValue
        RegisterHandler((byte? x) => x.HasValue, HandleNullableByteHasValue);
        RegisterHandler((sbyte? x) => x.HasValue, HandleNullableSByteHasValue);
        RegisterHandler((short? x) => x.HasValue, HandleNullableShortHasValue);
        RegisterHandler((ushort? x) => x.HasValue, HandleNullableUShortHasValue);
        RegisterHandler((uint? x) => x.HasValue, HandleNullableUIntHasValue);
        RegisterHandler((ulong? x) => x.HasValue, HandleNullableULongHasValue);
        RegisterHandler((BigInteger? x) => x.HasValue, HandleNullableBigIntegerHasValue);
        RegisterHandler((char? x) => x.HasValue, HandleNullableCharHasValue);
        RegisterHandler((int? x) => x.HasValue, HandleNullableIntHasValue);
        RegisterHandler((long? x) => x.HasValue, HandleNullableLongHasValue);
        RegisterHandler((bool? x) => x.HasValue, HandleNullableBoolHasValue);
        #endregion HasValue

        // Nullable GetValueOrDefault methods
        #region GetValueOrDefault
        RegisterHandler((byte? x) => x.GetValueOrDefault(), HandleNullableByteGetValueOrDefault);
        RegisterHandler((sbyte? x) => x.GetValueOrDefault(), HandleNullableSByteGetValueOrDefault);
        RegisterHandler((short? x) => x.GetValueOrDefault(), HandleNullableShortGetValueOrDefault);
        RegisterHandler((ushort? x) => x.GetValueOrDefault(), HandleNullableUShortGetValueOrDefault);
        RegisterHandler((uint? x) => x.GetValueOrDefault(), HandleNullableUIntGetValueOrDefault);
        RegisterHandler((ulong? x) => x.GetValueOrDefault(), HandleNullableULongGetValueOrDefault);
        RegisterHandler((BigInteger? x) => x.GetValueOrDefault(), HandleNullableBigIntegerGetValueOrDefault);
        RegisterHandler((char? x) => x.GetValueOrDefault(), HandleNullableCharGetValueOrDefault);
        RegisterHandler((int? x) => x.GetValueOrDefault(), HandleNullableIntGetValueOrDefault);
        RegisterHandler((long? x) => x.GetValueOrDefault(), HandleNullableLongGetValueOrDefault);
        RegisterHandler((bool? x) => x.GetValueOrDefault(), HandleNullableBoolGetValueOrDefault);
        #endregion GetValueOrDefault

        // Nullable Value methods
#pragma warning disable CS8629
        #region Value
        RegisterHandler((byte? x) => x.Value, HandleNullableByteValue);
        RegisterHandler((sbyte? x) => x.Value, HandleNullableSByteValue);
        RegisterHandler((short? x) => x.Value, HandleNullableShortValue);
        RegisterHandler((ushort? x) => x.Value, HandleNullableUShortValue);
        RegisterHandler((uint? x) => x.Value, HandleNullableUIntValue);
        RegisterHandler((ulong? x) => x.Value, HandleNullableULongValue);
        RegisterHandler((BigInteger? x) => x.Value, HandleNullableBigIntegerValue);
        RegisterHandler((char? x) => x.Value, HandleNullableCharValue);
        RegisterHandler((int? x) => x.Value, HandleNullableIntValue);
        RegisterHandler((long? x) => x.Value, HandleNullableLongValue);
        RegisterHandler((bool? x) => x.Value, HandleNullableBoolValue);
        #endregion Value
#pragma warning restore CS8629

        // Nullable ToString() methods
        #region ToString()
        RegisterHandler((bool? b) => b.ToString(), HandleNullableBoolToString);
        RegisterHandler((byte? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((sbyte? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((short? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((ushort? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((int? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((uint? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((long? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((ulong? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((char? x) => x.ToString(), HandleNullableToString);
        RegisterHandler((BigInteger? x) => x.ToString(), HandleNullableToString);
        #endregion ToString()

        // Nullable Equals() methods
        #region Equals
        RegisterHandler((bool? x, object? y) => x.Equals(y), HandleNullableBoolEquals);
        RegisterHandler((byte? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((sbyte? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((short? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((ushort? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((int? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((uint? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((long? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((ulong? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((char? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);
        RegisterHandler((BigInteger? x, object? y) => x.Equals(y), HandleNullableBigIntegerEquals);

        RegisterHandler((bool x, object? y) => x.Equals(y), HandleNullableBoolEqualsWithNonNullable);
        RegisterHandler((byte x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((sbyte x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((short x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((ushort x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((int x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((uint x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((long x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((ulong x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((char x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((BigInteger x, object? y) => x.Equals(y), HandleNullableBigIntegerEqualsWithNonNullable);
        RegisterHandler((string x, string? y) => x.Equals(y), HandleObjectEquals);
        #endregion Equals

        #region Out Methods
        // Numeric types
        RegisterHandler((string? s, byte result) => byte.TryParse(s, out result), HandleByteTryParseWithOut);
        RegisterHandler((string? s, sbyte result) => sbyte.TryParse(s, out result), HandleSByteTryParseWithOut);
        RegisterHandler((string? s, short result) => short.TryParse(s, out result), HandleShortTryParseWithOut);
        RegisterHandler((string? s, ushort result) => ushort.TryParse(s, out result), HandleUShortTryParseWithOut);
        RegisterHandler((string? s, int result) => int.TryParse(s, out result), HandleIntTryParseWithOut);
        RegisterHandler((string? s, uint result) => uint.TryParse(s, out result), HandleUIntTryParseWithOut);
        RegisterHandler((string? s, long result) => long.TryParse(s, out result), HandleLongTryParseWithOut);
        RegisterHandler((string? s, ulong result) => ulong.TryParse(s, out result), HandleULongTryParseWithOut);

        // Bool
        RegisterHandler((string? value, bool result) => bool.TryParse(value, out result), HandleBoolTryParseWithOut);
        #endregion
    }

    private static void RegisterBitOperationsHandlers()
    {
        // Static Methods
        RegisterHandler((uint value) => BitOperations.Log2(value), HandleBigIntegerLog2);
        RegisterHandler((ulong value) => BitOperations.Log2(value), HandleBigIntegerLog2);
    }

    private static void RegisterEnumHandlers()
    {
        RegisterHandler((Type enumType, string value) => Enum.Parse(enumType, value), HandleEnumParse);
        RegisterHandler((Type enumType, string value, bool ignoreCase) => Enum.Parse(enumType, value, ignoreCase), HandleEnumParseIgnoreCase);

#pragma warning disable CS8600
        RegisterHandler((Type enumType, string value, object result) => Enum.TryParse(enumType, value, out result), HandleEnumTryParse);
        RegisterHandler((Type enumType, string value, bool ignoreCase, object result) => Enum.TryParse(enumType, value, ignoreCase, out result), HandleEnumTryParseIgnoreCase);
#pragma warning restore CS8600

        RegisterHandler((Type enumType) => Enum.GetNames(enumType), HandleEnumGetNames);
        RegisterHandler((Type enumType) => Enum.GetValues(enumType), HandleEnumGetValues);
        RegisterHandler((Type enumType, object value) => Enum.IsDefined(enumType, value), HandleEnumIsDefined);
        RegisterHandler((Type enumType, string name) => Enum.IsDefined(enumType, name), HandleEnumIsDefinedByName);
        RegisterHandler((Enum value) => Enum.GetName(value.GetType(), value), HandleEnumGetName, "System.Enum.GetName<>()");
        RegisterHandler((Type enumType, object value) => Enum.GetName(enumType, value), HandleEnumGetNameWithType, "System.Enum.GetName()");

        // these two methods will not be supported, since we cannot apply format logic.
        // RegisterHandler((Enum value) => Enum.Format(value.GetType(), value, "G"), HandleEnumFormat);
        // RegisterHandler((Type enumType, object value, string format) => Enum.Format(enumType, value, format), HandleEnumFormatWithType);

        // these two methods will not be supported, since we don't have `Type` class support in neo csharp.
        // RegisterHandler((Enum value) => Enum.GetUnderlyingType(value.GetType()), HandleEnumGetUnderlyingType);
        // RegisterHandler((Type enumType) => Enum.GetUnderlyingType(enumType), HandleEnumGetUnderlyingTypeWithType);
    }
}
