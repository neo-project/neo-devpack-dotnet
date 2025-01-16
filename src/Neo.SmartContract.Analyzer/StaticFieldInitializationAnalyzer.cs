// Copyright (C) 2015-2024 The Neo Project.
//
// StaticFieldInitializationAnalyzer.cs file belongs to the neo project and is free
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
using System.Text.RegularExpressions;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StaticFieldInitializationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4023";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            "Invalid static field initialization",
            "{0}",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

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

            if (!fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                return;

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var variableTypeName = fieldDeclaration.Declaration.Type.ToString();
                var initializer = variable.Initializer;

                if (initializer == null)
                    continue;

                var literalExpression = initializer.Value as LiteralExpressionSyntax;
                if (literalExpression == null)
                    continue;

                var literalValue = literalExpression.Token.ValueText;

                if (variableTypeName == "UInt256")
                {
                    if (!IsValidUInt256(literalValue))
                    {
                        var diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), "UInt256 must be initialized with a 64-character hex string.");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (variableTypeName == "UInt160")
                {
                    if (!IsValidUInt160(literalValue))
                    {
                        var diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), "UInt160 must be initialized with a 40-character hex string or a 34-character string starting with 'N'.");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (variableTypeName == "ECPoint")
                {
                    if (!IsValidECPoint(literalValue))
                    {
                        var diagnostic = Diagnostic.Create(Rule, variable.GetLocation(), "ECPoint must be initialized with a 66-character hex string.");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool IsValidUInt256(string value)
        {
            return Regex.IsMatch(value, "^[0-9a-fA-F]{64}$");
        }

        private bool IsValidUInt160(string value)
        {
            return Regex.IsMatch(value, "^[0-9a-fA-F]{40}$") || Regex.IsMatch(value, "^N[0-9a-zA-Z]{33}$");
        }

        private bool IsValidECPoint(string value)
        {
            return Regex.IsMatch(value, "^[0-9a-fA-F]{66}$");
        }
    }
}
