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

namespace Neo.Compiler
{

    /// <summary>
    /// Converts a 'throw' statement into the corresponding throw instruction.
    /// This method handles the translation of a throw statement, typically used for exception handling,
    /// into an intermediate language instruction.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the throw statement.</param>
    /// <param name="syntax">The syntax representation of the throw statement being converted.</param>
    /// <remarks>
    /// The method takes a throw statement and converts the expression being thrown into
    /// an appropriate throw instruction in the target language or intermediate representation.
    /// This is essential for implementing exception handling in the converted code.
    /// </remarks>
    /// <example>
    /// Example of a throw statement syntax:
    /// <code>
    /// throw new Exception("Error message");
    /// </code>
    /// This example demonstrates a throw statement that creates and throws a new exception
    /// with a specified error message.
    /// </example>
    partial class MethodConvert
    {
        private void ConvertThrowStatement(SemanticModel model, ThrowStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
                Throw(model, syntax.Expression);
        }
    }
}
