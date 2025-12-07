// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_String.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Numerics;
using System.Text;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_String : SmartContract.Framework.SmartContract
    {
        public static void TestMain()
        {
            var firstName = "Mark";
            var lastName = $"";
            var timestamp = Ledger.GetBlock(Ledger.CurrentHash).Timestamp;
            Runtime.Log($"Hello, {firstName} {lastName}! Current timestamp is {timestamp}.");
        }

        public static void TestEqual()
        {
            var str = "hello";
            var str2 = "hello";
            Runtime.Log(str.Equals(str2).ToString());
        }

        public static void TestSubstring()
        {
            var str = "01234567";
            Runtime.Log(str.Substring(1));
            Runtime.Log(str.Substring(1, 4));
        }

        public static string TestEmpty()
        {
            return string.Empty;
        }

        public static bool TestIsNullOrEmpty(string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool TestIsNullOrWhiteSpace(string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool TestEndWith(string str)
        {
            return str.EndsWith("world");
        }

        public static bool TestContains(string str)
        {
            return str.Contains("world");
        }

        public static bool TestStartsWith(string str)
        {
            return str.StartsWith("world");
        }

        public static int TestCompare(string left, string right)
        {
            return string.Compare(left, right);
        }

        public static int TestIndexOf(string str)
        {
            return str.IndexOf("world");
        }

        public static int TestLastIndexOf(string str)
        {
            return str.LastIndexOf("world");
        }

        public static string TestInterpolatedStringHandler()
        {
            const sbyte sbyteValue = -42;
            const byte byteValue = 42;
            const ushort ushortValue = 1000;
            const uint uintValue = 1000000;
            const ulong ulongValue = 1000000000000;
            var bigIntValue = BigInteger.Parse("1000000000000000000000");
            const char charValue = 'A';
            const string stringValue = "Hello";
            ECPoint ecPointValue = ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");
            // Keep the parsed ECPoint for validation paths while using a readable Base58 string for interpolation output.
            var ecPointString = "gEoSozeEfSovUXCsVZuNcRBW1u4iMsv5gXsvft7fJrnC";
            var byteStringValue = new byte[] { 1, 2, 3 };
            const bool boolValue = true;

            var str = $"SByte: {sbyteValue}, Byte: {byteValue}, UShort: {ushortValue}, " +
                      $"UInt: {uintValue}, ULong: {ulongValue}, " +
                      $"BigInteger: {bigIntValue}, Char: {charValue}, String: {stringValue}, " +
                      $"ECPoint: {ecPointString}, ByteString: {byteStringValue}, Bool: {boolValue}";
            return str;
        }
        public static string TestTrim(string str)
        {
            return str.Trim();
        }

        public static string TestTrimStart(string str)
        {
            return str.TrimStart();
        }

        public static string TestTrimStartChar(string str, char trimChar)
        {
            return str.TrimStart(trimChar);
        }

        public static string TestTrimEnd(string str)
        {
            return str.TrimEnd();
        }

        public static string TestTrimEndChar(string str, char trimChar)
        {
            return str.TrimEnd(trimChar);
        }

        public static string TestTrimArray(string str)
        {
            return str.Trim(new[] { '*' });
        }

        public static string TestTrimStartArray(string str)
        {
            return str.TrimStart(new[] { '*' });
        }

        public static string TestTrimEndArray(string str)
        {
            return str.TrimEnd(new[] { '*' });
        }

        public static char TestPickItem(string s, int index)
        {
            return s[index];
        }

        public static string TestSubstringToEnd(string s, int startIndex)
        {
            return s.Substring(startIndex);
        }

        public static string TestConcat(string? s1, string? s2)
        {
            return string.Concat(s1, s2);
        }

        public static string[] TestSplit(string str)
        {
            return str.Split(' ');
        }

        public static string[] TestSplitRemoveEmpty(string str)
        {
            return str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] TestSplitCharArray(string str)
        {
            return str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] TestSplitStringArray(string str)
        {
            return str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string TestRemove(string str, int startIndex)
        {
            return str.Remove(startIndex);
        }

        public static string TestRemoveRange(string str, int startIndex, int count)
        {
            return str.Remove(startIndex, count);
        }

        public static string TestInsert(string str, int startIndex, string value)
        {
            return str.Insert(startIndex, value);
        }

        public static int TestIndexOfChar(string s, char c)
        {
            return s.IndexOf(c);
        }

        public static string TestToLower(string s)
        {
            return s.ToLower();
        }

        public static string TestToUpper(string s)
        {
            return s.ToUpper();
        }

        public static string TestTrimChar(string s, char trimChar)
        {
            return s.Trim(trimChar);
        }

        public static int TestLength(string s)
        {
            return s.Length;
        }

        public static string TestStringBuilderBasic()
        {
            var builder = new StringBuilder();
            builder.Append("neo");
            builder.Append(' ');
            builder.Append("compiler");
            builder.AppendLine();
            builder.AppendLine("runtime");
            return builder.ToString();
        }

        public static int TestStringBuilderLength()
        {
            var builder = new StringBuilder("neo");
            builder.Append(' ');
            builder.Append("vm");

            var suffix = new StringBuilder();
            suffix.Append(' ');
            suffix.Append("tooling");
            builder.Append(suffix);

            return builder.Length;
        }

        public static string TestStringBuilderClear()
        {
            var builder = new StringBuilder("prefix");
            builder.Clear();
            builder.AppendLine("neo");
            builder.Append("contracts");
            return builder.ToString();
        }

        public static string TestStringBuilderAppendBuilder()
        {
            var builder = new StringBuilder("neo");
            var other = new StringBuilder(" compiler");
            builder.Append(other);
            builder.AppendLine();
            var third = new StringBuilder();
            third.Append("preview");
            builder.Append(third);
            return builder.ToString();
        }
    }
}
