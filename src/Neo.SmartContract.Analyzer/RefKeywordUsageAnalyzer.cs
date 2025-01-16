// Copyright (C) 2015-2024 The Neo Project.
//
// RefKeywordUsageAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RefKeywordUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4010";
        private static readonly string Title = "Ref keyword usage";
        private static readonly string MessageFormat = "'ref' keyword used in {0}";
        private static readonly string Description = "Checks for the usage of 'ref' keywords.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,  // or DiagnosticSeverity.Error based on your requirements
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
                    if (parameter.Modifiers.Any(SyntaxKind.RefKeyword))
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
                    if (argument.RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword))
                    {
                        var diagnostic = Diagnostic.Create(Rule, argument.GetLocation(), "method invocation");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
