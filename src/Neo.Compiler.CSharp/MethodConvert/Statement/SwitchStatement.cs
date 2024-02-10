// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler
{
    partial class MethodConvert
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
            using (InsertSequencePoint(syntax.Expression))
            {
                ConvertExpression(model, syntax.Expression);
                AccessSlot(OpCode.STLOC, anonymousIndex);
            }
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
                        using (InsertSequencePoint(defaultSwitchLabel))
                        {
                            Jump(OpCode.JMP_L, target);
                        }
                        break;
                    default:
                        throw new CompilationException(label, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {label}");
                }
            }
            RemoveAnonymousVariable(anonymousIndex);
            Jump(OpCode.JMP_L, breakTarget);
            foreach (var (_, statements, target) in sections)
            {
                target.Instruction = AddInstruction(OpCode.NOP);
                foreach (StatementSyntax statement in statements)
                    ConvertStatement(model, statement);
            }
            breakTarget.Instruction = AddInstruction(OpCode.NOP);
            PopSwitchLabels();
            PopBreakTarget();
        }
    }
}
