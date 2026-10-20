// Copyright (C) 2015-2026 The Neo Project.
//
// StringConcatenationInLoopAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Neo.SmartContract.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StringConcatenationInLoopAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "NC4065";

    private static readonly DiagnosticDescriptor Rule = DiagnosticDescriptorFactory.Create(
        DiagnosticId,
        "String concatenation inside a loop",
        "String '{0}' is extended with concatenation inside a loop",
        "Usage",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Every concatenation copies the accumulated value. Bound the accumulated length and iteration count, or move the concatenation outside the loop.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeCompoundAssignment, OperationKind.CompoundAssignment);
        context.RegisterOperationAction(AnalyzeSimpleAssignment, OperationKind.SimpleAssignment);
    }

    private static void AnalyzeCompoundAssignment(OperationAnalysisContext context)
    {
        if (context.Operation is not ICompoundAssignmentOperation assignment)
            return;
        if (assignment.OperatorKind != BinaryOperatorKind.Add)
            return;
        if (assignment.Target.Type?.SpecialType != SpecialType.System_String)
            return;

        ReportIfAccumulatesInLoop(context, assignment.Target);
    }

    private static void AnalyzeSimpleAssignment(OperationAnalysisContext context)
    {
        if (context.Operation is not ISimpleAssignmentOperation { Value: IBinaryOperation binary } assignment)
            return;
        if (binary.OperatorKind != BinaryOperatorKind.Add || binary.Type?.SpecialType != SpecialType.System_String)
            return;

        if (!ContainsTarget(binary, assignment.Target))
            return;

        ReportIfAccumulatesInLoop(context, assignment.Target);
    }

    private static void ReportIfAccumulatesInLoop(OperationAnalysisContext context, IOperation target)
    {
        var loop = FindEnclosingLoop(context.Operation.Syntax);
        if (loop is null || !ReferencesSameStorage(target, target))
            return;

        // A local that is recreated on every iteration does not accumulate anything.
        if (Unwrap(target) is ILocalReferenceOperation { Local: var local }
            && local.DeclaringSyntaxReferences.Any(reference => IsRepeatedWithinLoop(loop, reference.GetSyntax(context.CancellationToken))))
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation(), target.Syntax.ToString()));
    }

    private static SyntaxNode? FindEnclosingLoop(SyntaxNode node)
    {
        for (var parent = node.Parent; parent is not null; parent = parent.Parent)
        {
            // A nested function runs when called, not when the enclosing loop creates it.
            if (parent is AnonymousFunctionExpressionSyntax or LocalFunctionStatementSyntax)
                return null;
            if (parent is ForStatementSyntax or ForEachStatementSyntax or ForEachVariableStatementSyntax
                or WhileStatementSyntax or DoStatementSyntax)
            {
                if (IsRepeatedWithinLoop(parent, node))
                    return parent;
            }
        }

        return null;
    }

    private static bool IsRepeatedWithinLoop(SyntaxNode loop, SyntaxNode node)
    {
        if (!loop.Span.Contains(node.Span))
            return false;

        // Initializers and the foreach collection expression execute once per loop entry.
        return loop switch
        {
            ForStatementSyntax statement => statement.Declaration?.Span.Contains(node.Span) != true
                && !statement.Initializers.Any(initializer => initializer.Span.Contains(node.Span)),
            CommonForEachStatementSyntax statement => !statement.Expression.Span.Contains(node.Span),
            _ => true,
        };
    }

    private static bool ContainsTarget(IOperation operation, IOperation target)
    {
        operation = Unwrap(operation);
        if (ReferencesSameStorage(operation, target))
            return true;
        return operation is IBinaryOperation { OperatorKind: BinaryOperatorKind.Add } binary
            && binary.Type?.SpecialType == SpecialType.System_String
            && (ContainsTarget(binary.LeftOperand, target) || ContainsTarget(binary.RightOperand, target));
    }

    private static bool ReferencesSameStorage(IOperation? left, IOperation? right)
    {
        if (left is null || right is null)
            return left is null && right is null;
        left = Unwrap(left);
        right = Unwrap(right);

        return (left, right) switch
        {
            (ILocalReferenceOperation a, ILocalReferenceOperation b) => SymbolEqualityComparer.Default.Equals(a.Local, b.Local),
            (IParameterReferenceOperation a, IParameterReferenceOperation b) => SymbolEqualityComparer.Default.Equals(a.Parameter, b.Parameter),
            (IInstanceReferenceOperation a, IInstanceReferenceOperation b) => a.ReferenceKind == InstanceReferenceKind.ContainingTypeInstance
                && b.ReferenceKind == InstanceReferenceKind.ContainingTypeInstance,
            (IFieldReferenceOperation a, IFieldReferenceOperation b) => SymbolEqualityComparer.Default.Equals(a.Field, b.Field)
                && ReferencesSameStorage(a.Instance, b.Instance),
            (IPropertyReferenceOperation a, IPropertyReferenceOperation b) => !a.Property.IsIndexer
                && SymbolEqualityComparer.Default.Equals(a.Property, b.Property)
                && ReferencesSameStorage(a.Instance, b.Instance),
            _ => false,
        };
    }

    private static IOperation Unwrap(IOperation operation)
    {
        while (operation is IConversionOperation { OperatorMethod: null } or IParenthesizedOperation)
            operation = operation.ChildOperations.First();
        return operation;
    }

}
