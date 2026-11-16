// Copyright (C) 2015-2026 The Neo Project.
//
// ContinueStatement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'continue' statement into a corresponding jump instruction in the intermediate language.
        /// This method handles the conversion of the 'continue' keyword within loops, particularly
        /// considering its behavior in try-catch blocks.
        /// </summary>
        /// <param name="syntax">The syntax representation of the 'continue' statement being converted.</param>
        /// <remarks>
        /// The method checks if the 'continue' statement is within a try-catch block. If it is and the
        /// continue target count is zero, it generates an `ENDTRY_L` opcode to properly handle the loop
        /// continuation within the try block. Otherwise, a standard jump (`JMP_L`) is used to continue
        /// the loop.
        /// C# 15 labeled continue statements (`continue outer;`) are resolved to the loop the label is
        /// attached to, unwinding every try block crossed on the way out.
        /// </remarks>
        /// <example>
        /// Example of a 'continue' statement in a loop:
        /// <code>
        /// for (int i = 0; i < 10; i++)
        /// {
        ///     if (i % 2 == 0)
        ///         continue; // Skips the current iteration for even numbers
        ///     // Other processing
        /// }
        /// </code>
        /// In this example, 'continue' is used to skip the current iteration for even values of 'i'.
        /// </example>
        private void ConvertContinueStatement(ContinueStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                JumpTarget? continueTarget = null;
                List<StatementContext> visitedTry = [];  // from shallow to deep
                if (syntax.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault() is { } label)
                {// C# 15 labeled continue: continue the loop the label is attached to
                    continueTarget = FindLabeledJumpTarget(label.Identifier.ValueText, isContinue: true, visitedTry);
                    if (continueTarget == null)
                        // labeled continue is not handled
                        throw CompilationException.UnsupportedSyntax(syntax, $"Cannot find the labeled loop '{label.Identifier.ValueText}' for the continue statement. Ensure the label is attached to an enclosing loop.");
                }
                else
                {
                    foreach (StatementContext sc in _generalStatementStack)
                    {// start from the deepest context
                        // find the final continue target
                        if (sc.ContinueTarget != null)
                        {
                            continueTarget = sc.ContinueTarget;
                            break;
                        }
                        // stage the try stacks on the way
                        if (sc.StatementSyntax is TryStatementSyntax)
                            visitedTry.Add(sc);
                    }
                    if (continueTarget == null)
                        // continue is not handled
                        throw CompilationException.UnsupportedSyntax(syntax, "Continue statement must be inside a loop (for, while, do-while, foreach). If this appears to be valid code, it may indicate a compiler bug.");
                }

                foreach (StatementContext sc in visitedTry)
                    // start from the most external try
                    // internal try should ENDTRY, targeting the correct external continue target
                    continueTarget = sc.AddEndTry(continueTarget);

                Jump(OpCode.JMP_L, continueTarget);
                // We could use ENDTRY if current statement calling `continue` is a try statement,
                // but this job can be done by the optimizer
                // Note that, do not Jump(OpCode.ENDTRY_L, continueTarget) here,
                // because the continueTarget here is already an ENDTRY_L for current try stack.
            }
        }
    }
}
