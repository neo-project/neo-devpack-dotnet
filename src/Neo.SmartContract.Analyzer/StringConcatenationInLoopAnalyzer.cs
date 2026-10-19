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
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Every concatenation copies the accumulated value, so the total gas cost grows with the iteration count multiplied by the string length. Use StringBuilder with a bounded length, or move the concatenation outside the loop.");

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

        var target = GetReferencedSymbol(assignment.Target);
        if (target is null)
            return;
        var operands = binary.ChildOperations.ToArray();
        if (operands.Length < 2)
            return;
        if (!SymbolEqualityComparer.Default.Equals(target, GetReferencedSymbol(operands[0]))
            && !SymbolEqualityComparer.Default.Equals(target, GetReferencedSymbol(operands[1])))
            return;

        ReportIfAccumulatesInLoop(context, assignment.Target);
    }

    private static void ReportIfAccumulatesInLoop(OperationAnalysisContext context, IOperation target)
    {
        var loop = FindEnclosingLoop(context.Operation.Syntax);
        if (loop is null)
            return;

        // A local that is recreated on every iteration does not accumulate anything.
        if (GetReferencedSymbol(target) is ILocalSymbol local
            && local.DeclaringSyntaxReferences.Any(reference => loop.Span.Contains(reference.Span)))
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation(), target.Syntax.ToString()));
    }

    private static SyntaxNode? FindEnclosingLoop(SyntaxNode node)
    {
        for (var parent = node.Parent; parent is not null; parent = parent.Parent)
        {
            if (parent is ForStatementSyntax or ForEachStatementSyntax or ForEachVariableStatementSyntax
                or WhileStatementSyntax or DoStatementSyntax)
                return parent;
        }

        return null;
    }

    private static ISymbol? GetReferencedSymbol(IOperation operation)
    {
        while (operation is IConversionOperation or IParenthesizedOperation)
            operation = operation.ChildOperations.First();

        return operation switch
        {
            ILocalReferenceOperation local => local.Local,
            IFieldReferenceOperation field => field.Field,
            IPropertyReferenceOperation property => property.Property,
            _ => null,
        };
    }
}
