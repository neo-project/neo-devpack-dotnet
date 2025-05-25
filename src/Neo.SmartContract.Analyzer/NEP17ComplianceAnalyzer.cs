// Copyright (C) 2015-2025 The Neo Project.
//
// NEP17ComplianceAnalyzer.cs file belongs to the neo project and is free
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
    public class NEP17ComplianceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4030";

        private static readonly LocalizableString Title = "NEP-17 compliance";
        private static readonly LocalizableString MessageFormat = "NEP-17 token standard requires method: {0}";
        private static readonly LocalizableString Description = "NEP-17 token standard requires specific methods to be implemented.";
        private const string Category = "Standard";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        // Required methods for NEP-17 compliance
        private static readonly Dictionary<string, (string[] Parameters, string ReturnType)> RequiredMethods = new()
        {
            { "symbol", (new string[] { }, "string") },
            { "decimals", (new string[] { }, "byte") },
            { "totalSupply", (new string[] { }, "BigInteger") },
            { "balanceOf", (new string[] { "UInt160" }, "BigInteger") },
            { "transfer", (new string[] { "UInt160", "UInt160", "BigInteger", "object" }, "bool") }
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Check if the class has the SupportedStandards attribute with NEP-17
            if (!HasNEP17SupportedStandard(namedTypeSymbol))
                return;

            // Check if all required methods are implemented
            foreach (var requiredMethod in RequiredMethods)
            {
                if (!HasRequiredMethod(namedTypeSymbol, requiredMethod.Key, requiredMethod.Value.Parameters, requiredMethod.Value.ReturnType))
                {
                    var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0],
                        $"{requiredMethod.Key}({string.Join(", ", requiredMethod.Value.Parameters)}) -> {requiredMethod.Value.ReturnType}");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private bool HasNEP17SupportedStandard(INamedTypeSymbol typeSymbol)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.Name == "SupportedStandardsAttribute")
                {
                    foreach (var arg in attribute.ConstructorArguments)
                    {
                        if (arg.Kind == TypedConstantKind.Array)
                        {
                            foreach (var value in arg.Values)
                            {
                                if (value.Value?.ToString() == "NepStandard.Nep17" ||
                                    value.Value?.ToString() == "Neo.SmartContract.Framework.NepStandard.Nep17")
                                {
                                    return true;
                                }
                            }
                        }
                        else if (arg.Value?.ToString() == "NepStandard.Nep17" ||
                                 arg.Value?.ToString() == "Neo.SmartContract.Framework.NepStandard.Nep17")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasRequiredMethod(INamedTypeSymbol typeSymbol, string methodName, string[] parameterTypes, string returnType)
        {
            foreach (var member in typeSymbol.GetMembers())
            {
                if (member is IMethodSymbol method &&
                    method.Name.ToLowerInvariant() == methodName.ToLowerInvariant() &&
                    method.IsStatic &&
                    method.DeclaredAccessibility == Accessibility.Public)
                {
                    // Check return type
                    if (!method.ReturnType.Name.EndsWith(returnType, System.StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Check parameters
                    if (method.Parameters.Length != parameterTypes.Length)
                        continue;

                    bool parametersMatch = true;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (!method.Parameters[i].Type.Name.EndsWith(parameterTypes[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            parametersMatch = false;
                            break;
                        }
                    }

                    if (parametersMatch)
                        return true;
                }
            }

            return false;
        }
    }
}
