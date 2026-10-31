// Copyright (C) 2015-2026 The Neo Project.
//
// SemanticMatrixProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract.Testing;
using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Compares language constructs that the compiler lowers by hand against the CLR.
/// </summary>
/// <remarks>
/// <para>
/// Every shape is compiled twice, once as a contract and once as plain C#, then executed with
/// the same arguments. Shapes that neither side accepts are reported as unsupported so the
/// probe keeps working while the supported subset grows.
/// </para>
/// <para>
/// Unlike the struct matrix, a shape may legitimately throw on both sides. Two throws count as
/// agreement; only a throw on one side and a value on the other is a mismatch.
/// </para>
/// </remarks>
public static class SemanticMatrixProbe
{
    private static readonly (string Label, string Declarations, string Body, int? Expected)[] Cases =
    [
        ("big integer division with negatives",
            """
                    private static BigInteger Div(int a, int b) { return (BigInteger)a / (BigInteger)b; }
            """,
            """
                    return (int)(Div(-a, b) * 1000 + Div(a, -b));
            """, null),

        ("big integer modulo with negatives",
            """
                    private static BigInteger Rem(int a, int b) { return (BigInteger)a % (BigInteger)b; }
            """,
            """
                    return (int)(Rem(-a, b) * 100 + Rem(a, -b));
            """, null),

        ("big integer power",
            """
                    private static BigInteger Pow(int a, int b) { return BigInteger.Pow(a, b); }
            """,
            """
                    return (int)Pow(a, 3);
            """, null),

        ("big integer shift",
            """
                    private static BigInteger Shift(int a, int b) { return (BigInteger)a << b; }
            """,
            """
                    return (int)Shift(a, 4);
            """, null),

        ("big integer parse and to string",
            """
                    private static int RoundTrip(int a)
                    {
                        string text = ((BigInteger)a).ToString();
                        return text.Length;
                    }
            """,
            """
                    return RoundTrip(a) * 10;
            """, null),


        ("checked arithmetic",
            """
                    private static int Add(int a, int b) { checked { return a + b; } }
            """,
            """
                    return Add(a, 1);
            """, null),

        ("unchecked arithmetic",
            """
                    private static int Add(int a, int b) { unchecked { return a + b; } }
            """,
            """
                    return Add(int.MaxValue, 1);
            """, null),

        ("lambda capturing and mutating a local",
            """
                    private static int Run(int a)
                    {
                        int total = 0;
                        Func<int> bump = () => { total = total + a; return total; };
                        return bump() * 100 + bump();
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("string interpolation",
            """
                    private static string Text(int a, int b) { return $"value {a} and {b + 1}"; }
            """,
            """
                    return Text(a, b).Length * 100 + (Text(a, b) == "value 7 and 4" ? 1 : 0);
            """, null),

        ("char arithmetic",
            """
                    private static int Next(char c) { return c + 2; }
            """,
            """
                    return Next('a') * 10 + (int)'z';
            """, null),

        ("nullable arithmetic and coalescing",
            """
                    private static int Run(int a)
                    {
                        int? value = a > 5 ? a : (int?)null;
                        return (value ?? 1) * 10 + (value.HasValue ? 1 : 0);
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("switch expression with constants",
            """
                    private static int Pick(int a)
                    {
                        return a switch { 1 => 10, 2 => 20, _ => 30 };
                    }
            """,
            """
                    return Pick(a) + Pick(2);
            """, null),

        // The framework list is a set of opcode-lowered stubs, so the CLR reference cannot run
        // it; the expected value is what the same loop returns over an array.
        ("foreach over a framework list",
            """
                    private static int Sum(int a, int b)
                    {
                        var values = new Neo.SmartContract.Framework.List<int>();
                        values.Add(a);
                        values.Add(b);
                        values.Add(a + b);
                        int total = 0;
                        foreach (int value in values) total = total + value;
                        return total;
                    }
            """,
            """
                    return Sum(a, b);
            """, 20),

        ("array element passed by ref-like out parameter",
            """
                    private static void Fill(out int value) { value = 7; }
                    private static int Run()
                    {
                        Fill(out int value);
                        return value;
                    }
            """,
            """
                    return Run();
            """, null),

        ("ternary with side effects",
            """
                    private static int Run(int a)
                    {
                        int calls = 0;
                        Func<bool, int> pick = flag => { calls = calls + 1; return flag ? a : 0; };
                        int result = a > 5 ? pick(true) : pick(false);
                        return result * 10 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("big integer bitwise operators",
            """
                    private static BigInteger Mix(int a, int b) { return ((BigInteger)a) & ((BigInteger)b); }
                    private static BigInteger Or(int a, int b) { return ((BigInteger)a) | ((BigInteger)b); }
                    private static BigInteger Xor(int a, int b) { return ((BigInteger)a) ^ ((BigInteger)b); }
            """,
            """
                    return (int)(Mix(a, b) * 10 + Or(a, b) + Xor(a, b));
            """, null),


        ("big integer min max",
            """
                    private static int Run(int a, int b)
                    {
                        return (int)BigInteger.Min(a, b) * 10 + (int)BigInteger.Max(a, b);
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("big integer power edge cases",
            """
                    private static BigInteger Run(int a, int b)
                    {
                        return BigInteger.Pow(a, 0) + BigInteger.Pow(a, 1) + BigInteger.Pow(b, 2);
                    }
            """,
            """
                    return (int)Run(a, b);
            """, null),

        ("big integer greatest common divisor",
            """
                    private static int Run(int a, int b)
                    {
                        return (int)BigInteger.GreatestCommonDivisor(a, b);
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("big integer mod pow",
            """
                    private static int Run(int a, int b)
                    {
                        return (int)BigInteger.ModPow(a, b, 5);
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("big integer comparison operators",
            """
                    private static int Run(int a, int b)
                    {
                        int total = 0;
                        if ((BigInteger)a > (BigInteger)b) total = total + 1;
                        if ((BigInteger)a < (BigInteger)b) total = total + 10;
                        if ((BigInteger)a == (BigInteger)b) total = total + 100;
                        return total;
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("math abs min max clamp",
            """
                    private static int Run(int a, int b)
                    {
                        return Math.Abs(-a) * 100 + Math.Min(a, b) * 10 + Math.Max(a, b) + Math.Clamp(a, 0, 5) * 1000;
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("math sign",
            """
                    private static int Run(int a, int b)
                    {
                        return Math.Sign(-a) * 10 + Math.Sign(b);
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("string substring and index",
            """
                    private static int Run(int a)
                    {
                        string text = "abcdef";
                        return text.Substring(2).Length * 100 + text.IndexOf("cd") * 10 + text.Substring(1, 2).Length;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("string case and trim",
            """
                    private static int Run(int a)
                    {
                        string text = "  Neo  ";
                        return text.Trim().Length * 100 + text.ToUpper().Length * 10 + text.ToLower().Length;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("string replace and contains",
            """
                    private static int Run(int a)
                    {
                        string text = "banana";
                        bool has = text.Contains("nan");
                        bool starts = text.StartsWith("ban");
                        return text.Replace("a", "o").Length * 100 + (has ? 10 : 0) + (starts ? 1 : 0);
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("string concat and compare",
            """
                    private static int Run(int a)
                    {
                        string left = "ab" + a;
                        string right = "ab7";
                        return left.Length * 100 + (left == right ? 10 : 0) + (left != right ? 1 : 0);
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("array length and index",
            """
                    private static int Run(int a)
                    {
                        var values = new int[] { a, a + 1, a + 2 };
                        return values.Length * 100 + values[2] * 10 + values[0];
                    }
            """,
            """
                    return Run(a);
            """, null),


        ("tuple deconstruction order",
            """
                    private static int Run(int a, int b)
                    {
                        var (first, second) = (a, b + 1);
                        return first * 10 + second;
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("tuple member assignment with constants",
            """
                    private sealed class Box { public int X; public int Y; }
                    private static int Run(int a, int b)
                    {
                        var left = new Box();
                        var right = new Box();
                        (left.X, _, right.Y) = (a, b, a + b);
                        return left.X * 100 + right.Y;
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("tuple member assignment with side-effecting RHS",
            """
                    private sealed class Box { public int X; public int Y; }
                    private static int Next(ref int state) { return ++state; }
                    private static int Run(int a)
                    {
                        var left = new Box();
                        var right = new Box();
                        int state = a;
                        (left.X, _, right.Y) = (Next(ref state), Next(ref state), Next(ref state));
                        return left.X * 100000 + right.Y * 10000 + state;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("tuple member assignment with side-effecting receivers",
            """
                    private sealed class Box { public int X; public int Y; }
                    private static Box Pick(Box value, ref int state) { state++; return value; }
                    private static int Run(int a, int b)
                    {
                        var left = new Box();
                        var right = new Box();
                        int state = a;
                        (Pick(left, ref state).X, _, Pick(right, ref state).Y) = (b, b + 1, b + 2);
                        return left.X * 100000 + right.Y * 10000 + state;
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("tuple member assignment with side-effecting RHS and receivers",
            """
                    private sealed class Box { public int X; public int Y; }
                    private static int Next(ref int state) { return ++state; }
                    private static Box Pick(Box value, ref int state) { state++; return value; }
                    private static int Run(int a)
                    {
                        var left = new Box();
                        var right = new Box();
                        int state = a;
                        (Pick(left, ref state).X, _, Pick(right, ref state).Y) = (Next(ref state), Next(ref state), Next(ref state));
                        return left.X * 100000 + right.Y * 10000 + state;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("params argument order",
            """
                    private static int Sum(params int[] values)
                    {
                        int total = 0;
                        foreach (int value in values) total = total + value;
                        return total;
                    }
            """,
            """
                    return Sum(a, b) * 10 + Sum(1, 2, 3);
            """, null),

        ("nested array literal order",
            """
                    private static int Run(int a, int b)
                    {
                        var rows = new int[][] { new int[] { a, b }, new int[] { b, a } };
                        return rows[0][0] * 100 + rows[0][1] * 10 + rows[1][0];
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        // C# masks the shift count by the operand width: 7 << 33 is 7 << 1. An unbounded
        // SHL in the VM would return 7 * 2^33 instead.
        ("int shift count beyond the width",
            """
                    private static int Shl(int a, int b) { return a << b; }
                    private static int Shr(int a, int b) { return a >> b; }
            """,
            """
                    return Shl(a, 33) * 10 + Shr(a, 33);
            """, null),

        ("int shift by a negative count",
            """
                    private static int Shl(int a, int b) { return a << b; }
            """,
            """
                    return Shl(a, -1) == (a << 31) ? 1 : 0;
            """, null),

        ("long shift count beyond the width",
            """
                    private static long Shl(long a, int b) { return a << b; }
                    private static long Shr(long a, int b) { return a >> b; }
            """,
            """
                    return (int)(Shl(a, 65) * 10 + Shr(a, 65));
            """, null),

        ("narrow type shift",
            """
                    private static int Shl(byte a, int b) { return a << b; }
            """,
            """
                    return Shl((byte)a, 9);
            """, null),

        ("integer division overflow",
            """
                    private static int Div(int a, int b) { return a / b; }
            """,
            """
                    return Div(int.MinValue, -1);
            """, null),

        ("integer remainder of the overflow pair",
            """
                    private static int Rem(int a, int b) { return a % b; }
            """,
            """
                    return Rem(int.MinValue, -1) + 5;
            """, null),

        ("negative integer remainder",
            """
                    private static int Rem(int a, int b) { return a % b; }
            """,
            """
                    return Rem(-a, b) * 10 + Rem(a, -b);
            """, null),

        ("unsigned comparison",
            """
                    private static int Run(int a)
                    {
                        uint value = (uint)a;
                        return value > 0 ? 1 : 0;
                    }
            """,
            """
                    return Run(-1) * 10 + Run(1);
            """, null),

        ("unsigned long arithmetic",
            """
                    private static int Run(int a)
                    {
                        ulong value = (ulong)a * 1000000000000UL;
                        return (int)(value % 1000);
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("unchecked narrow cast",
            """
                    private static int Run(int a)
                    {
                        byte value = unchecked((byte)(a * 100));
                        return value;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("long to int cast",
            """
                    private static int Run(int a)
                    {
                        long wide = a * 1000000000L;
                        return (int)wide;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("sbyte arithmetic overflow",
            """
                    private static int Run(int a, int b)
                    {
                        sbyte left = (sbyte)a;
                        sbyte right = (sbyte)b;
                        return unchecked((sbyte)(left * right));
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        // Storage has no CLR equivalent, so these shapes carry their expected value.
        ("storage find values only",
            """
                    private static int Run()
                    {
                        var context = Storage.CurrentContext;
                        Storage.Put(context, new byte[] { 0x01, 0x01 }, 1);
                        Storage.Put(context, new byte[] { 0x01, 0x02 }, 2);
                        Storage.Put(context, new byte[] { 0x02, 0x01 }, 3);
                        int total = 0;
                        foreach (var item in Storage.Find(context, new byte[] { 0x01 }, FindOptions.ValuesOnly))
                            total = total + (int)item;
                        return total;
                    }
            """,
            """
                    return Run();
            """, 3),

        ("storage find prefix count",
            """
                    private static int Run()
                    {
                        var context = Storage.CurrentContext;
                        Storage.Put(context, new byte[] { 0x01, 0x01 }, 1);
                        Storage.Put(context, new byte[] { 0x01, 0x02 }, 2);
                        Storage.Put(context, new byte[] { 0x02, 0x01 }, 3);
                        int count = 0;
                        foreach (var item in Storage.Find(context, new byte[] { 0x01 }, FindOptions.None))
                            count = count + 1;
                        return count;
                    }
            """,
            """
                    return Run();
            """, 2),

        ("string indexer and char conversion",
            """
                    private static int Run(int a)
                    {
                        string text = "abcdef";
                        char letter = text[1];
                        return letter + 1;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("byte string conversion",
            """
                    private static int Run(int a)
                    {
                        byte[] raw = new byte[] { (byte)a, 2, 3 };
                        ByteString text = (ByteString)raw;
                        return text.Length * 10 + text[0];
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("byte array to string round trip",
            """
                    private static int Run(int a)
                    {
                        string text = ((BigInteger)a).ToString();
                        byte[] raw = (byte[])text;
                        return raw.Length;
                    }
            """,
            """
                    return Run(a);
            """, null),



        ("string substring boundaries",
            """
                    private static int Run()
                    {
                        string text = "abc";
                        string empty = text.Substring(3);
                        string tail = text.Substring(2, 1);
                        return empty.Length * 100 + tail.Length * 10 + (tail == "c" ? 1 : 0);
                    }
            """,
            """
                    return Run();
            """, null),

        ("string split",
            """
                    private static int Run()
                    {
                        string[] parts = "a,b,c".Split(',');
                        return parts.Length * 100 + parts[0].Length + parts[2].Length;
                    }
            """,
            """
                    return Run();
            """, null),

        ("string pad left",
            """
                    private static int Run()
                    {
                        string padded = "7".PadLeft(4, '0');
                        return padded.Length * 10 + (padded == "0007" ? 1 : 0);
                    }
            """,
            """
                    return Run();
            """, null),

        ("string comparison",
            """
                    private static int Run()
                    {
                        int total = 0;
                        if (string.Compare("a", "b") < 0) total = total + 1;
                        if (string.Compare("b", "a") > 0) total = total + 10;
                        if (string.Compare("a", "a") == 0) total = total + 100;
                        return total;
                    }
            """,
            """
                    return Run();
            """, null),

        ("string starts ends with offsets",
            """
                    private static int Run()
                    {
                        string text = "neo-contract";
                        return (text.StartsWith("neo") ? 10 : 0) + (text.EndsWith("tract") ? 1 : 0);
                    }
            """,
            """
                    return Run();
            """, null),

        ("byte string length",
            """
                    private static int Run()
                    {
                        ByteString text = (ByteString)"abc";
                        return text.Length * 10 + (int)text[1];
                    }
            """,
            """
                    return Run();
            """, 128),

        ("two dimensional array sized",
            """
                    private static int Run(int a, int b)
                    {
                        var matrix = new int[2, 3];
                        matrix[0, 1] = a;
                        matrix[1, 2] = b;
                        return matrix[0, 1] + matrix[1, 2] + matrix[0, 0];
                    }
            """,
            """
                    return Run(a, b);
            """, null),

        ("two dimensional array initializer",
            """
                    private static int Run(int a)
                    {
                        var matrix = new int[,] { { 1, 2 }, { 3, 4 } };
                        return matrix[0, 0] + matrix[1, 1] + a;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("three dimensional array sized",
            """
                    private static int Run(int a)
                    {
                        var cube = new int[2, 2, 2];
                        cube[1, 1, 1] = a;
                        return cube[1, 1, 1] + cube[0, 0, 0];
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null conditional receiver chain",
            """
                    private sealed class NullNode { public int Value; public NullNode Child; }
                    private static int Next(ref int state) { return ++state; }
                    private static NullNode Pick(NullNode value, ref int state) { state++; return value; }
                    private static int Run(int a)
                    {
                        var root = a == 0 ? null : new NullNode { Value = a, Child = a == 1 ? null : new NullNode { Value = a + 10 } };
                        int calls = 0;
                        int value = Pick(root, ref calls)?.Child?.Value ?? Next(ref calls);
                        return value * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null conditional element chain",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Index(ref int state) { state++; return 0; }
                    private static int Run(int a)
                    {
                        int[][] rows = a == 0 ? null : new[] { new[] { a, a + 1 } };
                        int calls = 0;
                        int value = rows?[Index(ref calls)]?[1] ?? Next(ref calls);
                        return value * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null conditional delegate arguments",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Run(int a)
                    {
                        Func<int, int, int> f = a == 0 ? null : (left, right) => left * 10 + right + a;
                        int calls = 0;
                        int value = f?.Invoke(Next(ref calls), Next(ref calls)) ?? Next(ref calls);
                        return value * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null coalesce assignment targets",
            """
                    private sealed class NullNode
                    {
                        public string Text;
                        public string TextProperty { get; set; }
                    }
                    private static int Index(ref int state) { state++; return 0; }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run(int a)
                    {
                        string local = a == 0 ? null : "Local";
                        string[] values = { a == 1 ? null : "Array", "unused" };
                        var box = new NullNode { Text = a == 2 ? null : "Field", TextProperty = a == 3 ? null : "Property" };
                        int calls = 0;
                        local ??= NextText(ref calls);
                        values[Index(ref calls)] ??= NextText(ref calls);
                        box.Text ??= NextText(ref calls);
                        box.TextProperty ??= NextText(ref calls);
                        return local.Length * 1000000 + values[0].Length * 100000 + box.Text.Length * 10000 + box.TextProperty.Length * 1000 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null coalesce instance property nonnull",
            """
                    private sealed class Box { public string Text { get; set; } }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run()
                    {
                        var box = new Box { Text = "Child" };
                        int calls = 0;
                        box.Text ??= NextText(ref calls);
                        return box.Text.Length * 100 + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("null coalesce instance property assignment",
            """
                    private sealed class Box { public string Text { get; set; } }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run()
                    {
                        var box = new Box();
                        int calls = 0;
                        box.Text ??= NextText(ref calls);
                        return box.Text.Length * 100 + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("null conditional field coalesce nonnull",
            """
                    private sealed class Box { public string Text; }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run()
                    {
                        var box = new Box { Text = "Child" };
                        int calls = 0;
                        box?.Text ??= NextText(ref calls);
                        return box.Text.Length * 100 + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("null conditional field coalesce assignment",
            """
                    private sealed class Box { public string Text; }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run()
                    {
                        var box = new Box();
                        int calls = 0;
                        box?.Text ??= NextText(ref calls);
                        return box.Text.Length * 100 + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("null conditional compound assignment",
            """
                    private sealed class NullNode { public int Value; public NullNode Child; }
                    private static int Next(ref int state) { return ++state; }
                    private static int Run(int a)
                    {
                        var root = a == 0 ? null : new NullNode { Child = a == 1 ? null : new NullNode { Value = a } };
                        int calls = 0;
                        root?.Child?.Value += Next(ref calls);
                        return (root?.Child?.Value ?? -1) * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("null conditional coalesce assignment",
            """
                    private sealed class NullNode { public string Text; public NullNode Child; }
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run(int a)
                    {
                        var root = a == 0 ? null : new NullNode { Child = a == 1 ? null : new NullNode { Text = a == 2 ? null : "Child" } };
                        int calls = 0;
                        root?.Child?.Text ??= NextText(ref calls);
                        string value = root?.Child?.Text ?? "Missing";
                        return value.Length * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("coalesce skips first ref use of local",
            """
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run()
                    {
                        string value = "Child";
                        int calls = 0;
                        return (value ?? NextText(ref calls)).Length * 100 + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("coalesce skips first ref use of parameter",
            """
                    private static string NextText(ref int state) { state++; return "X"; }
                    private static int Run(int calls)
                    {
                        string value = "Child";
                        return (value ?? NextText(ref calls)).Length * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("logical and skips first ref use",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Run()
                    {
                        int calls = 0;
                        bool value = false && Next(ref calls) > 0;
                        return (value ? 100 : 0) + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("logical or skips first ref use",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Run()
                    {
                        int calls = 0;
                        bool value = true || Next(ref calls) > 0;
                        return (value ? 100 : 0) + calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("ternary skips first ref use",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Run(int a)
                    {
                        int calls = 0;
                        int value = a == 0 ? Next(ref calls) : 7;
                        return value * 100 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("if skips first ref use",
            """
                    private static int Next(ref int state) { return ++state; }
                    private static int Run(int a)
                    {
                        int calls = 0;
                        if (a == 0) calls = Next(ref calls);
                        return 700 + calls;
                    }
            """,
            """
                    return Run(a);
            """, null),

        ("byref aliases share captured storage",
            """
                    private static void BumpTwice(ref int first, ref int second) { first++; second++; }
                    private static int Run()
                    {
                        int calls = 0;
                        BumpTwice(ref calls, ref calls);
                        return calls;
                    }
            """,
            """
                    return Run();
            """, null),

        ("byref aliases then distinct calls",
            """
                    private static void BumpTwice(ref int first, ref int second) { first++; second++; }
                    private static void SetTwice(out int first, out int second) { first = 1; second = first + 1; }
                    private static int Run()
                    {
                        int alias = 0;
                        int first = 0;
                        int second = 10;
                        BumpTwice(ref alias, ref alias);
                        SetTwice(out alias, out alias);
                        BumpTwice(ref first, ref second);
                        SetTwice(out first, out second);
                        return alias * 10000 + first * 100 + second;
                    }
            """,
            """
                    return Run();
            """, null),

        ("byref aliases through direct calls",
            """
                    private static void Bump(ref int state) { state++; }
                    private static void BumpTwice(ref int first, ref int second)
                    {
                        Bump(ref first);
                        Bump(ref second);
                    }
                    private static int Run()
                    {
                        int state = 0;
                        BumpTwice(ref state, ref state);
                        return state;
                    }
            """,
            """
                    return Run();
            """, null),

        ("byref aliases through recursive calls",
            """
                    private static int Recurse(ref int first, ref int second, int depth)
                    {
                        if (depth == 0)
                            return first * 10 + second;
                        first += 2;
                        second += 3;
                        return Recurse(ref first, ref first, depth - 1) + 1;
                    }
                    private static int Run()
                    {
                        int state = 1;
                        int result = Recurse(ref state, ref state, 2);
                        return result * 100 + state;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref aliases",
            """
                    private class Base
                    {
                        public virtual int BumpTwice(ref int first, ref int second)
                        {
                            first += 2;
                            second += 3;
                            return first * 10 + second;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int BumpTwice(ref int first, ref int second)
                        {
                            first += 5;
                            second += 7;
                            return first * 10 + second;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int state = 1;
                        int result = value.BumpTwice(ref state, ref state);
                        return result * 100 + state;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref aliases then distinct calls",
            """
                    private class Base
                    {
                        public virtual int BumpTwice(ref int first, ref int second)
                        {
                            first += 2;
                            second += 3;
                            return first * 10 + second;
                        }
                    }
                    private class Derived : Base
                    {
                        public override int BumpTwice(ref int first, ref int second)
                        {
                            first += 5;
                            second += 7;
                            return first * 10 + second;
                        }
                        public virtual int Extra() { return 9; }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int alias = 1;
                        int first = 1;
                        int second = 10;
                        int aliasResult = value.BumpTwice(ref alias, ref alias);
                        int distinctResult = value.BumpTwice(ref first, ref second);
                        return aliasResult * 1000000 + alias * 10000 + distinctResult * 1000 + first * 100 + second;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref alias writeback isolation",
            """
                    private class Base
                    {
                        public virtual int BumpTwice(ref int first, ref int second)
                        {
                            first += 2;
                            second += 3;
                            return first * 10 + second;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int BumpTwice(ref int first, ref int second)
                        {
                            first += 5;
                            second += 7;
                            return first * 10 + second;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int alias = 1;
                        int first = 1;
                        int second = 10;
                        value.BumpTwice(ref alias, ref alias);
                        value.BumpTwice(ref first, ref second);
                        return alias * 1000000 + first * 1000 + second;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref alias signatures",
            """
                    private class Base
                    {
                        public virtual int Mix(ref int first, ref int second, ref int third)
                        {
                            first += 2;
                            second += 3;
                            third += 5;
                            return first * 100 + second * 10 + third;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int Mix(ref int first, ref int second, ref int third)
                        {
                            first += 5;
                            second += 7;
                            third += 11;
                            return first * 100 + second * 10 + third;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int state = 1;
                        int other = 20;
                        int firstResult = value.Mix(ref state, ref state, ref other);
                        int secondResult = value.Mix(ref state, ref other, ref other);
                        return firstResult * 1000000 + secondResult * 1000 + state * 100 + other;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref aliases through base calls",
            """
                    private class Base
                    {
                        public virtual int BumpTwice(ref int first, ref int second)
                        {
                            first += 2;
                            second += 3;
                            return first * 10 + second;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int BumpTwice(ref int first, ref int second)
                        {
                            int result = base.BumpTwice(ref first, ref second);
                            first += 5;
                            second += 7;
                            return result * 100 + first * 10 + second;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int state = 1;
                        int result = value.BumpTwice(ref state, ref state);
                        return result * 100 + state;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual byref aliases through recursive calls",
            """
                    private class Base
                    {
                        public virtual int Recurse(ref int first, ref int second, int depth)
                        {
                            if (depth == 0)
                                return first * 10 + second;
                            first += 2;
                            second += 3;
                            return Recurse(ref first, ref first, depth - 1) + 1;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int Recurse(ref int first, ref int second, int depth)
                        {
                            if (depth == 0)
                                return first * 100 + second * 10 + first;
                            first += 5;
                            second += 7;
                            return base.Recurse(ref first, ref first, depth - 1) + first;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int state = 1;
                        int result = value.Recurse(ref state, ref state, 2);
                        return result * 100 + state;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual distinct byref arguments",
            """
                    private class Base
                    {
                        public virtual int BumpTwice(ref int first, ref int second)
                        {
                            first += 2;
                            second += 3;
                            return first * 10 + second;
                        }
                    }
                    private sealed class Derived : Base
                    {
                        public override int BumpTwice(ref int first, ref int second)
                        {
                            first += 5;
                            second += 7;
                            return first * 10 + second;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int first = 1;
                        int second = 10;
                        int result = value.BumpTwice(ref first, ref second);
                        return result * 1000 + first * 100 + second;
                    }
            """,
            """
                    return Run();
            """, null),

        ("virtual out arguments with multilevel base calls",
            """
                    private class Base
                    {
                        public virtual int SetValues(out int first, out int second)
                        {
                            first = 2;
                            second = 3;
                            return first * 10 + second;
                        }
                    }
                    private class Middle : Base
                    {
                        public override int SetValues(out int first, out int second)
                        {
                            int result = base.SetValues(out first, out second);
                            first += 5;
                            second += 7;
                            return result * 100 + first * 10 + second;
                        }
                    }
                    private sealed class Derived : Middle
                    {
                        public override int SetValues(out int first, out int second)
                        {
                            int result = base.SetValues(out first, out second);
                            first += 11;
                            second += 13;
                            return result * 100 + first * 10 + second;
                        }
                    }
                    private static int Run()
                    {
                        Base value = new Derived();
                        int first = 0;
                        int second = 0;
                        int result = value.SetValues(out first, out second);
                        return result * 1000 + first * 100 + second;
                    }
            """,
            """
                    return Run();
            """, null),

        // The native crypto contracts have no CLR equivalent inside a contract; the expected
        // values are SHA256(01 02 03) and RIPEMD160(SHA256(01 02 03)).
        ("native sha256",
            """
                    private static int Run()
                    {
                        ByteString hash = CryptoLib.Sha256("abc");
                        return hash.Length * 1000 + (int)hash[0];
                    }
            """,
            """
                    return Run();
            """, 32186),

        ("native ripemd160 over sha256",
            """
                    private static int Run()
                    {
                        ByteString sha = CryptoLib.Sha256("abc");
                        ByteString hash = CryptoLib.Ripemd160(sha);
                        return hash.Length * 1000 + (int)hash[0];
                    }
            """,
            """
                    return Run();
            """, 20187),
    ];

    private static readonly CompilationOptions.OptimizationType[] Levels =
    [
        CompilationOptions.OptimizationType.None,
        CompilationOptions.OptimizationType.Basic,
        CompilationOptions.OptimizationType.Experimental,
        CompilationOptions.OptimizationType.All,
    ];

    /// <summary>
    /// Compiles one shape at a given optimization level and executes it.
    /// </summary>
    private static (bool Compiled, string Result) RunShape(string source, CompilationOptions.OptimizationType level)
    {
        using var harness = new CompilerHarness(new CompilationOptions { Optimize = level });
        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            if (context is not null)
                foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                    Console.Error.WriteLine($"  {diagnostic.Id}: {diagnostic.GetMessage()}");
            if (finding is not null)
                Console.Error.WriteLine($"  {finding.Detail}\n{finding.StackTrace}");
            return (false, string.Empty);
        }

        if (level == CompilationOptions.OptimizationType.None &&
            Environment.GetEnvironmentVariable("NEOFUZZ_DUMP_CAPTURED") == "1")
        {
            var field = typeof(CompilationContext).GetField("_capturedStaticFields", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var slots = (System.Collections.IDictionary)field.GetValue(context)!;
            Console.Error.WriteLine("Captured static fields:");
            foreach (System.Collections.DictionaryEntry entry in slots)
                Console.Error.WriteLine($"  {entry.Value}: {entry.Key}");
        }

        (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
        using var analyzer = new BehaviorDifferentialAnalyzer();
        (bool ok, string? error, Neo.VM.Types.StackItem? value) =
            analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), "probe", [7, 3]);

        return (true, ok ? BehaviorDifferentialAnalyzer.Render(value!) : "THROW");
    }

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of shapes where the VM disagrees with the CLR.</returns>
    public static int Run()
    {
        int mismatches = 0;
        int skipped = 0;
        string? filter = Environment.GetEnvironmentVariable("NEOFUZZ_SEMANTIC_FILTER");
        var cases = string.IsNullOrWhiteSpace(filter)
            ? Cases
            : Cases.Where(c => c.Label.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToArray();

        foreach ((string label, string declarations, string body, int? expected) in cases)
        {
            string contractSource = $$"""
                using System;
                using System.Numerics;
                using System.Collections.Generic;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Services;
                using Neo.SmartContract.Framework.Native;

                public class SemanticMatrixContract : SmartContract
                {
                {{declarations}}

                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            string referenceSource = $$"""
                using System;
                using System.Numerics;
                using System.Collections.Generic;

                public static class SemanticMatrixReference
                {
                {{declarations}}

                    public static int Probe(int a, int b)
                    {
                {{body}}
                    }
                }
                """;

            string? clrResult;
            if (expected is null)
            {
                using var oracle = new CrossLanguageOracle();
                Assembly? reference = oracle.TryCompileReference(referenceSource);
                if (reference is null)
                {
                    Console.WriteLine($"  {label,-46} => CLR rejected, skipped");
                    skipped++;
                    continue;
                }

                (bool okRef, object? refValue, _) =
                    oracle.TryInvoke(reference, "SemanticMatrixReference", "Probe", [7, 3]);
                clrResult = okRef ? refValue?.ToString() ?? "<null>" : "THROW";
            }
            else
            {
                clrResult = expected.Value.ToString();
            }

            bool agrees = true;
            var perLevel = new List<string>();

            foreach (CompilationOptions.OptimizationType level in Levels)
            {
                (bool compiled, string result) = RunShape(contractSource, level);
                if (!compiled)
                {
                    Console.WriteLine($"  {label,-46} => REJECTED at {level}");
                    agrees = false;
                    break;
                }

                perLevel.Add($"{level}:{result}");

                if (!string.Equals(clrResult, result, StringComparison.Ordinal))
                {
                    Console.WriteLine($"  {label,-46} => VM[{level}] {result,-30} C# {clrResult,-10} MISMATCH");
                    agrees = false;
                }
            }

            if (perLevel.Count > 0)
            {
                Console.WriteLine($"  {label,-46} => {string.Join(" ", perLevel),-58} C# {clrResult,-10} " +
                                  $"{(agrees ? "match" : "MISMATCH")}");
            }

            if (!agrees)
                mismatches++;
        }

        Console.WriteLine();
        Console.WriteLine(mismatches == 0
            ? $"SEMANTIC MATRIX PASSED: {cases.Length - skipped} shapes match, {skipped} skipped."
            : $"SEMANTIC MATRIX: {mismatches} shape(s) disagree with the CLR.");

        return mismatches;
    }

    private static string Trim(string? message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;
        const int limit = 60;
        return message.Length <= limit ? message : message[..limit] + "...";
    }
}
