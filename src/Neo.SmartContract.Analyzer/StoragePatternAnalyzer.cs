// Copyright (C) 2015-2025 The Neo Project.
//
// StoragePatternAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    /// <summary>
    /// Analyzer that detects inefficient storage patterns in Neo smart contracts.
    ///
    /// This analyzer detects three main patterns:
    /// 1. Repeated access to the same storage key (DiagnosticIdRepeatedAccess)
    /// 2. Using large string keys directly in storage operations (DiagnosticIdLargeKey)
    /// 3. Accessing storage inside loops (DiagnosticIdStorageInLoop)
    ///
    /// Each of these patterns can lead to higher GAS consumption and should be optimized.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StoragePatternAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic ID for repeated access to the same storage key.
        /// This is inefficient because each storage access costs GAS.
        /// The value should be cached in a local variable.
        /// </summary>
        public const string DiagnosticIdRepeatedAccess = "NC5050";

        /// <summary>
        /// Diagnostic ID for using large string keys directly in storage operations.
        /// Large keys consume more GAS than necessary.
        /// It's more efficient to hash large keys before using them for storage.
        /// </summary>
        public const string DiagnosticIdLargeKey = "NC5051";

        /// <summary>
        /// Diagnostic ID for accessing storage inside loops.
        /// This is inefficient because each storage access costs GAS.
        /// If the same key is accessed in each iteration, it should be retrieved once before the loop.
        /// </summary>
        public const string DiagnosticIdStorageInLoop = "NC5052";

        // Maximum key length before suggesting to hash the key
        private const int MaxKeyLength = 40;

        private static readonly LocalizableString RepeatedAccessTitle = "Repeated storage access";
        private static readonly LocalizableString RepeatedAccessMessageFormat = "Repeated access to storage key '{0}'";
        private static readonly LocalizableString RepeatedAccessDescription = "Accessing the same storage key multiple times is inefficient. Cache the value in a local variable.";

        private static readonly LocalizableString LargeKeyTitle = "Large storage key";
        private static readonly LocalizableString LargeKeyMessageFormat = "Storage key '{0}' is too long";
        private static readonly LocalizableString LargeKeyDescription = "Using large string keys directly in storage operations is inefficient. Consider hashing the key.";

        private static readonly LocalizableString StorageInLoopTitle = "Storage access in loop";
        private static readonly LocalizableString StorageInLoopMessageFormat = "{0}";
        private static readonly LocalizableString StorageInLoopDescription = "Accessing storage inside a loop is inefficient. Consider retrieving the data before the loop.";

        private const string Category = "Performance";

        private static readonly DiagnosticDescriptor RepeatedAccessRule = new(
            DiagnosticIdRepeatedAccess,
            RepeatedAccessTitle,
            RepeatedAccessMessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: RepeatedAccessDescription);

        private static readonly DiagnosticDescriptor LargeKeyRule = new(
            DiagnosticIdLargeKey,
            LargeKeyTitle,
            LargeKeyMessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: LargeKeyDescription);

        private static readonly DiagnosticDescriptor StorageInLoopRule = new(
            DiagnosticIdStorageInLoop,
            StorageInLoopTitle,
            StorageInLoopMessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: StorageInLoopDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(RepeatedAccessRule, LargeKeyRule, StorageInLoopRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.Body == null)
                return;

            // Track storage accesses by key
            var storageAccesses = new Dictionary<string, List<InvocationExpressionSyntax>>();

            // Analyze all invocations in the method
            var invocations = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                // Check if this is a Storage.Get/Put/Delete call
                if (IsStorageAccess(invocation, context.SemanticModel, out string key, out string methodName))
                {
                    // Skip if we couldn't determine the key
                    if (key == null)
                        continue;

                    // Add to our tracking dictionary
                    if (!storageAccesses.ContainsKey(key))
                    {
                        storageAccesses[key] = new List<InvocationExpressionSyntax>();
                    }
                    storageAccesses[key].Add(invocation);

                    // Check for large keys
                    if (key.Length > MaxKeyLength)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            LargeKeyRule,
                            invocation.GetLocation(),
                            key));
                    }

                    // Check for storage access in loop
                    var containingLoop = invocation.Ancestors().FirstOrDefault(
                        a => a is ForStatementSyntax ||
                             a is ForEachStatementSyntax ||
                             a is WhileStatementSyntax ||
                             a is DoStatementSyntax);

                    if (containingLoop != null)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            StorageInLoopRule,
                            invocation.GetLocation(),
                            "Consider retrieving data before the loop"));
                    }
                }
            }

            // Check for repeated accesses to the same key
            foreach (var kvp in storageAccesses)
            {
                string key = kvp.Key;
                var accesses = kvp.Value;

                // If we have more than one access to the same key
                if (accesses.Count > 1)
                {
                    // Report diagnostic for all accesses after the first one
                    for (int i = 1; i < accesses.Count; i++)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            RepeatedAccessRule,
                            accesses[i].GetLocation(),
                            key));
                    }
                }
            }
        }

        /// <summary>
        /// Determines if an invocation is a Storage.Get/Put/Delete call and extracts the key if possible.
        /// </summary>
        /// <param name="invocation">The invocation to check</param>
        /// <param name="semanticModel">The semantic model</param>
        /// <param name="key">Output parameter for the storage key if found</param>
        /// <param name="methodName">Output parameter for the method name (Get, Put, Delete)</param>
        /// <returns>True if this is a Storage access, false otherwise</returns>
        private bool IsStorageAccess(InvocationExpressionSyntax invocation, SemanticModel semanticModel, out string key, out string methodName)
        {
            key = string.Empty;
            methodName = string.Empty;

            // Check if this is a member access (e.g., Storage.Get)
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                // For testing purposes, let's check the raw syntax first
                var memberName = memberAccess.Name.Identifier.ValueText;
                var expressionText = memberAccess.Expression.ToString();

                // If it looks like Storage.Get/Put/Delete based on syntax alone
                if ((memberName == "Get" || memberName == "Put" || memberName == "Delete") &&
                    expressionText == "Storage")
                {
                    methodName = memberName;

                    // Try to extract the key from the first argument
                    if (invocation.ArgumentList.Arguments.Count > 0)
                    {
                        var firstArg = invocation.ArgumentList.Arguments[0].Expression;

                        // If it's a string literal, we can get the exact key
                        if (firstArg is LiteralExpressionSyntax literal &&
                            literal.Kind() == SyntaxKind.StringLiteralExpression)
                        {
                            key = literal.Token.ValueText;
                        }
                        else
                        {
                            // For non-literal keys, use a placeholder based on the expression text
                            key = firstArg.ToString();
                        }
                    }

                    return true;
                }

                // Also try the semantic approach
                var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (symbol != null)
                {
                    // Check if it's a Storage method
                    if ((symbol.Name == "Get" || symbol.Name == "Put" || symbol.Name == "Delete") &&
                        symbol.ContainingType.Name == "Storage")
                    {
                        methodName = symbol.Name;

                        // Try to extract the key from the first argument
                        if (invocation.ArgumentList.Arguments.Count > 0)
                        {
                            var firstArg = invocation.ArgumentList.Arguments[0].Expression;

                            // If it's a string literal, we can get the exact key
                            if (firstArg is LiteralExpressionSyntax literal &&
                                literal.Kind() == SyntaxKind.StringLiteralExpression)
                            {
                                key = literal.Token.ValueText;
                            }
                            else
                            {
                                // For non-literal keys, use a placeholder based on the expression text
                                key = firstArg.ToString();
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
