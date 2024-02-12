// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
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
                if (_tryStack.TryPeek(out ExceptionHandling? result) && result.BreakTargetCount == 0)
                    Jump(OpCode.ENDTRY_L, _breakTargets.Peek());
                else
                    Jump(OpCode.JMP_L, _breakTargets.Peek());
        }
    }
}
