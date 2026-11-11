// Copyright (C) 2015-2026 The Neo Project.
//
// ExpressionGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Text;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// The value types the expression generator works with.
/// </summary>
/// <remarks>
/// Narrow types are deliberately absent: C# promotes <c>byte</c>, <c>short</c>,
/// <c>char</c>, and their unsigned counterparts to <c>int</c> before any arithmetic, so
/// generating arithmetic over them would produce expressions whose type does not match
/// the declared one. They still appear, but only through an explicit cast.
/// </remarks>
public enum FuzzType
{
    /// <summary>32-bit signed integer.</summary>
    Int,

    /// <summary>64-bit signed integer.</summary>
    Long,

    /// <summary>Arbitrary-precision integer.</summary>
    BigInt,

    /// <summary>Boolean.</summary>
    Bool,

    /// <summary>UTF-8 string.</summary>
    Str,

    /// <summary>Byte array.</summary>
    Bytes,

    /// <summary>32-bit signed integer array.</summary>
    IntArray,

    /// <summary>8-bit unsigned integer, used as a narrowing target.</summary>
    Byte,

    /// <summary>8-bit signed integer, used as a narrowing target.</summary>
    SByte,

    /// <summary>16-bit signed integer, used as a narrowing target.</summary>
    Short,

    /// <summary>16-bit unsigned integer, used as a narrowing target.</summary>
    UShort,

    /// <summary>32-bit unsigned integer, used as a narrowing target.</summary>
    UInt,

    /// <summary>64-bit unsigned integer, used as a narrowing target.</summary>
    ULong,

    /// <summary>UTF-16 code unit, used as a narrowing target.</summary>
    Char,
}

/// <summary>
/// A node of the generated expression tree.
/// </summary>
/// <remarks>
/// The tree is rendered into text only at the end. Keeping it as a tree means the
/// contract version and the plain C# reference are rendered from the same structure,
/// so the two can never drift apart.
/// </remarks>
public abstract record ExprNode(FuzzType Type);

/// <summary>A literal value.</summary>
public sealed record LiteralNode(FuzzType ValueType, string Text) : ExprNode(ValueType);

/// <summary>A reference to a method parameter.</summary>
public sealed record VariableNode(FuzzType ValueType, string Name) : ExprNode(ValueType);

/// <summary>A binary operation.</summary>
public sealed record BinaryNode(FuzzType ResultType, ExprNode Left, string Operator, ExprNode Right)
    : ExprNode(ResultType);

/// <summary>A unary operation.</summary>
public sealed record UnaryNode(FuzzType ResultType, string Operator, ExprNode Operand)
    : ExprNode(ResultType);

/// <summary>A cast, optionally checked.</summary>
public sealed record CastNode(FuzzType ResultType, ExprNode Operand, bool Checked)
    : ExprNode(ResultType);

/// <summary>A conditional expression.</summary>
public sealed record ConditionalNode(FuzzType ResultType, ExprNode Condition, ExprNode WhenTrue, ExprNode WhenFalse)
    : ExprNode(ResultType);

/// <summary>A static method call.</summary>
public sealed record CallNode(FuzzType ResultType, string Callee, IReadOnlyList<ExprNode> Arguments)
    : ExprNode(ResultType);

/// <summary>A member access such as <c>.Length</c>.</summary>
public sealed record MemberNode(FuzzType ResultType, ExprNode Target, string Member)
    : ExprNode(ResultType);

/// <summary>An element access.</summary>
public sealed record IndexNode(FuzzType ResultType, ExprNode Target, ExprNode Index)
    : ExprNode(ResultType);

/// <summary>
/// Generates random, type-correct C# expressions.
/// </summary>
/// <remarks>
/// <para>
/// The repository's earlier generators produced a fixed shape with randomised constants:
/// a handful of templates with a number swapped in. That explores almost nothing, because
/// every run generates the same program structure.
/// </para>
/// <para>
/// This generator instead walks a typed grammar: every node carries the type it produces,
/// and the operator chosen for a node is one that is legal for that type. The result is a
/// genuinely different expression tree on every call, while still being valid C# that
/// reaches code generation rather than being rejected by the front end.
/// </para>
/// <para>
/// Two rules keep the generated programs from being pathological:
/// </para>
/// <list type="bullet">
/// <item><description>Division and remainder always use a divisor that cannot be zero,
/// so the program does not immediately fault.</description></item>
/// <item><description>Only the system methods the compiler actually supports are called,
/// so the input is not rejected with <c>NC1002 Unknown method</c>.</description></item>
/// </list>
/// </remarks>
public sealed class ExpressionGenerator
{
    /// <summary>The parameters available to generated expressions.</summary>
    public static readonly (FuzzType Type, string Name, string Declaration)[] Parameters =
    [
        (FuzzType.Int, "a", "int a"),
        (FuzzType.Int, "b", "int b"),
        (FuzzType.Long, "l", "long l"),
        (FuzzType.Long, "m", "long m"),
        (FuzzType.BigInt, "big", "BigInteger big"),
        (FuzzType.Bool, "flag", "bool flag"),
        (FuzzType.Str, "text", "string text"),
        (FuzzType.IntArray, "values", "int[] values"),
    ];

    private static readonly string[] IntLiterals =
    [
        "0", "1", "2", "3", "7", "10", "255", "256", "65535", "65536",
        "-1", "-2", "-128", "-32768", "2147483647", "-2147483648",
    ];

    private static readonly string[] LongLiterals =
    [
        "0L", "1L", "2L", "7L", "255L", "4294967296L", "-1L", "-2147483649L",
        "9223372036854775807L", "-9223372036854775808L",
    ];

    private static readonly string[] StringLiterals =
    [
        "\"\"", "\"a\"", "\"fuzz\"", "\"neo\"", "\"\\u00e9\"", "\"\\u4f60\\u597d\"",
    ];

    private readonly Random _random;

    /// <summary>
    /// Creates a generator with an explicit seed.
    /// </summary>
    /// <param name="seed">Seed for the random number generator.</param>
    public ExpressionGenerator(int seed)
    {
        _random = new Random(seed);
    }

    /// <summary>
    /// Generates a random expression of the requested type.
    /// </summary>
    /// <param name="target">The type the expression must produce.</param>
    /// <param name="depth">Remaining nesting depth.</param>
    /// <returns>The generated expression tree.</returns>
    public ExprNode Generate(FuzzType target, int depth)
    {
        if (depth <= 0)
        {
            return Leaf(target);
        }

        return target switch
        {
            FuzzType.Int => GenerateInt(depth),
            FuzzType.Long => GenerateLong(depth),
            FuzzType.BigInt => GenerateBigInteger(depth),
            FuzzType.Bool => GenerateBool(depth),
            FuzzType.Str => GenerateString(depth),
            FuzzType.Bytes => GenerateBytes(depth),
            FuzzType.IntArray => Leaf(FuzzType.IntArray),
            _ => Leaf(target),
        };
    }

    private ExprNode GenerateInt(int depth)
    {
        return _random.Next(14) switch
        {
            0 => Leaf(FuzzType.Int),
            1 => new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), Pick("+", "-"),
                Generate(FuzzType.Int, depth - 1)),
            2 => new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), Pick("*"),
                Generate(FuzzType.Int, depth - 1)),
            3 => new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), Pick("&", "|", "^"),
                Generate(FuzzType.Int, depth - 1)),
            4 => new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), Pick("/", "%"),
                NonZero(FuzzType.Int, depth - 1)),
            5 => new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1),
                Pick("<<", ">>"), new LiteralNode(FuzzType.Int, _random.Next(0, 32).ToString())),
            6 => new CastNode(FuzzType.Int, Generate(FuzzType.Long, depth - 1), _random.Next(2) == 0),
            7 => new ConditionalNode(FuzzType.Int, Generate(FuzzType.Bool, depth - 1),
                Generate(FuzzType.Int, depth - 1), Generate(FuzzType.Int, depth - 1)),
            8 => new UnaryNode(FuzzType.Int, Pick("-", "~"), Generate(FuzzType.Int, depth - 1)),
            9 => new CallNode(FuzzType.Int, Pick("Math.Abs", "Math.Sign"),
                [Generate(FuzzType.Int, depth - 1)]),
            10 => new CallNode(FuzzType.Int, Pick("Math.Max", "Math.Min"),
                [Generate(FuzzType.Int, depth - 1), Generate(FuzzType.Int, depth - 1)]),
            11 => NarrowingRoundTrip(depth),
            12 => new MemberNode(FuzzType.Int, Generate(FuzzType.IntArray, depth - 1), "Length"),
            13 => new IndexNode(FuzzType.Int, Generate(FuzzType.IntArray, depth - 1),
                new BinaryNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), "&",
                    new LiteralNode(FuzzType.Int, "1"))),
            _ => Leaf(FuzzType.Int),
        };
    }

    /// <summary>
    /// Builds a value that is narrowed to a smaller width and widened back.
    /// </summary>
    /// <param name="depth">Remaining nesting depth.</param>
    /// <returns>An expression whose truncation and sign-extension semantics matter.</returns>
    /// <remarks>
    /// Truncation and sign extension are a rich source of defects: the VM computes in
    /// arbitrary precision, so the compiler has to insert the explicit conversion that
    /// reproduces the C# width. Narrowing and then widening makes that conversion
    /// observable in the result.
    /// </remarks>
    private ExprNode NarrowingRoundTrip(int depth)
    {
        FuzzType[] narrow =
        [
            FuzzType.Byte, FuzzType.SByte, FuzzType.Short,
            FuzzType.UShort, FuzzType.Char, FuzzType.UInt,
        ];

        FuzzType target = narrow[_random.Next(narrow.Length)];
        bool isChecked = _random.Next(3) == 0;

        return new CastNode(
            FuzzType.Int,
            new CastNode(target, Generate(FuzzType.Int, depth - 1), isChecked),
            Checked: false);
    }

    private ExprNode GenerateLong(int depth)
    {
        return _random.Next(10) switch
        {
            0 => Leaf(FuzzType.Long),
            1 => new BinaryNode(FuzzType.Long, Generate(FuzzType.Long, depth - 1), Pick("+", "-"),
                Generate(FuzzType.Long, depth - 1)),
            2 => new BinaryNode(FuzzType.Long, Generate(FuzzType.Long, depth - 1), Pick("*"),
                Generate(FuzzType.Long, depth - 1)),
            3 => new BinaryNode(FuzzType.Long, Generate(FuzzType.Long, depth - 1), Pick("&", "|", "^"),
                Generate(FuzzType.Long, depth - 1)),
            4 => new BinaryNode(FuzzType.Long, Generate(FuzzType.Long, depth - 1), Pick("/", "%"),
                NonZero(FuzzType.Long, depth - 1)),
            5 => new BinaryNode(FuzzType.Long, Generate(FuzzType.Long, depth - 1),
                Pick("<<", ">>"), new LiteralNode(FuzzType.Int, _random.Next(0, 64).ToString())),
            6 => new CastNode(FuzzType.Long, Generate(FuzzType.Int, depth - 1), _random.Next(2) == 0),
            7 => new ConditionalNode(FuzzType.Long, Generate(FuzzType.Bool, depth - 1),
                Generate(FuzzType.Long, depth - 1), Generate(FuzzType.Long, depth - 1)),
            8 => new UnaryNode(FuzzType.Long, Pick("-", "~"), Generate(FuzzType.Long, depth - 1)),
            _ => new CastNode(FuzzType.Long, Generate(FuzzType.BigInt, depth - 1), _random.Next(2) == 0),
        };
    }

    private ExprNode GenerateBigInteger(int depth)
    {
        return _random.Next(8) switch
        {
            0 => Leaf(FuzzType.BigInt),
            1 => new CastNode(FuzzType.BigInt, Generate(FuzzType.Int, depth - 1), Checked: false),
            2 => new CallNode(FuzzType.BigInt, Pick("BigInteger.Add", "BigInteger.Subtract",
                "BigInteger.Multiply", "BigInteger.Max", "BigInteger.Min"),
                [Generate(FuzzType.BigInt, depth - 1), Generate(FuzzType.BigInt, depth - 1)]),
            3 => new CallNode(FuzzType.BigInt, Pick("BigInteger.Divide", "BigInteger.Remainder"),
                [Generate(FuzzType.BigInt, depth - 1), NonZero(FuzzType.BigInt, depth - 1)]),
            4 => new CallNode(FuzzType.BigInt, Pick("BigInteger.Abs", "BigInteger.Negate"),
                [Generate(FuzzType.BigInt, depth - 1)]),
            // The base is a small constant on purpose: raising a parameter to a high power
            // exceeds the VM's 32-byte integer limit on every input, which would make the
            // whole method unusable without adding any coverage.
            5 => new CallNode(FuzzType.BigInt, "BigInteger.Pow",
                [new LiteralNode(FuzzType.BigInt, _random.Next(2, 5).ToString()),
                 new LiteralNode(FuzzType.Int, _random.Next(0, 4).ToString())]),
            6 => new CastNode(FuzzType.BigInt, Generate(FuzzType.Long, depth - 1), Checked: false),
            _ => Leaf(FuzzType.BigInt),
        };
    }

    private ExprNode GenerateBool(int depth)
    {
        return _random.Next(8) switch
        {
            0 => Leaf(FuzzType.Bool),
            1 => new BinaryNode(FuzzType.Bool, Generate(FuzzType.Int, depth - 1),
                Pick("<", ">", "<=", ">=", "==", "!="), Generate(FuzzType.Int, depth - 1)),
            2 => new BinaryNode(FuzzType.Bool, Generate(FuzzType.Long, depth - 1),
                Pick("<", ">", "==", "!="), Generate(FuzzType.Long, depth - 1)),
            3 => new BinaryNode(FuzzType.Bool, Generate(FuzzType.BigInt, depth - 1),
                Pick("<", ">", "==", "!="), Generate(FuzzType.BigInt, depth - 1)),
            4 => new BinaryNode(FuzzType.Bool, Generate(FuzzType.Bool, depth - 1), Pick("&&", "||"),
                Generate(FuzzType.Bool, depth - 1)),
            5 => new UnaryNode(FuzzType.Bool, "!", Generate(FuzzType.Bool, depth - 1)),
            6 => new BinaryNode(FuzzType.Bool, Generate(FuzzType.Str, depth - 1),
                Pick("==", "!="), Generate(FuzzType.Str, depth - 1)),
            _ => new CallNode(FuzzType.Bool, Pick("char.IsDigit", "char.IsLetter", "char.IsWhiteSpace"),
                [new CastNode(FuzzType.Int, Generate(FuzzType.Int, depth - 1), Checked: false)]),
        };
    }

    private ExprNode GenerateString(int depth)
    {
        return _random.Next(5) switch
        {
            0 => Leaf(FuzzType.Str),
            1 => new VariableNode(FuzzType.Str, "text"),
            2 => new BinaryNode(FuzzType.Str, Generate(FuzzType.Str, depth - 1), "+",
                Generate(FuzzType.Str, depth - 1)),
            // Length is an int, not a string; using it here produced a body whose type did
            // not match the declared return type.
            3 => new CallNode(FuzzType.Str, "text.ToUpper", []),
            _ => Leaf(FuzzType.Str),
        };
    }

    private ExprNode GenerateBytes(int depth)
    {
        // The compiler does not support Array.Copy or the generic Array.Reverse, and it
        // does not support conversions between byte[] and the framework's ByteString in
        // this position, so byte arrays are only read and indexed.
        return Leaf(FuzzType.Bytes);
    }

    /// <summary>
    /// Produces a leaf expression: a literal, a parameter, or a zero-argument member.
    /// </summary>
    private ExprNode Leaf(FuzzType target) => target switch
    {
        FuzzType.Int => _random.Next(3) == 0
            ? new VariableNode(FuzzType.Int, _random.Next(2) == 0 ? "a" : "b")
            : new LiteralNode(FuzzType.Int, Pick(IntLiterals)),
        FuzzType.Long => _random.Next(3) == 0
            ? new VariableNode(FuzzType.Long, _random.Next(2) == 0 ? "l" : "m")
            : new LiteralNode(FuzzType.Long, Pick(LongLiterals)),
        FuzzType.BigInt => _random.Next(2) == 0
            ? new VariableNode(FuzzType.BigInt, "big")
            : new LiteralNode(FuzzType.BigInt, Pick(IntLiterals)),
        FuzzType.Bool => new LiteralNode(FuzzType.Bool, _random.Next(2) == 0 ? "true" : "false"),
        FuzzType.Str => new LiteralNode(FuzzType.Str, Pick(StringLiterals)),
        FuzzType.Bytes => new VariableNode(FuzzType.Bytes, "payload"),
        _ => new VariableNode(FuzzType.IntArray, "values"),
    };

    /// <summary>
    /// Produces an expression that cannot evaluate to zero, for use as a divisor.
    /// </summary>
    /// <remarks>
    /// A literal divisor is used where possible. Otherwise the expression is combined
    /// with <c>| 1</c>, which cannot change a value into zero.
    /// </remarks>
    private ExprNode NonZero(FuzzType target, int depth)
    {
        if (_random.Next(2) == 0)
        {
            return new LiteralNode(target, target switch
            {
                FuzzType.Long => _random.Next(2) == 0 ? "3L" : "7L",
                FuzzType.BigInt => _random.Next(2) == 0 ? "3" : "7",
                _ => _random.Next(2) == 0 ? "3" : "7",
            });
        }

        return new BinaryNode(target, Generate(target, depth), "|",
            new LiteralNode(target, target == FuzzType.Long ? "1L" : "1"));
    }

    private string Pick(params string[] options) => options[_random.Next(options.Length)];

    private T Pick<T>(IReadOnlyList<T> options) => options[_random.Next(options.Count)];
}

/// <summary>
/// Renders an expression tree into C# source text.
/// </summary>
public static class ExpressionRenderer
{
    /// <summary>
    /// Renders an expression tree.
    /// </summary>
    /// <param name="node">The tree to render.</param>
    /// <returns>C# expression text.</returns>
    public static string Render(ExprNode node)
    {
        var sb = new StringBuilder();
        Render(node, sb);
        return sb.ToString();
    }

    private static void Render(ExprNode node, StringBuilder sb)
    {
        switch (node)
        {
            case LiteralNode literal:
                sb.Append(literal.Text);
                break;

            case VariableNode variable:
                sb.Append(variable.Name);
                break;

            case BinaryNode binary:
                sb.Append('(');
                Render(binary.Left, sb);
                sb.Append(' ').Append(binary.Operator).Append(' ');
                Render(binary.Right, sb);
                sb.Append(')');
                break;

            case UnaryNode unary:
                sb.Append('(').Append(unary.Operator);
                Render(unary.Operand, sb);
                sb.Append(')');
                break;

            case CastNode cast:
                // The operand is parenthesised as well: a cast applied to a negative literal,
                // `(T)-1`, is a syntax error in C# (CS0075) rather than a cast of -1.
                sb.Append(cast.Checked ? "checked(((" : "((");
                sb.Append(TypeName(cast.Type)).Append(")(");
                Render(cast.Operand, sb);
                sb.Append("))");
                if (cast.Checked)
                {
                    sb.Append(')');
                }
                break;

            case ConditionalNode conditional:
                sb.Append('(');
                Render(conditional.Condition, sb);
                sb.Append(" ? ");
                Render(conditional.WhenTrue, sb);
                sb.Append(" : ");
                Render(conditional.WhenFalse, sb);
                sb.Append(')');
                break;

            case CallNode call:
                sb.Append(call.Callee).Append('(');
                for (int i = 0; i < call.Arguments.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    Render(call.Arguments[i], sb);
                }
                sb.Append(')');
                break;

            case MemberNode member:
                Render(member.Target, sb);
                sb.Append('.').Append(member.Member);
                break;

            case IndexNode index:
                Render(index.Target, sb);
                sb.Append('[');
                Render(index.Index, sb);
                sb.Append(']');
                break;

            default:
                throw new InvalidOperationException($"Unsupported expression node '{node.GetType().Name}'.");
        }
    }

    /// <summary>
    /// Maps a fuzz type to its C# spelling.
    /// </summary>
    /// <param name="type">The type to map.</param>
    /// <returns>The C# type name.</returns>
    public static string TypeName(FuzzType type) => type switch
    {
        FuzzType.Int => "int",
        FuzzType.Long => "long",
        FuzzType.BigInt => "BigInteger",
        FuzzType.Bool => "bool",
        FuzzType.Str => "string",
        FuzzType.Bytes => "byte[]",
        FuzzType.IntArray => "int[]",
        FuzzType.Byte => "byte",
        FuzzType.SByte => "sbyte",
        FuzzType.Short => "short",
        FuzzType.UShort => "ushort",
        FuzzType.UInt => "uint",
        FuzzType.ULong => "ulong",
        FuzzType.Char => "char",
        _ => "int",
    };

    /// <summary>
    /// Counts the nodes of an expression tree.
    /// </summary>
    /// <param name="node">The tree to measure.</param>
    /// <returns>The number of nodes.</returns>
    public static int Size(ExprNode node) => node switch
    {
        BinaryNode binary => 1 + Size(binary.Left) + Size(binary.Right),
        UnaryNode unary => 1 + Size(unary.Operand),
        CastNode cast => 1 + Size(cast.Operand),
        ConditionalNode conditional => 1 + Size(conditional.Condition) + Size(conditional.WhenTrue) + Size(conditional.WhenFalse),
        CallNode call => 1 + call.Arguments.Sum(Size),
        MemberNode member => 1 + Size(member.Target),
        IndexNode index => 1 + Size(index.Target) + Size(index.Index),
        _ => 1,
    };
}
