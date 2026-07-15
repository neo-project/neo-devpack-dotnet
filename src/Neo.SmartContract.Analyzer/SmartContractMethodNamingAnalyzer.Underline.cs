// Copyright (C) 2015-2026 The Neo Project.
//
// SmartContractMethodNamingAnalyzer.Underline.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SmartContractMethodNamingAnalyzerUnderline : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4020";
        private const string Title = "SmartContract method naming violation";
        private const string MessageFormat = "Method names starting with '_' are not allowed except '_deploy' or '_initial'";
        private const string Description = "Ensure method names in SmartContract subclasses follow the naming convention.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning, // or Error based on your preference
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDecl = (MethodDeclarationSyntax)context.Node;
            var methodName = methodDecl.Identifier.ValueText;
            var isStatic = methodDecl.Modifiers.Any(SyntaxKind.StaticKeyword);
            var isPublic = methodDecl.Modifiers.Any(SyntaxKind.PublicKeyword);

            if (isStatic && isPublic && methodName.StartsWith("_") &&
                methodName != "_deploy" && methodName != "_initial")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, methodDecl.Identifier.GetLocation()));
            }
        }
    }
}
