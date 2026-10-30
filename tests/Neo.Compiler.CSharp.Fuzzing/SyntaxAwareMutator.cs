// Copyright (C) 2015-2026 The Neo Project.
//
// SyntaxAwareMutator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Parses the source with Roslyn and applies structure-aware edits that text-level
/// regex mutation cannot express reliably: swapping binary operands, rewriting
/// operators in place, negating conditions, deleting statements, and replacing
/// numeric literals with boundary values.
/// </summary>
/// <remarks>
/// Text mutation often produces code that no longer parses, which the harness then
/// rejects; syntax-aware mutation always emits a parseable program, so every mutated
/// iteration actually reaches the compiler and the VM.
/// </remarks>
public sealed class SyntaxAwareMutator
{
    public const string KindSyntaxLiteral = "syntax-literal";
    public const string KindSyntaxBinary = "syntax-binary";
    public const string KindSyntaxNegate = "syntax-negate";
    public const string KindSyntaxDeleteStatement = "syntax-delete-statement";

    private static readonly string[] AllKinds =
        [KindSyntaxLiteral, KindSyntaxBinary, KindSyntaxNegate, KindSyntaxDeleteStatement];

    private static readonly string[] BoundaryLiterals =
    [
        "0", "1", "-1", "2", "-2", "int.MaxValue", "int.MinValue",
        "byte.MaxValue", "255", "256", "65535", "65536", "-128", "127",
    ];

    private readonly Random _random;

    public SyntaxAwareMutator(int seed) => _random = new Random(seed);

    /// <summary>Picks a random supported structural edit kind.</summary>
    /// <returns>The edit kind.</returns>
    public string RandomKind() => AllKinds[_random.Next(AllKinds.Length)];

    /// <summary>
    /// Applies one structural edit of the given kind.
    /// </summary>
    /// <param name="kind">The edit kind.</param>
    /// <param name="source">The source to edit.</param>
    /// <returns>The edited source, or null when the kind is unknown or the program admits no edit.</returns>
    public string? Apply(string kind, string source)
    {
        var root = Parse(source).GetRoot();
        var rewritten = kind switch
        {
            KindSyntaxLiteral => RewriteRandomLiteral(root),
            KindSyntaxBinary => RewriteRandomBinaryOperator(root),
            KindSyntaxNegate => NegateRandomIfCondition(root),
            KindSyntaxDeleteStatement => DeleteRandomStatement(root),
            _ => null,
        };
        return rewritten?.ToFullString();
    }

    private static SyntaxTree Parse(string source) =>
        CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));

    private SyntaxNode? RewriteRandomLiteral(SyntaxNode root)
    {
        var literals = root.DescendantNodes()
            .OfType<LiteralExpressionSyntax>()
            .Where(l => l.IsKind(SyntaxKind.NumericLiteralExpression))
            .ToList();
        if (literals.Count == 0)
            return null;
        var victim = literals[_random.Next(literals.Count)];
        var replacement = BoundaryLiterals[_random.Next(BoundaryLiterals.Length)];
        return root.ReplaceNode(victim, SyntaxFactory.ParseExpression(replacement).WithTriviaFrom(victim));
    }

    private SyntaxNode? RewriteRandomBinaryOperator(SyntaxNode root)
    {
        var binaries = root.DescendantNodes().OfType<BinaryExpressionSyntax>()
            .Where(b => BinaryOperators.Contains(b.OperatorToken.ValueText))
            .ToList();
        if (binaries.Count == 0)
            return null;
        var victim = binaries[_random.Next(binaries.Count)];
        // Swap operands half the time, otherwise substitute the operator.
        if (_random.Next(2) == 0)
        {
            return root.ReplaceNode(victim, victim.WithLeft(victim.Right).WithRight(victim.Left));
        }
        string op = BinaryOperators[_random.Next(BinaryOperators.Length)];
        var rewritten = victim.WithOperatorToken(SyntaxFactory.ParseToken($" {op} ").WithTriviaFrom(victim.OperatorToken));
        return root.ReplaceNode(victim, rewritten);
    }

    private SyntaxNode? NegateRandomIfCondition(SyntaxNode root)
    {
        var conditions = root.DescendantNodes()
            .OfType<IfStatementSyntax>()
            .Select(i => i.Condition)
            .ToList();
        if (conditions.Count == 0)
            return null;
        var victim = conditions[_random.Next(conditions.Count)];
        var negated = SyntaxFactory.ParseExpression($"!({victim.ToFullString()})").WithTriviaFrom(victim);
        return root.ReplaceNode(victim, negated);
    }

    private SyntaxNode? DeleteRandomStatement(SyntaxNode root)
    {
        var statements = root.DescendantNodes()
            .OfType<ExpressionStatementSyntax>()
            .Where(s => !s.ToString().Contains("return", StringComparison.Ordinal))
            .ToList();
        if (statements.Count == 0)
            return null;
        var victim = statements[_random.Next(statements.Count)];
        return root.ReplaceNode(victim, SyntaxFactory.EmptyStatement().WithTriviaFrom(victim));
    }

    private static readonly string[] BinaryOperators =
    ["+", "-", "*", "/", "%", "<", ">", "<=", ">=", "==", "!=", "&", "|", "^", "<<", ">>"];
}
