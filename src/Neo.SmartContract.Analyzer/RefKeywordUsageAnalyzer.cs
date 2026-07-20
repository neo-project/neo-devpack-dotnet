// Copyright (C) 2015-2026 The Neo Project.
//
// RefKeywordUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RefKeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4010";
        private static readonly string Title = "Ref keyword usage";
        private static readonly string MessageFormat = "Unsupported by-reference usage in {0}";
        private static readonly string Description = "Reports by-reference invocation forms that the Neo compiler cannot lower.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            foreach (var argument in invocationExpression.ArgumentList.Arguments)
            {
                if ((argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword) ||
                     argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) &&
                    argument.Expression is ElementAccessExpressionSyntax elementAccess &&
                    GetElementContainerKind(context, elementAccess.Expression) is { } containerKind)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rule,
                        argument.GetLocation(),
                        $"{containerKind} element argument; bind it to a ref local before forwarding"));
                    continue;
                }

                // Report only on unsupported 'in' arguments (regular ref/out are supported)
                if (argument.RefOrOutKeyword.IsKind(SyntaxKind.InKeyword) &&
                    !IsDelegateInvocation(context, invocationExpression))
                {
                    var diagnostic = Diagnostic.Create(Rule, argument.GetLocation(), "method invocation ('in' argument)");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static string? GetElementContainerKind(
            SyntaxNodeAnalysisContext context,
            ExpressionSyntax expression)
        {
            var type = context.SemanticModel.GetTypeInfo(expression, context.CancellationToken).Type;
            if (type is IArrayTypeSymbol)
            {
                return "array";
            }

            if (type is INamedTypeSymbol namedType &&
                namedType.ContainingNamespace.ToDisplayString() == "System" &&
                namedType.Name == "Span")
            {
                return "Span";
            }

            return null;
        }

        private static bool IsDelegateInvocation(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            return context.SemanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol is IMethodSymbol
            {
                MethodKind: MethodKind.DelegateInvoke
            };
        }
    }
}
