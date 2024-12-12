using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BigIntegerUninitializedAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4027";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Uninitialized BigInteger",
            "BigInteger must be initialized when declared",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;

            // Check if the type is BigInteger
            if (context.SemanticModel.GetTypeInfo(variableDeclaration.Type).Type?.ToString() != "System.Numerics.BigInteger")
                return;

            // Check if the declaration is inside a struct
            bool isInStruct = variableDeclaration.Ancestors().OfType<StructDeclarationSyntax>().Any();

            foreach (var variable in variableDeclaration.Variables)
            {
                if (variable.Initializer == null)
                {
                    if (!isInStruct)
                    {
                        // Report diagnostic for non-struct BigInteger declarations without initialization
                        var diagnostic = Diagnostic.Create(Rule, variable.GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        // For structs, check if there's a constructor that initializes this field
                        var structDeclaration = variableDeclaration.Ancestors().OfType<StructDeclarationSyntax>().First();
                        var constructors = structDeclaration.Members.OfType<ConstructorDeclarationSyntax>();

                        bool isInitializedInConstructor = false;
                        foreach (var constructor in constructors)
                        {
                            // Check if the field is initialized in any constructor
                            var assignments = constructor.Body?.DescendantNodes()
                                .OfType<AssignmentExpressionSyntax>()
                                .Where(assignment =>
                                {
                                    if (assignment.Left is MemberAccessExpressionSyntax memberAccess)
                                    {
                                        return memberAccess.Name.Identifier.ValueText == variable.Identifier.ValueText;
                                    }
                                    return false;
                                });

                            if (assignments != null && assignments.Any())
                            {
                                isInitializedInConstructor = true;
                                break;
                            }
                        }

                        // if (!isInitializedInConstructor && !constructors.Any())
                        // {
                        //     // Report diagnostic if the BigInteger field is not initialized in any constructor
                        //     // and there are no explicit constructors
                        //     var diagnostic = Diagnostic.Create(Rule, variable.GetLocation());
                        //     context.ReportDiagnostic(diagnostic);
                        // }
                    }
                }
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BigIntegerUninitializedCodeFixProvider)), Shared]
    public class BigIntegerUninitializedCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(BigIntegerUninitializedAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root?.FindToken(diagnosticSpan.Start)
                                .Parent?.AncestorsAndSelf()
                                .OfType<VariableDeclaratorSyntax>()
                                .First();

            if (declaration is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Initialize with zero",
                    createChangedDocument: c => InitializeWithZero(context.Document, declaration, c),
                    equivalenceKey: "Initialize with zero"),
                diagnostic);
        }

        private static async Task<Document> InitializeWithZero(Document document,
            VariableDeclaratorSyntax declarator,
            CancellationToken cancellationToken)
        {
            var initializer = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(0)));

            var newDeclarator = declarator.WithInitializer(initializer);

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = root?.ReplaceNode(declarator, newDeclarator);

            return document.WithSyntaxRoot(newRoot!);
        }
    }
}
