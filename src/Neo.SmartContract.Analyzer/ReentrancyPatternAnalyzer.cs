// Copyright (C) 2015-2025 The Neo Project.
//
// ReentrancyPatternAnalyzer.cs file belongs to the neo project and is free
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReentrancyPatternAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4035";

        private static readonly LocalizableString Title = "Potential reentrancy vulnerability";
        private static readonly LocalizableString MessageFormat = "Potential reentrancy vulnerability: {0}";
        private static readonly LocalizableString Description = "Writing to storage after calling an external contract can lead to reentrancy vulnerabilities.";
        private const string Category = "Security";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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

            // Find all external contract calls and storage writes
            var externalCalls = new List<StatementSyntax>();
            var storageWrites = new List<StatementSyntax>();

            foreach (var statement in methodDeclaration.Body.Statements)
            {
                if (ContainsExternalContractCall(statement, context.SemanticModel))
                {
                    externalCalls.Add(statement);
                }

                if (ContainsStorageWrite(statement, context.SemanticModel))
                {
                    storageWrites.Add(statement);
                }
            }

            // Check if there are storage writes after external calls
            foreach (var externalCall in externalCalls)
            {
                int externalCallIndex = methodDeclaration.Body.Statements.IndexOf(externalCall);

                foreach (var storageWrite in storageWrites)
                {
                    int storageWriteIndex = methodDeclaration.Body.Statements.IndexOf(storageWrite);

                    if (storageWriteIndex > externalCallIndex)
                    {
                        var diagnostic = Diagnostic.Create(Rule, storageWrite.GetLocation(),
                            "Storage write after external contract call. Consider using the checks-effects-interactions pattern.");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool ContainsExternalContractCall(StatementSyntax statement, SemanticModel semanticModel)
        {
            // Look for Contract.Call invocations
            return statement.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(i => IsExternalContractCall(i, semanticModel));
        }

        private bool IsExternalContractCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (symbol == null) return false;

                // Check if it's Contract.Call
                if (symbol.Name == "Call" &&
                    symbol.ContainingType.Name == "Contract" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    return true;
                }
            }
            return false;
        }

        private bool ContainsStorageWrite(StatementSyntax statement, SemanticModel semanticModel)
        {
            // Look for Storage.Put or Storage.Delete invocations
            return statement.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(i => IsStorageWrite(i, semanticModel));
        }

        private bool IsStorageWrite(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (symbol == null) return false;

                // Check if it's Storage.Put or Storage.Delete
                if ((symbol.Name == "Put" || symbol.Name == "Delete") &&
                    symbol.ContainingType.Name == "Storage" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
