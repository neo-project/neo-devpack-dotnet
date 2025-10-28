// Copyright (C) 2015-2025 The Neo Project.
//
// NepStandardImplementationAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NepStandardImplementationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4029";
    public const string InterfaceDiagnosticId = "NC4030";

    private const string StandardPropertyName = "Standard";
    private const string MissingMembersPropertyName = "MissingMembers";
    private const string InterfacePropertyName = "Interface";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Supported standard requires mandatory API surface",
        "Contract declares support for {0} but is missing required members or interfaces: {1}",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "When a contract declares support for NEP standards it must expose the required members or implement the canonical interface.");

    private static readonly DiagnosticDescriptor InterfaceRule = new(
        InterfaceDiagnosticId,
        "Supported standard requires interface implementation",
        "Contract declares support for {0} but does not implement required interface {1}",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Certain NEP standards mandate that contracts implement a specific interface.");

    private static readonly ImmutableArray<string> Nep17RequiredMembers = ImmutableArray.Create(
        "Symbol",
        "Decimals",
        "TotalSupply",
        "BalanceOf",
        "Transfer");

    private static readonly ImmutableArray<string> Nep11AdditionalMembers = ImmutableArray.Create(
        "OwnerOf",
        "Properties",
        "Tokens",
        "TokensOf");

    private static readonly ImmutableDictionary<NepStandardKind, ImmutableArray<string>> RequiredMembersByStandard =
        new Dictionary<NepStandardKind, ImmutableArray<string>>
        {
            { NepStandardKind.Nep17, Nep17RequiredMembers },
            { NepStandardKind.Nep11, Nep17RequiredMembers.AddRange(Nep11AdditionalMembers) }
        }.ToImmutableDictionary();

    private static readonly ImmutableDictionary<NepStandardKind, string> RequiredInterfaceByStandard =
        new Dictionary<NepStandardKind, string>
        {
            { NepStandardKind.Nep24, "Neo.SmartContract.Framework.Interfaces.INep24" },
            { NepStandardKind.Nep26, "Neo.SmartContract.Framework.Interfaces.INEP26" },
            { NepStandardKind.Nep27, "Neo.SmartContract.Framework.Interfaces.INEP27" },
            { NepStandardKind.Nep29, "Neo.SmartContract.Framework.Interfaces.INEP29" },
            { NepStandardKind.Nep30, "Neo.SmartContract.Framework.Interfaces.INEP30" }
        }.ToImmutableDictionary();

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule, InterfaceRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedType ||
            namedType.TypeKind != TypeKind.Class ||
            namedType.IsAbstract)
        {
            return;
        }

        var supportedAttribute = namedType.GetAttributes()
            .FirstOrDefault(attr =>
                attr.AttributeClass?.ToDisplayString() ==
                "Neo.SmartContract.Framework.Attributes.SupportedStandardsAttribute");

        if (supportedAttribute is null)
            return;

        var supportedStandards = ResolveStandards(supportedAttribute);
        if (supportedStandards.Count == 0)
            return;

        foreach (var standard in supportedStandards)
        {
            if (RequiredInterfaceByStandard.TryGetValue(standard, out var interfaceName))
            {
                if (!ImplementsInterface(namedType, interfaceName))
                {
                    ReportInterfaceDiagnostic(context, supportedAttribute, namedType, standard, interfaceName);
                }
            }

            if (!RequiredMembersByStandard.TryGetValue(standard, out var requiredMembers) || requiredMembers.IsDefaultOrEmpty)
                continue;

            var missingMembers = FindMissingMembers(namedType, requiredMembers, standard);
            if (missingMembers.Count == 0)
                continue;

            ReportMemberDiagnostic(context, supportedAttribute, namedType, standard, missingMembers);
        }
    }

    private static List<string> FindMissingMembers(INamedTypeSymbol typeSymbol, ImmutableArray<string> requiredMembers, NepStandardKind standard)
    {
        var missing = new List<string>();

        foreach (var memberName in requiredMembers)
        {
            if (HasMember(typeSymbol, memberName, standard))
                continue;

            missing.Add(memberName);
        }

        return missing;
    }

    private static bool HasMember(INamedTypeSymbol typeSymbol, string memberName, NepStandardKind standard)
    {
        bool Predicate(IMethodSymbol method) =>
            !method.IsImplicitlyDeclared &&
            method.Parameters.Length switch
            {
                0 when memberName is "Symbol" or "Decimals" or "TotalSupply" or "Tokens" => true,
                1 when memberName is "BalanceOf" or "OwnerOf" or "TokensOf" or "Properties" => true,
                3 when memberName == "Transfer" && standard == NepStandardKind.Nep11 => true,
                4 when memberName == "Transfer" && standard == NepStandardKind.Nep17 => true,
                _ => false
            };

        bool PropertyPredicate(IPropertySymbol property) =>
            !property.IsImplicitlyDeclared &&
            memberName is "Symbol" or "Decimals" or "TotalSupply";

        for (var current = typeSymbol; current is not null; current = current.BaseType)
        {
            foreach (var member in current.GetMembers(memberName))
            {
                if (member is IMethodSymbol method)
                {
                    if (Predicate(method))
                        return true;
                }
                else if (member is IPropertySymbol property)
                {
                    if (PropertyPredicate(property))
                        return true;
                }
            }
        }

        return false;
    }

    private static void ReportMemberDiagnostic(SymbolAnalysisContext context, AttributeData supportedAttribute, INamedTypeSymbol namedType, NepStandardKind standard, List<string> missingMembers)
    {
        var properties = ImmutableDictionary<string, string?>.Empty
            .Add(StandardPropertyName, FormatStandardName(standard))
            .Add(MissingMembersPropertyName, string.Join(",", missingMembers));

        var location = supportedAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) switch
        {
            AttributeSyntax syntax => syntax.GetLocation(),
            _ => namedType.Locations.FirstOrDefault()
        } ?? namedType.Locations.FirstOrDefault();

        if (location is null)
            return;

        var diagnostic = Diagnostic.Create(
            Rule,
            location,
            properties,
            FormatStandardName(standard),
            string.Join(", ", missingMembers));

        context.ReportDiagnostic(diagnostic);
    }

    private static void ReportInterfaceDiagnostic(SymbolAnalysisContext context, AttributeData supportedAttribute, INamedTypeSymbol namedType, NepStandardKind standard, string interfaceName)
    {
        var location = supportedAttribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) switch
        {
            AttributeSyntax syntax => syntax.GetLocation(),
            _ => namedType.Locations.FirstOrDefault()
        } ?? namedType.Locations.FirstOrDefault();

        if (location is null)
            return;

        var diagnostic = Diagnostic.Create(
            InterfaceRule,
            location,
            ImmutableDictionary<string, string?>.Empty
                .Add(StandardPropertyName, FormatStandardName(standard))
                .Add(InterfacePropertyName, interfaceName),
            FormatStandardName(standard),
            interfaceName);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool ImplementsInterface(INamedTypeSymbol typeSymbol, string interfaceMetadataName) =>
        typeSymbol.AllInterfaces.Any(i => i.ToDisplayString() == interfaceMetadataName);

    private static HashSet<NepStandardKind> ResolveStandards(AttributeData attributeData)
    {
        var result = new HashSet<NepStandardKind>();

        foreach (var argument in attributeData.ConstructorArguments)
        {
            switch (argument.Kind)
            {
                case TypedConstantKind.Array:
                    foreach (var item in argument.Values)
                        TryAddStandard(item, result);
                    break;
                default:
                    TryAddStandard(argument, result);
                    break;
            }
        }

        return result;
    }

    private static void TryAddStandard(TypedConstant constant, ISet<NepStandardKind> standards)
    {
        if (constant.IsNull)
            return;

        if (constant.Type is null)
            return;

        if (constant.Type.TypeKind == TypeKind.Enum &&
            constant.Type.ToDisplayString() == "Neo.SmartContract.Framework.NepStandard")
        {
            var enumValue = constant.Value?.ToString();
            if (Enum.TryParse<NepStandardKind>(enumValue, ignoreCase: true, out var standard))
                standards.Add(standard);
            return;
        }

        if (constant.Type.SpecialType == SpecialType.System_String &&
            constant.Value is string stringValue)
        {
            var normalized = new string(stringValue
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpperInvariant)
                .ToArray());

            switch (normalized)
            {
                case "NEP11":
                    standards.Add(NepStandardKind.Nep11);
                    break;
                case "NEP17":
                    standards.Add(NepStandardKind.Nep17);
                    break;
                case "NEP24":
                    standards.Add(NepStandardKind.Nep24);
                    break;
                case "NEP26":
                    standards.Add(NepStandardKind.Nep26);
                    break;
                case "NEP27":
                    standards.Add(NepStandardKind.Nep27);
                    break;
                case "NEP29":
                    standards.Add(NepStandardKind.Nep29);
                    break;
                case "NEP30":
                    standards.Add(NepStandardKind.Nep30);
                    break;
            }
        }
    }

    private static string FormatStandardName(NepStandardKind standard) =>
        standard switch
        {
            NepStandardKind.Nep11 => "NEP-11",
            NepStandardKind.Nep17 => "NEP-17",
            NepStandardKind.Nep24 => "NEP-24",
            NepStandardKind.Nep26 => "NEP-26",
            NepStandardKind.Nep27 => "NEP-27",
            NepStandardKind.Nep29 => "NEP-29",
            NepStandardKind.Nep30 => "NEP-30",
            _ => standard.ToString()
        };

    private enum NepStandardKind
    {
        Unknown,
        Nep11,
        Nep17,
        Nep24,
        Nep26,
        Nep27,
        Nep29,
        Nep30
    }
}
