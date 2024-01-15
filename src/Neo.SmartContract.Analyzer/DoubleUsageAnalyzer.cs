using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoubleUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4004";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of double is not allowed in neo contract",
            "Neo contract does not support double data type: {0}",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.PredefinedType);
        }

        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var predefinedTypeSyntax = (PredefinedTypeSyntax)context.Node;

            if (predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.DoubleKeyword))
            {
                var diagnostic = Diagnostic.Create(Rule, predefinedTypeSyntax.GetLocation(), predefinedTypeSyntax.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoubleUsageCodeFixProvider)), Shared]
    public class DoubleUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DoubleUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();
            if (declaration is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Cast to int or long",
                    createChangedDocument: c => CastDoubleToIntOrLong(context.Document, declaration, c),
                    equivalenceKey: "Cast to int or long"),
                diagnostic);
        }

        private static async Task<Document> CastDoubleToIntOrLong(Document document, VariableDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = new SyntaxEditor(root, document.Project.Solution.Workspace);

            foreach (var variable in declaration.Variables)
            {
                var initializer = variable.Initializer;
                if (initializer != null)
                {
                    // Determine if the type of the variable is explicitly double
                    bool isDoubleType = declaration.Type.IsKind(SyntaxKind.PredefinedType) &&
                                        ((PredefinedTypeSyntax)declaration.Type).Keyword.IsKind(SyntaxKind.DoubleKeyword);

                    ExpressionSyntax updatedExpression = initializer.Value;

                    // Check if the initializer contains a cast to long
                    bool containsCastToLong = updatedExpression.DescendantNodesAndSelf()
                        .OfType<CastExpressionSyntax>()
                        .Any(cast => cast.Type is PredefinedTypeSyntax pts && pts.Keyword.IsKind(SyntaxKind.LongKeyword));

                    if (isDoubleType)
                    {
                        // Change the type to long for double types
                        var newType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
                        editor.ReplaceNode(declaration.Type, newType);

                        // If not already casted to long, add a cast to long
                        if (!containsCastToLong)
                        {
                            updatedExpression = SyntaxFactory.CastExpression(
                                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                                updatedExpression);
                        }

                        var newInitializer = SyntaxFactory.EqualsValueClause(updatedExpression)
                            .WithLeadingTrivia(initializer.GetLeadingTrivia())
                            .WithTrailingTrivia(initializer.GetTrailingTrivia());

                        var newVariable = variable.WithInitializer(newInitializer);
                        editor.ReplaceNode(variable, newVariable);
                    }
                    else if (declaration.Type.IsVar && !containsCastToLong)
                    {
                        // If it's a var declaration and not already casted, explicitly cast to long
                        updatedExpression = SyntaxFactory.CastExpression(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                            updatedExpression);

                        var newInitializer = SyntaxFactory.EqualsValueClause(updatedExpression)
                            .WithLeadingTrivia(initializer.GetLeadingTrivia())
                            .WithTrailingTrivia(initializer.GetTrailingTrivia());

                        var newVariable = variable.WithInitializer(newInitializer);
                        editor.ReplaceNode(variable, newVariable);
                    }
                }
            }

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
