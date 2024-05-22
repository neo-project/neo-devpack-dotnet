using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Xunit.Sdk;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CollectionTypesUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4013";

        private readonly string[] _unsupportedCollectionTypes = {
            "System.Collections.Generic.Dictionary<TKey, TValue>",
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
            "Do not use collection type: {0}. Use List<T> or Map<TKey, TValue> instead.",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.VariableDeclaration);
        }

        private void AnalyzeOperation(OperationAnalysisContext context)
        {
            if (context.Operation is not IVariableDeclarationOperation variableDeclaration) return;

            var variableType = variableDeclaration.GetDeclaredVariables()[0].Type;
            var originalType = variableType.OriginalDefinition.ToString() ?? throw new NullException("originalType is null");

            if (_unsupportedCollectionTypes.Contains(originalType))
            {
                var suggestedType = originalType.Contains("Dictionary") ? "Map<TKey, TValue>" : "List<T>";
                var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), originalType, suggestedType);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CollectionTypesUsageCodeFixProvider)), Shared]
    public class CollectionTypesUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CollectionTypesUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            // var diagnosticSpan = diagnostic.Location.SourceSpan;
            // var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use recommended collection type",
                    createChangedDocument: c => UseRecommendedCollectionTypeAsync(context.Document, c),
                    equivalenceKey: "Use recommended collection type"),
                diagnostic);
            return Task.CompletedTask;
        }

        private async Task<Document> UseRecommendedCollectionTypeAsync(Document document, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            // Traverse all variable declarations
            var variableDeclarations = root!.DescendantNodes().OfType<VariableDeclarationSyntax>();

            foreach (var declaration in variableDeclarations)
            {
                var typeSymbol = semanticModel.GetTypeInfo(declaration.Type, cancellationToken).ConvertedType as INamedTypeSymbol;

                // Check if it's a type we want to replace
                if (typeSymbol != null && (ShouldReplaceWithList(typeSymbol) || ShouldReplaceWithMap(typeSymbol)))
                {
                    TypeSyntax? newTypeSyntax = null;
                    if (ShouldReplaceWithList(typeSymbol))
                    {
                        // Generate new List<T> type
                        var genericTypeArg = typeSymbol.TypeArguments[0];
                        newTypeSyntax = SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("List"),
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.ParseTypeName(genericTypeArg.ToString()!))));
                    }
                    else if (ShouldReplaceWithMap(typeSymbol))
                    {
                        // Generate new Map<TKey, TValue> type
                        var keyType = typeSymbol.TypeArguments[0];
                        var valueType = typeSymbol.TypeArguments[1];
                        newTypeSyntax = SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("Map"),
                            SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(new[] {
                        SyntaxFactory.ParseTypeName(keyType.ToString()!),
                        SyntaxFactory.ParseTypeName(valueType.ToString()!)
                            })));
                    }

                    if (newTypeSyntax != null)
                    {
                        // Replace the type in the variable declaration
                        editor.ReplaceNode(declaration.Type, newTypeSyntax.WithTriviaFrom(declaration.Type));

                        // Update the initializer for each variable
                        foreach (var variable in declaration.Variables)
                        {
                            if (variable.Initializer != null)
                            {
                                var newInitializer = SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.ObjectCreationExpression(newTypeSyntax)
                                        .WithArgumentList(SyntaxFactory.ArgumentList())
                                        .WithInitializer(variable.Initializer.Value is ObjectCreationExpressionSyntax oce ? oce.Initializer : null));

                                editor.ReplaceNode(variable.Initializer, newInitializer);
                            }
                        }
                    }
                }
            }

            return editor.GetChangedDocument();
        }

        private bool ShouldReplaceWithList(ISymbol symbol)
        {
            var singleElementListTypes = new HashSet<string>
            {
                "System.Collections.Generic.Stack`1",
                "System.Collections.Generic.Queue`1",
                "System.Array",
                "System.Collections.Generic.List<T>",
                "System.Collections.Generic.LinkedList<T>",
                "System.Collections.Generic.Queue<T>",
                "System.Collections.Generic.Stack<T>",
                "System.Collections.Generic.HashSet<T>",
                "System.Collections.Generic.SortedSet<T>",
                "System.Collections.ObjectModel.Collection<T>",
                "System.Collections.ObjectModel.ObservableCollection<T>",
                "System.Collections.ObjectModel.ReadOnlyCollection<T>",
                "System.Collections.Generic.ConcurrentBag<T>",
                "System.Collections.Concurrent.ConcurrentQueue<T>",
                "System.Collections.Concurrent.ConcurrentStack<T>",
                "System.Collections.Immutable.ImmutableList<T>",
                "System.Collections.Immutable.ImmutableQueue<T>",
                "System.Collections.Immutable.ImmutableStack<T>",
                "System.Collections.Immutable.ImmutableHashSet<T>",
                "System.Collections.Immutable.ImmutableSortedSet<T>",
                "System.Collections.Immutable.ImmutableArray<T>",
            };

            var originalDefinition = symbol.OriginalDefinition.ToString();
            return singleElementListTypes.Contains(originalDefinition!);
        }

        private bool ShouldReplaceWithMap(ISymbol symbol)
        {
            var mapElementListTypes = new HashSet<string>
            {
                "System.Collections.Generic.Dictionary<TKey, TValue>",
                "System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>",
                "System.Collections.Immutable.ImmutableDictionary<TKey, TValue>",
                "System.Collections.Generic.SortedDictionary<TKey, TValue>",
                "System.Collections.ObjectModel.KeyedCollection<TKey, TItem>",
                "System.Collections.Immutable.ImmutableSortedDictionary<TKey, TValue>",
                "System.Collections.Specialized.NameValueCollection",
                "System.Collections.Specialized.StringCollection",
                "System.Collections.Specialized.HybridDictionary",
                "System.Collections.Specialized.OrderedDictionary",
                "System.Collections.Hashtable",
            };

            var originalDefinition = symbol.OriginalDefinition.ToString();
            return mapElementListTypes.Contains(originalDefinition!);
        }
    }
}
