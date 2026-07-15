// Copyright (C) 2015-2026 The Neo Project.
//
// DoubleUsageAnalyzer.cs file belongs to the neo project and is free
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
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoubleUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4004";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Usage of double is not allowed in neo contract",
            "Neo contract does not support double data type: {0}",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Use an integer or BigInteger with an explicit application-defined scale for fixed-point arithmetic.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(
                static context => UnsupportedTypeUsageAnalyzerHelpers.AnalyzeMethodDeclaration(
                    context,
                    SpecialType.System_Double,
                    Rule,
                    static type => new object?[] { type.ToString() }),
                SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(
                static context => UnsupportedTypeUsageAnalyzerHelpers.AnalyzeParameter(
                    context,
                    SpecialType.System_Double,
                    Rule,
                    static type => new object?[] { type.ToString() }),
                SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(
                static context => UnsupportedTypeUsageAnalyzerHelpers.AnalyzePropertyDeclaration(
                    context,
                    SpecialType.System_Double,
                    Rule,
                    static type => new object?[] { type.ToString() }),
                SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeOperation(OperationAnalysisContext context)
        {
            if (context.Operation is not IVariableDeclarationOperation variableDeclaration) return;
            var variableType = variableDeclaration.GetDeclaredVariables()[0].Type;
            if (!variableDeclaration.GetDeclaredVariables().Any(p => p.Type.SpecialType == SpecialType.System_Double)) return;

            var diagnostic = Diagnostic.Create(Rule, variableDeclaration.Syntax.GetLocation(), variableType.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }

}
