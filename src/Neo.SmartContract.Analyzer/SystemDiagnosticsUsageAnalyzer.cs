// Copyright (C) 2015-2026 The Neo Project.
//
// SystemDiagnosticsUsageAnalyzer.cs file belongs to the neo project and is free
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
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemDiagnosticsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4028";
        private const string DiagnosticsNamespace = "System.Diagnostics";
        private const string CodeAnalysisNamespace = "System.Diagnostics.CodeAnalysis";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of System.Diagnostics is not allowed in Neo smart contracts",
            "Neo smart contracts do not support System.Diagnostics: {0}",
            "Namespace",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.UsingDirective, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case UsingDirectiveSyntax usingDirective:
                    AnalyzeUsingDirective(context, usingDirective);
                    break;
                case IdentifierNameSyntax identifierName:
                    AnalyzeIdentifierName(context, identifierName);
                    break;
            }
        }

        private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            var name = usingDirective.Name?.ToString();
            if (IsDiagnosticsNamespace(name) && !IsCodeAnalysisUsingDirective(context, usingDirective))
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierName)
        {
            if (identifierName.FirstAncestorOrSelf<UsingDirectiveSyntax>() is { } usingDirective &&
                IsCodeAnalysisUsingDirective(context, usingDirective))
            {
                return;
            }

            if (IsCodeAnalysisAttribute(context, identifierName))
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierName, context.CancellationToken);
            var symbol = symbolInfo.Symbol;

            if (symbol != null && symbol.ContainingNamespace != null)
            {
                var namespaceName = symbol.ContainingNamespace.ToDisplayString();
                if (IsDiagnosticsNamespace(namespaceName))
                {
                    var diagnostic = Diagnostic.Create(Rule, identifierName.GetLocation(), namespaceName);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool IsCodeAnalysisAttribute(SyntaxNodeAnalysisContext context, IdentifierNameSyntax identifierName)
        {
            if (identifierName.FirstAncestorOrSelf<AttributeSyntax>() is not { } attribute)
            {
                return false;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol;
            var attributeType = symbol switch
            {
                IMethodSymbol constructor => constructor.ContainingType,
                INamedTypeSymbol type => type,
                _ => null
            };

            return attributeType?.ContainingNamespace.ToDisplayString() == CodeAnalysisNamespace;
        }

        private static bool IsCodeAnalysisUsingDirective(SyntaxNodeAnalysisContext context, UsingDirectiveSyntax usingDirective)
        {
            if (usingDirective.Name is null)
            {
                return false;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(usingDirective.Name, context.CancellationToken).Symbol;
            return symbol switch
            {
                INamespaceSymbol namespaceSymbol => namespaceSymbol.ToDisplayString() == CodeAnalysisNamespace,
                INamedTypeSymbol typeSymbol => typeSymbol.ContainingNamespace.ToDisplayString() == CodeAnalysisNamespace,
                _ => false
            };
        }

        private static bool IsDiagnosticsNamespace(string? namespaceName) =>
            namespaceName == DiagnosticsNamespace || namespaceName?.StartsWith(DiagnosticsNamespace + ".") == true;
    }
}
