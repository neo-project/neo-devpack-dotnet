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

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts a checked statement to the corresponding operations. This method is used for parsing
        /// the syntax of checked statements within the context of a semantic model. Checked statements
        /// in C# are used to explicitly enable overflow checking for arithmetic operations and conversions.
        /// </summary>
        /// <param name="model">The semantic model that provides information about the checked statement.</param>
        /// <param name="syntax">The syntax of the checked statement to be converted.</param>
        /// <remarks>
        /// This method begins by pushing the current state (checked or unchecked) onto a stack to keep
        /// track of the context. It then processes the block of code within the checked statement.
        /// After the block is processed, the previous state is restored by popping from the stack.
        /// This ensures that arithmetic operations within the block are correctly handled according
        /// to the overflow checking rules specified by the checked statement.
        /// </remarks>
        /// <example>
        /// Here is an example of a checked statement syntax:
        ///
        /// <code>
        /// checked
        /// {
        ///     int x = int.MaxValue;
        ///     int y = x + 1; // This will cause an overflow exception
        /// }
        /// </code>
        ///
        /// In this example, arithmetic operations inside the checked block are performed with
        /// overflow checking enabled.
        /// </example>
        private void ConvertCheckedStatement(SemanticModel model, CheckedStatementSyntax syntax)
        {
            _checkedStack.Push(syntax.Keyword.IsKind(SyntaxKind.CheckedKeyword));
            ConvertBlockStatement(model, syntax.Block);
            _checkedStack.Pop();
        }
    }
}
