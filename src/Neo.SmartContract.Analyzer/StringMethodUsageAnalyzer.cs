// Copyright (C) 2015-2026 The Neo Project.
//
// StringMethodUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringMethodUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4007";

        // Add string method names to this array as needed
        private readonly string[] _unsupportedStringMethods =
        {
            "Clone", "CompareOrdinal", "CompareTo",
            "Copy", "CopyTo", "Format", "GetEnumerator",
            "GetHashCode", "GetType", "GetTypeCode",
            "IndexOfAny", "Intern", "IsInterned", "IsNormalized",
            "Join", "LastIndexOfAny", "Normalize",
            "ToCharArray", "ToLowerInvariant", "ToUpperInvariant"
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported string method is used",
            "Unsupported string method: {0}",
            "Method",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpression) return;

            // Check if the method belongs to String class or is an Object method called on string
            if (context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is not IMethodSymbol memberSymbol)
                return;

            var isUnsupportedMethod = _unsupportedStringMethods.Contains(memberSymbol.Name);
            var containingType = memberSymbol.ContainingType?.SpecialType;
            if (containingType == SpecialType.System_String)
            {
                if (!isUnsupportedMethod &&
                    !HasUnsupportedTrimArrayArgument(
                        memberSymbol,
                        invocationExpression,
                        context.SemanticModel,
                        context.CancellationToken))
                    return;
            }
            else if (containingType == SpecialType.System_Object)
            {
                if (!isUnsupportedMethod)
                    return;

                // Object method - only report if called on a string instance
                var receiverType = GetReceiverType(context, invocationExpression);
                if (receiverType?.SpecialType != SpecialType.System_String)
                    return;
            }
            else
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), memberSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool HasUnsupportedTrimArrayArgument(
            IMethodSymbol method,
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            System.Threading.CancellationToken cancellationToken)
        {
            if (method.Name is not ("Trim" or "TrimStart" or "TrimEnd") ||
                method.Parameters.Length != 1 ||
                method.Parameters[0].Type is not IArrayTypeSymbol { Rank: 1 } arrayType ||
                arrayType.ElementType.SpecialType != SpecialType.System_Char)
                return false;

            var arguments = invocation.ArgumentList.Arguments;
            if (arguments.Count != 1)
                return true;

            var initializer = arguments[0].Expression switch
            {
                ArrayCreationExpressionSyntax { Initializer: { } value } => value,
                ImplicitArrayCreationExpressionSyntax { Initializer: { } value } => value,
                InitializerExpressionSyntax value => value,
                _ => null
            };

            if (initializer?.Expressions.Count != 1)
                return true;

            var constant = semanticModel.GetConstantValue(initializer.Expressions[0], cancellationToken);
            return !constant.HasValue || constant.Value is not char and not (string { Length: 1 });
        }

        private static ITypeSymbol? GetReceiverType(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (invocationExpression.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                return context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type;
            }
            return null;
        }
    }
}
