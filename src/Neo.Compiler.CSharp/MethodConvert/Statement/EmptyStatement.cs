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
        /// Converts an empty statement into a OpCode.NOP instruction.
        /// This method is used for handling empty statements in the code, which have no effect on the program execution.
        /// </summary>
        /// <param name="syntax">The syntax representation of the empty statement being converted.</param>
        /// <remarks>
        /// An empty statement in programming is typically represented by a standalone semicolon (;).
        /// This method inserts a NOP (no-operation) instruction for such statements,
        /// which effectively means 'do nothing'. This is useful in scenarios where the syntax requires
        /// a statement but the logic does not require any action.
        /// </remarks>
        /// <example>
        /// Example of an empty statement syntax:
        /// <code>
        /// ;
        /// </code>
        /// In this example, the semicolon represents an empty statement, requiring no action.
        /// </example>
        private void ConvertEmptyStatement(EmptyStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                AddInstruction(OpCode.NOP);
        }
    }
}
