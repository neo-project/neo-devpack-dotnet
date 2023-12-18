using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OutKeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4009";
        private static readonly string Title = "Out keyword usage";
        private static readonly string MessageFormat = "'out' keyword used in {0}";
        private static readonly string Description = "Checks for the usage of 'out' keywords.";
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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax methodDeclaration)
            {
                foreach (var parameter in methodDeclaration.ParameterList.Parameters)
                {
                    if (parameter.Modifiers.Any(SyntaxKind.OutKeyword))
                    {
                        var diagnostic = Diagnostic.Create(Rule, parameter.GetLocation(), "method declaration");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
            else if (context.Node is InvocationExpressionSyntax invocationExpression)
            {
                foreach (var argument in invocationExpression.ArgumentList.Arguments)
                {
                    if (!argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword)) continue;
                    var diagnostic = Diagnostic.Create(Rule, argument.GetLocation(), "method invocation");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OutKeywordUsageCodeFixProvider)), Shared]
    public class OutKeywordUsageCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Convert 'out' parameter to method return";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(OutKeywordUsageAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Convert 'out' parameter to return value",
                    createChangedDocument: c => ConvertOutParameterToReturnValue(context.Document, declaration, c),
                    equivalenceKey: "Convert 'out' parameter to return value"),
                diagnostic);
        }

        private async Task<Document> ConvertOutParameterToReturnValue(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            var outParameters = methodDecl.ParameterList.Parameters
                .Where(p => p.Modifiers.Any(SyntaxKind.OutKeyword))
                .ToList();

            if (outParameters.Count == 0) return document;

            var originalReturnType = methodDecl.ReturnType;

            var newBody = UpdateMethodBody(methodDecl.Body, outParameters, originalReturnType);

            TypeSyntax newReturnType = DetermineNewReturnType(originalReturnType, outParameters);

            var newParameters = methodDecl.ParameterList.Parameters
                .Where(p => !p.Modifiers.Any(SyntaxKind.OutKeyword))
                .ToList();

            var separatedSyntaxList = SyntaxFactory.SeparatedList(newParameters);

            var newMethod = methodDecl.WithReturnType(newReturnType)
                .WithParameterList(methodDecl.ParameterList.WithParameters(separatedSyntaxList))
                .WithBody(newBody);

            var newRoot = root.ReplaceNode(methodDecl, newMethod);

            return document.WithSyntaxRoot(newRoot);
        }

        private BlockSyntax UpdateMethodBody(BlockSyntax originalBody, List<ParameterSyntax> outParameters, TypeSyntax originalReturnType)
        {
            var newStatements = new List<StatementSyntax>();
            ExpressionSyntax originalReturnExpression = null;
            foreach (var outParam in outParameters)
            {
                var localDeclaration = SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.VariableDeclaration(
                        outParam.Type,
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(outParam.Identifier)
                        )));
                newStatements.Add(localDeclaration);
            }

            foreach (var statement in originalBody.Statements)
            {
                if (statement is ReturnStatementSyntax returnStatement)
                {
                    originalReturnExpression = returnStatement.Expression;
                }
                else
                {
                    newStatements.Add(statement);
                }
            }

            ReturnStatementSyntax newReturnStatement;
            if (originalReturnType.ToString() == "void" && outParameters.Count == 1)
            {
                newReturnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(outParameters[0].Identifier));
            }
            else
            {
                var returnExpressions = outParameters.Select(p => (ExpressionSyntax)SyntaxFactory.IdentifierName(p.Identifier)).ToList();
                if (originalReturnType.ToString() != "void" && originalReturnExpression != null)
                {
                    returnExpressions.Insert(0, originalReturnExpression);
                }
                newReturnStatement = SyntaxFactory.ReturnStatement(
                    SyntaxFactory.TupleExpression(SyntaxFactory.SeparatedList(returnExpressions.Select(SyntaxFactory.Argument))));
            }
            newStatements.Add(newReturnStatement);
            return SyntaxFactory.Block(newStatements);
        }

        private TypeSyntax DetermineNewReturnType(TypeSyntax originalReturnType, List<ParameterSyntax> outParameters)
        {
            if (originalReturnType.ToString() == "void" && outParameters.Count == 1)
            {
                return outParameters.First().Type;
            }
            if (originalReturnType.ToString() == "void" && outParameters.Count > 1)
            {
                var tupleElements = outParameters.Select(p => SyntaxFactory.TupleElement(p.Type)).ToList();
                return SyntaxFactory.TupleType(SyntaxFactory.SeparatedList(tupleElements));
            }
            var returnTypes = new List<TupleElementSyntax>
    {
        SyntaxFactory.TupleElement(originalReturnType)
    };

            returnTypes.AddRange(outParameters.Select(p => SyntaxFactory.TupleElement(p.Type)));

            return SyntaxFactory.TupleType(SyntaxFactory.SeparatedList(returnTypes));
        }
    }
}
