using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractMethodNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4018";

        private static readonly DiagnosticDescriptor Rule = new(
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
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            var model = context.SemanticModel;

            // Get the symbol for the class and cast it to INamedTypeSymbol
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol) return;

            // Check if class inherits from SmartContract directly or indirectly
            if (!InheritsFromSmartContract(classSymbol))
                return;

            // Group methods by name and parameter count
            var methods = classSymbol.GetMembers().OfType<IMethodSymbol>();
            var groupedMethods = methods.Where(m => m.DeclaredAccessibility == Accessibility.Public)
                .GroupBy(m => new { m.Name, ParamCount = m.Parameters.Length });

            foreach (var group in groupedMethods)
            {
                if (group.Count() <= 1) continue; // Naming conflict detected
                foreach (var method in group)
                {
                    var location = method.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
                    var diagnostic = Diagnostic.Create(Rule, location, method.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool InheritsFromSmartContract(INamedTypeSymbol? symbol)
        {
            while (symbol != null)
            {
                if (symbol.BaseType?.ToString() == "SmartContract")
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SmartContractMethodNamingCodeFixProvider)), Shared]
    public class SmartContractMethodNamingCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(SmartContractMethodNamingAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodDeclaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
            if (methodDeclaration is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Rename to avoid conflict",
                    createChangedDocument: c => RenameConflictingMethod(context.Document, methodDeclaration, c),
                    equivalenceKey: "Rename to avoid conflict"),
                diagnostic);
        }

        private static async Task<Document> RenameConflictingMethod(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
            if (symbol is null) return document;

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            if (root is null) return document;

            var newName = GenerateNewMethodName(symbol);
            var newMethod = methodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName))
                                             .WithTriviaFrom(methodDeclaration);
            var newRoot = root.ReplaceNode(methodDeclaration, newMethod);
            return document.WithSyntaxRoot(newRoot);
        }

        private static string GenerateNewMethodName(IMethodSymbol methodSymbol)
        {
            var baseName = methodSymbol.Name.TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
            var counter = 1;
            while (methodSymbol.ContainingType.GetMembers(baseName + counter).Any())
                counter++;

            return baseName + counter;
        }
    }
}
