// Copyright (C) 2015-2026 The Neo Project.
//
// CatchOnlySystemExceptionAnalyzer.cs file belongs to the neo project and is free
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

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatchOnlySystemExceptionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4027";

        private static readonly LocalizableString Title = "Catch System.Exception";
        private static readonly LocalizableString MessageFormat = "Neo smart contract supports catching System.Exception only. The compiler will catch all exceptions even if you want to catch a limited class of exception.";
        private static readonly LocalizableString Description = "This analyzer enforces catching only System.Exception.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
        }

        private void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            var catchClause = (CatchClauseSyntax)context.Node;
            var declaration = catchClause.Declaration;

            if (declaration == null) return;

            var type = declaration.Type;
            if (type == null) return;

            var exceptionType = context.SemanticModel.GetTypeInfo(type).Type;
            if (exceptionType?.ToDisplayString() == "System.Exception") return;

            var diagnostic = Diagnostic.Create(Rule, type.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

}
