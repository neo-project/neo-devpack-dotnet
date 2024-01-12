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
