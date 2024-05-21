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
    public class DecimalUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4003";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of decimal or double is not allowed in neo contract",
            "Neo contract does not support the {0} data type: {1}",
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

            if (variableType.SpecialType == SpecialType.System_Decimal || variableType.SpecialType == SpecialType.System_Double)
            {
                var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.SpecialType.ToString(), variableType.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DecimalUsageCodeFixProvider)), Shared]
    public class DecimalUsageCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DecimalUsageAnalyzer.DiagnosticId);

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
                    title: "Change to long",
                    createChangedDocument: c => ChangeToLong(context.Document, declaration, c),
                    equivalenceKey: "Change to long"),
                diagnostic);
        }

        private static async Task<Document> ChangeToLong(Document document, VariableDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var editor = new SyntaxEditor(root!, document.Project.Solution.Workspace);
#pragma warning restore CS0618 // Type or member is obsolete

            // Change the type to long
            var newType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
            editor.ReplaceNode(declaration.Type, newType);

            // Cast the initializer value to long if it's a decimal or double literal
            foreach (var variable in declaration.Variables)
            {
                if (variable.Initializer?.Value is LiteralExpressionSyntax literalExpression)
                {
                    var newInitializer = SyntaxFactory.CastExpression(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
                        literalExpression
                    );
                    editor.ReplaceNode(literalExpression, newInitializer);
                }
            }

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
