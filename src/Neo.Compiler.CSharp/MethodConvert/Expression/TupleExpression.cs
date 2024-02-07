// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler
{
    partial class MethodConvert
    {
        /// <summary>
        /// Converts tuple expressions to executable code.
        /// </summary>
        /// <param name="model">The semantic model</param>
        /// <param name="expression">The tuple expression syntax</param>
        /// <remarks>
        /// Handles syntax like:
        ///
        /// (10, 20, 30)
        /// ("Hello", name, value)
        ///
        /// Emits instructions to create a new array instance, duplicate
        /// its reference, convert and append each element expression.
        ///
        /// This builds up the tuple array containing the element values.
        /// </remarks>
        private void ConvertTupleExpression(SemanticModel model, TupleExpressionSyntax expression)
        {
            AddInstruction(OpCode.NEWSTRUCT0);
            foreach (ArgumentSyntax argument in expression.Arguments)
            {
                AddInstruction(OpCode.DUP);
                ConvertExpression(model, argument.Expression);
                AddInstruction(OpCode.APPEND);
            }
        }
    }
}
