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
        /// Converts a local variable declaration statement into a series of instructions.
        /// This method is used for processing variable declarations, excluding those marked as 'const'.
        /// </summary>
        /// <param name="model">The semantic model providing context and information about the local declaration statement.</param>
        /// <param name="syntax">The syntax representation of the local declaration statement being converted.</param>
        /// <remarks>
        /// For each variable in the declaration, the method checks if it has an initializer. If it does,
        /// the method converts the initializer expression and stores the result in the newly declared
        /// variable. This process involves adding a local variable to the current context and
        /// generating the necessary instructions to initialize it. Constants are not processed by this method.
        /// </remarks>
        /// <example>
        /// Example of a local variable declaration statement syntax:
        /// <code>
        /// int x = 5;
        /// long y;
        /// </code>
        /// In this example, 'x' is initialized with the value 5, while 'y' is declared without an initializer.
        /// </example>
        private void ConvertLocalDeclarationStatement(SemanticModel model, LocalDeclarationStatementSyntax syntax)
        {
            if (syntax.IsConst) return;
            foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
            {
                ILocalSymbol symbol = (ILocalSymbol)model.GetDeclaredSymbol(variable)!;
                byte variableIndex = AddLocalVariable(symbol);
                if (variable.Initializer is not null)
                    using (InsertSequencePoint(variable))
                    {
                        ConvertExpression(model, variable.Initializer.Value);
                        AccessSlot(OpCode.STLOC, variableIndex);
                    }
            }
        }
    }
}
