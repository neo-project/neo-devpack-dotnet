// Copyright (C) 2015-2025 The Neo Project.
//
// BlockStatement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler
{
    internal partial class MethodConvert
    {
        /// <summary>
        /// Converts a block statement to OpCodes. This method is used for parsing
        /// the syntax of block statements within the context of a semantic model. A block statement
        /// typically consists of a series of statements enclosed in braces `{}`.
        /// </summary>
        /// <param name="model">The semantic model that provides information about the block statement.</param>
        /// <param name="syntax">The syntax of the block statement to be converted.</param>
        /// <remarks>
        /// This method starts by initializing a new list of local symbols for the current block.
        /// It then iterates through each statement within the block, converting each to the appropriate
        /// set of instructions. Local symbols are tracked and removed once the block is fully converted.
        /// </remarks>
        /// <example>
        /// Here is an example of a block statement syntax:
        ///
        /// <code>
        /// {
        ///     string x = "Hello world.";
        ///     Runtime.Log(x);
        /// }
        /// </code>
        ///
        /// In this example, the block contains two statements: a variable declaration and
        /// a method call.
        /// </example>
        private void ConvertBlockStatement(SemanticModel model, BlockSyntax syntax)
        {
            StatementContext sc = new(syntax);
            _generalStatementStack.Push(sc);
            foreach (StatementSyntax label in syntax.Statements)
                if (label is LabeledStatementSyntax l)
                {
                    ILabelSymbol symbol = (ILabelSymbol)model.GetDeclaredSymbol(l)!;
                    JumpTarget target = AddLabel(symbol);
                    sc.AddLabel(symbol, target);
                }
            _blockSymbols.Push(new List<ILocalSymbol>());
            using (InsertSequencePoint(syntax.OpenBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (StatementSyntax child in syntax.Statements)
                ConvertStatement(model, child);
            using (InsertSequencePoint(syntax.CloseBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (ILocalSymbol symbol in _blockSymbols.Pop())
                RemoveLocalVariable(symbol);
            if (_generalStatementStack.Pop() != sc)
                throw CompilationException.UnsupportedSyntax(syntax, "Internal compiler error: Statement stack mismatch in block statement handling. This is a compiler bug that should be reported.");
        }
    }
}
