// Copyright (C) 2015-2026 The Neo Project.
//
// CollectionTypesUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CollectionTypesUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4013";

        private readonly string[] _unsupportedCollectionTypes = {
            "System.Collections.Generic.Dictionary<TKey, TValue>",
            "System.Collections.Generic.List<T>",
            "System.Collections.Generic.Stack<T>",
            "System.Collections.Generic.Queue<T>",
            "System.Collections.Generic.HashSet<T>",
            "System.Collections.Generic.SortedSet<T>",
            "System.Collections.Generic.LinkedList<T>",
            "System.Collections.ObjectModel.ObservableCollection<T>",
            "System.Collections.Concurrent.ConcurrentQueue<T>",
            "System.Collections.Concurrent.ConcurrentStack<T>",
            "System.Collections.Concurrent.ConcurrentBag<T>",
            "System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>",
            "System.Collections.Immutable.ImmutableList<T>",
            "System.Collections.Immutable.ImmutableStack<T>",
            "System.Collections.Immutable.ImmutableQueue<T>",
            "System.Collections.Immutable.ImmutableHashSet<T>",
            "System.Collections.Immutable.ImmutableSortedSet<T>",
            "System.Collections.Immutable.ImmutableDictionary<TKey, TValue>",
            "System.Collections.Generic.SortedDictionary<TKey, TValue>",
            "System.Collections.ObjectModel.KeyedCollection<TKey, TItem>",
            "System.Collections.Immutable.ImmutableArray<T>",
            "System.Collections.Immutable.ImmutableSortedDictionary<TKey, TValue>",
            "System.Collections.Concurrent.BlockingCollection<T>",
            "System.Collections.Specialized.NameValueCollection",
            "System.Collections.Specialized.StringCollection",
            "System.Collections.Specialized.HybridDictionary",
            "System.Collections.Specialized.OrderedDictionary",
            "System.Collections.ArrayList",
            "System.Collections.Hashtable",
            "System.Collections.SortedList",
            "System.Collections.BitArray",
            "System.Collections.ObjectModel.Collection<T>",
            "System.Collections.ObjectModel.ReadOnlyCollection<T>",
            "System.Collections.ObjectModel.ReadOnlyObservableCollection<T>"
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported collection type is used",
            "Do not use collection type: {0}. Use {1} instead.",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzeOperation(OperationAnalysisContext context)
        {
            if (context.Operation is not IVariableDeclarationOperation variableDeclaration) return;

            var variableType = variableDeclaration.GetDeclaredVariables().FirstOrDefault()?.Type;
            ReportIfUnsupportedCollectionType(context, variableDeclaration.Syntax.GetLocation(), variableType);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MethodDeclarationSyntax methodDeclaration) return;

            var type = context.SemanticModel.GetTypeInfo(methodDeclaration.ReturnType, context.CancellationToken).Type;
            ReportIfUnsupportedCollectionType(context, methodDeclaration.ReturnType.GetLocation(), type);
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ParameterSyntax parameter || parameter.Type is null) return;

            var type = (context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken) as IParameterSymbol)?.Type;
            ReportIfUnsupportedCollectionType(context, parameter.Type.GetLocation(), type);
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not PropertyDeclarationSyntax propertyDeclaration) return;

            var type = (context.SemanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken) as IPropertySymbol)?.Type;
            ReportIfUnsupportedCollectionType(context, propertyDeclaration.Type.GetLocation(), type);
        }

        private void ReportIfUnsupportedCollectionType(OperationAnalysisContext context, Location location, ITypeSymbol? type)
        {
            if (GetUnsupportedCollectionType(type) is not { } originalType) return;

            var diagnostic = Diagnostic.Create(Rule, location, originalType, GetSuggestedType(originalType));
            context.ReportDiagnostic(diagnostic);
        }

        private void ReportIfUnsupportedCollectionType(SyntaxNodeAnalysisContext context, Location location, ITypeSymbol? type)
        {
            if (GetUnsupportedCollectionType(type) is not { } originalType) return;

            var diagnostic = Diagnostic.Create(Rule, location, originalType, GetSuggestedType(originalType));
            context.ReportDiagnostic(diagnostic);
        }

        private string? GetUnsupportedCollectionType(ITypeSymbol? type)
        {
            if (type is not INamedTypeSymbol namedType) return null;

            var originalType = namedType.OriginalDefinition.ToString() ?? throw new ArgumentNullException("originalType is null");
            return _unsupportedCollectionTypes.Contains(originalType) ? originalType : null;
        }

        private static string GetSuggestedType(string originalType)
        {
            return originalType.Contains("Dictionary") ? "Map<TKey, TValue>" : "List<T>";
        }
    }
}
