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

    /// <summary>
    /// Checks that mutation did not alter the bounds or progress of any loop.
    /// </summary>
    public static bool PreservesLoopControl(string originalSource, string mutatedSource)
    {
        var original = LoopControlNodes(Parse(originalSource).GetRoot()).ToArray();
        var mutated = LoopControlNodes(Parse(mutatedSource).GetRoot()).ToArray();
        if (original.Length != mutated.Length)
            return false;

        for (int i = 0; i < original.Length; i++)
        {
            if (!SyntaxFactory.AreEquivalent(original[i], mutated[i]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Detects the legacy boundary mutation that starts a ++ loop at Int32.MinValue or
    /// Int64.MinValue and runs toward a non-negative literal bound.
    /// </summary>
    public static bool HasExcessiveLoop(string source) => Parse(source).GetRoot()
        .DescendantNodes().OfType<ForStatementSyntax>().Any(HasExcessiveTripCount);

    private static bool HasExcessiveTripCount(ForStatementSyntax loop)
    {
        if (loop.Declaration is not { Variables.Count: 1 } declaration)
            return false;

        VariableDeclaratorSyntax variable = declaration.Variables[0];
        if (variable.Initializer?.Value.ToString() is not ("int.MinValue" or "long.MinValue"))
            return false;

        if (loop.Condition is not BinaryExpressionSyntax condition ||
            !(condition.IsKind(SyntaxKind.LessThanExpression) ||
              condition.IsKind(SyntaxKind.LessThanOrEqualExpression)) ||
            condition.Left is not IdentifierNameSyntax left ||
            left.Identifier.ValueText != variable.Identifier.ValueText ||
            condition.Right is not LiteralExpressionSyntax bound ||
            !bound.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            return false;
        }

        string name = variable.Identifier.ValueText;
        return loop.Incrementors.Any(incrementor => incrementor switch
        {
            PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.PostIncrementExpression) =>
                postfix.Operand is IdentifierNameSyntax id && id.Identifier.ValueText == name,
            PrefixUnaryExpressionSyntax prefix when prefix.IsKind(SyntaxKind.PreIncrementExpression) =>
                prefix.Operand is IdentifierNameSyntax id && id.Identifier.ValueText == name,
            _ => false,
        });
    }

    private static IEnumerable<SyntaxNode> LoopControlNodes(SyntaxNode root)
    {
        foreach (SyntaxNode node in root.DescendantNodes())
        {
            switch (node)
            {
                case ForStatementSyntax loop:
                    if (loop.Declaration is not null)
                        yield return loop.Declaration;
                    foreach (ExpressionSyntax initializer in loop.Initializers)
                        yield return initializer;
                    if (loop.Condition is not null)
                        yield return loop.Condition;
                    foreach (ExpressionSyntax incrementor in loop.Incrementors)
                        yield return incrementor;
                    break;
                case WhileStatementSyntax loop:
                    yield return loop.Condition;
                    break;
                case DoStatementSyntax loop:
                    yield return loop.Condition;
                    break;
            }
        }
    }

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
            .Where(b => BinaryOperators.Contains(b.OperatorToken.ValueText) && !IsLoopCondition(b))
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

    private static bool IsLoopCondition(SyntaxNode node) => node.Ancestors().Any(ancestor => ancestor switch
    {
        WhileStatementSyntax loop => loop.Condition.Span.Contains(node.Span),
        DoStatementSyntax loop => loop.Condition.Span.Contains(node.Span),
        ForStatementSyntax loop => loop.Condition is { } condition && condition.Span.Contains(node.Span),
        _ => false,
    });

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
