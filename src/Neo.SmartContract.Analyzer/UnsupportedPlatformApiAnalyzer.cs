// Copyright (C) 2015-2026 The Neo Project.
//
// UnsupportedPlatformApiAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsupportedPlatformApiAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4058";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Unsupported platform API is used",
        "Neo smart contracts do not support platform API: {0}",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly string[] ForbiddenNamespaces =
    [
        "System.IO",
        "System.Net",
        "System.Reflection",
        "System.Threading"
    ];

    private static readonly string[] ForbiddenSystemTypes =
    [
        "Activator",
        "Console",
        "DateTime",
        "DateTimeOffset",
        "Environment",
        "GC",
        "Guid",
        "Random",
        "TimeSpan",
        "Type"
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
        context.RegisterSyntaxNodeAction(AnalyzeQualifiedName, SyntaxKind.QualifiedName);
        context.RegisterSyntaxNodeAction(AnalyzeIdentifierName, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not UsingDirectiveSyntax usingDirective || usingDirective.Name is null)
        {
            return;
        }

        var name = usingDirective.Name.ToString();
        if (IsForbiddenNamespace(name))
        {
            Report(context, usingDirective.GetLocation(), name);
        }
    }

    private static void AnalyzeQualifiedName(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not QualifiedNameSyntax qualifiedName)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(qualifiedName, context.CancellationToken).Type;
        ReportIfForbiddenType(context, qualifiedName.GetLocation(), type);
    }

    private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not IdentifierNameSyntax identifier ||
            identifier.IsVar ||
            identifier.Parent is QualifiedNameSyntax or MemberAccessExpressionSyntax or UsingDirectiveSyntax)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(identifier, context.CancellationToken).Type;
        ReportIfForbiddenType(context, identifier.GetLocation(), type);
    }

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ObjectCreationExpressionSyntax objectCreation)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(objectCreation, context.CancellationToken).Type;
        ReportIfForbiddenType(context, objectCreation.Type.GetLocation(), type);
    }

    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
        if (symbol is INamedTypeSymbol type)
        {
            ReportIfForbiddenType(context, memberAccess.GetLocation(), type);
        }
    }

    private static void ReportIfForbiddenType(SyntaxNodeAnalysisContext context, Location location, ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol namedType || !IsForbiddenType(namedType))
        {
            return;
        }

        Report(context, location, namedType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
    }

    private static bool IsForbiddenType(INamedTypeSymbol type)
    {
        var namespaceName = type.ContainingNamespace.ToDisplayString();
        if (IsForbiddenNamespace(namespaceName))
        {
            return true;
        }

        return namespaceName == "System" && Array.IndexOf(ForbiddenSystemTypes, type.Name) >= 0;
    }

    private static bool IsForbiddenNamespace(string namespaceName)
    {
        foreach (var forbiddenNamespace in ForbiddenNamespaces)
        {
            if (namespaceName == forbiddenNamespace ||
                namespaceName.StartsWith(forbiddenNamespace + ".", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static void Report(SyntaxNodeAnalysisContext context, Location location, string api)
    {
        var diagnostic = Diagnostic.Create(Rule, location, api);
        context.ReportDiagnostic(diagnostic);
    }
}
