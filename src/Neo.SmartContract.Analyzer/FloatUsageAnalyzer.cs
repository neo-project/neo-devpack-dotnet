using System;
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
    public class FloatUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4002";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of float is not allowed in neo contract",
            "Neo contract does not support float data type: {0}",
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

            var declaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<VariableDeclarationSyntax>().First();
            if (declaration is null) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Cast to int",
                    createChangedDocument: c => CastFloatToInt(context.Document, declaration, c),
                    equivalenceKey: "Cast to int"),
                diagnostic);
        }

        private static async Task<Document> CastFloatToInt(Document document, VariableDeclarationSyntax declaration, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
#pragma warning disable CS0618 // Type or member is obsolete
            var editor = new SyntaxEditor(root!, document.Project.Solution.Workspace);
#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var variable in declaration.Variables)
            {
                var initializer = variable.Initializer;
                if (initializer != null)
                {
                    // Determine if the type of the variable is explicitly float or if it's a var presumed to be float
                    bool isFloatType = declaration.Type.IsKind(SyntaxKind.PredefinedType) &&
                                       ((PredefinedTypeSyntax)declaration.Type).Keyword.IsKind(SyntaxKind.FloatKeyword);
                    bool isVarType = declaration.Type.IsKind(SyntaxKind.IdentifierName) &&
                                     ((IdentifierNameSyntax)declaration.Type).Identifier.Text == "var";

                    ExpressionSyntax updatedExpression = initializer.Value;

                    if (isFloatType || (isVarType && IsPotentialFloatInitializer(initializer.Value)))
                    {
                        // Change the type to int for float types or var declarations presumed to be float
                        var newType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
                        editor.ReplaceNode(declaration.Type, newType);

                        // Check if the initializer value is a cast expression and specifically cast to float
                        if (initializer.Value is CastExpressionSyntax castExpression &&
                            castExpression.Type is PredefinedTypeSyntax predefinedTypeSyntax &&
                            predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.FloatKeyword))
                        {
                            // Replace float cast with int cast
                            updatedExpression = SyntaxFactory.CastExpression(
                                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                                castExpression.Expression);
                        }
                        else if (initializer.Value is LiteralExpressionSyntax literalExpression &&
                                 literalExpression.Kind() == SyntaxKind.NumericLiteralExpression)
                        {
                            // Convert float literal to int cast
                            var literalValue = double.Parse(literalExpression.Token.ValueText);
                            updatedExpression = SyntaxFactory.CastExpression(
                                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(literalValue)));
                        }

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

        private static bool IsPotentialFloatInitializer(ExpressionSyntax expression)
        {
            // Check if the expression is a literal expression with a float value
            if (expression is LiteralExpressionSyntax literalExpression)
            {
                if (literalExpression.Kind() == SyntaxKind.NumericLiteralExpression)
                {
                    var literalValue = double.Parse(literalExpression.Token.ValueText);
                    return literalValue != Math.Floor(literalValue);
                }
            }

            // Check if the expression is a cast expression to float
            if (expression is CastExpressionSyntax castExpression &&
                castExpression.Type is PredefinedTypeSyntax predefinedTypeSyntax &&
                predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.FloatKeyword))
            {
                return true;
            }

            return false;
        }
    }
}
