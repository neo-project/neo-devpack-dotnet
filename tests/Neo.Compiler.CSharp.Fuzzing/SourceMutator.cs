// Copyright (C) 2015-2026 The Neo Project.
//
// SourceMutator.cs file belongs to the neo project and is free
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
/// Applies structural and textual mutations to an existing source file.
/// </summary>
/// <remarks>
/// Generation explores the space of well-formed programs; mutation explores the space
/// of near-miss programs that sit just outside the boundaries the generator respects
/// (overflow edge cases, unusual operator combinations, boundary constants). Both are
/// needed: generation gives breadth, mutation gives depth around known-tricky constructs.
/// </remarks>
public sealed class SourceMutator
{
    /// <summary>
    /// Mutation strategy identifiers, shared by the text and syntax-aware passes.
    /// </summary>
    public const string KindReplaceConstant = "replace-constant";
    public const string KindInsertOperator = "insert-operator";
    public const string KindDuplicateStatement = "duplicate-statement";
    public const string KindNegateCondition = "negate-condition";
    public const string KindSwapOperators = "swap-operators";
    public const string KindTruncateExpression = "truncate-expression";
    public const string KindBoundaryConstant = "boundary-constant";
    public const string KindZeroDivisor = "zero-divisor";
    public const string KindReplaceAllConstants = "replace-all-constants";
    public const string KindSyntaxLiteral = "syntax-literal";
    public const string KindSyntaxBinary = "syntax-binary";
    public const string KindSyntaxNegate = "syntax-negate";
    public const string KindSyntaxDeleteStatement = "syntax-delete-statement";

    private static readonly string[] InterestingNumbers =
    [
        "0", "1", "-1", "2", "-2", "int.MaxValue", "int.MinValue",
        "long.MaxValue", "long.MinValue", "byte.MaxValue", "short.MinValue",
        "255", "256", "65535", "65536", "-128", "127", "2147483647", "-2147483648",
        "9223372036854775807", "-9223372036854775808",
    ];

    private static readonly string[] InterestingTokens =
    [
        "checked", "unchecked", "stackalloc", "sizeof", "default", "typeof",
        "as", "is", "=>", "??=", "?.", "++", "--", "<<=", ">>=", ">>>",
    ];

    private readonly Random _random;
    private readonly SyntaxAwareMutator _syntaxAware;

    /// <summary>
    /// The mutation kinds applied by the last <see cref="Mutate"/> call, in order.
    /// </summary>
    public IReadOnlyList<string> LastKinds { get; private set; } = [];

    private readonly Dictionary<string, int> _attempts = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> _hits = new(StringComparer.Ordinal);

    /// <summary>
    /// Records that a mutation kind produced new coverage, which raises the weight the
    /// kind receives in future inputs (the libFuzzer feedback loop).
    /// </summary>
    /// <param name="kind">The mutation kind identifier.</param>
    public void Reward(string kind)
    {
        _hits[kind] = _hits.GetValueOrDefault(kind) + 1;
    }

    /// <summary>
    /// Picks a kind with probability proportional to 1 + rewarded hits, falling back to
    /// uniform for kinds that have never been rewarded.
    /// </summary>
    /// <param name="kinds">Candidate kinds.</param>
    /// <returns>The selected kind.</returns>
    public string WeightedKind(params string[] kinds)
    {
        long total = 0;
        var weights = new long[kinds.Length];
        for (int i = 0; i < kinds.Length; i++)
        {
            weights[i] = 1 + _hits.GetValueOrDefault(kinds[i]);
            total += weights[i];
        }
        long roll = (_random.Next() % total);
        for (int i = 0; i < kinds.Length; i++)
        {
            roll -= weights[i];
            if (roll < 0)
                return kinds[i];
        }
        return kinds[^1];
    }

    /// <summary>
    /// Creates a mutator with an explicit seed.
    /// </summary>
    /// <param name="seed">Seed for the random number generator.</param>
    public SourceMutator(int seed)
    {
        _random = new Random(seed);
        _syntaxAware = new SyntaxAwareMutator(_random.Next());
    }

    /// <summary>
    /// Applies a random mutation to the source.
    /// </summary>
    /// <param name="source">The source to mutate.</param>
    /// <returns>The mutated source.</returns>
    public string Mutate(string source)
    {
        // Stacked mutations: two to four independent edits per input, with the edit kind
        // chosen by the coverage-feedback weights (kinds that recently produced new
        // coverage are selected more often). Half the inputs start with a Roslyn-based
        // structural edit, which always keeps the program parseable.
        var kinds = new List<string>();
        string current = source;

        if (_random.Next(2) == 0)
        {
            string syntaxKind = _syntaxAware.RandomKind();
            string? edited = _syntaxAware.Apply(syntaxKind, current);
            if (edited is not null)
            {
                current = edited;
                kinds.Add(syntaxKind);
            }
        }

        int passes = 2 + _random.Next(3);
        for (int i = 0; i < passes; i++)
        {
            string kind = WeightedKind(
                KindReplaceConstant, KindInsertOperator, KindDuplicateStatement,
                KindNegateCondition, KindSwapOperators, KindTruncateExpression,
                KindBoundaryConstant, KindZeroDivisor, KindReplaceAllConstants);
            current = kind switch
            {
                KindReplaceConstant => ReplaceConstant(current),
                KindInsertOperator => InsertOperator(current),
                KindDuplicateStatement => DuplicateStatement(current),
                KindNegateCondition => NegateCondition(current),
                KindSwapOperators => SwapOperators(current),
                KindTruncateExpression => TruncateExpression(current),
                KindBoundaryConstant => InsertBoundaryConstant(current),
                KindZeroDivisor => ZeroDivisor(current),
                _ => ReplaceAllConstants(current),
            };
            kinds.Add(kind);
            _attempts[kind] = _attempts.GetValueOrDefault(kind) + 1;
        }

        LastKinds = kinds;
        return current;
    }

    /// <summary>
    /// Records coverage hits for the kinds of the last mutation.
    /// </summary>
    /// <param name="hit">Delegate deciding whether a given kind produced new coverage.</param>
    public void RewardKinds(Func<string, bool> hit)
    {
        foreach (string kind in LastKinds)
        {
            if (hit(kind))
                Reward(kind);
        }
    }


    /// <summary>
    /// Replaces the right operand of the first division or remainder with zero.
    /// </summary>
    /// <param name="source">The source to mutate.</param>
    /// <returns>The mutated source, or the input when no division is present.</returns>
    /// <remarks>
    /// The catchable-divide-by-zero semantic gap (#2212/#2217) and the catch-in-loop
    /// shape both needed the divisor to actually be zero at runtime; generation rarely
    /// produces that, so mutation targets it directly.
    /// </remarks>
    private string ZeroDivisor(string source)
    {
        foreach (string op in new[] { "/", "%" })
        {
            int searchFrom = 0;
            while (true)
            {
                int index = source.IndexOf(op, searchFrom, StringComparison.Ordinal);
                if (index < 0)
                {
                    return source;
                }
                // Skip compound assignments and identifiers containing the operator char.
                if (index > 0 && (source[index - 1] is '/' or '<' or '>' or '=' or '!' || char.IsLetter(source[index - 1])))
                {
                    searchFrom = index + 1;
                    continue;
                }
                int operandStart = index + op.Length;
                while (operandStart < source.Length && source[operandStart] == ' ')
                {
                    operandStart++;
                }
                int operandEnd = operandStart;
                while (operandEnd < source.Length && (char.IsLetterOrDigit(source[operandEnd]) || source[operandEnd] == '_'))
                {
                    operandEnd++;
                }
                if (operandEnd == operandStart)
                {
                    searchFrom = index + 1;
                    continue;
                }
                return source[..operandStart] + "0" + source[operandEnd..];
            }
        }
        return source;
    }

    /// <summary>
    /// Replaces every numeric literal in the source with one interesting boundary value.
    /// </summary>
    /// <param name="source">The source to mutate.</param>
    /// <returns>The mutated source, or the input when no literal is present.</returns>
    private string ReplaceAllConstants(string source)
    {
        string replacement = InterestingNumbers[_random.Next(InterestingNumbers.Length)];
        var sb = new System.Text.StringBuilder();
        bool replaced = false;
        foreach (string token in Tokenize(source))
        {
            if (IsNumericLiteral(token))
            {
                sb.Append(' ').Append(replacement).Append(' ');
                replaced = true;
            }
            else
            {
                sb.Append(token);
            }
        }
        return replaced ? sb.ToString() : source;
    }

    private string ReplaceConstant(string source)
    {
        string replacement = InterestingNumbers[_random.Next(InterestingNumbers.Length)];
        var sb = new StringBuilder();
        bool replaced = false;

        foreach (string token in Tokenize(source))
        {
            if (!replaced && IsNumericLiteral(token))
            {
                sb.Append(' ').Append(replacement).Append(' ');
                replaced = true;
            }
            else
            {
                sb.Append(token);
            }
        }

        return replaced ? sb.ToString() : source;
    }

    private string InsertOperator(string source)
    {
        string token = InterestingTokens[_random.Next(InterestingTokens.Length)];
        int index = FindReturnIndex(source);
        if (index < 0)
        {
            return source;
        }

        // Insert a cast around the returned expression to exercise conversion paths.
        int close = source.IndexOf(';', index);
        if (close < 0)
        {
            return source;
        }

        string expr = source[(index + "return ".Length)..close];
        return source[..index] + $"return ({token} {expr});" + source[(close + 1)..];
    }

    private string DuplicateStatement(string source)
    {
        var lines = source.Split('\n');
        var candidates = new List<int>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("return ", StringComparison.Ordinal) ||
                lines[i].Contains("+= ", StringComparison.Ordinal))
            {
                candidates.Add(i);
            }
        }

        if (candidates.Count == 0)
        {
            return source;
        }

        int pick = candidates[_random.Next(candidates.Count)];
        var result = new List<string>(lines);
        result.Insert(pick + 1, lines[pick]);
        return string.Join('\n', result);
    }

    private string NegateCondition(string source)
    {
        int index = source.IndexOf("if (", StringComparison.Ordinal);
        if (index < 0)
        {
            return source;
        }
        int open = source.IndexOf('(', index);
        int close = MatchParen(source, open);
        if (open < 0 || close < 0)
        {
            return source;
        }
        string cond = source[(open + 1)..close];
        return source[..(open + 1)] + "!" + "(" + cond + ")" + source[(close + 1)..];
    }

    private string SwapOperators(string source)
    {
        string[] pairs = ["+", "-", "*", "/", "%", "&", "|", "^", "<<", ">>"];
        foreach (string op in pairs)
        {
            int index = source.IndexOf(op, StringComparison.Ordinal);
            if (index < 0)
            {
                continue;
            }
            // Skip compound assignments and shifts are fine to swap with each other.
            if (index > 0 && source[index - 1] is '<' or '>' or '=' or '!')
            {
                continue;
            }
            string other = pairs[_random.Next(pairs.Length)];
            if (other == op)
            {
                continue;
            }
            return source[..index] + other + source[(index + op.Length)..];
        }
        return source;
    }

    private string TruncateExpression(string source)
    {
        int index = FindReturnIndex(source);
        if (index < 0)
        {
            return source;
        }
        int close = source.IndexOf(';', index);
        if (close < 0)
        {
            return source;
        }
        string expr = source[(index + "return ".Length)..close].Trim();
        int paren = expr.LastIndexOf(')');
        if (paren > 0 && expr.StartsWith('('))
        {
            expr = expr[1..paren];
        }
        return source[..index] + $"return {expr};" + source[(close + 1)..];
    }

    private string InsertBoundaryConstant(string source)
    {
        string constant = InterestingNumbers[_random.Next(InterestingNumbers.Length)];
        int index = FindReturnIndex(source);
        if (index < 0)
        {
            return source;
        }
        int close = source.IndexOf(';', index);
        if (close < 0)
        {
            return source;
        }
        string expr = source[(index + "return ".Length)..close];
        return source[..index] + $"return ({expr}) + ({constant});" + source[(close + 1)..];
    }

    private static int FindReturnIndex(string source) => source.IndexOf("return ", StringComparison.Ordinal);

    private static int MatchParen(string source, int open)
    {
        if (open < 0)
        {
            return -1;
        }
        int depth = 0;
        for (int i = open; i < source.Length; i++)
        {
            if (source[i] == '(') depth++;
            else if (source[i] == ')')
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private static bool IsNumericLiteral(string token)
    {
        if (token.Length == 0 || !char.IsDigit(token[0]) && token[0] != '-')
        {
            return false;
        }
        return token.All(c => char.IsDigit(c) || c == '-');
    }

    /// <summary>
    /// Splits a source into tokens while preserving whitespace, so mutations do not
    /// destroy the overall structure of the file.
    /// </summary>
    private static IEnumerable<string> Tokenize(string source)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < source.Length)
        {
            if (char.IsDigit(source[i]) && (i == 0 || !char.IsLetterOrDigit(source[i - 1])))
            {
                if (sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Clear();
                }
                int start = i;
                while (i < source.Length && char.IsDigit(source[i]))
                {
                    i++;
                }
                yield return source[start..i];
                continue;
            }
            sb.Append(source[i]);
            i++;
        }
        if (sb.Length > 0)
        {
            yield return sb.ToString();
        }
    }
}
