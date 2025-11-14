// Copyright (C) 2015-2025 The Neo Project.
//
// UnsupportedSyntaxAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsupportedSyntaxAnalyzer : DiagnosticAnalyzer
{
    private const string Category = "Syntax";

    public const string UnsafeCodeRuleId = "NC4033";
    public const string AnonymousMethodRuleId = "NC4034";
    public const string IteratorRuleId = "NC4035";
    public const string QueryExpressionRuleId = "NC4036";
    public const string DynamicBindingRuleId = "NC4037";
    public const string AsyncMethodRuleId = "NC4038";
    public const string AwaitExpressionRuleId = "NC4039";
    public const string ExceptionFilterRuleId = "NC4040";
    public const string PatternMatchingRuleId = "NC4041";
    public const string LocalFunctionRuleId = "NC4042";
    public const string RefLocalOrReturnRuleId = "NC4043";
    public const string RangeExpressionRuleId = "NC4044";
    public const string AwaitForEachRuleId = "NC4045";
    public const string NativeIntRuleId = "NC4046";
    public const string TopLevelStatementRuleId = "NC4047";
    public const string FunctionPointerRuleId = "NC4048";
    public const string GlobalUsingRuleId = "NC4049";
    public const string ListPatternRuleId = "NC4050";
    public const string Utf8LiteralRuleId = "NC4051";
    public const string DefaultInterfaceMethodRuleId = "NC4052";
    public const string FileLocalTypeRuleId = "NC4053";
    public const string RefReadonlyParameterRuleId = "NC4054";

    private static readonly DiagnosticDescriptor UnsafeCodeRule = CreateDescriptor(
        UnsafeCodeRuleId,
        "Unsafe code is not supported",
        "Unsafe code blocks are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor AnonymousMethodRule = CreateDescriptor(
        AnonymousMethodRuleId,
        "Anonymous methods are not supported",
        "Anonymous methods (delegate statements) are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor IteratorRule = CreateDescriptor(
        IteratorRuleId,
        "Iterator blocks are not supported",
        "Yield-based iterator blocks are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor QueryExpressionRule = CreateDescriptor(
        QueryExpressionRuleId,
        "LINQ query expressions are not supported",
        "Query expression syntax is not supported; use explicit helper calls instead.");

    private static readonly DiagnosticDescriptor DynamicBindingRule = CreateDescriptor(
        DynamicBindingRuleId,
        "Dynamic binding is not supported",
        "The 'dynamic' type is not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor AsyncMethodRule = CreateDescriptor(
        AsyncMethodRuleId,
        "Async methods are not supported",
        "Methods marked with 'async' are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor AwaitExpressionRule = CreateDescriptor(
        AwaitExpressionRuleId,
        "Await expressions are not supported",
        "Await expressions are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor ExceptionFilterRule = CreateDescriptor(
        ExceptionFilterRuleId,
        "Exception filters are not supported",
        "Exception filters (when clauses) are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor PatternMatchingRule = CreateDescriptor(
        PatternMatchingRuleId,
        "Pattern matching is not supported",
        "Pattern matching expressions are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor LocalFunctionRule = CreateDescriptor(
        LocalFunctionRuleId,
        "Local functions are not supported",
        "Local function declarations are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor RefLocalOrReturnRule = CreateDescriptor(
        RefLocalOrReturnRuleId,
        "Ref locals and returns are not supported",
        "Ref locals, ref returns, and ref expressions are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor RangeExpressionRule = CreateDescriptor(
        RangeExpressionRuleId,
        "Index and range operators are not supported",
        "Index (^) and range (..) operators are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor AwaitForEachRule = CreateDescriptor(
        AwaitForEachRuleId,
        "Async streams are not supported",
        "'await foreach' loops are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor NativeIntRule = CreateDescriptor(
        NativeIntRuleId,
        "Native-sized integers are not supported",
        "'nint' and 'nuint' types are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor TopLevelStatementRule = CreateDescriptor(
        TopLevelStatementRuleId,
        "Top-level statements are not supported",
        "Top-level statements are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor FunctionPointerRule = CreateDescriptor(
        FunctionPointerRuleId,
        "Function pointers are not supported",
        "Function pointer syntax is not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor GlobalUsingRule = CreateDescriptor(
        GlobalUsingRuleId,
        "Global using directives are not supported",
        "Global using directives are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor ListPatternRule = CreateDescriptor(
        ListPatternRuleId,
        "List patterns are not supported",
        "List pattern matching syntax is not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor Utf8LiteralRule = CreateDescriptor(
        Utf8LiteralRuleId,
        "UTF-8 string literals are not supported",
        "String literals with the 'u8' suffix are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor DefaultInterfaceMethodRule = CreateDescriptor(
        DefaultInterfaceMethodRuleId,
        "Default interface members are not supported",
        "Interface members cannot provide bodies in Neo smart contracts.");

    private static readonly DiagnosticDescriptor FileLocalTypeRule = CreateDescriptor(
        FileLocalTypeRuleId,
        "File-local types are not supported",
        "Types declared with the 'file' modifier are not supported by the Neo compiler.");

    private static readonly DiagnosticDescriptor RefReadonlyParameterRule = CreateDescriptor(
        RefReadonlyParameterRuleId,
        "'ref readonly' or 'in' parameters are not supported",
        "'ref readonly' or 'in' parameters are not supported by the Neo compiler.");

    private static DiagnosticDescriptor CreateDescriptor(string id, string title, string message) =>
        new(id, title, message, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: message);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        UnsafeCodeRule,
        AnonymousMethodRule,
        IteratorRule,
        QueryExpressionRule,
        DynamicBindingRule,
        AsyncMethodRule,
        AwaitExpressionRule,
        ExceptionFilterRule,
        PatternMatchingRule,
        LocalFunctionRule,
        RefLocalOrReturnRule,
        RangeExpressionRule,
        AwaitForEachRule,
        NativeIntRule,
        TopLevelStatementRule,
        FunctionPointerRule,
        GlobalUsingRule,
        ListPatternRule,
        Utf8LiteralRule,
        DefaultInterfaceMethodRule,
        FileLocalTypeRule,
        RefReadonlyParameterRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(UnsafeCodeRule), SyntaxKind.UnsafeStatement);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(AnonymousMethodRule), SyntaxKind.AnonymousMethodExpression);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(IteratorRule), SyntaxKind.YieldBreakStatement, SyntaxKind.YieldReturnStatement);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(QueryExpressionRule), SyntaxKind.QueryExpression);
        context.RegisterSyntaxNodeAction(AnalyzeDynamicBinding, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(AwaitExpressionRule), SyntaxKind.AwaitExpression);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(ExceptionFilterRule), SyntaxKind.CatchFilterClause);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(PatternMatchingRule), SyntaxKind.IsPatternExpression, SyntaxKind.SwitchExpression);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(LocalFunctionRule), SyntaxKind.LocalFunctionStatement);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(RefLocalOrReturnRule), SyntaxKind.RefType, SyntaxKind.RefExpression);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(RangeExpressionRule), SyntaxKind.RangeExpression, SyntaxKind.IndexExpression);
        context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement, SyntaxKind.ForEachVariableStatement);
        context.RegisterSyntaxNodeAction(AnalyzeNativeSizedIntegers, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(TopLevelStatementRule), SyntaxKind.GlobalStatement);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(FunctionPointerRule), SyntaxKind.FunctionPointerType);
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
        context.RegisterSyntaxNodeAction(AnalyzeUsingLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
        context.RegisterSyntaxNodeAction(ReportSimpleDiagnostic(ListPatternRule), SyntaxKind.ListPattern);
        context.RegisterSyntaxNodeAction(AnalyzeUtf8Literal, SyntaxKind.Utf8StringLiteralExpression, SyntaxKind.StringLiteralExpression);
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration, SyntaxKind.EnumDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
    }

    private static Action<SyntaxNodeAnalysisContext> ReportSimpleDiagnostic(DiagnosticDescriptor descriptor) =>
        context => context.ReportDiagnostic(Diagnostic.Create(descriptor, context.Node.GetLocation()));

    private static void AnalyzeDynamicBinding(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not IdentifierNameSyntax identifier ||
            !string.Equals(identifier.Identifier.ValueText, "dynamic", StringComparison.Ordinal))
        {
            return;
        }

        // 'dynamic' used as an identifier is only valid in type positions; flag it eagerly.
        context.ReportDiagnostic(Diagnostic.Create(DynamicBindingRule, identifier.GetLocation()));
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax method)
            return;

        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(AsyncMethodRule, method.Modifiers.First(m => m.IsKind(SyntaxKind.AsyncKeyword)).GetLocation()));
        }

        if (method.Parent is InterfaceDeclarationSyntax && (method.Body is not null || method.ExpressionBody is not null))
        {
            var location = method.Identifier.GetLocation();
            context.ReportDiagnostic(Diagnostic.Create(DefaultInterfaceMethodRule, location));
        }
    }

    private static void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CommonForEachStatementSyntax forEach)
            return;

        if (!forEach.AwaitKeyword.IsKind(SyntaxKind.None))
        {
            context.ReportDiagnostic(Diagnostic.Create(AwaitForEachRule, forEach.AwaitKeyword.GetLocation()));
        }
    }

    private static void AnalyzeNativeSizedIntegers(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not IdentifierNameSyntax identifier)
            return;

        if (!string.Equals(identifier.Identifier.ValueText, "nint", StringComparison.Ordinal) &&
            !string.Equals(identifier.Identifier.ValueText, "nuint", StringComparison.Ordinal))
        {
            return;
        }

        // When used as identifiers (and not variable names), the symbol will be a type.
        var symbol = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;
        if (symbol is ITypeSymbol)
        {
            context.ReportDiagnostic(Diagnostic.Create(NativeIntRule, identifier.GetLocation()));
        }
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is UsingDirectiveSyntax directive && !directive.GlobalKeyword.IsKind(SyntaxKind.None))
        {
            context.ReportDiagnostic(Diagnostic.Create(GlobalUsingRule, directive.GlobalKeyword.GetLocation()));
        }
    }

    private static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is UsingStatementSyntax usingStatement && !usingStatement.AwaitKeyword.IsKind(SyntaxKind.None))
        {
            context.ReportDiagnostic(Diagnostic.Create(AwaitExpressionRule, usingStatement.AwaitKeyword.GetLocation()));
        }
    }

    private static void AnalyzeUsingLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not LocalDeclarationStatementSyntax localDeclaration)
        {
            return;
        }

        if (localDeclaration.UsingKeyword.IsKind(SyntaxKind.None))
        {
            return;
        }

        var awaitToken = !localDeclaration.AwaitKeyword.IsKind(SyntaxKind.None)
            ? localDeclaration.AwaitKeyword
            : localDeclaration.DescendantTokens().FirstOrDefault(token => token.IsKind(SyntaxKind.AwaitKeyword));

        if (awaitToken.IsKind(SyntaxKind.None))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(AwaitExpressionRule, awaitToken.GetLocation()));
    }

    private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not SwitchStatementSyntax switchStatement)
            return;

        foreach (var label in switchStatement.Sections.SelectMany(static section => section.Labels))
        {
            if (label is CasePatternSwitchLabelSyntax patternLabel)
            {
                context.ReportDiagnostic(Diagnostic.Create(PatternMatchingRule, patternLabel.GetLocation()));
            }
        }
    }

    private static void AnalyzeUtf8Literal(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not LiteralExpressionSyntax literal)
            return;

        var token = literal.Token;
        if (literal.IsKind(SyntaxKind.Utf8StringLiteralExpression) ||
            token.Text.EndsWith("u8", StringComparison.OrdinalIgnoreCase))
        {
            context.ReportDiagnostic(Diagnostic.Create(Utf8LiteralRule, literal.GetLocation()));
        }
    }

    private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is TypeDeclarationSyntax typeDeclaration &&
            typeDeclaration.Modifiers.Any(SyntaxKind.FileKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(FileLocalTypeRule, typeDeclaration.Modifiers.First(m => m.IsKind(SyntaxKind.FileKeyword)).GetLocation()));
        }
    }

    private static void AnalyzeParameter(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ParameterSyntax parameter)
            return;

        var modifiers = parameter.Modifiers;
        bool hasRefReadonly = modifiers.Any(SyntaxKind.RefKeyword) && modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        bool hasInModifier = modifiers.Any(SyntaxKind.InKeyword);

        if (!hasRefReadonly && !hasInModifier)
        {
            return;
        }

        var location = hasRefReadonly
            ? modifiers.First(m => m.IsKind(SyntaxKind.RefKeyword)).GetLocation()
            : modifiers.First(m => m.IsKind(SyntaxKind.InKeyword)).GetLocation();

        context.ReportDiagnostic(Diagnostic.Create(RefReadonlyParameterRule, location));
    }
}
