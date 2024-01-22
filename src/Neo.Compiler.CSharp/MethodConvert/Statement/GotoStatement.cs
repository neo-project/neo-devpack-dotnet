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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler
{
    partial class MethodConvert
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
            using (InsertSequencePoint(syntax))
                if (syntax.CaseOrDefaultKeyword.IsKind(SyntaxKind.None))
                {
                    ILabelSymbol symbol = (ILabelSymbol)model.GetSymbolInfo(syntax.Expression!).Symbol!;
                    JumpTarget target = AddLabel(symbol, false);
                    if (_tryStack.TryPeek(out ExceptionHandling? result) && result.State != ExceptionHandlingState.Finally && !result.Labels.Contains(symbol))
                        result.PendingGotoStatments.Add(Jump(OpCode.ENDTRY_L, target));
                    else
                        Jump(OpCode.JMP_L, target);
                }
                else
                {
                    var labels = _switchStack.Peek();
                    JumpTarget target = default!;
                    if (syntax.CaseOrDefaultKeyword.IsKind(SyntaxKind.DefaultKeyword))
                    {
                        target = labels.First(p => p.Item1 is DefaultSwitchLabelSyntax).Item2;
                    }
                    else
                    {
                        object? value = model.GetConstantValue(syntax.Expression!).Value;
                        foreach (var (l, t) in labels)
                        {
                            if (l is not CaseSwitchLabelSyntax cl) continue;
                            object? clValue = model.GetConstantValue(cl.Value).Value;
                            if (value is null)
                            {
                                if (clValue is null)
                                {
                                    target = t;
                                    break;
                                }
                            }
                            else
                            {
                                if (value.Equals(clValue))
                                {
                                    target = t;
                                    break;
                                }
                            }
                        }
                    }
                    if (_tryStack.TryPeek(out ExceptionHandling? result) && result.SwitchCount == 0)
                        Jump(OpCode.ENDTRY_L, target);
                    else
                        Jump(OpCode.JMP_L, target);
                }
        }
    }
}
