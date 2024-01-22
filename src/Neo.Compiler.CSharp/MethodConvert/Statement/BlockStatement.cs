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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts a block statement to a sequence of instructions. This method is used for parsing
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
            _blockSymbols.Push(new List<ILocalSymbol>());
            using (InsertSequencePoint(syntax.OpenBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (StatementSyntax child in syntax.Statements)
                ConvertStatement(model, child);
            using (InsertSequencePoint(syntax.CloseBraceToken))
                AddInstruction(OpCode.NOP);
            foreach (ILocalSymbol symbol in _blockSymbols.Pop())
                RemoveLocalVariable(symbol);
        }
    }
}
