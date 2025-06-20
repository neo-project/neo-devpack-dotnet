// Copyright (C) 2015-2025 The Neo Project.
//
// BreakStatement.cs file belongs to the neo project and is free
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
        /// Converts a break statement into the corresponding jump instruction. This method handles
        /// the parsing and translation of a break statement within loop or switch constructs, converting
        /// it to an appropriate jump instruction in the neo vm language.
        /// </summary>
        /// <param name="syntax">The syntax of the break statement to be converted.</param>
        /// <remarks>
        /// This method determines the target of the break statement based on the current context. If the
        /// break statement is within a try block with no specific break target, it generates an `ENDTRY_L`
        /// jump instruction to exit the try block. Otherwise, it generates a `JMP_L` instruction to jump
        /// to the standard break target. This ensures that break statements behave correctly in both
        /// normal loops/switches and those within try-catch blocks.
        /// </remarks>
        /// <example>
        /// An example of a break statement syntax in a loop:
        ///
        /// <code>
        /// for (int i = 0; i < 10; i++)
        /// {
        ///     if (i == 5)
        ///         break;
        /// }
        /// </code>
        ///
        /// In this example, the break statement exits the loop when `i` equals 5.
        /// </example>
        private void ConvertBreakStatement(BreakStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                JumpTarget? breakTarget = null;
                List<StatementContext> visitedTry = [];  // from shallow to deep
                foreach (StatementContext sc in _generalStatementStack)
                {// start from the deepest context
                    // find the final break target
                    if (sc.BreakTarget != null)
                    {
                        breakTarget = sc.BreakTarget;
                        break;
                    }
                    // stage the try stacks on the way
                    if (sc.StatementSyntax is TryStatementSyntax)
                        visitedTry.Add(sc);
                }
                if (breakTarget == null)
                    // break is not handled
                    throw CompilationException.UnsupportedSyntax(syntax, "Break statement must be inside a loop (for, while, do-while, foreach) or switch statement. If this appears to be valid code, it may indicate a compiler bug.");

                foreach (StatementContext sc in visitedTry)
                    // start from the most external try
                    // internal try should ENDTRY, targeting the correct external break target
                    breakTarget = sc.AddEndTry(breakTarget);

                Jump(OpCode.JMP_L, breakTarget);
                // We could use ENDTRY if current statement calling `break` is a try statement,
                // but this job can be done by the optimizer
                // Note that, do not Jump(OpCode.ENDTRY_L, breakTarget) here,
                // because the breakTarget here is already an ENDTRY_L for current try stack.
            }
        }
    }
}
