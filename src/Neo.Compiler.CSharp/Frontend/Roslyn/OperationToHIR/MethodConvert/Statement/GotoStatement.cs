// Copyright (C) 2015-2025 The Neo Project.
//
// GotoStatement.cs file belongs to the neo project and is free
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
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'goto' statement into a jump instruction. This method handles both simple
        /// 'goto' statements and those used in the context of a switch statement, including 'goto case'
        /// and 'goto default'.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the 'goto' statement.</param>
        /// <param name="syntax">The syntax representation of the 'goto' statement being converted.</param>
        /// <remarks>
        /// For a standard 'goto', it finds the target label and adds a jump instruction to it. In
        /// a switch statement, it identifies the correct case or default label to jump to based on
        /// the provided expression. The method also handles jumps out of 'try' blocks by adding
        /// necessary instructions to maintain valid control flow.
        /// </remarks>
        /// <example>
        /// Example of a 'goto' statement syntax:
        /// <code>
        /// goto myLabel;
        /// myLabel:
        ///     // Code to execute after the jump
        /// </code>
        /// Example of 'goto case' in a switch statement:
        /// <code>
        /// switch (value)
        /// {
        ///     case 1:
        ///         // ...
        ///         goto case 2;
        ///     case 2:
        ///         // ...
        ///         break;
        ///     default:
        ///         // ...
        ///         goto default;
        /// }
        /// </code>
        /// </example>
        private void ConvertGotoStatement(SemanticModel model, GotoStatementSyntax syntax)
        {
            if (syntax.CaseOrDefaultKeyword.IsKind(SyntaxKind.None))
                ConvertGotoNormalLabel(model, syntax);
            else
                ConvertGotoSwitchLabel(model, syntax);
        }

        private void ConvertGotoNormalLabel(SemanticModel model, GotoStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                JumpTarget? gotoTarget = null;
                List<StatementContext> visitedTry = [];  // from shallow to deep
                ILabelSymbol label = (ILabelSymbol)model.GetSymbolInfo(syntax.Expression!).Symbol!;
                foreach (StatementContext sc in _generalStatementStack)
                {// start from the deepest context
                    // find the final goto target
                    if (sc.TryGetLabel(label, out gotoTarget))
                        break;
                    // stage the try stacks on the way
                    if (sc.StatementSyntax is TryStatementSyntax)
                        visitedTry.Add(sc);
                }
                if (gotoTarget == null)
                    // goto is not handled
                    throw CompilationException.UnsupportedSyntax(syntax, $"Cannot find the target label '{label.Name}' for goto statement. Ensure the label is defined within the same method and is reachable from this location.");

                foreach (StatementContext sc in visitedTry)
                    // start from the most external try
                    // internal try should ENDTRY, targeting the correct external goto target
                    gotoTarget = sc.AddEndTry(gotoTarget);

                Jump(OpCode.JMP_L, gotoTarget);
                // We could use ENDTRY if current statement calling `goto` is a try statement,
                // but this job can be done by the optimizer
                // Note that, do not Jump(OpCode.ENDTRY_L, gotoTarget) here,
                // because the gotoTarget here is already an ENDTRY_L for current try stack.
            }
        }

        private void ConvertGotoSwitchLabel(SemanticModel model, GotoStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                JumpTarget? gotoTarget = null;
                List<StatementContext> visitedTry = [];  // from shallow to deep
                foreach (StatementContext sc in _generalStatementStack)
                {// start from the deepest context
                    // find the final goto target
                    if (sc.StatementSyntax is SwitchStatementSyntax switch_)
                    {
                        if (syntax.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword))
                            gotoTarget = sc.SwitchLabels!.First(p => p.Key is DefaultSwitchLabelSyntax).Value;
                        else
                        {
                            object? value = model.GetConstantValue(syntax.Expression!).Value;
                            foreach ((SwitchLabelSyntax label, JumpTarget target) in sc.SwitchLabels!)
                            {
                                if (label is not CaseSwitchLabelSyntax cl)
                                    continue;
                                object? clValue = model.GetConstantValue(cl.Value).Value;
                                if (value is null)
                                {
                                    if (clValue is null)
                                    {
                                        gotoTarget = target;
                                        break;
                                    }
                                }
                                else if (value.Equals(clValue))
                                {
                                    gotoTarget = target;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    // stage the try stacks on the way
                    if (sc.StatementSyntax is TryStatementSyntax)
                        visitedTry.Add(sc);
                }
                if (gotoTarget == null)
                    // goto is not handled
                    throw CompilationException.UnsupportedSyntax(syntax, syntax.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword)
                        ? "Cannot find 'default' label in switch statement for 'goto default'."
                        : $"Cannot find matching case label for 'goto case {syntax.Expression}' in switch statement.");

                foreach (StatementContext sc in visitedTry)
                    // start from the most external try
                    // internal try should ENDTRY, targeting the correct external goto target
                    gotoTarget = sc.AddEndTry(gotoTarget);

                Jump(OpCode.JMP_L, gotoTarget);
                // We could use ENDTRY if current statement calling `goto` is a try statement,
                // but this job can be done by the optimizer
                // Note that, do not Jump(OpCode.ENDTRY_L, gotoTarget) here,
                // because the gotoTarget here is already an ENDTRY_L for current try stack.
            }
        }
    }
}
