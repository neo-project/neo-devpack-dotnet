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
using Microsoft.CodeAnalysis.Editing;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InitialValueAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4009";
        private static readonly LocalizableString Title = "Convert attribute to literal initialization";
        private static readonly LocalizableString MessageFormat = "Convert '{0}' attribute to literal initialization";
        private static readonly LocalizableString Description = "This field can be initialized with a literal value instead of using an attribute.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                if (variable.Initializer?.Value is LiteralExpressionSyntax literal && literal.Token.ValueText == "default!")
                {
                    var attribute = fieldDeclaration.AttributeLists
                        .SelectMany(al => al.Attributes)
                        .FirstOrDefault(attr => IsTargetAttribute(attr.Name.ToString()));

                    if (attribute != null)
                    {
                        var argumentList = attribute.ArgumentList;
                        if (argumentList != null && argumentList.Arguments.Count > 0)
                        {
                            var argument = argumentList.Arguments[0].ToString();
                            var diagnostic = Diagnostic.Create(Rule, fieldDeclaration.GetLocation(), attribute.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }

        private bool IsTargetAttribute(string attributeName)
        {
            return attributeName == "Hash160" || attributeName == "ByteArray" || attributeName == "UInt256";
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitialValueCodeFixProvider)), Shared]
    public class InitialValueCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InitialValueAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert to literal initialization",
                    createChangedDocument: c => ConvertToLiteralInitializationAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(InitialValueCodeFixProvider)),
                diagnostic);
        }

        private async Task<Document> ConvertToLiteralInitializationAsync(Document document, FieldDeclarationSyntax fieldDeclaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

            var attribute = fieldDeclaration.AttributeLists
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(attr => IsTargetAttribute(attr.Name.ToString()));

            if (attribute != null && attribute.ArgumentList?.Arguments.Count > 0)
            {
                var argument = attribute.ArgumentList.Arguments[0].ToString();
                var newInitializer = SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(argument));

                var newField = fieldDeclaration
                    .RemoveNodes(fieldDeclaration.AttributeLists, SyntaxRemoveOptions.KeepNoTrivia)
                    .WithDeclaration(fieldDeclaration.Declaration.WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            fieldDeclaration.Declaration.Variables[0].WithInitializer(newInitializer))));

                editor.ReplaceNode(fieldDeclaration, newField);
            }

            return editor.GetChangedDocument();
        }

        private bool IsTargetAttribute(string attributeName)
        {
            return attributeName == "Hash160" || attributeName == "ByteArray" || attributeName == "UInt256";
        }
    }
}
