using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NotifyEventNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4019";
        private const string Title = "Check event name in Notify call";
        private const string MessageFormat = "The name '{0}' does not match any defined event's DisplayName";
        private const string Description = "The name passed to Notify must match the DisplayName of an event.";
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
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;
            var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;

            if (memberAccessExpr?.Name.Identifier.ValueText == "Notify")
            {
                var argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
                if (argumentList?.Arguments.Count > 0)
                {
                    var firstArgument = argumentList.Arguments[0].Expression as LiteralExpressionSyntax;
                    var passedName = firstArgument?.Token.ValueText;

                    // Get all events in the containing type
                    var containingType = memberAccessExpr.FirstAncestorOrSelf<TypeDeclarationSyntax>();
                    var events = containingType.Members.OfType<EventFieldDeclarationSyntax>();

                    // Check if the passed name matches any event's DisplayName
                    bool nameMatches = events.Any(ev => EventHasMatchingDisplayName(ev, passedName, context.SemanticModel));

                    if (!nameMatches)
                    {
                        var diagnostic = Diagnostic.Create(Rule, firstArgument.GetLocation(), passedName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool EventHasMatchingDisplayName(EventFieldDeclarationSyntax eventDeclaration, string name, SemanticModel semanticModel)
        {
            return eventDeclaration.Declaration.Variables
                .Select(variable => semanticModel.GetDeclaredSymbol(variable) as IEventSymbol)
                .Select(symbol => symbol?.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Name == "DisplayNameAttribute"))
                .OfType<AttributeData>()
                .Select(displayNameAttr => displayNameAttr.ConstructorArguments.FirstOrDefault())
                .Any(displayNameArg => displayNameArg.Value as string == name);

        }
    }
}
