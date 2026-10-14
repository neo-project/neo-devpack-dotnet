// Copyright (C) 2015-2026 The Neo Project.
//
// SmartContractMethodNamingAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractMethodNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4018";

        private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorFactory.Create(
            DiagnosticId,
            "Method Naming Conflict in Smart Contract",
            "Method '{0}' has a naming conflict. Consider renaming to avoid conflict.",
            "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            if (context.Symbol is not INamedTypeSymbol classSymbol || !InheritsFromSmartContract(classSymbol))
                return;

            var methods = classSymbol.GetMembers().OfType<IMethodSymbol>()
                .Where(method => method.DeclaredAccessibility == Accessibility.Public).ToArray();
            var reported = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            // The compiler first rejects ambiguous C# name/arity pairs, then exported ABI names.
            ReportConflicts(methods.GroupBy(method => (method.Name, method.Parameters.Length)));
            if (classSymbol is { IsAbstract: false, DeclaredAccessibility: Accessibility.Public })
            {
                ReportConflicts(methods.Where(method => !method.IsAbstract && method.MethodKind is
                        MethodKind.Ordinary or MethodKind.PropertyGet or MethodKind.PropertySet)
                    .GroupBy(method => (GetAbiName(method), method.Parameters.Length)));
            }

            void ReportConflicts(IEnumerable<IGrouping<(string, int), IMethodSymbol>> groups)
            {
                foreach (var group in groups.Where(group => group.Count() > 1))
                {
                    foreach (var method in group)
                    {
                        if (!reported.Add(method) || method.DeclaringSyntaxReferences.FirstOrDefault() is not { } syntax)
                            continue;
                        context.ReportDiagnostic(Diagnostic.Create(Rule,
                            syntax.GetSyntax(context.CancellationToken).GetLocation(), method.Name));
                    }
                }
            }
        }

        private static string GetAbiName(IMethodSymbol method)
        {
            static string? DisplayName(ISymbol symbol) => symbol.GetAttributes()
                .FirstOrDefault(attribute => attribute.AttributeClass?.Name == "DisplayNameAttribute")?
                .ConstructorArguments.FirstOrDefault().Value as string;

            if (DisplayName(method) is { } name) return name;
            if (method.MethodKind == MethodKind.PropertySet) return "set" + method.Name.Substring(4);
            ISymbol member = method.MethodKind == MethodKind.PropertyGet ? method.AssociatedSymbol! : method;
            if (DisplayName(member) is { } memberName) return memberName;
            var value = member.Name;
            return value.Length > 1 && value[0] == '_'
                ? "_" + char.ToLowerInvariant(value[1]) + value.Substring(2)
                : char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static bool InheritsFromSmartContract(INamedTypeSymbol? symbol)
        {
            while (symbol != null)
            {
                if (symbol.BaseType?.Name == "SmartContract" &&
                    symbol.BaseType.ContainingNamespace?.ToString() == "Neo.SmartContract.Framework")
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }
    }
}
