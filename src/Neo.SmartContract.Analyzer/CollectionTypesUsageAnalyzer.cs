using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;

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
            // "System.Collections.Generic.List<T>",
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
            "Do not use collection type: {0}",
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

            // Check if the declared variable is of type Stack<T>
            if (variableDeclaration.GetDeclaredVariables().Any(v => _unsupportedCollectionTypes.Contains(v.Type.OriginalDefinition.ToString())))
            {
                var variableType = variableDeclaration.GetDeclaredVariables()[0].Type;
                var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
