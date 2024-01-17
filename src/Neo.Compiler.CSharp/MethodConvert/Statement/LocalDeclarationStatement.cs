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
