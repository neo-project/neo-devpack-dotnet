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
                if (_tryStack.TryPeek(out ExceptionHandling? result) && result.ContinueTargetCount == 0)
                    Jump(OpCode.ENDTRY_L, _continueTargets.Peek());
                else
                    Jump(OpCode.JMP_L, _continueTargets.Peek());
        }
    }
}
