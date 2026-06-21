// Copyright (C) 2015-2026 The Neo Project.
//
// NotifyEventNameAnalyzer.cs file belongs to the neo project and is free
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
    public class NotifyEventNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4019";
        private const string Title = "Check event name in Notify call";
        private const string MessageFormat = "The name '{0}' does not match any defined event's DisplayName";
        private const string Description = "The name passed to Notify must match the DisplayName of an event.";
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

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
                return;

            if (memberAccessExpr.Name.Identifier.ValueText != "Notify")
                return;

            if (!(invocationExpr.ArgumentList?.Arguments.Count > 0))
                return;

            // Only a string literal event name can be checked statically.
            if (invocationExpr.ArgumentList.Arguments[0].Expression is not LiteralExpressionSyntax firstArgument ||
                !firstArgument.IsKind(SyntaxKind.StringLiteralExpression))
                return;

            var passedName = firstArgument.Token.ValueText;

            // Only analyze calls that actually bind to the framework's Runtime.Notify; a method
            // merely named "Notify" on some unrelated type must not be flagged.
            if (context.SemanticModel.GetSymbolInfo(invocationExpr).Symbol is not IMethodSymbol method ||
                !IsRuntimeNotify(method))
                return;

            // Resolve event names the way the compiler does: an event's [DisplayName] when present,
            // otherwise the event field name. Events inherited from base types count as well.
            var containingTypeSyntax = memberAccessExpr.FirstAncestorOrSelf<TypeDeclarationSyntax>();
            if (containingTypeSyntax is null ||
                context.SemanticModel.GetDeclaredSymbol(containingTypeSyntax) is not INamedTypeSymbol containingType)
                return;

            if (EnumerateEventNames(containingType).Any(name => name == passedName))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, firstArgument.GetLocation(), passedName));
        }

        private static IEnumerable<string> EnumerateEventNames(INamedTypeSymbol type)
        {
            for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
            {
                foreach (var eventSymbol in current.GetMembers().OfType<IEventSymbol>())
                {
                    var displayName = eventSymbol.GetAttributes()
                        .FirstOrDefault(attr => attr.AttributeClass?.Name == "DisplayNameAttribute")
                        ?.ConstructorArguments.FirstOrDefault().Value as string;
                    yield return displayName ?? eventSymbol.Name;
                }
            }
        }

        private static bool IsRuntimeNotify(IMethodSymbol method)
        {
            return method.Name == "Notify" &&
                   method.ContainingType?.Name == "Runtime" &&
                   method.ContainingType.ContainingNamespace?.ToDisplayString() == "Neo.SmartContract.Framework.Services";
        }
    }
}
