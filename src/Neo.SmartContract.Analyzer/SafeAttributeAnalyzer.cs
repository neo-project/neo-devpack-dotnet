// Copyright (C) 2015-2025 The Neo Project.
//
// SafeAttributeAnalyzer.cs file belongs to the neo project and is free
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
    public class SafeAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4036";

        private static readonly LocalizableString Title = "Incorrect Safe attribute usage";
        private static readonly LocalizableString MessageFormat = "Method marked with [Safe] attribute should not modify state: {0}";
        private static readonly LocalizableString Description = "Methods marked with the [Safe] attribute should not modify blockchain state.";
        private const string Category = "Usage";

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

            // Check if the method has the Safe attribute
            if (!HasSafeAttribute(methodDeclaration))
                return;

            // Check if the method modifies state
            if (ModifiesState(methodDeclaration, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(),
                    "Methods marked as [Safe] should not modify blockchain state");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool HasSafeAttribute(MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name.ToString() == "Safe");
        }

        private bool ModifiesState(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            if (methodDeclaration.Body == null)
                return false;

            // Check for state-modifying operations
            return methodDeclaration.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Any(i => IsStateModifyingOperation(i, semanticModel));
        }

        private bool IsStateModifyingOperation(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                if (symbol == null) return false;

                // Check for Storage.Put/Delete
                if ((symbol.Name == "Put" || symbol.Name == "Delete") &&
                    symbol.ContainingType.Name == "Storage" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    return true;
                }

                // Check for ContractManagement.Update/Deploy
                if ((symbol.Name == "Update" || symbol.Name == "Deploy") &&
                    symbol.ContainingType.Name == "ContractManagement" &&
                    symbol.ContainingNamespace.ToString().Contains("Neo.SmartContract.Framework"))
                {
                    return true;
                }

                // Check for Runtime.Notify
                if (symbol.Name == "Notify" &&
                    symbol.ContainingType.Name == "Runtime" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    return true;
                }

                // Check for Contract.Call with CallFlags that include WriteStates
                if (symbol.Name == "Call" &&
                    symbol.ContainingType.Name == "Contract" &&
                    symbol.ContainingNamespace.ToString() == "Neo.SmartContract.Framework.Services")
                {
                    // Check if the CallFlags parameter includes WriteStates
                    if (invocation.ArgumentList.Arguments.Count >= 3)
                    {
                        var flagsArg = invocation.ArgumentList.Arguments[2].Expression;
                        if (flagsArg is MemberAccessExpressionSyntax flagsMemberAccess)
                        {
                            string flagsName = flagsMemberAccess.Name.Identifier.Text;
                            if (flagsName.Contains("WriteStates") || flagsName == "All")
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
