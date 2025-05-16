// Copyright (C) 2015-2025 The Neo Project.
//
// GasOptimizationAnalyzer.cs file belongs to the neo project and is free
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
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GasOptimizationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdNestedLoop = "NC4039";
        public const string DiagnosticIdLargeString = "NC4040";
        public const string DiagnosticIdUnnecessaryConversion = "NC4041";

        private static readonly LocalizableString TitleNestedLoop = "Nested loop detected";
        private static readonly LocalizableString MessageFormatNestedLoop = "Nested loops can consume excessive gas: {0}";
        private static readonly LocalizableString DescriptionNestedLoop = "Nested loops can consume excessive gas. Consider refactoring to reduce nesting.";

        private static readonly LocalizableString TitleLargeString = "Large string literal";
        private static readonly LocalizableString MessageFormatLargeString = "Large string literal detected: {0}";
        private static readonly LocalizableString DescriptionLargeString = "Large string literals consume more gas. Consider storing large strings off-chain.";

        private static readonly LocalizableString TitleUnnecessaryConversion = "Unnecessary type conversion";
        private static readonly LocalizableString MessageFormatUnnecessaryConversion = "Unnecessary type conversion detected: {0}";
        private static readonly LocalizableString DescriptionUnnecessaryConversion = "Unnecessary type conversions consume more gas. Use the appropriate type directly.";

        private const string Category = "Performance";

        private static readonly DiagnosticDescriptor RuleNestedLoop = new(
            DiagnosticIdNestedLoop,
            TitleNestedLoop,
            MessageFormatNestedLoop,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionNestedLoop);

        private static readonly DiagnosticDescriptor RuleLargeString = new(
            DiagnosticIdLargeString,
            TitleLargeString,
            MessageFormatLargeString,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionLargeString);

        private static readonly DiagnosticDescriptor RuleUnnecessaryConversion = new(
            DiagnosticIdUnnecessaryConversion,
            TitleUnnecessaryConversion,
            MessageFormatUnnecessaryConversion,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionUnnecessaryConversion);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RuleNestedLoop, RuleLargeString, RuleUnnecessaryConversion);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeForStatement, SyntaxKind.ForStatement);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLiteralExpression, SyntaxKind.StringLiteralExpression);
            context.RegisterSyntaxNodeAction(AnalyzeCastExpression, SyntaxKind.CastExpression);
        }

        private void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;
            CheckNestedLoops(context, forStatement);
        }

        private void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (ForEachStatementSyntax)context.Node;
            CheckNestedLoops(context, forEachStatement);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;
            CheckNestedLoops(context, whileStatement);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;
            CheckNestedLoops(context, doStatement);
        }

        private void CheckNestedLoops(SyntaxNodeAnalysisContext context, SyntaxNode loopNode)
        {
            // Check if this loop is nested inside another loop
            SyntaxNode parent = loopNode.Parent;
            while (parent != null)
            {
                if (parent is ForStatementSyntax ||
                    parent is ForEachStatementSyntax ||
                    parent is WhileStatementSyntax ||
                    parent is DoStatementSyntax)
                {
                    var diagnostic = Diagnostic.Create(RuleNestedLoop, loopNode.GetLocation(),
                        "Consider refactoring to reduce loop nesting");
                    context.ReportDiagnostic(diagnostic);
                    break;
                }
                parent = parent.Parent;
            }
        }

        private void AnalyzeLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var literalExpression = (LiteralExpressionSyntax)context.Node;

            // Check if it's a string literal
            if (literalExpression.Token.Value is string stringValue && stringValue.Length > 100)
            {
                // Skip checking string literals in contract attributes
                if (IsPartOfContractAttribute(literalExpression))
                {
                    return;
                }

                var diagnostic = Diagnostic.Create(RuleLargeString, literalExpression.GetLocation(),
                    $"String of length {stringValue.Length} characters");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsPartOfContractAttribute(LiteralExpressionSyntax literalExpression)
        {
            // Check if the literal is part of an attribute argument
            var attributeArgument = literalExpression.Ancestors().OfType<AttributeArgumentSyntax>().FirstOrDefault();
            if (attributeArgument == null)
                return false;

            // Check if the attribute is a contract attribute
            var attribute = attributeArgument.Ancestors().OfType<AttributeSyntax>().FirstOrDefault();
            if (attribute == null)
                return false;

            // Get the attribute name
            var attributeName = attribute.Name.ToString();

            // Check if it's a contract attribute
            return attributeName.Contains("Contract") ||
                   attributeName.EndsWith("Attribute") && attributeName.Contains("Contract");
        }

        private void AnalyzeCastExpression(SyntaxNodeAnalysisContext context)
        {
            var castExpression = (CastExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            // Get the type of the expression being cast
            var expressionType = semanticModel.GetTypeInfo(castExpression.Expression).Type;
            if (expressionType == null) return;

            // Get the type being cast to
            var targetType = semanticModel.GetTypeInfo(castExpression.Type).Type;
            if (targetType == null) return;

            // Check for unnecessary conversions
            if (IsUnnecessaryConversion(expressionType, targetType))
            {
                var diagnostic = Diagnostic.Create(RuleUnnecessaryConversion, castExpression.GetLocation(),
                    $"Conversion from {expressionType.Name} to {targetType.Name}");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsUnnecessaryConversion(ITypeSymbol sourceType, ITypeSymbol targetType)
        {
            // Check for common unnecessary conversions in Neo smart contracts

            // ByteString to byte[]
            if (sourceType.Name == "ByteString" && targetType.Name == "Byte[]")
                return true;

            // byte[] to ByteString
            if (sourceType.Name == "Byte[]" && targetType.Name == "ByteString")
                return true;

            // int to BigInteger
            if (sourceType.SpecialType == SpecialType.System_Int32 && targetType.Name == "BigInteger")
                return true;

            return false;
        }
    }
}
