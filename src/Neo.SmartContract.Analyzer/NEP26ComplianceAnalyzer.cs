// Copyright (C) 2015-2025 The Neo Project.
//
// NEP26ComplianceAnalyzer.cs file belongs to the neo project and is free
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
    public class NEP26ComplianceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4038";

        private static readonly LocalizableString Title = "NEP-26 compliance";
        private static readonly LocalizableString MessageFormat = "NEP-26 standard requires method: {0}";
        private static readonly LocalizableString Description = "NEP-26 standard requires specific methods to be implemented.";
        private const string Category = "Standard";

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
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Check if the class has the SupportedStandards attribute with NEP-26
            if (!HasNEP26SupportedStandard(namedTypeSymbol))
                return;

            // Check if the OnNEP11Payment method is implemented
            if (!HasOnNEP11PaymentMethod(namedTypeSymbol))
            {
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0],
                    "OnNEP11Payment(UInt160 from, BigInteger amount, string tokenId, object? data = null)");
                context.ReportDiagnostic(diagnostic);
            }

            // Check if the class implements INEP26 interface
            if (!ImplementsINEP26Interface(namedTypeSymbol))
            {
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0],
                    "Class should implement INEP26 interface");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool HasNEP26SupportedStandard(INamedTypeSymbol typeSymbol)
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
                                if (value.Value?.ToString() == "NepStandard.Nep26" ||
                                    value.Value?.ToString() == "Neo.SmartContract.Framework.NepStandard.Nep26")
                                {
                                    return true;
                                }
                            }
                        }
                        else if (arg.Value?.ToString() == "NepStandard.Nep26" ||
                                 arg.Value?.ToString() == "Neo.SmartContract.Framework.NepStandard.Nep26")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasOnNEP11PaymentMethod(INamedTypeSymbol typeSymbol)
        {
            foreach (var member in typeSymbol.GetMembers())
            {
                if (member is IMethodSymbol method &&
                    method.Name == "OnNEP11Payment" &&
                    method.DeclaredAccessibility == Accessibility.Public)
                {
                    // Check parameters
                    if (method.Parameters.Length >= 3)
                    {
                        var param1 = method.Parameters[0];
                        var param2 = method.Parameters[1];
                        var param3 = method.Parameters[2];

                        if (param1.Type.Name.EndsWith("UInt160") &&
                            param2.Type.Name.EndsWith("BigInteger") &&
                            param3.Type.Name.EndsWith("String"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool ImplementsINEP26Interface(INamedTypeSymbol typeSymbol)
        {
            foreach (var iface in typeSymbol.AllInterfaces)
            {
                if (iface.Name == "INEP26")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
