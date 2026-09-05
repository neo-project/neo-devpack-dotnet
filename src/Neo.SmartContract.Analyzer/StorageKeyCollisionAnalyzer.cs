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
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
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
            description: "Duplicate constant StorageMap/LocalStorageMap prefixes in the same contract, or a prefix that reuses a reserved prefix of an inherited framework base class, can cause storage namespace collisions.",
            customTags: [WellKnownDiagnosticTags.CompilationEnd]);

        // Reserved single-byte storage prefixes used internally by framework base classes. A
        // derived contract that builds a StorageMap with one of these prefixes silently shares the
        // base class's storage namespace (e.g. corrupting the stored owner).
        private static readonly Dictionary<string, byte[]> ReservedBasePrefixes = new(StringComparer.Ordinal)
        {
            ["global::Neo.SmartContract.Framework.Ownable"] = [0xFF],
            ["global::Neo.SmartContract.Framework.Ownable2Step"] = [0xFD, 0xFC, 0xFB],
            ["global::Neo.SmartContract.Framework.Pausable"] = [0xFE],
            ["global::Neo.SmartContract.Framework.PausableOwnable"] = [0xFE],
            ["global::Neo.SmartContract.Framework.AccessControl"] = [0xFB],
            ["global::Neo.SmartContract.Framework.TokenContract"] = [0x00, 0x01],
            ["global::Neo.SmartContract.Framework.Nep17Token"] = [0x00, 0x01],
            ["global::Neo.SmartContract.Framework.Nep11Token`1"] = [0x02, 0x03, 0x04],
            ["global::Neo.SmartContract.Framework.RoyaltyNep11Token`1"] = [0x05, 0x06],
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(static startContext =>
            {
                ConcurrentBag<PrefixUsageCandidate> candidates = new();
                ConcurrentDictionary<SyntaxTree, SemanticModel> semanticModels = new();
                startContext.RegisterSemanticModelAction(semanticModelContext =>
                    semanticModels.TryAdd(
                        semanticModelContext.SemanticModel.SyntaxTree,
                        semanticModelContext.SemanticModel));
                startContext.RegisterSyntaxNodeAction(
                    syntaxContext => CollectClassDeclaration(syntaxContext, candidates),
                    SyntaxKind.ClassDeclaration);
                startContext.RegisterCompilationEndAction(
                    compilationContext => AnalyzeCollectedPrefixes(
                        compilationContext,
                        candidates,
                        semanticModels));
            });
        }

        private static void CollectClassDeclaration(
            SyntaxNodeAnalysisContext context,
            ConcurrentBag<PrefixUsageCandidate> candidates)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken) is not INamedTypeSymbol typeSymbol ||
                typeSymbol.TypeKind != TypeKind.Class)
                return;

            foreach (VariableDeclaratorSyntax declarator in classDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .SelectMany(fieldDeclaration => fieldDeclaration.Declaration.Variables))
            {
                if (context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken) is not IFieldSymbol field)
                    continue;

                if (!IsStorageNamespaceType(field.Type))
                    continue;

                if (declarator.Initializer?.Value is not ExpressionSyntax initializerValue)
                    continue;

                candidates.Add(new PrefixUsageCandidate(
                    typeSymbol,
                    field,
                    initializerValue,
                    context.SemanticModel,
                    declarator.Identifier.GetLocation()));
            }
        }

        private static void AnalyzeCollectedPrefixes(
            CompilationAnalysisContext context,
            ConcurrentBag<PrefixUsageCandidate> candidates,
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels)
        {
            Dictionary<INamedTypeSymbol, List<CollectedPrefixUsage>> usagesByType =
                new(SymbolEqualityComparer.Default);

            foreach (PrefixUsageCandidate candidate in candidates)
            {
                if (!TryGetPrefixExpression(
                        candidate.InitializerValue,
                        candidate.SemanticModel,
                        semanticModels,
                        context.CancellationToken,
                        new HashSet<ISymbol>(SymbolEqualityComparer.Default),
                        out ExpressionSyntax? prefixExpression) ||
                    prefixExpression is null)
                {
                    continue;
                }

                if (!TryNormalizePrefix(
                        prefixExpression,
                        candidate.SemanticModel,
                        semanticModels,
                        context.CancellationToken,
                        new HashSet<ISymbol>(SymbolEqualityComparer.Default),
                        out string normalizedPrefix))
                {
                    continue;
                }

                CollectedPrefixUsage usage = new(
                    candidate.Type,
                    candidate.Field,
                    normalizedPrefix,
                    candidate.Location);

                if (!usagesByType.TryGetValue(usage.Type, out List<CollectedPrefixUsage>? typeUsages))
                {
                    typeUsages = new List<CollectedPrefixUsage>();
                    usagesByType.Add(usage.Type, typeUsages);
                }

                typeUsages.Add(usage);
            }

            foreach (KeyValuePair<INamedTypeSymbol, List<CollectedPrefixUsage>> pair in usagesByType)
            {
                INamedTypeSymbol typeSymbol = pair.Key;
                List<CollectedPrefixUsage> typeUsages = pair.Value;
                Dictionary<string, PrefixUsage> seenPrefixes = new(StringComparer.Ordinal);
                SeedInheritedReservedPrefixes(typeSymbol, seenPrefixes);

                foreach (CollectedPrefixUsage usage in typeUsages
                    .OrderBy(item => item.Location.SourceTree?.FilePath ?? string.Empty, StringComparer.Ordinal)
                    .ThenBy(item => item.Location.SourceSpan.Start))
                {
                    if (seenPrefixes.TryGetValue(usage.NormalizedPrefix, out PrefixUsage existing))
                    {
                        if (!SymbolEqualityComparer.Default.Equals(existing.Field, usage.Field))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                Rule,
                                usage.Location,
                                usage.NormalizedPrefix,
                                usage.Field.Name,
                                existing.Description));
                        }

                        continue;
                    }

                    seenPrefixes[usage.NormalizedPrefix] = new PrefixUsage(usage.Field, usage.Field.Name);
                }
            }
        }

        private static void SeedInheritedReservedPrefixes(INamedTypeSymbol typeSymbol, Dictionary<string, PrefixUsage> seenPrefixes)
        {
            for (INamedTypeSymbol? baseType = typeSymbol.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                string baseName = GetReservedBaseTypeName(baseType);
                if (!ReservedBasePrefixes.TryGetValue(baseName, out byte[] reservedPrefixes))
                    continue;

                foreach (byte reserved in reservedPrefixes)
                {
                    string normalizedPrefix = reserved.ToString("X2");
                    if (seenPrefixes.ContainsKey(normalizedPrefix))
                        continue;

                    seenPrefixes[normalizedPrefix] = new PrefixUsage(
                        null,
                        $"the reserved prefix of base class {baseType.Name}");
                }
            }
        }

        private static string GetReservedBaseTypeName(INamedTypeSymbol baseType)
        {
            string namespaceName = baseType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return $"{namespaceName}.{baseType.MetadataName}";
        }

        private static bool IsStorageNamespaceType(ITypeSymbol typeSymbol)
        {
            string fullName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return fullName is "global::Neo.SmartContract.Framework.Services.StorageMap"
                or "global::Neo.SmartContract.Framework.Services.LocalStorageMap";
        }

        private static SemanticModel? GetSemanticModelForNode(
            SyntaxNode node,
            SemanticModel semanticModel,
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels)
        {
            if (node.SyntaxTree == semanticModel.SyntaxTree)
                return semanticModel;

            return semanticModels.TryGetValue(node.SyntaxTree, out SemanticModel? declaringSemanticModel) &&
                ReferenceEquals(declaringSemanticModel.Compilation, semanticModel.Compilation)
                ? declaringSemanticModel
                : null;
        }

        private static bool TryGetPrefixExpression(
            ExpressionSyntax? initializerValue,
            SemanticModel semanticModel,
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out ExpressionSyntax? prefixExpression)
        {
            prefixExpression = null;

            if (initializerValue is null)
                return false;

            SemanticModel? initializerSemanticModel = GetSemanticModelForNode(
                initializerValue,
                semanticModel,
                semanticModels);
            if (initializerSemanticModel is null)
                return false;
            semanticModel = initializerSemanticModel;

            if (initializerValue is BaseObjectCreationExpressionSyntax creation)
            {
                if (semanticModel.GetOperation(creation, cancellationToken) is not IObjectCreationOperation operation)
                    return false;

                var prefixArgument = operation.Arguments.FirstOrDefault(argument => argument.Parameter?.Name == "prefix");
                prefixExpression = prefixArgument?.Value.Syntax as ExpressionSyntax;
                return prefixExpression is not null;
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
                        semanticModels,
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
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out string normalizedPrefix)
        {
            normalizedPrefix = string.Empty;

            if (TryGetByteSequence(
                    expression,
                    semanticModel,
                    semanticModels,
                    cancellationToken,
                    visitedSymbols,
                    out byte[] bytes))
            {
                normalizedPrefix = BitConverter.ToString(bytes).Replace("-", string.Empty);
                return true;
            }

            return false;
        }

        private static bool TryGetByteSequence(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out byte[] bytes)
        {
            bytes = Array.Empty<byte>();

            SemanticModel? expressionSemanticModel = GetSemanticModelForNode(
                expression,
                semanticModel,
                semanticModels);
            if (expressionSemanticModel is null)
                return false;
            semanticModel = expressionSemanticModel;

            switch (expression)
            {
                case LiteralExpressionSyntax literal:
                    return TryGetLiteralBytes(literal.Token.Value, out bytes);
                case CastExpressionSyntax castExpression:
                    return TryGetByteSequence(castExpression.Expression, semanticModel, semanticModels, cancellationToken, visitedSymbols, out bytes);
                case PrefixUnaryExpressionSyntax unaryExpression:
                    return TryGetByteSequence(unaryExpression.Operand, semanticModel, semanticModels, cancellationToken, visitedSymbols, out bytes);
                case ArrayCreationExpressionSyntax arrayCreation when arrayCreation.Initializer is not null:
                    return TryGetByteArray(arrayCreation.Initializer.Expressions, semanticModel, semanticModels, cancellationToken, visitedSymbols, out bytes);
                case ImplicitArrayCreationExpressionSyntax implicitArray when implicitArray.Initializer is not null:
                    return TryGetByteArray(implicitArray.Initializer.Expressions, semanticModel, semanticModels, cancellationToken, visitedSymbols, out bytes);
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

                foreach (SyntaxReference syntaxReference in fieldSymbol.DeclaringSyntaxReferences)
                {
                    if (syntaxReference.GetSyntax(cancellationToken) is VariableDeclaratorSyntax declarator &&
                        declarator.Initializer is not null &&
                        TryGetByteSequence(
                            declarator.Initializer.Value,
                            semanticModel,
                            semanticModels,
                            cancellationToken,
                            visitedSymbols,
                            out bytes))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetByteArray(
            SeparatedSyntaxList<ExpressionSyntax> expressions,
            SemanticModel semanticModel,
            IReadOnlyDictionary<SyntaxTree, SemanticModel> semanticModels,
            CancellationToken cancellationToken,
            HashSet<ISymbol> visitedSymbols,
            out byte[] bytes)
        {
            List<byte> values = new(expressions.Count);
            foreach (ExpressionSyntax expression in expressions)
            {
                if (!TryGetByteSequence(
                        expression,
                        semanticModel,
                        semanticModels,
                        cancellationToken,
                        visitedSymbols,
                        out byte[] elementBytes))
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

        private sealed class PrefixUsageCandidate
        {
            public PrefixUsageCandidate(
                INamedTypeSymbol type,
                IFieldSymbol field,
                ExpressionSyntax initializerValue,
                SemanticModel semanticModel,
                Location location)
            {
                Type = type;
                Field = field;
                InitializerValue = initializerValue;
                SemanticModel = semanticModel;
                Location = location;
            }

            public INamedTypeSymbol Type { get; }

            public IFieldSymbol Field { get; }

            public ExpressionSyntax InitializerValue { get; }

            public SemanticModel SemanticModel { get; }

            public Location Location { get; }
        }

        private sealed class CollectedPrefixUsage
        {
            public CollectedPrefixUsage(
                INamedTypeSymbol type,
                IFieldSymbol field,
                string normalizedPrefix,
                Location location)
            {
                Type = type;
                Field = field;
                NormalizedPrefix = normalizedPrefix;
                Location = location;
            }

            public INamedTypeSymbol Type { get; }

            public IFieldSymbol Field { get; }

            public string NormalizedPrefix { get; }

            public Location Location { get; }
        }

        private sealed class PrefixUsage
        {
            public PrefixUsage(IFieldSymbol? field, string description)
            {
                Field = field;
                Description = description;
            }

            public IFieldSymbol? Field { get; }

            public string Description { get; }
        }
    }
}
