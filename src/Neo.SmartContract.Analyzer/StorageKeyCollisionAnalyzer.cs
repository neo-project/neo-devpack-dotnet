// Copyright (C) 2015-2026 The Neo Project.
//
// StorageKeyCollisionAnalyzer.cs file belongs to the neo project and is free
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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class StorageKeyCollisionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4056";
        private const string Category = "Security";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Duplicate storage prefix may collide",
            "Storage prefix '{0}' used by '{1}' collides with '{2}'",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Duplicate constant StorageMap/LocalStorageMap prefixes in the same contract can cause storage namespace collisions.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken) is not INamedTypeSymbol typeSymbol ||
                typeSymbol.TypeKind != TypeKind.Class)
                return;

            Dictionary<string, PrefixUsage> seenPrefixes = new(StringComparer.Ordinal);
            foreach (VariableDeclaratorSyntax declarator in classDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .SelectMany(fieldDeclaration => fieldDeclaration.Declaration.Variables))
            {
                if (context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken) is not IFieldSymbol field)
                    continue;

                if (!IsStorageNamespaceType(field.Type))
                    continue;

                if (!TryGetPrefixExpression(
                        declarator.Initializer?.Value,
                        context.SemanticModel,
                        context.CancellationToken,
                        new HashSet<ISymbol>(SymbolEqualityComparer.Default),
                        out ExpressionSyntax? prefixExpression))
                {
                    continue;
                }

                if (prefixExpression is null)
                    continue;

                if (!TryNormalizePrefix(prefixExpression, context.SemanticModel, context.CancellationToken, new HashSet<ISymbol>(SymbolEqualityComparer.Default), out string normalizedPrefix))
                    continue;

                if (seenPrefixes.TryGetValue(normalizedPrefix, out PrefixUsage existing))
                {
                    if (!SymbolEqualityComparer.Default.Equals(existing.Field, field))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Rule,
                            declarator.Identifier.GetLocation(),
                            normalizedPrefix,
                            field.Name,
                            existing.Field.Name));
                    }

                    continue;
                }

                seenPrefixes[normalizedPrefix] = new PrefixUsage(field);
            }
        }

        private static bool IsStorageNamespaceType(ITypeSymbol typeSymbol)
        {
            string fullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return fullName is "global::Neo.SmartContract.Framework.Services.StorageMap"
                or "global::Neo.SmartContract.Framework.Services.LocalStorageMap";
        }

        private static bool TryGetPrefixExpression(
            ExpressionSyntax? initializerValue,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out ExpressionSyntax? prefixExpression)
        {
            prefixExpression = null;

            if (initializerValue is null)
                return false;

            if (initializerValue is BaseObjectCreationExpressionSyntax creation)
            {
                if (creation.ArgumentList is null || creation.ArgumentList.Arguments.Count == 0)
                    return false;

                prefixExpression = creation.ArgumentList.Arguments[creation.ArgumentList.Arguments.Count - 1].Expression;
                return true;
            }

            if (initializerValue is not InvocationExpressionSyntax invocation)
                return false;

            if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol methodSymbol)
                return false;

            if (!IsStorageNamespaceType(methodSymbol.ReturnType))
                return false;

            if (methodSymbol.Parameters.Length != 0)
                return false;

            if (!visitedSymbols.Add(methodSymbol))
                return false;

            foreach (SyntaxReference syntaxReference in methodSymbol.DeclaringSyntaxReferences)
            {
                if (syntaxReference.SyntaxTree != semanticModel.SyntaxTree)
                    continue;

                if (syntaxReference.GetSyntax(cancellationToken) is not MethodDeclarationSyntax methodDeclaration)
                    continue;

                ExpressionSyntax? returnedExpression = methodDeclaration.ExpressionBody?.Expression;
                if (returnedExpression is null &&
                    methodDeclaration.Body?.Statements.Count == 1 &&
                    methodDeclaration.Body.Statements[0] is ReturnStatementSyntax returnStatement)
                {
                    returnedExpression = returnStatement.Expression;
                }

                if (returnedExpression is null)
                    continue;

                if (TryGetPrefixExpression(
                        returnedExpression,
                        semanticModel,
                        cancellationToken,
                        visitedSymbols,
                        out prefixExpression))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryNormalizePrefix(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out string normalizedPrefix)
        {
            normalizedPrefix = string.Empty;

            if (TryGetByteSequence(expression, semanticModel, cancellationToken, visitedSymbols, out byte[] bytes))
            {
                normalizedPrefix = BitConverter.ToString(bytes).Replace("-", string.Empty);
                return true;
            }

            return false;
        }

        private static bool TryGetByteSequence(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out byte[] bytes)
        {
            bytes = Array.Empty<byte>();

            switch (expression)
            {
                case LiteralExpressionSyntax literal:
                    return TryGetLiteralBytes(literal.Token.Value, out bytes);
                case CastExpressionSyntax castExpression:
                    return TryGetByteSequence(castExpression.Expression, semanticModel, cancellationToken, visitedSymbols, out bytes);
                case PrefixUnaryExpressionSyntax unaryExpression:
                    return TryGetByteSequence(unaryExpression.Operand, semanticModel, cancellationToken, visitedSymbols, out bytes);
                case ArrayCreationExpressionSyntax arrayCreation when arrayCreation.Initializer is not null:
                    return TryGetByteArray(arrayCreation.Initializer.Expressions, semanticModel, cancellationToken, visitedSymbols, out bytes);
                case ImplicitArrayCreationExpressionSyntax implicitArray when implicitArray.Initializer is not null:
                    return TryGetByteArray(implicitArray.Initializer.Expressions, semanticModel, cancellationToken, visitedSymbols, out bytes);
            }

            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(expression, cancellationToken);
            if (symbolInfo.Symbol is null && semanticModel.GetConstantValue(expression, cancellationToken) is { HasValue: true } constantValue)
            {
                return TryGetLiteralBytes(constantValue.Value, out bytes);
            }

            if (symbolInfo.Symbol is IFieldSymbol fieldSymbol)
            {
                if (!visitedSymbols.Add(fieldSymbol))
                    return false;

                if (fieldSymbol.HasConstantValue && TryGetLiteralBytes(fieldSymbol.ConstantValue, out bytes))
                    return true;

                SyntaxReference? syntaxReference = fieldSymbol.DeclaringSyntaxReferences.FirstOrDefault(reference => reference.SyntaxTree == semanticModel.SyntaxTree);
                if (syntaxReference?.GetSyntax(cancellationToken) is VariableDeclaratorSyntax declarator &&
                    declarator.Initializer is not null)
                {
                    return TryGetByteSequence(declarator.Initializer.Value, semanticModel, cancellationToken, visitedSymbols, out bytes);
                }
            }

            return false;
        }

        private static bool TryGetByteArray(
            SeparatedSyntaxList<ExpressionSyntax> expressions,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out byte[] bytes)
        {
            List<byte> values = new(expressions.Count);
            foreach (ExpressionSyntax expression in expressions)
            {
                if (!TryGetByteSequence(expression, semanticModel, cancellationToken, visitedSymbols, out byte[] elementBytes))
                {
                    bytes = Array.Empty<byte>();
                    return false;
                }

                if (elementBytes.Length != 1)
                {
                    bytes = Array.Empty<byte>();
                    return false;
                }

                values.Add(elementBytes[0]);
            }

            bytes = values.ToArray();
            return true;
        }

        private static bool TryGetLiteralBytes(object? value, out byte[] bytes)
        {
            switch (value)
            {
                case null:
                    bytes = Array.Empty<byte>();
                    return false;
                case string text:
                    bytes = Encoding.UTF8.GetBytes(text);
                    return true;
                case byte byteValue:
                    bytes = [byteValue];
                    return true;
                case sbyte sbyteValue:
                    bytes = [unchecked((byte)sbyteValue)];
                    return true;
                case short shortValue when shortValue >= byte.MinValue && shortValue <= byte.MaxValue:
                    bytes = [(byte)shortValue];
                    return true;
                case ushort ushortValue when ushortValue <= byte.MaxValue:
                    bytes = [(byte)ushortValue];
                    return true;
                case int intValue when intValue >= byte.MinValue && intValue <= byte.MaxValue:
                    bytes = [(byte)intValue];
                    return true;
                case uint uintValue when uintValue <= byte.MaxValue:
                    bytes = [(byte)uintValue];
                    return true;
                case long longValue when longValue >= byte.MinValue && longValue <= byte.MaxValue:
                    bytes = [(byte)longValue];
                    return true;
                case ulong ulongValue when ulongValue <= byte.MaxValue:
                    bytes = [(byte)ulongValue];
                    return true;
                default:
                    bytes = Array.Empty<byte>();
                    return false;
            }
        }

        private sealed class PrefixUsage
        {
            public PrefixUsage(IFieldSymbol field)
            {
                Field = field;
            }

            public IFieldSymbol Field { get; }
        }
    }
}
