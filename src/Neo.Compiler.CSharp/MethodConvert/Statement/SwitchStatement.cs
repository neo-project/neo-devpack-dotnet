// Copyright (C) 2015-2026 The Neo Project.
//
// SwitchStatement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'switch' statement into a set of conditional jump instructions and targets.
        /// This method handles the translation of 'switch' statements, including various forms of
        /// case labels (like pattern matching cases and default cases) into executable instructions.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the switch statement.</param>
        /// <param name="syntax">The syntax representation of the switch statement being converted.</param>
        /// <remarks>
        /// The method first evaluates the switch expression and then iterates over the different cases,
        /// generating conditional jumps based on the case labels. It supports pattern matching,
        /// value comparison, and default cases. The method ensures the correct control flow between
        /// different cases and handles the 'break' statement logic for exiting the switch.
        /// </remarks>
        /// <example>
        /// Example of a switch statement syntax:
        /// <code>
        /// switch (expression)
        /// {
        ///     case 1:
        ///         // Code for case 1
        ///         break;
        ///     case 2 when condition:
        ///         // Code for case 2 with condition
        ///         break;
        ///     default:
        ///         // Default case code
        ///         break;
        /// }
        /// </code>
        /// In this example, the switch statement includes different case scenarios, including a
        /// conditional case and a default case.
        /// </example>
        private void ConvertSwitchStatement(SemanticModel model, SwitchStatementSyntax syntax)
        {
            var sections = syntax.Sections.Select(p => (p.Labels, p.Statements, Target: new JumpTarget())).ToArray();
            var labels = sections.SelectMany(p => p.Labels, (p, l) => (l, p.Target)).ToArray();
            PushSwitchLabels(labels);
            JumpTarget breakTarget = new();
            byte anonymousIndex = AddAnonymousVariable();
            PushBreakTarget(breakTarget);
            StatementContext sc = new(syntax, breakTarget: breakTarget, switchLabels: labels.ToDictionary());
            _generalStatementStack.Push(sc);

            // handle possible normal labels in all sections of this switch
            foreach (SwitchSectionSyntax section in syntax.Sections)
                foreach (StatementSyntax label in section.Statements)
                    if (label is LabeledStatementSyntax l)
                    {
                        ILabelSymbol symbol = (ILabelSymbol)model.GetDeclaredSymbol(l)!;
                        JumpTarget target = AddLabel(symbol);
                        sc.AddLabel(symbol, target);
                    }

            using (InsertSequencePoint(syntax.Expression))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, anonymousIndex);
            }

            bool emittedOptimizedDispatch = _context.Options.Optimize.HasFlag(CompilationOptions.OptimizationType.Basic)
                && TryEmitOptimizedIntegerSwitchDispatch(model, labels, anonymousIndex, breakTarget);

            JumpTarget? defaultTarget = null;
            DefaultSwitchLabelSyntax? defaultLabel = null;
            if (!emittedOptimizedDispatch)
            {
                foreach (var (label, target) in labels)
                {
                    switch (label)
                    {
                        case CasePatternSwitchLabelSyntax casePatternSwitchLabel:
                            using (InsertSequencePoint(casePatternSwitchLabel))
                            {
                                JumpTarget endTarget = new();
                                ConvertPattern(model, casePatternSwitchLabel.Pattern, anonymousIndex);
                                Jump(OpCode.JMPIFNOT_L, endTarget);
                                if (casePatternSwitchLabel.WhenClause is not null)
                                {
                                    ConvertExpression(model, casePatternSwitchLabel.WhenClause.Condition);
                                    Jump(OpCode.JMPIFNOT_L, endTarget);
                                }
                                Jump(OpCode.JMP_L, target);
                                endTarget.Instruction = AddInstruction(OpCode.NOP);
                            }
                            break;
                        case CaseSwitchLabelSyntax caseSwitchLabel:
                            using (InsertSequencePoint(caseSwitchLabel))
                            {
                                AccessSlot(OpCode.LDLOC, anonymousIndex);
                                ConvertExpression(model, caseSwitchLabel.Value);
                                AddInstruction(OpCode.EQUAL);
                                Jump(OpCode.JMPIF_L, target);
                            }
                            break;
                        case DefaultSwitchLabelSyntax defaultSwitchLabel:
                            if (defaultTarget is not null)
                                throw CompilationException.UnsupportedSyntax(defaultSwitchLabel, "Switch statement contains multiple default labels.");
                            defaultTarget = target;
                            defaultLabel = defaultSwitchLabel;
                            break;
                        default:
                            throw CompilationException.UnsupportedSyntax(label, $"Unsupported switch label type '{label.GetType().Name}'. Use 'case value:' or 'default:' labels.");
                    }
                }
            }
            RemoveAnonymousVariable(anonymousIndex);

            if (!emittedOptimizedDispatch)
            {
                if (defaultTarget is null)
                {
                    Jump(OpCode.JMP_L, breakTarget);
                }
                else
                {
                    using (InsertSequencePoint(defaultLabel!))
                    {
                        Jump(OpCode.JMP_L, defaultTarget);
                    }
                }
            }

            foreach (var (_, statements, target) in sections)
            {
                target.Instruction = AddInstruction(OpCode.NOP);
                foreach (StatementSyntax statement in statements)
                    ConvertStatement(model, statement);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopSwitchLabels();
            PopBreakTarget();
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in switch statement handling. This is a compiler bug that should be reported.");
        }

        private bool TryEmitOptimizedIntegerSwitchDispatch(
            SemanticModel model,
            (SwitchLabelSyntax label, JumpTarget target)[] labels,
            byte valueSlot,
            JumpTarget breakTarget)
        {
            // We only optimize "simple" switches: constant integral case labels without patterns.
            // This keeps semantics identical while reducing runtime gas for large switches.
            const int MinCaseCountForOptimization = 8;
            const int LinearLeafSize = 4;

            var cases = new List<(BigInteger value, CaseSwitchLabelSyntax labelSyntax, JumpTarget target)>();
            JumpTarget? defaultTarget = null;

            foreach (var (label, target) in labels)
            {
                switch (label)
                {
                    case DefaultSwitchLabelSyntax:
                        if (defaultTarget is not null)
                            throw CompilationException.UnsupportedSyntax(label, "Switch statement contains multiple default labels.");
                        defaultTarget = target;
                        break;
                    case CaseSwitchLabelSyntax caseLabel:
                        if (!TryGetIntegerConstant(model, caseLabel.Value, out BigInteger value))
                            return false;
                        cases.Add((value, caseLabel, target));
                        break;
                    default:
                        return false; // Pattern / when / unsupported label kinds.
                }
            }

            if (cases.Count < MinCaseCountForOptimization)
                return false;

            // Ensure all case labels map to constant values and are unique.
            var orderedCases = cases
                .GroupBy(c => c.value)
                .Select(g => g.First())
                .OrderBy(c => c.value)
                .ToArray();

            if (orderedCases.Length != cases.Count)
                return false;

            defaultTarget ??= breakTarget;
            EmitIntegerSwitchDispatchTree(orderedCases, 0, orderedCases.Length, valueSlot, defaultTarget, LinearLeafSize);
            return true;
        }

        private void EmitIntegerSwitchDispatchTree(
            (BigInteger value, CaseSwitchLabelSyntax labelSyntax, JumpTarget target)[] cases,
            int start,
            int end,
            byte valueSlot,
            JumpTarget defaultTarget,
            int linearLeafSize)
        {
            int count = end - start;
            if (count <= 0)
            {
                Jump(OpCode.JMP_L, defaultTarget);
                return;
            }

            if (count <= linearLeafSize)
            {
                for (int i = start; i < end; i++)
                {
                    var (value, labelSyntax, target) = cases[i];
                    using (InsertSequencePoint(labelSyntax))
                    {
                        AccessSlot(OpCode.LDLOC, valueSlot);
                        Push(value);
                        Jump(OpCode.JMPEQ_L, target);
                    }
                }
                Jump(OpCode.JMP_L, defaultTarget);
                return;
            }

            int pivotIndex = start + (count / 2);
            BigInteger pivotValue = cases[pivotIndex].value;

            JumpTarget rightTarget = new();
            AccessSlot(OpCode.LDLOC, valueSlot);
            Push(pivotValue);
            Jump(OpCode.JMPGE_L, rightTarget);

            // Left side: values < pivot
            EmitIntegerSwitchDispatchTree(cases, start, pivotIndex, valueSlot, defaultTarget, linearLeafSize);

            // Right side: values >= pivot
            rightTarget.Instruction = AddInstruction(OpCode.NOP);
            EmitIntegerSwitchDispatchTree(cases, pivotIndex, end, valueSlot, defaultTarget, linearLeafSize);
        }

        private static bool TryGetIntegerConstant(SemanticModel model, ExpressionSyntax valueExpression, out BigInteger value)
        {
            value = default;
            var constant = model.GetConstantValue(valueExpression);
            if (!constant.HasValue || constant.Value is null)
                return false;

            switch (constant.Value)
            {
                case sbyte v:
                    value = v;
                    return true;
                case byte v:
                    value = v;
                    return true;
                case short v:
                    value = v;
                    return true;
                case ushort v:
                    value = v;
                    return true;
                case int v:
                    value = v;
                    return true;
                case uint v:
                    value = v;
                    return true;
                case long v:
                    value = v;
                    return true;
                case ulong v:
                    value = v;
                    return true;
                case char v:
                    value = (ushort)v;
                    return true;
                case bool v:
                    value = v ? BigInteger.One : BigInteger.Zero;
                    return true;
                case BigInteger v:
                    value = v;
                    return true;
                default:
                    return false;
            }
        }
    }
}
