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
