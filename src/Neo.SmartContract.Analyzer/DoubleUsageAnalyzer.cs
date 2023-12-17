using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
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
using Microsoft.CodeAnalysis.Operations;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoubleUsageAnalyzer : DiagnosticAnalyzer
    {
        // public const string FloatingPointNumber = "NC2004";
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
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.VariableDeclaration);
        }

        private static void AnalyzeOperation(OperationAnalysisContext context)
        {
            if (context.Operation is not IVariableDeclarationOperation variableDeclaration) return;
            var variableType = variableDeclaration.GetDeclaredVariables()[0].Type;
            if (variableDeclaration.GetDeclaredVariables().All(p => p.Type.SpecialType != SpecialType.System_Double)) return;

            var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.ToString());
            context.ReportDiagnostic(diagnostic);
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Cast to int or long",
                    createChangedDocument: c => CastDoubleToIntOrLong(context.Document, declaration, c),
                    equivalenceKey: "Cast to int or long"),
                diagnostic);
        }

        private async Task<Document> CastDoubleToIntOrLong(Document document, VariableDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = new SyntaxEditor(root, document.Project.Solution.Workspace);

            foreach (var variable in declaration.Variables)
            {
                var initializer = variable.Initializer;
                if (initializer != null)
                {
                    // Attempt to evaluate the expression as a constant
                    var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                    var constantValue = semanticModel.GetConstantValue(initializer.Value);

                    // Decide whether to cast to int or long
                    TypeSyntax castType;
                    if (constantValue.HasValue && constantValue.Value is double doubleValue)
                    {
                        castType = Math.Abs(doubleValue) <= int.MaxValue ?
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)) :
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
                    }
                    else
                    {
                        // Default to long if it's not a constant or too large for int
                        castType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
                    }

                    var castExpression = SyntaxFactory.CastExpression(
                        castType,
                        initializer.Value)
                        .WithLeadingTrivia(initializer.Value.GetLeadingTrivia())
                        .WithTrailingTrivia(initializer.Value.GetTrailingTrivia());

                    editor.ReplaceNode(initializer.Value, castExpression);
                }
            }

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
