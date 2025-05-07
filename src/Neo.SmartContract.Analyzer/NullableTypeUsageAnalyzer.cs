// Copyright (C) 2015-2025 The Neo Project.
//
// NullableTypeUsageAnalyzer.cs file belongs to the neo project and is free
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
    public class NullableTypeUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NC4044";

        // Nullable types that are not supported in Neo smart contracts
        // Based on the methods implemented in src\Neo.Compiler.CSharp\MethodConvert\System\SystemCall.Nullable.cs
        private readonly string[] _unsupportedNullableTypes = {
            "float?", "double?", "decimal?"
            // Supported nullable types (not in this list):
            // - byte?
            // - sbyte?
            // - short?
            // - ushort?
            // - int?
            // - uint?
            // - long?
            // - ulong?
            // - bool?
            // - BigInteger?
        };

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            "Unsupported nullable type is used",
            "Unsupported nullable type: {0}",
            "Type",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeVariableDeclaration, SyntaxKind.VariableDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeVariableDeclaration(SyntaxNodeAnalysisContext context)
        {
            var variableDeclaration = (VariableDeclarationSyntax)context.Node;
            CheckType(context, variableDeclaration.Type);
        }

        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            var parameter = (ParameterSyntax)context.Node;
            if (parameter.Type != null)
            {
                CheckType(context, parameter.Type);
            }
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            CheckType(context, property.Type);
        }

        private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var field = (FieldDeclarationSyntax)context.Node;
            CheckType(context, field.Declaration.Type);
        }

        private void CheckType(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(typeSyntax).Type;
            if (typeSymbol == null) return;

            if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                var nullableType = typeSymbol.ToString();

                foreach (var unsupportedType in _unsupportedNullableTypes)
                {
                    if (nullableType.StartsWith(unsupportedType))
                    {
                        var diagnostic = Diagnostic.Create(Rule, typeSyntax.GetLocation(), nullableType);
                        context.ReportDiagnostic(diagnostic);
                        break;
                    }
                }
            }
        }
    }
}
