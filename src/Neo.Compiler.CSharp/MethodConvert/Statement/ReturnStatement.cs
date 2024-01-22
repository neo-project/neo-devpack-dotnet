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

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts a 'return' statement into the corresponding jump instructions.
        /// This method handles the conversion of return statements, including those with a return value,
        /// within the context of try blocks and normal execution flow.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the return statement.</param>
        /// <param name="syntax">The syntax representation of the return statement being converted.</param>
        /// <remarks>
        /// If the return statement includes an expression, the method converts this expression.
        /// Depending on whether the return is within a try block, it generates different jump
        /// instructions to handle the control flow properly. This ensures that the function exits
        /// correctly, either by leaving the try block first or by jumping directly to the return target.
        /// </remarks>
        /// <example>
        /// Example of a return statement syntax:
        /// <code>
        /// return x + y;
        /// </code>
        /// In this example, the method will convert the expression 'x + y' and then jump to
        /// the return target, handling any try block logic if necessary.
        /// </example>
        private void ConvertReturnStatement(SemanticModel model, ReturnStatementSyntax syntax)
        {
            using (InsertSequencePoint(syntax))
            {
                if (syntax.Expression is not null)
                    ConvertExpression(model, syntax.Expression);
                if (_tryStack.Count > 0)
                    Jump(OpCode.ENDTRY_L, _returnTarget);
                else
                    Jump(OpCode.JMP_L, _returnTarget);
            }
        }
    }
}
