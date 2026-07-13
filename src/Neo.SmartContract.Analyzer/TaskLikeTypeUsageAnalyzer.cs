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
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
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

    private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not PropertyDeclarationSyntax propertyDeclaration)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(propertyDeclaration.Type, context.CancellationToken).Type;
        ReportIfTaskLike(context, propertyDeclaration.Type.GetLocation(), type);
    }

    private static void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not VariableDeclarationSyntax variableDeclaration)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(variableDeclaration.Type, context.CancellationToken).Type;
        ReportIfTaskLike(context, variableDeclaration.Type.GetLocation(), type);
    }

    private static void ReportIfTaskLike(SyntaxNodeAnalysisContext context, Location location, ITypeSymbol? type)
    {
        if (FindTaskLikeType(type) is not { } taskLikeType)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, location, taskLikeType.ToDisplayString());
        context.ReportDiagnostic(diagnostic);
    }

    private static INamedTypeSymbol? FindTaskLikeType(ITypeSymbol? type)
    {
        switch (type)
        {
            case IArrayTypeSymbol arrayType:
                return FindTaskLikeType(arrayType.ElementType);
            case IFunctionPointerTypeSymbol functionPointerType:
                if (FindTaskLikeType(functionPointerType.Signature.ReturnType) is { } returnType)
                {
                    return returnType;
                }

                foreach (var parameter in functionPointerType.Signature.Parameters)
                {
                    if (FindTaskLikeType(parameter.Type) is { } parameterType)
                    {
                        return parameterType;
                    }
                }

                return null;
            case INamedTypeSymbol namedType:
                if (IsTaskLikeType(namedType))
                {
                    return namedType;
                }

                if (FindTaskLikeType(namedType.ContainingType) is { } containingTypeMatch)
                {
                    return containingTypeMatch;
                }

                foreach (var typeArgument in namedType.TypeArguments)
                {
                    if (FindTaskLikeType(typeArgument) is { } typeArgumentMatch)
                    {
                        return typeArgumentMatch;
                    }
                }

                return null;
            default:
                return null;
        }
    }

    private static bool IsTaskLikeType(INamedTypeSymbol namedType)
    {
        var originalType = namedType.OriginalDefinition;
        if (originalType.ContainingNamespace.ToDisplayString() != "System.Threading.Tasks")
        {
            return false;
        }

        return originalType.MetadataName is "Task" or "Task`1" or "ValueTask" or "ValueTask`1";
    }
}
