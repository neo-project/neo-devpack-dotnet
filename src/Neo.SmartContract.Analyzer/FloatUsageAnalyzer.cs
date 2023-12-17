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
    public class FloatUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4002";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of float is not allowed in neo contract",
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
            if (variableDeclaration.GetDeclaredVariables().All(p => p.Type.SpecialType != SpecialType.System_Single)) return;

            var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FloatUsageCodeFixProvider)), Shared]
    public class FloatUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FloatUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Cast to int",
                    createChangedDocument: c => CastFloatToInt(context.Document, declaration, c),
                    equivalenceKey: "Cast to int"),
                diagnostic);
        }

        private async Task<Document> CastFloatToInt(Document document, VariableDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = new SyntaxEditor(root, document.Project.Solution.Workspace);

            foreach (var variable in declaration.Variables)
            {
                var initializer = variable.Initializer;
                if (initializer != null)
                {
                    var castExpression = SyntaxFactory.CastExpression(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
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
