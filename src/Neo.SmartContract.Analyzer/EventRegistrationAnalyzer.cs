// Copyright (C) 2015-2025 The Neo Project.
//
// EventRegistrationAnalyzer.cs file belongs to the neo project and is free
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
    public class EventRegistrationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdUnregisteredEvent = "NC4049";
        public const string DiagnosticIdImproperEventDefinition = "NC4050";

        private static readonly LocalizableString TitleUnregisteredEvent = "Unregistered event usage";
        private static readonly LocalizableString MessageFormatUnregisteredEvent = "Event '{0}' is used but not properly registered as a delegate";
        private static readonly LocalizableString DescriptionUnregisteredEvent = "Events must be registered as delegates before being used in Runtime.Notify calls.";

        private static readonly LocalizableString TitleImproperEventDefinition = "Improper event definition";
        private static readonly LocalizableString MessageFormatImproperEventDefinition = "Event '{0}' is not properly defined: {1}";
        private static readonly LocalizableString DescriptionImproperEventDefinition = "Events in Neo smart contracts should be defined as delegates with proper parameter types.";

        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor RuleUnregisteredEvent = new(
            DiagnosticIdUnregisteredEvent,
            TitleUnregisteredEvent,
            MessageFormatUnregisteredEvent,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: DescriptionUnregisteredEvent);

        private static readonly DiagnosticDescriptor RuleImproperEventDefinition = new(
            DiagnosticIdImproperEventDefinition,
            TitleImproperEventDefinition,
            MessageFormatImproperEventDefinition,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DescriptionImproperEventDefinition);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(RuleUnregisteredEvent, RuleImproperEventDefinition);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                // Track registered events and their usage
                var registeredEvents = new Dictionary<string, ISymbol>();
                var eventUsages = new Dictionary<string, List<SyntaxNode>>();

                // Register for delegate declarations to find event definitions
                compilationStartContext.RegisterSyntaxNodeAction(
                    context => CollectEventDefinition(context, registeredEvents),
                    SyntaxKind.DelegateDeclaration);

                // Register for field declarations to find event definitions
                compilationStartContext.RegisterSyntaxNodeAction(
                    context => CollectEventField(context, registeredEvents),
                    SyntaxKind.FieldDeclaration);

                // Register for method invocations to find Runtime.Notify calls
                compilationStartContext.RegisterSyntaxNodeAction(
                    context => CollectEventUsage(context, eventUsages),
                    SyntaxKind.InvocationExpression);

                // At the end of compilation, check for unregistered events
                compilationStartContext.RegisterCompilationEndAction(
                    context => VerifyEventRegistration(context, registeredEvents, eventUsages));
            });
        }

        private void CollectEventDefinition(SyntaxNodeAnalysisContext context, Dictionary<string, ISymbol> registeredEvents)
        {
            var delegateDeclaration = (DelegateDeclarationSyntax)context.Node;

            // Check if this is in a smart contract class
            if (!IsInSmartContractClass(context.SemanticModel, delegateDeclaration))
                return;

            // Get the delegate symbol
            var delegateSymbol = context.SemanticModel.GetDeclaredSymbol(delegateDeclaration);
            if (delegateSymbol == null)
                return;

            // No longer checking if the delegate is static - events don't need to be static

            // Add to registered events
            registeredEvents[delegateSymbol.Name] = delegateSymbol;
        }

        private void CollectEventField(SyntaxNodeAnalysisContext context, Dictionary<string, ISymbol> registeredEvents)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            // Check if this is in a smart contract class
            if (!IsInSmartContractClass(context.SemanticModel, fieldDeclaration))
                return;

            // Check if this is a delegate field
            if (!IsDelegateType(context.SemanticModel, fieldDeclaration.Declaration.Type))
                return;

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                if (fieldSymbol == null)
                    continue;

                // No longer checking if the field is static - events don't need to be static

                // Add to registered events
                registeredEvents[fieldSymbol.Name] = fieldSymbol;
            }
        }

        private void CollectEventUsage(SyntaxNodeAnalysisContext context, Dictionary<string, List<SyntaxNode>> eventUsages)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            // Check if this is a Runtime.Notify call
            if (!IsRuntimeNotifyCall(context.SemanticModel, invocation))
                return;

            // Check if the first argument is an event name
            if (invocation.ArgumentList.Arguments.Count == 0)
                return;

            var firstArg = invocation.ArgumentList.Arguments[0].Expression;
            if (firstArg is IdentifierNameSyntax identifierName)
            {
                var eventName = identifierName.Identifier.Text;
                if (!eventUsages.TryGetValue(eventName, out var usages))
                {
                    usages = new List<SyntaxNode>();
                    eventUsages[eventName] = usages;
                }
                usages.Add(invocation);
            }
        }

        private void VerifyEventRegistration(CompilationAnalysisContext context, Dictionary<string, ISymbol> registeredEvents, Dictionary<string, List<SyntaxNode>> eventUsages)
        {
            foreach (var usage in eventUsages)
            {
                var eventName = usage.Key;
                var usageNodes = usage.Value;

                // Check if the event is registered
                if (!registeredEvents.ContainsKey(eventName))
                {
                    foreach (var node in usageNodes)
                    {
                        var diagnostic = Diagnostic.Create(RuleUnregisteredEvent,
                            node.GetLocation(),
                            eventName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool IsInSmartContractClass(SemanticModel semanticModel, SyntaxNode node)
        {
            var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            if (classDeclaration == null)
                return false;

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

        private bool IsDelegateType(SemanticModel semanticModel, TypeSyntax typeSyntax)
        {
            var typeSymbol = semanticModel.GetTypeInfo(typeSyntax).Type;
            return typeSymbol?.TypeKind == TypeKind.Delegate;
        }

        private bool IsRuntimeNotifyCall(SemanticModel semanticModel, InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                if (memberAccess.Name.Identifier.Text != "Notify")
                    return false;

                if (memberAccess.Expression is IdentifierNameSyntax identifier && identifier.Identifier.Text == "Runtime")
                {
                    var symbol = semanticModel.GetSymbolInfo(memberAccess).Symbol as IMethodSymbol;
                    return symbol?.ContainingType?.Name == "Runtime" &&
                           symbol.ContainingNamespace?.ToString() == "Neo.SmartContract.Framework.Services";
                }
            }
            return false;
        }
    }
}
