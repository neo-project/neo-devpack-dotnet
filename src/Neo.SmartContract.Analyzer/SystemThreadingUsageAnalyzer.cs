// Copyright (C) 2015-2025 The Neo Project.
//
// SystemThreadingUsageAnalyzer.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Analyzer
{
    /// <summary>
    /// Analyzer that detects usage of System.Threading namespace and related threading constructs
    /// which are not supported in Neo smart contracts due to VM limitations.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemThreadingUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4026";
        private const string Title = "Threading operations are not supported";
        private const string MessageFormat = "Use of threading operations is not allowed in Neo smart contracts. Neo operates synchronously: {0}.";
        private const string Description = "Neo smart contracts run synchronously on the Neo Virtual Machine. Threading, async/await, and parallel operations are not supported. Use sequential execution patterns instead.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: "https://docs.neo.org/docs/n3/develop/write/limitation");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeNode,
                SyntaxKind.UsingDirective,
                SyntaxKind.IdentifierName,
                SyntaxKind.QualifiedName,
                SyntaxKind.InvocationExpression,
                SyntaxKind.VariableDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.LocalFunctionStatement);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case UsingDirectiveSyntax usingDirective:
                    AnalyzeUsingDirective(context, usingDirective);
                    break;
                case IdentifierNameSyntax identifierName:
                    AnalyzeIdentifierName(context, identifierName);
                    break;
                case QualifiedNameSyntax qualifiedName:
                    AnalyzeQualifiedName(context, qualifiedName);
                    break;
                case InvocationExpressionSyntax invocation:
                    AnalyzeInvocation(context, invocation);
                    break;
                case VariableDeclarationSyntax variableDeclaration:
                    AnalyzeVariableDeclaration(context, variableDeclaration);
                    break;
                case MethodDeclarationSyntax methodDeclaration:
                    AnalyzeMethodDeclaration(context, methodDeclaration);
                    break;
                case LocalFunctionStatementSyntax localFunction:
                    AnalyzeLocalFunction(context, localFunction);
                    break;
            }
        }

        private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            var namespaceName = usingDirective.Name?.ToString();
            if (IsThreadingNamespace(namespaceName))
            {
                ReportDiagnostic(context, usingDirective.GetLocation(), $"using {namespaceName} is not allowed");
            }
        }

        private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierName)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName);
            if (symbolInfo.Symbol != null)
            {
                var namespaceName = symbolInfo.Symbol.ContainingNamespace?.ToDisplayString();
                if (IsThreadingNamespace(namespaceName))
                {
                    ReportDiagnostic(context, identifierName.GetLocation(), $"Type from {namespaceName} is not supported");
                }
            }
        }

        private static void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context, QualifiedNameSyntax qualifiedName)
        {
            var namespaceName = qualifiedName.Left.ToString();
            if (IsThreadingNamespace(namespaceName))
            {
                ReportDiagnostic(context, qualifiedName.GetLocation(), $"Type from {namespaceName} is not supported");
            }
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
        {
            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is IMethodSymbol method)
            {
                var namespaceName = method.ContainingNamespace?.ToDisplayString();
                if (IsThreadingNamespace(namespaceName))
                {
                    ReportDiagnostic(context, invocation.GetLocation(), $"Method call from {namespaceName} is not supported");
                }
            }
        }

        private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax variableDeclaration)
        {
            var typeInfo = context.SemanticModel.GetTypeInfo(variableDeclaration.Type);
            if (typeInfo.Type != null)
            {
                var namespaceName = typeInfo.Type.ContainingNamespace?.ToDisplayString();
                if (IsThreadingNamespace(namespaceName))
                {
                    ReportDiagnostic(context, variableDeclaration.GetLocation(), $"Variable of type from {namespaceName} is not supported");
                }
            }
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                ReportDiagnostic(context, methodDeclaration.GetLocation(), "async methods are not supported");
            }
        }

        private static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context, LocalFunctionStatementSyntax localFunction)
        {
            if (localFunction.Modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                ReportDiagnostic(context, localFunction.GetLocation(), "async local functions are not supported");
            }
        }

        private static bool IsThreadingNamespace(string? namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return false;

            return namespaceName!.StartsWith("System.Threading") ||
                   namespaceName.Equals("System.Threading") ||
                   namespaceName.StartsWith("System.Threading.Tasks") ||
                   namespaceName.StartsWith("System.Threading.Channels");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, Location location, string message)
        {
            var diagnostic = Diagnostic.Create(Rule, location, message);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
