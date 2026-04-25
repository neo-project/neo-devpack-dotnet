// Copyright (C) 2015-2026 The Neo Project.
//
// TaskLikeTypeUsageAnalyzer.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TaskLikeTypeUsageAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4057";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Usage of task-like types is not allowed in Neo smart contracts",
        "Neo smart contracts do not support task-like type: {0}",
        "Type",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration ||
            methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(methodDeclaration.ReturnType, context.CancellationToken).Type;
        ReportIfTaskLike(context, methodDeclaration.ReturnType.GetLocation(), type);
    }

    private static void AnalyzeParameter(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ParameterSyntax parameter || parameter.Type is null)
        {
            return;
        }

        var type = context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken)?.Type;
        ReportIfTaskLike(context, parameter.Type.GetLocation(), type);
    }

    private static void ReportIfTaskLike(SyntaxNodeAnalysisContext context, Location location, ITypeSymbol? type)
    {
        if (!IsTaskLikeType(type))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, location, type!.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsTaskLikeType(ITypeSymbol? type)
    {
        if (type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        var originalType = namedType.OriginalDefinition;
        if (originalType.ContainingNamespace.ToDisplayString() != "System.Threading.Tasks")
        {
            return false;
        }

        return originalType.MetadataName is "Task" or "Task`1" or "ValueTask" or "ValueTask`1";
    }
}
