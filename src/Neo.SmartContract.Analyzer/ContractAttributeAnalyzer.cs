// Copyright (C) 2015-2025 The Neo Project.
//
// ContractAttributeAnalyzer.cs file belongs to the neo project and is free
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Neo.SmartContract.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContractAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdMissingAttributes = "NC4046";
        public const string DiagnosticIdInvalidAttribute = "NC4047";

        private static readonly LocalizableString TitleMissingAttributes = "Missing recommended contract attributes";
        private static readonly LocalizableString MessageFormatMissingAttributes = "Smart contract class is missing recommended attributes: {0}";
        private static readonly LocalizableString DescriptionMissingAttributes = "Smart contract classes should include recommended attributes for better metadata.";

        private static readonly LocalizableString TitleInvalidAttribute = "Invalid contract attribute usage";
        private static readonly LocalizableString MessageFormatInvalidAttribute = "Invalid usage of contract attribute: {0}";
        private static readonly LocalizableString DescriptionInvalidAttribute = "Contract attributes should be used correctly according to Neo smart contract specifications.";

        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor RuleMissingAttributes = new(
            DiagnosticIdMissingAttributes,
            TitleMissingAttributes,
            MessageFormatMissingAttributes,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionMissingAttributes);

        private static readonly DiagnosticDescriptor RuleInvalidAttribute = new(
            DiagnosticIdInvalidAttribute,
            TitleInvalidAttribute,
            MessageFormatInvalidAttribute,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionInvalidAttribute);

        // List of recommended contract attributes
        private readonly string[] _recommendedAttributes = {
            // "DisplayName",
            "ContractDescription",
            "ContractVersion",
            "ContractPermission"
        };

        // List of all valid contract attributes
        private readonly string[] _validAttributes = {
            "DisplayName",
            "ContractDescription",
            "ContractVersion",
            "ContractPermission",
            "ContractAuthor",
            "ContractEmail",
            "ContractSourceCode",
            "SupportedStandards",
            "ManifestExtra",
            "Contract",
            "ContractHash"
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RuleMissingAttributes, RuleInvalidAttribute);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            // Check if this is a smart contract class (inherits from SmartContract)
            if (!IsSmartContractClass(context.SemanticModel, classDeclaration))
                return;

            // Check for recommended attributes
            var presentAttributes = new HashSet<string>();
            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = GetAttributeName(attribute);
                    if (_recommendedAttributes.Contains(attributeName))
                    {
                        presentAttributes.Add(attributeName);
                    }
                }
            }

            // Report missing recommended attributes
            var missingAttributes = _recommendedAttributes.Except(presentAttributes).ToList();
            if (missingAttributes.Count > 0)
            {
                var diagnostic = Diagnostic.Create(RuleMissingAttributes,
                    classDeclaration.Identifier.GetLocation(),
                    string.Join(", ", missingAttributes));
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;
            var attributeName = GetAttributeName(attribute);

            // Check if this is a contract attribute
            if (!_validAttributes.Contains(attributeName))
                return;

            // Get the parent class
            var classDeclaration = attribute.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
                return;

            // Check if this is a smart contract class
            if (!IsSmartContractClass(context.SemanticModel, classDeclaration))
                return;

            // Validate specific attributes
            switch (attributeName)
            {
                case "DisplayName":
                    ValidateDisplayNameAttribute(context, attribute);
                    break;
                case "ContractVersion":
                    ValidateContractVersionAttribute(context, attribute);
                    break;
                case "ContractPermission":
                    ValidateContractPermissionAttribute(context, attribute);
                    break;
                case "SupportedStandards":
                    ValidateSupportedStandardsAttribute(context, attribute);
                    break;
            }
        }

        private bool IsSmartContractClass(SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.BaseList == null)
                return false;

            foreach (var baseType in classDeclaration.BaseList.Types)
            {
                var typeInfo = semanticModel.GetTypeInfo(baseType.Type);
                if (typeInfo.Type?.Name == "SmartContract" &&
                    typeInfo.Type.ContainingNamespace?.ToString() == "Neo.SmartContract.Framework")
                {
                    return true;
                }
            }

            return false;
        }

        private string GetAttributeName(AttributeSyntax attribute)
        {
            var name = attribute.Name.ToString();
            if (name.EndsWith("Attribute"))
                name = name.Substring(0, name.Length - "Attribute".Length);
            return name;
        }

        private void ValidateDisplayNameAttribute(SyntaxNodeAnalysisContext context, AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    attribute.GetLocation(),
                    "DisplayName attribute requires a string parameter");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            var argument = attribute.ArgumentList.Arguments[0].Expression;
            if (argument is not LiteralExpressionSyntax literalExpression ||
                literalExpression.Kind() != SyntaxKind.StringLiteralExpression)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    argument.GetLocation(),
                    "DisplayName attribute parameter must be a string literal");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void ValidateContractVersionAttribute(SyntaxNodeAnalysisContext context, AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    attribute.GetLocation(),
                    "ContractVersion attribute requires a string parameter");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            var argument = attribute.ArgumentList.Arguments[0].Expression;
            if (argument is not LiteralExpressionSyntax literalExpression ||
                literalExpression.Kind() != SyntaxKind.StringLiteralExpression)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    argument.GetLocation(),
                    "ContractVersion attribute parameter must be a string literal");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void ValidateContractPermissionAttribute(SyntaxNodeAnalysisContext context, AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    attribute.GetLocation(),
                    "ContractPermission attribute requires at least one parameter");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void ValidateSupportedStandardsAttribute(SyntaxNodeAnalysisContext context, AttributeSyntax attribute)
        {
            if (attribute.ArgumentList == null || attribute.ArgumentList.Arguments.Count == 0)
            {
                var diagnostic = Diagnostic.Create(RuleInvalidAttribute,
                    attribute.GetLocation(),
                    "SupportedStandards attribute requires at least one parameter");
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
