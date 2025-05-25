// Copyright (C) 2015-2025 The Neo Project.
//
// NumericMethodsUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class NumericMethodsUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4029";

        // Dictionary of numeric types and their unsupported methods
        // Based on the methods implemented in src\Neo.Compiler.CSharp\MethodConvert\System\
        private readonly Dictionary<string, string[]> _unsupportedNumericMethods = new Dictionary<string, string[]>
        {
            {
                "byte", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                }
            },
            {
                "sbyte", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                    // - CopySign
                }
            },
            {
                "short", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                    // - CopySign
                }
            },
            {
                "ushort", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                }
            },
            {
                "int", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                    // - CopySign
                }
            },
            {
                "uint", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                }
            },
            {
                "long", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                    // - CopySign
                }
            },
            {
                "ulong", new[]
                {
                    "GetHashCode", "GetType", "GetTypeCode", "TryFormat"
                    // Supported methods (not in this list):
                    // - Parse
                    // - ToString
                    // - RotateLeft
                    // - RotateRight
                    // - PopCount
                    // - LeadingZeroCount
                    // - TrailingZeroCount
                }
            }
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported numeric method is used",
            "Unsupported method '{0}' on type '{1}'",
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

            // Get the method symbol
            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (methodSymbol == null) return;

            // Check if the method is on a numeric type
            var containingType = methodSymbol.ContainingType?.SpecialType.ToString();
            if (containingType == null) return;

            string numericType = null;
            switch (containingType)
            {
                case "System_Byte":
                    numericType = "byte";
                    break;
                case "System_SByte":
                    numericType = "sbyte";
                    break;
                case "System_Int16":
                    numericType = "short";
                    break;
                case "System_UInt16":
                    numericType = "ushort";
                    break;
                case "System_Int32":
                    numericType = "int";
                    break;
                case "System_UInt32":
                    numericType = "uint";
                    break;
                case "System_Int64":
                    numericType = "long";
                    break;
                case "System_UInt64":
                    numericType = "ulong";
                    break;
                default:
                    return;
            }

            // Check if the method is unsupported
            if (_unsupportedNumericMethods.TryGetValue(numericType, out var unsupportedMethods) &&
                unsupportedMethods.Contains(methodSymbol.Name))
            {
                var diagnostic = Diagnostic.Create(Rule, invocationExpression.GetLocation(), methodSymbol.Name, numericType);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
