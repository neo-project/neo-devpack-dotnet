using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnonymousFunctionUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4016";
        private static readonly string Title = "Anonymous Function usage";
        private static readonly string MessageFormat = "Use of anonymous function is not allowed";
        private static readonly string Description = "Anonymous functions are restricted in this project.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AnonymousMethodExpression, SyntaxKind.SimpleLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not AnonymousMethodExpressionSyntax && context.Node is not LambdaExpressionSyntax) return;
            var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnonymousFunctionToPrivateMethodCodeFixProvider))]
    public class AnonymousFunctionToPrivateMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AnonymousFunctionUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var anonymousFunction = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AnonymousFunctionExpressionSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert to private method",
                    createChangedDocument: c => ConvertToPrivateMethodAsync(context.Document, anonymousFunction, c),
                    equivalenceKey: "Convert to private method"),
                diagnostic);
        }

        private async Task<Document> ConvertToPrivateMethodAsync(Document document, AnonymousFunctionExpressionSyntax anonymousFunction, CancellationToken cancellationToken)
        {
            var editor = new SyntaxEditor(await document.GetSyntaxRootAsync(cancellationToken), document.Project.Solution.Workspace);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            // Generate a unique method name
            string methodName = "GeneratedMethod";

            // Determine the return type and parameters of the lambda expression
            var symbolInfo = semanticModel.GetSymbolInfo(anonymousFunction);
            var delegateType = symbolInfo.Symbol as IMethodSymbol;
            var returnType = delegateType.ReturnType;
            var parameters = delegateType.Parameters.Select(p =>
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name)).WithType(SyntaxFactory.ParseTypeName(p.Type.ToDisplayString())));

            // Create a new method declaration
            var newMethod = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType.ToDisplayString()), methodName)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)))
                .WithBody(anonymousFunction.Body as BlockSyntax ?? SyntaxFactory.Block());

            // Insert the new method at the end of the class
            var classDeclaration = anonymousFunction.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            editor.InsertAfter(classDeclaration.Members.Last(), newMethod);

            // Replace the anonymous function with a call to the new method
            var invocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(methodName))
                .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                    delegateType.Parameters.Select(p => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Name))))));
            editor.ReplaceNode(anonymousFunction, invocationExpression);

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
