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
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts an expression statement into a sequence of instructions.
        /// This method handles the translation of a given expression within a statement,
        /// considering the type of the expression to determine the necessary instructions.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the expression.</param>
        /// <param name="syntax">The syntax representation of the expression statement being converted.</param>
        /// <remarks>
        /// The method evaluates the type of the expression to decide if a 'DROP' instruction
        /// is needed. This is particularly important for expressions that leave a value on the stack,
        /// which might not be needed. For example, a method call that returns a value but
        /// whose return value is not used in the surrounding code.
        /// </remarks>
        /// <example>
        /// Example of an expression statement syntax:
        /// <code>
        /// CalculateSum(10, 20);
        /// </code>
        /// In this example, the expression is a method call to 'CalculateSum'.
        /// If 'CalculateSum' returns a value and it is not used, a 'DROP' instruction will be added.
        /// </example>
        private void ConvertExpressionStatement(SemanticModel model, ExpressionStatementSyntax syntax)
        {
            ITypeSymbol type = model.GetTypeInfo(syntax.Expression).Type!;
            using (InsertSequencePoint(syntax.Expression))
            {
                ConvertExpression(model, syntax.Expression);
                if (type.SpecialType != SpecialType.System_Void)
                    AddInstruction(OpCode.DROP);
            }
        }
    }
}
