// Copyright (C) 2015-2025 The Neo Project.
//
// LabeledStatement.cs file belongs to the neo project and is free
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

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a labeled statement into a jump target and its associated instructions.
        /// This method is used for handling labeled statements, creating a target for 'goto' statements
        /// to jump to and processing the subsequent statement after the label.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the labeled statement.</param>
        /// <param name="syntax">The syntax representation of the labeled statement being converted.</param>
        /// <remarks>
        /// The method first declares a new jump target for the label. If the label is referenced in
        /// any pending 'goto' statements within a try block, it updates those 'goto' instructions
        /// to jump to the newly created label. Then, it converts the statement following the label.
        /// This is essential in implementing the label and goto mechanism found in many programming languages.
        /// </remarks>
        /// <example>
        /// Example of a labeled statement syntax:
        /// <code>
        /// myLabel:
        ///     // Code to execute after jumping to this label
        ///
        /// // Elsewhere in the code
        /// goto myLabel;
        /// </code>
        /// This example shows a labeled statement 'myLabel' and a 'goto' statement that jumps to it.
        /// </example>
        private void ConvertLabeledStatement(SemanticModel model, LabeledStatementSyntax syntax)
        {
            ILabelSymbol symbol = model.GetDeclaredSymbol(syntax)!;
            JumpTarget target = AddLabel(symbol);
            target.Instruction = AddInstruction(OpCode.NOP);
            ConvertStatement(model, syntax.Statement);
        }
    }
}
